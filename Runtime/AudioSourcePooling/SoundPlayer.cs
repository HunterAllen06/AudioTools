using System;
using System.Threading.Tasks;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace HunterAllen.AudioTools
{
    [AddComponentMenu("Hunter Allen/Audio Tools/Pools/Sound Player")]
    public class SoundPlayer : MonoBehaviour
    {
        public SoundData SoundData;

        [Space]
        public SoundType SoundType;
        [Range(0f, 1f)] public float Volume = 1f;
        [Range(-3f, 3f)] public float Pitch = 1f;
        public bool UseSoundData;
        #if ODIN_INSPECTOR
        [HideIf("@this.UseSoundData")]
        #endif
        [SerializeField]
        float _maxDistance = 25;

        #if ODIN_INSPECTOR
        [HideIf("@this.UseSoundData")]
        #endif
        [SerializeField]
        bool _playOnAwake;

        #if ODIN_INSPECTOR
        [HideIf("@this.UseSoundData")]
        #endif
        [SerializeField]
        bool _loop;

        public float MaxDistance => UseSoundData ? SoundData.MaxDistance : _maxDistance;
        public bool PlayOnAwake => UseSoundData ? SoundData.PlayOnAwake : _playOnAwake;
        public bool Loop => UseSoundData ? SoundData.Loop : _loop;

        internal SoundSource CurrentAudioSource { get => _currentAudioSource; }
        SoundSource _currentAudioSource;
        public float CurrentDistanceToListener { get; private set; }

        public short Id => SoundData.Id;
        public bool NeedsPlayer => IsPlaying && CurrentAudioSource == null;
        public bool IsPlaying { get; private set; }
        public bool IsStopQueued { get; private set; }

        public event Action OnPlay;
        public event Action OnStop;

        void OnEnable()
        {
            if (PlayOnAwake)
            {
                PlaySound();
            }
        }
        void Update()
        {
            HandleTransform();
        }

        /// <summary>
        /// Plays a sound.
        /// </summary>
        public virtual async void PlaySound()
        {
            IsPlaying = true;
            SoundManager.PlaySound(this);
            await Task.Yield();
            if (Application.exitCancellationToken.IsCancellationRequested) return;
            HandleTransform();
            OnPlay?.Invoke();
        }
        /// <summary>
        /// Stops a sound.
        /// </summary>
        /// <param name="queueStop">Whether or not to register this sound player to a queue to be stopped.</param>
        public void StopSound(bool queueStop = true)
        {
            IsPlaying = false;
            IsStopQueued = queueStop;
            ClearAudioSource();
            OnStop?.Invoke();
        }

        /// <summary>
        /// Assigns an audio source to this sound player.
        /// </summary>
        internal void SetAudioSource(SoundSource audioSource)
        {
            _currentAudioSource = audioSource;
            PlayAudioSource();
            HandleTransform();
            CalculateDistanceToListener(null);
        }
        /// <summary>
        /// Clears this sound player's audio source.
        /// </summary>
        internal void ClearAudioSource()
        {
            _currentAudioSource = null;
        }

        /// <summary>
        /// Plays the audio source attached to this sound player.
        /// </summary>
        internal void PlayAudioSource()
        {
            if (CurrentAudioSource != null) CurrentAudioSource.AudioSource.Play();
        }

        /// <summary>
        /// Handles the transform of the connected audio source.
        /// </summary>
        internal void HandleTransform()
        {
            if (CurrentAudioSource == null) return;

            if (SoundType == SoundType._2D)
            {
                SetParent(null, Vector3.zero);
                return;
            }

            if (transform.parent != null) SetParent(transform.parent, transform.localPosition);
            else SetPosition(transform.position);
        }
        internal void SetParent(Transform parent, Vector3 localPosition)
        {
            if (CurrentAudioSource == null) return;
            CurrentAudioSource.transform.SetParent(parent);
            CurrentAudioSource.transform.localPosition = localPosition;
        }
        internal void SetPosition(Vector3 position)
        {
            if (CurrentAudioSource == null) return;
            CurrentAudioSource.transform.position = position;
        }

        /// <summary>
        /// Calculates distance to the listener.
        /// </summary>
        internal void CalculateDistanceToListener(Transform listener)
        {
            var listener_ = listener ?? Camera.main?.transform ?? null;
            if (listener_ == null)
            {
                CurrentDistanceToListener = float.MaxValue;
                return;
            }
            CurrentDistanceToListener = Vector3.Distance(transform.position, listener_.position);
        }

        public static implicit operator SoundData(SoundPlayer soundPlayerBase) => soundPlayerBase.SoundData;
    }
    public enum SoundType : byte
    {
        [InspectorName("2D")] _2D,
        [InspectorName("3D")] _3D
    }
}