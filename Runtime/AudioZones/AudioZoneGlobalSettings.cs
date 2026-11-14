#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace HunterAllen.AudioTools
{
    [CreateAssetMenu(fileName = "AudioZoneGlobalSettings", menuName = "Audio Zone Global Settings")]
    public class AudioZoneGlobalSettings : ScriptableObject
    {
        #if ODIN_INSPECTOR
        [BoxGroup("Values")]
        #endif
        public float MaxRoomSize = 6250;
        #if ODIN_INSPECTOR
        [BoxGroup("Values")]
        #endif
        public LayerMask AudioZoneCollisionLayer;
        #if ODIN_INSPECTOR
        [BoxGroup("Values")]
        #endif
        public LayerMask OcclusionLayer;
        #if ODIN_INSPECTOR
        [BoxGroup("Values")]
        #endif
        public AnimationCurve AbsorptionCurve;

        #if ODIN_INSPECTOR
        [BoxGroup("Presets")]
        #endif
        public AudioReverbProperties SmallRoomFullAbsorption;
        #if ODIN_INSPECTOR
        [BoxGroup("Presets")]
        #endif
        public AudioReverbProperties BigRoomFullAbsorption;
        #if ODIN_INSPECTOR
        [BoxGroup("Presets")]
        #endif
        public AudioReverbProperties SmallRoomNoAbsorption;
        #if ODIN_INSPECTOR
        [BoxGroup("Presets")]
        #endif
        public AudioReverbProperties BigRoomNoAbsorption;

        [Space]
        #if ODIN_INSPECTOR
        [BoxGroup("Presets")]
        [InlineButton(nameof(SetSmallRoomFullAbsorption))]
        #endif
        public AudioReverbPreset SmallRoomFullAbsorptionPreset;
        #if ODIN_INSPECTOR
        [BoxGroup("Presets")]
        [InlineButton(nameof(SetBigRoomFullAbsorption))]
        #endif
        public AudioReverbPreset BigRoomFullAbsorptionPreset;
        #if ODIN_INSPECTOR
        [BoxGroup("Presets")]
        [InlineButton(nameof(SetSmallRoomNoAbsorption))]
        #endif
        public AudioReverbPreset SmallRoomNoAbsorptionPreset;
        #if ODIN_INSPECTOR
        [BoxGroup("Presets")]
        [InlineButton(nameof(SetBigRoomNoAbsorption))]
        #endif
        public AudioReverbPreset BigRoomNoAbsorptionPreset;

        #if UNITY_EDITOR
        public static AudioZoneGlobalSettings Instance
        {
            get
            {
                var settings = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/_Project/ScriptableObjects/AudioZones/AudioZoneGlobalSettings.asset", typeof(AudioZoneGlobalSettings));
                return settings as AudioZoneGlobalSettings;
            }
        }
        #endif

        void SetSmallRoomFullAbsorption() => SmallRoomFullAbsorption = AudioZoneConstants.ReverbProperties[SmallRoomFullAbsorptionPreset];
        void SetBigRoomFullAbsorption() => BigRoomFullAbsorption = AudioZoneConstants.ReverbProperties[BigRoomFullAbsorptionPreset];
        void SetSmallRoomNoAbsorption() => SmallRoomNoAbsorption = AudioZoneConstants.ReverbProperties[SmallRoomNoAbsorptionPreset];
        void SetBigRoomNoAbsorption() => BigRoomNoAbsorption = AudioZoneConstants.ReverbProperties[BigRoomNoAbsorptionPreset];
    }
}