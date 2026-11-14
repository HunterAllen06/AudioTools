using UnityEngine;
using UnityEngine.Audio;

namespace HunterAllen.AudioTools
{
    [System.Serializable]
    public struct Sound
    {
        public short Id;
#if UNITY_6000_0_OR_NEWER
        public AudioResource Audio;
#else
        public AudioClip Audio;
#endif
        public AudioMixerGroup MixerGroup;
        public int MaxCount;
    }
}
