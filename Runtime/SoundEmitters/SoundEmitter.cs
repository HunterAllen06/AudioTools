using UnityEngine;

namespace HunterAllen.AudioTools
{
    [AddComponentMenu("Hunter Allen/Audio Tools/Emissions/Sound Emitter")]
    public class SoundEmitter : MonoBehaviour
    {
        [SerializeField] internal float Volume = 1f;
        [SerializeField] internal float MaxDistance = 25f;
        [SerializeField] internal float RolloffExponent = 1.2f;
        [SerializeField] internal float Duration = 0f;
        
        [ContextMenu("Emit Sound")]
        public void EmitSound()
        {
            SoundEmissionManager.PlaySound(transform.position, Volume, MaxDistance, RolloffExponent, Duration);
        }
    }
}