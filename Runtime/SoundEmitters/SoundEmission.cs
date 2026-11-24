using System;
using UnityEngine;

namespace HunterAllen.AudioTools
{
    [System.Serializable]
    public struct SoundEmission
    {
        public Func<Vector3> Position;
        public float Volume;
        public float MaxDistance;
        public float RolloffExponent;
        public float EndTime;
        public ushort Id;
        public string Tag;
    }
}