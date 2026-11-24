using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HunterAllen.AudioTools
{
    [AddComponentMenu("Hunter Allen/Audio Tools/Emissions/Sound Emission Listener")]
    public class SoundListener : MonoBehaviour
    {
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
            if (CurrentEmissions.ContainsKey(emission.Id))
            {
                CurrentEmissions[emission.Id] = emission;
            }
            else
            {
                CurrentEmissions.Add(emission.Id, emission);
                OnHearSound?.Invoke(emission);
            }
        }
        internal void StopHearingSound(SoundEmission emission)
        {
            if (!CurrentEmissions.ContainsKey(emission.Id)) return;
            CurrentEmissions.Remove(emission.Id);
            OnStopHearingSound?.Invoke(emission);
        }

        void OnDrawGizmos()
        {
            foreach (var kvp in CurrentEmissions)
            {
                var e = kvp.Value;
                Gizmos.color = Color.Lerp(Color.blue, Color.green, e.Volume);

                Gizmos.DrawWireSphere(e.Position(), Mathf.Lerp(0.2f, 0.6f, e.Volume));
            }
        }
    }
}