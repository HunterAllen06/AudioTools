using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace HunterAllen.AudioTools
{
    [CreateAssetMenu(fileName = "SoundData", menuName = "SoundData")]
    public class SoundData : ScriptableObject
    {
        #if ODIN_INSPECTOR
        [InlineProperty, HideLabel, BoxGroup("Sound")]
        #endif
        public Sound Sound;
        public short Id => Sound.Id;

        public static implicit operator Sound(SoundData data) => data.Sound;

        public float MaxDistance = 25;
        public bool PlayOnAwake;
        public bool Loop;

        public void SetId(short id)
        {
            var sound = Sound;
            sound.Id = id;
            Sound = sound;
        }
    }
}