using UnityEngine;

namespace HunterAllen.AudioTools
{
    [System.Serializable]
    internal struct SoundEmission
    {
        internal Vector3 Position;
        internal float Volume;
        internal float MaxDistance;
        internal float RolloffExponent;
        internal float EndTime;
        internal ushort Id;
    }
}