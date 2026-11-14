using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
#if UNITY_6000_0_OR_NEWER
using UnityEngine.Audio;
#endif

namespace HunterAllen.AudioTools
{
    [DefaultExecutionOrder(-1)]
    [AddComponentMenu("Hunter Allen/Audio Tools/Pools/Sound Pool Manager")]
    public class SoundManager : MonoBehaviour
    {
        public float ReallocationIntervalInSeconds = 0.03f;
        [Tooltip("Optional: If left empty, a gameobject with the sound source component will be spawned instead.")]
        [SerializeField]
        internal SoundSource SoundSourcePrefab;
        public List<SoundData> Sounds = new();
        
        static SoundSource _soundSourcePrefab => _instance.SoundSourcePrefab;
        static Dictionary<int, List<SoundSource>> _audioSourcePools = new();
        static Dictionary<int, List<SoundPlayer>> _soundPlayersById = new();

        static Transform _containerObject;
        static Transform _listener;
        static SoundManager _instance;
        static List<SoundPlayer> _queue = new();

        static float _nextAllocationTime;

        #region Initialization
        void OnEnable()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Initialize();
        }
        void OnDisable()
        {
            if (_instance == this)
            {
                _instance = null;
                _listener = null;
                _queue = new();
                _containerObject = null;
            }
        }

        async void Initialize()
        {
            _instance = this;

            _containerObject = new GameObject("SOUND POOL [RUNTIME]").transform;
            _audioSourcePools = new();
            _soundPlayersById = new();

            while (_listener == null)
            {
                if (Application.exitCancellationToken.IsCancellationRequested || _instance == null) return;
                _listener = Camera.main?.transform ?? null;
                await Task.Yield();
            }
        }
        #endregion

        #region Update Behaviour
        void Update()
        {
            HandleQueue();
            HandleSoundPlayers();
        }

        static void HandleQueue()
        {
            if (_queue.Count == 0) return;
            foreach (var soundPlayer in _queue)
            {
                if (soundPlayer == null) continue;
                _PlaySound(soundPlayer);
            }
            _queue.Clear();
        }
        static void HandleSoundPlayers()
        {
            if (_listener == null) return;

            foreach (KeyValuePair<int, List<SoundPlayer>> soundPlayersById in _soundPlayersById)
            {
                if (soundPlayersById.Value.Count == 0) continue;

                // Initial handling
                foreach (var soundPlayer in soundPlayersById.Value)
                {
                    HandleSoundPlayer(soundPlayer);
                }

                if (Time.time < _nextAllocationTime) return;
                _nextAllocationTime = Time.time + _instance.ReallocationIntervalInSeconds;

                ReallocateSoundSources(soundPlayersById);
            }
        }

        static void ReallocateSoundSources(KeyValuePair<int, List<SoundPlayer>> soundPlayersById)
        {
            var id = soundPlayersById.Key;
            // Closest is first in list
            var soundPlayers = soundPlayersById.Value
                .Where(x => x.IsPlaying && x.SoundType == SoundType._3D)
                .OrderBy(x => x.CurrentDistanceToListener)
                .ToList();

            if (soundPlayers.Count == 0) return;
            
            var audioSources = _audioSourcePools[id];
            var sound = soundPlayers[0].SoundData.Sound;
            HashSet<SoundPlayer> soundPlayersWithAudioSource = new();

            // If already has a audio source, add to list of sound players with an audio source
            for (int i = 0; i < Mathf.Min(sound.MaxCount, soundPlayers.Count); i++)
            {
                var soundPlayer = soundPlayers[i];
                if (soundPlayersWithAudioSource.Count >= sound.MaxCount) break;
                // Break out early if audio source limit is reached
                if (soundPlayer.CurrentAudioSource != null) soundPlayersWithAudioSource.Add(soundPlayer);
            }

            // Assign available audio sources to sound players without one
            foreach (var soundPlayer in soundPlayers)
            {
                // Break out early if audio source limit is reached
                if (soundPlayersWithAudioSource.Count >= sound.MaxCount) break;
                if (soundPlayersWithAudioSource.Contains(soundPlayer)) continue;

                var audioSource =
                    audioSources.FirstOrDefault(x => x.ParentSoundPlayer == null) ??
                    soundPlayers.LastOrDefault(x => x.CurrentAudioSource != null)?.CurrentAudioSource ?? null;

                if (audioSource == null && TryGetFurthestValidSoundPlayer(soundPlayer, out var soundPlayer_))
                {
                    audioSource = soundPlayer_.CurrentAudioSource;
                }

                if (audioSource != null)
                {
                    audioSource.SetParentSoundPlayer(soundPlayer);
                    soundPlayersWithAudioSource.Add(soundPlayer);
                }
            }

            // Remove audio source from all sound players that didn't receive one in the first two for loops
            foreach (var soundPlayer in soundPlayersById.Value)
            {
                if (soundPlayer.CurrentAudioSource != null && !soundPlayersWithAudioSource.Contains(soundPlayer))
                {
                    soundPlayer.ClearAudioSource();
                }
            }
        }

        static void HandleSoundPlayer(SoundPlayer soundPlayer)
        {
            if (_listener == null) return;
            soundPlayer.CalculateDistanceToListener(_listener);
        }
        #endregion

        #region Play Sound Functions
        /// <summary>
        /// Queues a sound to be played.
        /// </summary>
        /// <param name="soundPlayer"></param>
        public static void PlaySound(SoundPlayer soundPlayer)
        {
            _queue.Add(soundPlayer);
        }
        /// <summary>
        /// Plays a sound with whatever available source there is.
        /// </summary>
        static void _PlaySound(SoundPlayer soundPlayer)
        {
            if (soundPlayer.CurrentAudioSource == null)
            {
                var audioSource = GetAvailableSource(soundPlayer);
                audioSource.SetParentSoundPlayer(soundPlayer);
            }

            soundPlayer.PlayAudioSource();
            _soundPlayersById.TryAddToListAtIndex(soundPlayer.Id, soundPlayer);
            StopSoundAfterDonePlaying(soundPlayer);
        }

        /// <summary>
        /// Stops a sound player once it is done playing sound.
        /// </summary>
        static async void StopSoundAfterDonePlaying(SoundPlayer soundPlayer)
        {
            while (soundPlayer.IsPlaying && !soundPlayer.IsStopQueued)
            {
                await Task.Yield();
                if (Application.exitCancellationToken.IsCancellationRequested || _instance == null) return;
            }

            soundPlayer.StopSound(false);
        }

        /// <summary>
        /// Stops a sound on a sound player.
        /// </summary>
        /// <returns> The sound source that belonged to the sound player.</returns>
        static SoundSource StealAudioSourceFromSoundPlayer(SoundPlayer soundPlayer)
        {
            if (soundPlayer == null || soundPlayer.CurrentAudioSource == null) return null;
            var audioSource = soundPlayer.CurrentAudioSource;
            audioSource.ClearParentSoundPlayer();
            if (!soundPlayer.Loop) soundPlayer.StopSound();
            return audioSource;
        }
        #endregion

        #region Get Functions
        /// <summary>
        /// Returns what is considered the most available source.
        /// <para/>First, we check to see if any sound player isn't playing a sound. If so, return that sound player.
        /// <para/>Otherwise, if we aren't at the sound limit, create a new source.
        /// <para/>Otherwise, get the furthest valid sound player and steal its audio source.
        /// <para/>Finally, if none of these return, return the first available audio source in the sound pool.
        /// </summary>
        static SoundSource GetAvailableSource(SoundPlayer soundPlayer)
        {
            var soundData = soundPlayer.SoundData;
            
            // Create id -> audio source pool index and the audio source pool itself
            if (!_audioSourcePools.TryGetValue(soundData.Id, out var audioSources))
            {
                audioSources = new();
                _audioSourcePools.Add(soundData.Id, audioSources);
            }

            // Find a source that isn't playing in the corresponding audio source pool
            foreach (SoundSource audioSource in audioSources)
            {
                if (audioSource.AudioSource.isPlaying) continue;
                return audioSource;
            }

            if (!IsSoundAtPlayerLimit(soundData))
            {
                return CreateSourceAndAddToPool(soundPlayer);
            }

            if (soundPlayer.SoundType == SoundType._3D && TryGetFurthestValidSoundPlayer(soundData, out SoundPlayer sp))
            {
                return StealAudioSourceFromSoundPlayer(sp);
            }

            return _audioSourcePools[soundData.Id].FirstOrDefault(x => x.ParentSoundPlayer == null)??_audioSourcePools[soundData.Id][0];
        }

        /// <summary>
        /// Gets the furthest away sound player for a certain sound.
        /// </summary>
        static bool TryGetFurthestValidSoundPlayer(SoundData soundData, out SoundPlayer soundPlayer)
        {
            soundPlayer = null;

            if (!_soundPlayersById.TryGetValue(soundData.Id, out var soundPlayers) || soundPlayers.Count == 0) return false;

            var soundPlayersOrderedByDistance = soundPlayers.OrderByDescending(x => x.CurrentDistanceToListener);
            SoundPlayer furthestPlayer = soundPlayersOrderedByDistance.FirstOrDefault(x => x != null && x.CurrentAudioSource != null);
            soundPlayer = furthestPlayer;
            return soundPlayer != null;
        }

        /// <summary>
        /// Returns the audio that belongs to sound data.
        /// </summary>
#if UNITY_6000_0_OR_NEWER
        static AudioResource GetAudio(SoundData soundData)
#else
        static AudioClip GetAudio(SoundData soundData)
#endif
        {
            return soundData.Sound.Audio;
        }
#if UNITY_6000_0_OR_NEWER
        static bool TryGetAudio(SoundData soundData, out AudioResource audio)
#else
        static bool TryGetAudio(SoundData soundData, out AudioClip audio)
#endif
        {
            bool isValid = TryGetSound(soundData.Id, out var sound);
            audio = soundData.Sound.Audio;
            return isValid;
        }

        /// <summary>
        /// Tries to get a sound from an id.
        /// </summary>
        static bool TryGetSound(int id, out Sound sound)
        {
            sound = default;
            foreach (Sound s in _instance.Sounds)
            {
                if (s.Id == id)
                {
                    sound = s;
                    return true;
                }
            }
            Debug.LogWarning($"SoundManager encountered an issue while trying to get sound with id [{id}]");
            return false;
        }
        #endregion

        #region Create Source
        /// <summary>
        /// Creates an audio source for a sound player.
        /// </summary>
        static SoundSource CreateSourceAndAddToPool(SoundPlayer soundPlayer)
        {
            // if (_audioSourcePools[soundPlayer.Id].Count >= soundPlayer.SoundData.Sound.MaxCount) return null;
            if (IsSoundAtPlayerLimit(soundPlayer.SoundData))
            {
                return null;
            }

            HandleSoundPlayer(soundPlayer);
            var id = soundPlayer.Id;
            var source = _soundSourcePrefab != null ? Instantiate(_soundSourcePrefab) : new GameObject().AddComponent<SoundSource>();
            var hasPool = _audioSourcePools.TryGetValue(id, out var sources);
            var audio = GetAudio(soundPlayer.SoundData);

            // Create GameObject
            SetSourceName(source, $"[{audio.name}] Player ({(hasPool ? sources.Count : "0")})");
            SetSourceParent(source, _containerObject);

            // Set audio-related data
#if UNITY_6000_0_OR_NEWER
            source.AudioSource.resource = audio;
#else
            source.AudioSource.clip = audio;
#endif
            SetSourceParameters(source, soundPlayer);

            _audioSourcePools.TryAddToListAtIndex(id, source);

            return source;
        }
        internal static void SetSourceName(SoundSource source, string name) => source.name = name;
        internal static void SetSourceParent(SoundSource source, Transform containerObject) => source.transform.SetParent(containerObject);
        /// <summary>
        /// Sets volume, pitch, loop and other parameters for an audio source.
        /// </summary>
        internal static void SetSourceParameters(SoundSource source, SoundPlayer soundPlayer)
        {
            var source_ = source.AudioSource;
            if (soundPlayer.SoundData.Sound.MixerGroup != null) source_.outputAudioMixerGroup = soundPlayer.SoundData.Sound.MixerGroup;
            source_.volume = soundPlayer.Volume;
            source_.pitch = soundPlayer.Pitch;
            source_.loop = soundPlayer.Loop;

            // Set attenuation-related fields
            source_.rolloffMode = AudioRolloffMode.Logarithmic;
            source_.maxDistance = soundPlayer.MaxDistance;
            source_.spatialBlend = soundPlayer.SoundType switch { SoundType._2D => 0f, SoundType._3D => 1f, _ => 0f };
        }
        #endregion

        #region Validation
        /// <summary>
        /// True if there are too many audio sources for a sound.
        /// </summary>
        internal static bool IsSoundAtPlayerLimit(SoundData soundData)
        {
            if (!_soundPlayersById.TryGetValue(soundData.Id, out var soundPlayers)) return false;
            return soundData.Sound.MaxCount > 0 && soundPlayers.Count >= soundData.Sound.MaxCount;
        }
        #endregion
        
        #if UNITY_EDITOR
        [ContextMenu("Get All Sound Data")]
        void GetAllSoundData()
        {
            Sounds = SoundDataCreatorEditor.SetIds();
            var stage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            if (stage != null)
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(stage.scene);
            }
        }
        #endif
    }
    internal static class HelperFunctions
    {
        #region Dictionary Utility Functions
        internal static void TryAddToListAtIndex<T1, T2>(this T1 self, int index, T2 t, bool avoidDuplicates = true) where T1 : Dictionary<int, List<T2>>
        {
            if (self.TryGetValue(index, out var list))
            {
                // Will happen so long as we don't avoid duplicates OR the list does not contain the item
                // Same thing as avoiding duplicates AND list contains the item = false
                if (!avoidDuplicates || !list.Contains(t))
                {
                    self[index].Add(t);
                    return;
                }
                return;
            }

            self[index] = new() { t };
        }
        internal static void TryRemoveFromListAtIndex<T1, T2>(this T1 self, int index, T2 t) where T1 : Dictionary<int, List<T2>>
        {
            if (self.TryGetValue(index, out var list) && list.Contains(t)) self[index].Remove(t);
        }
        #endregion
    }
}