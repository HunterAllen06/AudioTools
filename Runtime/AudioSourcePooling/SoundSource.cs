using UnityEngine;

namespace HunterAllen.AudioTools
{
    [RequireComponent(typeof(AudioSource))]
    [AddComponentMenu("Hunter Allen/Audio Tools/Pools/Sound Source")]
    internal class SoundSource : MonoBehaviour
    {
        internal SoundPlayer ParentSoundPlayer { get; private set; }
        internal AudioSource AudioSource => _audioSource ??= GetComponent<AudioSource>();
        AudioSource _audioSource;

        internal void SetParentSoundPlayer(SoundPlayer soundPlayer)
        {
            if (ParentSoundPlayer != null) ParentSoundPlayer.ClearAudioSource();
            ParentSoundPlayer = soundPlayer;
            ParentSoundPlayer.SetAudioSource(this);
        }
        internal void ClearParentSoundPlayer()
        {
            if (ParentSoundPlayer == null) return;
            ParentSoundPlayer.ClearAudioSource();
            ParentSoundPlayer = null;
        }

        public static implicit operator AudioSource(SoundSource soundSource) => soundSource.AudioSource;
    }
}
