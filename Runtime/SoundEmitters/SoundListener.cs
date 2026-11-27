using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HunterAllen.AudioTools
{
    [AddComponentMenu("Hunter Allen/Audio Tools/Emissions/Sound Emission Listener")]
    public class SoundListener : MonoBehaviour
    {
        #if UNITY_EDITOR
        [SerializeField]
        bool _drawGizmos;
        #endif
        
        public IEnumerable<SoundEmission> ActiveEmissions => CurrentEmissions.Values;
        
        internal Dictionary<ushort, SoundEmission> CurrentEmissions { get; private set; } = new();

        public event Action<SoundEmission> OnHearSound;
        public event Action<SoundEmission> OnStopHearingSound;

        void OnEnable()
        {
            SoundEmissionManager.Register(this);
        }
        void OnDisable()
        {
            SoundEmissionManager.Deregister(this);
        }

        internal void HearSound(SoundEmission emission)
        {
            if (CurrentEmissions.ContainsKey(emission.SoundId))
            {
                CurrentEmissions[emission.SoundId] = emission;
            }
            else
            {
                CurrentEmissions.Add(emission.SoundId, emission);
                OnHearSound?.Invoke(emission);
            }
        }
        internal void StopHearingSound(SoundEmission emission)
        {
            if (!CurrentEmissions.ContainsKey(emission.SoundId)) return;
            CurrentEmissions.Remove(emission.SoundId);
            OnStopHearingSound?.Invoke(emission);
        }

        #if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!_drawGizmos) return;
            foreach (var kvp in CurrentEmissions)
            {
                var e = kvp.Value;
                Gizmos.color = Color.Lerp(Color.blue, Color.green, e.Volume);

                Gizmos.DrawWireSphere(e.Position(), Mathf.Lerp(0.5f, 0.6f, e.Volume));
            }
        }
        #endif
    }
}