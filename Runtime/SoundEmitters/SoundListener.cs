using System.Collections.Generic;
using UnityEngine;

namespace HunterAllen.AudioTools
{
    [AddComponentMenu("Hunter Allen/Audio Tools/Emissions/Sound Emission Listener")]
    public class SoundListener : MonoBehaviour
    {
        internal Dictionary<ushort, SoundEmission> CurrentEmissions { get; private set; } = new();

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
            // if (CurrentEmissions.Contains(emission)) return;
            if (CurrentEmissions.ContainsKey(emission.Id)) CurrentEmissions[emission.Id] = emission;
            else CurrentEmissions.Add(emission.Id, emission);
        }
        internal void StopHearingSound(SoundEmission emission)
        {
            if (!CurrentEmissions.ContainsKey(emission.Id)) return;
            CurrentEmissions.Remove(emission.Id);
        }

        void OnDrawGizmos()
        {
            foreach (var kvp in CurrentEmissions)
            {
                var e = kvp.Value;
                Gizmos.color = Color.Lerp(Color.black, Color.yellow, e.Volume);

                Gizmos.DrawWireSphere(e.Position, 0.5f);
            }
        }
    }
}