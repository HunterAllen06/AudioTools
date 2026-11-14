using HunterAllen.Utility;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace HunterAllen.AudioTools
{
    [RequireComponent(typeof(AudioLowPassFilter))]
    [RequireComponent(typeof(AudioReverbFilter))]
    [AddComponentMenu("Hunter Allen/Audio Tools/Zones/Audio Zone Member")]
    public class AudioZoneMember : MonoBehaviour
    {
        [SerializeField]
        #if ODIN_INSPECTOR
        [ReadOnly]
        #endif
        AudioLowPassFilter _lowPass;
        [SerializeField]
        #if ODIN_INSPECTOR
        [ReadOnly]
        #endif
        AudioReverbFilter _reverb;

        AudioReverbProperties _current = AudioZoneConstants.ReverbProperties[AudioReverbPreset.Generic];

        public AudioZone[] ActiveZones = new AudioZone[4];
        public int ZoneCount;

        Vector3 _previousPosition;

        void Awake()
        {
            if (!AudioZoneManager.Exists)
            if (!gameObject.TryGetComponent(out Rigidbody rigidbody) && !gameObject.TryGetComponentInParent(out rigidbody))
            {
                Debug.LogError($"{gameObject.name} does not have an attached Rigidbody component!", gameObject);
            }
            if (!gameObject.TryGetComponent(out SphereCollider collider))
            {
                Debug.LogError($"{gameObject.name} does not have an attached SphereCollider component!", gameObject);
            }
        }
        void OnDestroy()
        {
            for (int i = 0; i < ActiveZones.Length; i++)
            {
                ActiveZones[i] = null;
            }

            ZoneCount = 0;
        }

        public void SetCutoffFrequency(float frequency) => _lowPass.cutoffFrequency = frequency;
        public void SetReverb()
        {
            if (_reverb == null) Debug.LogWarning("Fuckkkkkkk no reverb", gameObject);
            if (transform.position == _previousPosition || _reverb == null) return;

            _previousPosition = transform.position;
            
            _reverb.dryLevel = _current.DryLevel;
            _reverb.room = _current.Room;
            _reverb.roomHF = _current.RoomHF;
            _reverb.roomLF = _current.RoomLF;
            _reverb.decayTime = _current.DecayTime;
            _reverb.decayHFRatio = _current.DecayHFRatio;
            _reverb.reflectionsLevel = _current.ReflectionsLevel;
            _reverb.reflectionsDelay = _current.ReflectionsDelay;
            _reverb.reverbLevel = _current.ReverbLevel;
            _reverb.reverbDelay = _current.ReverbDelay;
            _reverb.hfReference = _current.HFReference;
            _reverb.lfReference = _current.LFReference;
            _reverb.diffusion = _current.Diffusion;
            _reverb.density = _current.Density;
        }
        public void SetReverbProperties(AudioReverbProperties reverb) => _current = reverb;

        public void EnterZone(AudioZone zone)
        {
            for (int i = 0; i < ActiveZones.Length; i++)
            {
                if (ActiveZones[i] == zone) return;
                if (ActiveZones[i] == null)
                {
                    ActiveZones[i] = zone;
                    ZoneCount++;
                    return;
                }
            }
        }
        public void ExitZone(AudioZone zone)
        {
            for (int i = 0; i < ActiveZones.Length; i++)
            {
                if (ActiveZones[i] == zone)
                {
                    var swap = ActiveZones[ZoneCount - 1];
                    ActiveZones[i] = swap;
                    ActiveZones[ZoneCount - 1] = null;
                    ZoneCount--;
                    return;
                }
            }
        }

#if UNITY_EDITOR
        void Reset()
        {
            OnValidate();
        }
        // NEED TO DO THIS BEFORE RUNTIME STORE IT GOOD OK KEEP IT SAFE OR ELSE YOU ARE FIREDD!!!!!
        void OnValidate()
        {
            if (_lowPass == null)
            {
                _lowPass = GetComponent<AudioLowPassFilter>();
                _lowPass.cutoffFrequency = 22000f;
            }
            if (_reverb == null)
            {
                _reverb = GetComponent<AudioReverbFilter>();
                _reverb.reverbPreset = AudioReverbPreset.User;
            }

            if (Application.isPlaying) return;

            if (!TryGetComponent(out SphereCollider collider) && gameObject.activeInHierarchy)
            {
                collider = gameObject.AddComponent<SphereCollider>();
            }

            collider.radius = 0.1f;
            collider.isTrigger = true;

            if (gameObject.activeInHierarchy && !TryGetComponent(out Rigidbody rigidbody) && !gameObject.TryGetComponentInParent(out rigidbody))
            {
                Debug.LogError($"{gameObject.name} needs a Rigidbody!", gameObject);
                // rigidbody = gameObject.AddComponent<Rigidbody>();
                // rigidbody.isKinematic = true;
            }
            
            if (!CompareTag(AudioZoneConstants.MemberTag)) tag = AudioZoneConstants.MemberTag;
            
            if (AudioZoneGlobalSettings.Instance != null && !gameObject.CompareLayer(AudioZoneGlobalSettings.Instance.AudioZoneCollisionLayer))
            {
                gameObject.layer = AudioZoneGlobalSettings.Instance.AudioZoneCollisionLayer.ToLayer();
            }
        }
#endif
    }
}