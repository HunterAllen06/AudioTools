using System;
using System.Collections.Generic;
using UnityEngine;

namespace HunterAllen.AudioTools
{
    [DefaultExecutionOrder(-1)]
    [AddComponentMenu("Hunter Allen/Audio Tools/Emissions/Sound Emission Manager")]
    public class SoundEmissionManager : MonoBehaviour
    {
        static SoundEmissionManager _instance;

        static List<SoundListener> _listeners = new();
        // static List<SoundEmitter> _emitters = new();
        static List<SoundEmission> _activeEmissions = new();

        static ushort _emitterIndex;

        #region Initialization
        void OnEnable()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(_instance);
                return;
            }

            _instance = this;

            Initialize();
        }
        void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        void Initialize()
        {
            _listeners = new();
            _activeEmissions = new();
            _emitterIndex = 0;
        }

        internal static void Register(SoundListener listener)
        {
            VerifyInstance();
            if (!_listeners.Contains(listener)) _listeners.Add(listener);   
        }
        internal static void Deregister(SoundListener listener)
        {
            if (_listeners.Contains(listener)) _listeners.Remove(listener);   
        }
        #endregion

        #region Update Behaviour
        void Update()
        {
            if (_instance != this) return;

            HandleListeners();
            HandleEmissions();
        }

        static void HandleListeners()
        {
            foreach (var listener in _listeners)
            {
                HandleListener(listener);
            }
        }
        static void HandleListener(SoundListener listener)
        {
            for (int i = _activeEmissions.Count - 1; i >= 0; i--)
            {
                var emission = _activeEmissions[i];
                var distance = Vector3.Distance(emission.Position(), listener.transform.position);
                var power = GetPower(distance, emission.MaxDistance, emission.RolloffExponent);
                emission.Volume = power;

                if (Time.time >= emission.EndTime)
                {
                    listener.HearSound(emission);
                    listener.StopHearingSound(emission);
                    continue;
                }

                if (distance > emission.MaxDistance)
                {
                    listener.StopHearingSound(emission);
                    continue;
                }

                listener.HearSound(emission);
            }
        }
        static void HandleEmissions()
        {
            for (int i = _activeEmissions.Count - 1; i >= 0; i--)
            {
                var emission = _activeEmissions[i];

                if (Time.time >= emission.EndTime)
                {
                    _activeEmissions.Remove(emission);
                }
            }
        }
        #endregion

        #region Sound Emission Management
        public static void PlaySound(Func<Vector3> position, float volume = 1f, float maxDistance = 20f, float rolloffExponent = 1.2f, float duration = 0f, string tag = "Default")
        {
            VerifyInstance();
            var emission = new SoundEmission()
            {
                Id = _emitterIndex,
                Position = position,
                Volume = volume,
                MaxDistance = maxDistance,
                RolloffExponent = rolloffExponent,
                EndTime = Time.time + Mathf.Max(duration, 0.005f),
                Tag = tag
            };
            _activeEmissions.Add(emission);
            _emitterIndex++;
        }
        #endregion

        #region Utility
        static float GetPower(float distance, float maxDistance, float rolloffExponent) => Mathf.Pow(1f - (distance / maxDistance), rolloffExponent);
        static void VerifyInstance()
        {
            _instance ??= new GameObject("SoundEmissionManager").AddComponent<SoundEmissionManager>();
        }
        #endregion
    }
}