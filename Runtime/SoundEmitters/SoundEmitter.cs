using UnityEngine;

namespace HunterAllen.AudioTools
{
    [AddComponentMenu("Hunter Allen/Audio Tools/Emissions/Sound Emitter")]
    public class SoundEmitter : MonoBehaviour
    {
        public float Volume = 1f;
        public float MaxDistance = 25f;
        public float RolloffExponent = 1.2f;
        public float Duration = 0f;
        public string Tag = "Default";

        #if UNITY_6000_0_OR_NEWER
        string _id => GetEntityId().ToString();
        #else
        string _id => GetInstanceID().ToString();
        #endif
        
        [ContextMenu("Emit Sound")]
        public void EmitSound()
        {
            SoundEmissionManager.PlaySound(_id, () => transform.position, Volume, MaxDistance, RolloffExponent, Duration, Tag);
        }
        public void EmitSound(bool dontMove = false, float? volume = null, float? maxDistance = null, float? rolloffExponent = null, float? duration = null)
        {
            var position = transform.position;
            SoundEmissionManager.PlaySound(
                _id,
                () => dontMove ? position : transform.position,
                (float)(volume ?? Volume),
                (float)(maxDistance ?? MaxDistance),
                (float)(rolloffExponent ?? RolloffExponent),
                (float)(duration ?? Duration),
                Tag);
        }
    }
}