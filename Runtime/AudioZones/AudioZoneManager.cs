using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace HunterAllen.AudioTools
{
    [AddComponentMenu("Hunter Allen/Audio Tools/Zones/Audio Zone Manager")]
    public class AudioZoneManager : MonoBehaviour
    {
        static AudioReverbProperties _off = AudioZoneConstants.ReverbProperties[AudioReverbPreset.Off];

        static Transform _listener;
        
        static AudioZoneMember[] _members = new AudioZoneMember[32];
        static byte _memberCount;

        [SerializeField]
        #if ODIN_INSPECTOR
        [Required]
        #endif
        AudioZoneGlobalSettings _globalSettings;

        public static AudioZoneGlobalSettings GlobalSettings { get; private set; }
        // public static AudioZoneGlobalSettings GlobalSettings => AudioZoneGlobalSettings.Instance;

        public static bool Exists;

        void OnEnable() => Exists = true;
        void OnDisable() => Exists = false;

        async void Start()
        {
            GlobalSettings = _globalSettings;

            while (_listener == null)
            {
                _listener = Camera.main?.transform;
                await Task.Yield();
                if (Application.exitCancellationToken.IsCancellationRequested) return;
            }
        }

        void Update()
        {
            if (_listener == null) return;

            for (int i = 0; i < _memberCount; i++)
            {
                // if (_members[i] == null) continue;
                if (_members.ElementAt(i) == null) continue;
                HandleMember(_members[i]);
            }
        }

        void HandleMember(AudioZoneMember member)
        {
            CalculateReverb(member);
            CalculateAbsorption(member);
        }

        #region Reverb
        AudioZoneBlend[] _blendsCache = new AudioZoneBlend[4];
        AudioZone _currentZone;
        void CalculateReverb(AudioZoneMember member)
        {
            if (member.ZoneCount == 0) return;

            var zones = member.ActiveZones;
            int propsCount = 0;
            // int priority = 0;
            var totalBlend = 0f;
            var finalProps = _off;

            for (int i = 0; i < member.ZoneCount; i++)
            {
                _currentZone = zones[i];

                var blend = CalculateBlend(member, _currentZone);
                
                _blendsCache[propsCount] = new()
                {
                    AbsorptionCoefficient = _currentZone.AbsorptionCoefficient,
                    ReverbCoefficient = _currentZone.ReverbCoefficient,
                    Size = _currentZone.CubedSize / GlobalSettings.MaxRoomSize,
                    Blend = blend,
                    Priority = _currentZone.Priority
                };

                totalBlend += CalculateBlend(member, _currentZone);
                propsCount++;
            }

            if (propsCount == 1)
            {
                var blend = _blendsCache[0];
                var smallProps = GlobalSettings.SmallRoomFullAbsorption.Lerp(GlobalSettings.SmallRoomNoAbsorption, blend.ReverbCoefficient);
                var bigProps = GlobalSettings.BigRoomFullAbsorption.Lerp(GlobalSettings.BigRoomNoAbsorption, blend.ReverbCoefficient);
                finalProps = _off.Lerp(smallProps.Lerp(bigProps, blend.Size), totalBlend);
            }
            else
            {
                _blendsCache.OrderBy(x => x.Priority);
                AudioZoneBlend[] _blendsFinal = new AudioZoneBlend[totalBlend < 1f ? propsCount + 1 : propsCount];

                if (totalBlend < 1f)
                {
                    _blendsFinal[0] = new AudioZoneBlend()
                    {
                        AbsorptionCoefficient = 0,
                        ReverbCoefficient = 0,
                        Size = 0,
                        Blend = 1 - totalBlend,
                        Priority = -1
                    };

                    for (int i = 1; i < propsCount + 1; i++)
                    {
                        _blendsFinal[i] = _blendsCache[i];
                    }
                }
                else
                {
                    for (int i = 0; i < propsCount; i++)
                    {
                        _blendsFinal[i] = _blendsCache[i];
                    }
                }

                (float, float) reverbAndSize = Average(_blendsFinal);
                var smallProps = GlobalSettings.SmallRoomFullAbsorption.Lerp(GlobalSettings.SmallRoomNoAbsorption, reverbAndSize.Item1);
                var bigProps = GlobalSettings.BigRoomFullAbsorption.Lerp(GlobalSettings.BigRoomNoAbsorption, reverbAndSize.Item1);
                finalProps = smallProps.Lerp(bigProps, reverbAndSize.Item2);
            }

            member.SetReverbProperties(finalProps);
            member.SetReverb();
        }
        float CalculateBlend(AudioZoneMember member, AudioZone zone)
        {
            var distance = GetPenetration(zone.Origin, 0.5f * zone.Size, member.transform.position);
            return zone.Blend == 0 ? 1 : Mathf.InverseLerp(0, zone.Blend, distance);
        }
        #endregion

        #region Absorption
        void CalculateAbsorption(AudioZoneMember member)
        {
            var pos = member.transform.position;
            var dir = _listener.transform.position - member.transform.position;
            if (!Physics.Raycast(pos, dir.normalized, out RaycastHit hit, dir.magnitude - 0.5f, GlobalSettings.OcclusionLayer))
            {
                member.SetCutoffFrequency(22000);
                return;
            }

            var absorption = 0f;
            var priority = -1;
            var dot = Vector3.Dot(hit.normal, Vector3.up);

            for (int i = 0; i < member.ZoneCount; i++)
            {
                var zone = member.ActiveZones[i];
                if (zone.Priority > priority)
                {
                    // absorption = zone.AbsorptionCoefficient;
                    // Wall
                    if (dot >= -0.5f && dot < 0.5f)
                    {
                        absorption = AudioZoneConstants.AbsorptionCoefficients[zone.WallMaterial] * zone.AbsorptionMultiplier;
                    }
                    // Floor
                    else if (dot >= 0.5f)
                    {
                        absorption = AudioZoneConstants.AbsorptionCoefficients[zone.FloorMaterial] * zone.AbsorptionMultiplier;
                    }
                    // Ceiling
                    else if (dot < -0.5f)
                    {
                        absorption = AudioZoneConstants.AbsorptionCoefficients[zone.CeilingMaterial] * zone.AbsorptionMultiplier;
                    }
                    priority = zone.Priority;
                }
            }
            
            var blend = GlobalSettings.AbsorptionCurve.Evaluate(Vector3.Distance(hit.point, _listener.transform.position) * absorption);
            member.SetCutoffFrequency(Mathf.Lerp(200, 22000, blend));
        }
        #endregion

        #region Utility
        Vector3 GetClosestPointOnBounds(Vector3 position, Vector3 bounds, Vector3 point)
        {
            var halfExtents = 0.5f * bounds;
            // float x = GetIntersection(position.x, bounds.x, point.x);
            // float y = GetIntersection(position.y, bounds.y, point.y);
            // float z = GetIntersection(position.z, bounds.z, point.z);

            // Clamp to be inside of bounds
            float x = Mathf.Clamp(point.x, position.x - halfExtents.x, position.x + halfExtents.x);
            float y = Mathf.Clamp(point.y, position.y - halfExtents.y, position.y + halfExtents.y);
            float z = Mathf.Clamp(point.z, position.z - halfExtents.z, position.z + halfExtents.z);

            float xPenetrationDistance = GetPenetration(position.x, halfExtents.x, point.x);
            float yPenetrationDistance = GetPenetration(position.y, halfExtents.y, point.y);
            float zPenetrationDistance = GetPenetration(position.z, halfExtents.z, point.z);

            bool xSmallest = xPenetrationDistance < yPenetrationDistance && xPenetrationDistance < zPenetrationDistance;
            bool ySmallest = yPenetrationDistance < zPenetrationDistance && yPenetrationDistance < xPenetrationDistance;

            if (xSmallest) x = GetIntersection(position.x, halfExtents.x, x);
            else if (ySmallest) y = GetIntersection(position.y, halfExtents.y, y);
            else z = GetIntersection(position.z, halfExtents.z, z);

            return new Vector3(x, y, z);
        }
        float GetPenetration(float origin, float halfExtents, float point)
        {
            return Mathf.Max(0, halfExtents - Mathf.Abs(point - origin));
        }
        float GetPenetration(Vector3 origin, Vector3 halfExtents, Vector3 point)
        {
            var x = GetPenetration(origin.x, halfExtents.x, point.x);
            var y = GetPenetration(origin.y, halfExtents.y, point.y);
            var z = GetPenetration(origin.z, halfExtents.z, point.z);
            return Mathf.Min(x, y, z);
        }
        float GetIntersection(float origin, float halfExtents, float point)
        {
            return point > origin ? origin + halfExtents : origin - halfExtents;
        }

        (float, float) Average(params AudioZoneBlend[] properties)
        {
            // var currentProps = AudioReverbProperties.Zero;
            float currentReverb = 0;
            float currentSize = 0;
            var priority = 0;
            var index = 0;

            for (int i = 0; i < properties.Length; i++)
            {
                var current = properties[i];

                if (current.Priority > priority && i > 0)
                {
                    // currentProps = currentProps.Lerp(current.Reverb, current.Blend);
                    currentReverb = Mathf.Lerp(currentReverb, current.ReverbCoefficient, current.Blend);
                    currentSize = Mathf.Lerp(currentSize, current.Size, current.Blend);
                    priority = current.Priority;
                    index = 1;
                    continue;
                }

                // currentProps += current.Reverb * current.Blend;
                currentReverb += current.ReverbCoefficient * current.Blend;
                currentSize += current.Size * current.Blend;
                index++;
            }

            return (currentReverb / index, currentSize / index);
        }
        #endregion

        internal static void TryRegister(AudioZoneMember member)
        {
            if (member.ZoneCount > 0) return;

            for (int i = 0; i < _members.Length; i++)
            {
                if (_members[i] == null)
                {
                    _members[i] = member;
                    _memberCount++;
                    return;
                }
            }
        }
        internal static void TryDeregister(AudioZoneMember member)
        {
            if (member.ZoneCount > 0) return;

            for (int i = 0; i < _members.Length; i++)
            {
                if (_members[i] == member)
                {
                    var swap = _members[_memberCount - 1];
                    _members[i] = swap;
                    _members[_memberCount - 1] = null;
                    _memberCount--;
                    return;
                }
            }
        }

        static void DrawAxisAt(Vector3 point, Color color)
        {
            Debug.DrawLine(point - Vector3.right * 0.5f, point + Vector3.right * 0.5f, color);
            Debug.DrawLine(point - Vector3.up * 0.5f, point + Vector3.up * 0.5f, color);
            Debug.DrawLine(point - Vector3.forward * 0.5f, point + Vector3.forward * 0.5f, color);
        }
    }
}
