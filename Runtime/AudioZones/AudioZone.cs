using System;
using HunterAllen.Utility;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace HunterAllen.AudioTools
{
    [RequireComponent(typeof(BoxCollider))]
    [AddComponentMenu("Hunter Allen/Audio Tools/Zones/Audio Zone")]
    public class AudioZone : MonoBehaviour
    {
        #if ODIN_INSPECTOR
        [BoxGroup("Zone")]
        #endif
        public int Priority = 0;
        #if ODIN_INSPECTOR
        [BoxGroup("Zone")]
        #endif
        [Min(0.1f)]
        public Vector3 Size = Vector3.one;
        #if ODIN_INSPECTOR
        [BoxGroup("Zone")]
        [ValidateInput("@this.Blend < this.Size.x && this.Blend < this.Size.y && this.Blend < this.Size.z", "Blend may not exceed Size dimensions!")]
        #endif
        [Min(0f)]
        public float Blend = 0f;
        #if ODIN_INSPECTOR
        [BoxGroup("Zone")]
        #endif
        [Min(0f)]
        public float AbsorptionMultiplier = 1f;

        [Space]
        #if ODIN_INSPECTOR
        [BoxGroup("Zone")]
        #endif
        public AudioZoneSurface FloorMaterial = AudioZoneSurface.Generic;
        #if ODIN_INSPECTOR
        [BoxGroup("Zone")]
        #endif
        public AudioZoneSurface WallMaterial = AudioZoneSurface.Generic;
        #if ODIN_INSPECTOR
        [BoxGroup("Zone")]
        #endif
        public AudioZoneSurface CeilingMaterial = AudioZoneSurface.Generic;

        // [Space]
        // [BoxGroup("Reverb Settings")]
        // [SerializeField]
        // AudioReverbPreset _reverbPreset = AudioReverbPreset.Generic;

        #if ODIN_INSPECTOR
        [BoxGroup("Components")]
        [ReadOnly]
        #endif
        public BoxCollider Collider;

        #if ODIN_INSPECTOR
        [BoxGroup("Runtime Values")]
        [ReadOnly]
        [ShowInInspector]
        #endif
        public float CubedSize { get; private set; } = 0f;
        #if ODIN_INSPECTOR
        [BoxGroup("Runtime Values")]
        [ReadOnly]
        [ShowInInspector]
        #endif
        [Obsolete("This is not used!")]
        public float AbsorptionCoefficient { get; private set; } = 0.5f;
        #if ODIN_INSPECTOR
        [BoxGroup("Runtime Values")]
        [ReadOnly]
        [ShowInInspector]
        #endif
        public float ReverbCoefficient { get; private set; } = 0.5f;
        #if ODIN_INSPECTOR
        [BoxGroup("Runtime Values")]
        [ReadOnly]
        [ShowInInspector]
        #endif
        public AudioReverbProperties Reverb;
        #if ODIN_INSPECTOR
        [BoxGroup("Runtime Values")]
        [ReadOnly]
        [ShowInInspector]
        #endif
        public Vector3 Origin { get; private set; }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(AudioZoneConstants.MemberTag) || !other.TryGetComponent(out AudioZoneMember member)) return;
            AudioZoneManager.TryRegister(member);
            member.EnterZone(this);
        }
        void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(AudioZoneConstants.MemberTag) || !other.TryGetComponent(out AudioZoneMember member)) return;
            member.ExitZone(this);
            AudioZoneManager.TryDeregister(member);
        }

        void CalculateRoomCoefficients()
        {
            float floorAbsorption = AudioZoneConstants.AbsorptionCoefficients[FloorMaterial];
            float wallAbsorption = AudioZoneConstants.AbsorptionCoefficients[WallMaterial];
            float ceilingAbsorption = AudioZoneConstants.AbsorptionCoefficients[CeilingMaterial];

            float floorReverb = AudioZoneConstants.ReverbCoefficients[FloorMaterial];
            float wallReverb = AudioZoneConstants.ReverbCoefficients[WallMaterial];
            float ceilingReverb = AudioZoneConstants.ReverbCoefficients[CeilingMaterial];

            float floorArea = Size.x * Size.z;
            float wallArea = Size.x * Size.y * 2 + Size.z * Size.y * 2;
            float totalArea = floorArea * 2 + wallArea;

            float wallRatio = wallArea / totalArea;
            float floorRatio = (1f - wallRatio) * 0.5f;

            AbsorptionCoefficient = (floorRatio * floorAbsorption) + (wallRatio * wallAbsorption) + (floorRatio * ceilingAbsorption) * AbsorptionMultiplier;
            ReverbCoefficient = (floorRatio * floorReverb) + (wallRatio * wallReverb) + (floorRatio * ceilingReverb);
        }

#if UNITY_EDITOR
        Color ca = Color.yellow - Color.black * 0.55f;
        Color cb = Color.yellow - Color.black * 0.85f;
        void OnDrawGizmosSelected()
        {
            Gizmos.color = ca;
            Gizmos.DrawWireCube(transform.position, Size);
            if (Blend == 0) return;
            Gizmos.color = cb;
            Vector3 blendedSize = Size - Blend * 2 * Vector3.one;
            Gizmos.DrawWireCube(transform.position, blendedSize);
        }

        void Reset()
        {
            if (FindAnyObjectByType<AudioZoneManager>() == null)
            {
                Debug.LogError("No AudioZoneManager is in the scene, add one!");
            }

            OnValidate();
        }
        void OnValidate()
        {
            if (Collider == null)
            {
                Collider = GetComponent<BoxCollider>();
            }

            Collider.size = Size;
            CubedSize = Size.x * Size.y * Size.z;
            Origin = Collider.transform.position;

            CalculateRoomCoefficients();

            if (!CompareTag(AudioZoneConstants.ZoneTag)) tag = AudioZoneConstants.ZoneTag;
            
            if (AudioZoneGlobalSettings.Instance != null && !gameObject.CompareLayer(AudioZoneGlobalSettings.Instance.AudioZoneCollisionLayer))
            {
                gameObject.layer = AudioZoneGlobalSettings.Instance.AudioZoneCollisionLayer.ToLayer();
            }
        }
#endif
    }
}