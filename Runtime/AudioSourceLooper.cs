using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace HunterAllen.AudioTools
{
    public class AudioSourceLooper : MonoBehaviour
    {
        public bool PlayOnAwake;

        public bool UseBpm;
        public float Bpm = 140f;
        
        [Space]
        public AudioSegment[] Segments;
        public List<Interval> Intervals = new();
        List<Interval> _defaultIntervals;
        List<Interval> _allIntervals => _defaultIntervals.Concat(Intervals).ToList();

        AudioSource _activeAudioSource;
        AudioSource _availableAudioSource => _useOtherAudioSource ? _audioSourceTwo : _audioSourceOne;
        [SerializeField] AudioSource _audioSourceOne;
        [SerializeField] AudioSource _audioSourceTwo;

        AudioSegment _currentSegment => Segments[_currentSegmentIndex];
        Interval _currentInterval;
        int _currentSegmentIndex;
        bool _useOtherAudioSource;
        bool _isNextSegmentReady;
        
        public event Action OnStart;
        public event Action OnBeat;
        public event Action OnBar;
        public event Action OnStop;

        void Awake()
        {
            if (UseBpm)
            {
                var beatInterval = new Interval(Bpm, beats: 1f);
                var barInterval = new Interval(Bpm, beats: 4f);
                _defaultIntervals = new List<Interval>(2) { beatInterval, barInterval };
                beatInterval.OnTrigger.AddListener(TryInvokeBeat);
                barInterval.OnTrigger.AddListener(TryInvokeBar);

                void TryInvokeBeat() => OnBeat?.Invoke();
                void TryInvokeBar() => OnBar?.Invoke();
            }
            
            if (!PlayOnAwake) return;

            PlayCurrentSegment();
        }
        
        void Update()
        {
            if (!_activeAudioSource.isPlaying) return;

            foreach (var interval in _allIntervals)
            {
                float sampledTime = _activeAudioSource.timeSamples / (_activeAudioSource.clip.frequency * interval.IntervalLength);
                interval.CheckForNewInterval(sampledTime);
            }
        }

        [ContextMenu("Play Current Segment")]
        public void PlayCurrentSegment()
        {
            PlayClip(_currentSegment.AudioClip);
        }
        public void PlayClip(AudioClip clip)
        {
            // Play audio
            _activeAudioSource = _availableAudioSource;
            _activeAudioSource.clip = clip;
            _activeAudioSource.Play();

            // Flip source
            _useOtherAudioSource = !_useOtherAudioSource;

            // Reset intervals
            foreach (var interval in _allIntervals)
            {
                interval.Reset();
            }

            // Initialize intervals
            if (_currentInterval != null)
            {
                Intervals.Remove(_currentInterval);
                _currentInterval.OnTrigger.RemoveAllListeners();
                _currentInterval = null;
            }
            
            var segment = _currentSegment;
            _currentInterval = segment.UseBpm ? new Interval(Bpm, beats: segment.Beats) : new Interval(length: segment.Length);
            _currentInterval.OnTrigger.AddListener(TryPlayNextSegment);
            Intervals.Add(_currentInterval);
            
            OnStart?.Invoke();
        }

        public Interval AddInterval(float beats)
        {
            var interval = new Interval(Bpm, beats: beats);
            Intervals.Add(interval);
            return interval;
        }

        public void TryPlayNextSegment()
        {
            bool increment = _isNextSegmentReady || _currentSegment.AutomaticallyPlayNextSegment;

            // If the next segment is ready OR automatically play next segement, continue.
            // If the above is false AND looping is off, return.
            if (!increment && !_currentSegment.LoopUntilNextSegementPlays) return;

            if (increment) _currentSegmentIndex++;
            _isNextSegmentReady = false;

            PlayCurrentSegment();
        }
        public void ToggleNextSegmentReady(bool ready) => _isNextSegmentReady = ready;

        void OnValidate()
        {
            List<AudioSource> sources = GetComponents<AudioSource>().ToList();

            if (sources.Count != 2)
            {
                Debug.LogWarning("Please make sure there are 2 audio sources on this object.");
                return;
            }

            if (_audioSourceOne == null) _audioSourceOne = sources[0];
            if (_audioSourceTwo == null) _audioSourceTwo = sources[0];
        }
    }
    [Serializable]
    public struct AudioSegment
    {
        public AudioClip AudioClip;
        [Space]
        [Tooltip("Will loop based on beats instead of length.")] public bool UseBpm;
        public int Beats;
        public float Length;
        [Space]
        public bool AutomaticallyPlayNextSegment;
        public bool LoopUntilNextSegementPlays;
    }
    [Serializable]
    public class Interval
    {
        public UnityEvent OnTrigger = new UnityEvent();

        public float IntervalLength;
        int _lastInterval;
        bool _useBpm;

        /// <summary>Creates an interval that triggers N times per beat, or once every N seconds.</summary>
        /// <param name="bpm">Beats per minute.</param>
        /// <param name="triggersPerBeat">How many triggers will occur per beat if Bpm is being used.</param>
        /// <param name="length">How many seconds between each trigger.</param>
        public Interval(float bpm = -1f, float triggersPerBeat = -1f, float length = -1f)
        {
            _useBpm = triggersPerBeat > 0;
            IntervalLength = _useBpm ? 60f * triggersPerBeat / bpm : length;
        }
        /// <summary>Creates an interval that triggers once every N beats.</summary>
        /// <param name="bpm">Beats per minute.</param>
        /// <param name="beats">How many beats between each trigger.</param>
        public Interval(float bpm, float beats)
        {
            IntervalLength = 60f / bpm * beats;
        }
        public void CheckForNewInterval(float interval)
        {
            if (Mathf.FloorToInt(interval) != _lastInterval)
            {
                _lastInterval = Mathf.FloorToInt(interval);
                OnTrigger.Invoke();
            }
        }

        public void Reset()
        {
            _lastInterval = 0;
        }
    }
}