using System.Collections.Generic;
using UnityEngine;

namespace HunterAllen.AudioTools
{
    public static class AudioZoneConstants
    {
        public const string ZoneTag = "AudioZone";
        public const string MemberTag = "AudioZoneMember";

        public static Dictionary<AudioZoneSurface, float> ReverbCoefficients = new Dictionary<AudioZoneSurface, float>()
        {
            { AudioZoneSurface.None, 0.5f },
            { AudioZoneSurface.Generic, 0.3f },
            { AudioZoneSurface.Wood, 0.5f },
            { AudioZoneSurface.RoughStone, 0.4f },
            { AudioZoneSurface.SmoothStone, 0.65f },
            { AudioZoneSurface.Metal, 0.8f },
            { AudioZoneSurface.Carpet, 0.03f },
            { AudioZoneSurface.Dirt, 0.05f },
            { AudioZoneSurface.Flesh, 0.5f },
        };
        public static Dictionary<AudioZoneSurface, float> AbsorptionCoefficients = new Dictionary<AudioZoneSurface, float>()
        {
            { AudioZoneSurface.None, 0f },
            { AudioZoneSurface.Generic, 0.7f },
            { AudioZoneSurface.Wood, 0.25f },
            { AudioZoneSurface.RoughStone, 0.7f },
            { AudioZoneSurface.SmoothStone, 0.8f },
            { AudioZoneSurface.Metal, 0.3f },
            { AudioZoneSurface.Carpet, 0.85f },
            { AudioZoneSurface.Dirt, 0.7f },
            { AudioZoneSurface.Flesh, 0.7f },
        };
        public static Dictionary<AudioReverbPreset, AudioReverbProperties> ReverbProperties = new Dictionary<AudioReverbPreset, AudioReverbProperties>()
        {
            {
                AudioReverbPreset.Off, new AudioReverbProperties()
                {
                    DryLevel = 0f,
                    Room = -10000f,
                    RoomHF = -10000f,
                    RoomLF = 0f,
                    DecayTime = 1f,
                    DecayHFRatio = 1f,
                    ReflectionsLevel = -2602f,
                    ReflectionsDelay = 0f,
                    ReverbLevel = 200f,
                    ReverbDelay = 0.011f,
                    HFReference = 5000f,
                    LFReference = 250f,
                    Diffusion = 0f,
                    Density = 0f
                }
            },
            {
                AudioReverbPreset.Generic, new AudioReverbProperties()
                {
                    DryLevel = 0f,
                    Room = -1000f,
                    RoomHF = -100f,
                    RoomLF = 0f,
                    DecayTime = 1.49f,
                    DecayHFRatio = 0.83f,
                    ReflectionsLevel = -2602f,
                    ReflectionsDelay = 0f,
                    ReverbLevel = 200f,
                    ReverbDelay = 0.011f,
                    HFReference = 5000f,
                    LFReference = 250f,
                    Diffusion = 100f,
                    Density = 100f
                }
            },
            {
                AudioReverbPreset.PaddedCell, new AudioReverbProperties()
                {
                    DryLevel = 0f,
                    Room = -1000f,
                    RoomHF = -6000f,
                    RoomLF = 0f,
                    DecayTime = 0.17f,
                    DecayHFRatio = 0.1f,
                    ReflectionsLevel = -1204f,
                    ReflectionsDelay = 0f,
                    ReverbLevel = 207f,
                    ReverbDelay = 0.002f,
                    HFReference = 5000f,
                    LFReference = 250f,
                    Diffusion = 100f,
                    Density = 100f
                }
            },
            {
                AudioReverbPreset.Room, new AudioReverbProperties()
                {
                    DryLevel = 0f,
                    Room = -1000f,
                    RoomHF = -454f,
                    RoomLF = 0f,
                    DecayTime = 0.4f,
                    DecayHFRatio = 0.83f,
                    ReflectionsLevel = -1646f,
                    ReflectionsDelay = 0f,
                    ReverbLevel = 53f,
                    ReverbDelay = 0.003f,
                    HFReference = 5000f,
                    LFReference = 250f,
                    Diffusion = 100f,
                    Density = 100f
                }
            },
            {
                AudioReverbPreset.Bathroom, new AudioReverbProperties()
                {
                    DryLevel = 0f,
                    Room = -1000f,
                    RoomHF = -1200f,
                    RoomLF = 0f,
                    DecayTime = 1.49f,
                    DecayHFRatio = 0.54f,
                    ReflectionsLevel = -370f,
                    ReflectionsDelay = 0f,
                    ReverbLevel = 1030f,
                    ReverbDelay = 0.011f,
                    HFReference = 5000f,
                    LFReference = 250f,
                    Diffusion = 100f,
                    Density = 60f
                }
            },
            {
                AudioReverbPreset.Livingroom, new AudioReverbProperties()
                {
                    DryLevel = 0f,
                    Room = -1000f,
                    RoomHF = -6000f,
                    RoomLF = 0f,
                    DecayTime = 0.5f,
                    DecayHFRatio = 0.1f,
                    ReflectionsLevel = -1376f,
                    ReflectionsDelay = 0f,
                    ReverbLevel = -1104f,
                    ReverbDelay = 0.004f,
                    HFReference = 5000f,
                    LFReference = 250f,
                    Diffusion = 100f,
                    Density = 100f
                }
            },
            {
                AudioReverbPreset.Stoneroom, new AudioReverbProperties()
                {
                    DryLevel = 0f,
                    Room = -1000f,
                    RoomHF = -300f,
                    RoomLF = 0f,
                    DecayTime = 2.31f,
                    DecayHFRatio = 0.64f,
                    ReflectionsLevel = -711f,
                    ReflectionsDelay = 0f,
                    ReverbLevel = 83f,
                    ReverbDelay = 0.017f,
                    HFReference = 5000f,
                    LFReference = 250f,
                    Diffusion = 100f,
                    Density = 100f
                }
            },
            {
                AudioReverbPreset.Auditorium, new AudioReverbProperties()
                {
                    DryLevel = 0f,
                    Room = -1000f,
                    RoomHF = -476f,
                    RoomLF = 0f,
                    DecayTime = 4.32f,
                    DecayHFRatio = 0.59f,
                    ReflectionsLevel = -789f,
                    ReflectionsDelay = 0f,
                    ReverbLevel = -289f,
                    ReverbDelay = 0.03f,
                    HFReference = 5000f,
                    LFReference = 250f,
                    Diffusion = 100f,
                    Density = 100f
                }
            },
            {
                AudioReverbPreset.Concerthall, new AudioReverbProperties()
                {
                    DryLevel = 0f,
                    Room = -1000f,
                    RoomHF = -500f,
                    RoomLF = 0f,
                    DecayTime = 3.92f,
                    DecayHFRatio = 0.7f,
                    ReflectionsLevel = -1230f,
                    ReflectionsDelay = 0f,
                    ReverbLevel = -2f,
                    ReverbDelay = 0.029f,
                    HFReference = 5000f,
                    LFReference = 250f,
                    Diffusion = 100f,
                    Density = 100f
                }
            },
            {
                AudioReverbPreset.Cave, new AudioReverbProperties()
                {
                    DryLevel = 0f,
                    Room = -1000f,
                    RoomHF = 0f,
                    RoomLF = 0f,
                    DecayTime = 2.91f,
                    DecayHFRatio = 1.3f,
                    ReflectionsLevel = -602f,
                    ReflectionsDelay = 0f,
                    ReverbLevel = -302f,
                    ReverbDelay = 0.022f,
                    HFReference = 5000f,
                    LFReference = 250f,
                    Diffusion = 100f,
                    Density = 100f
                }
            },
            {
                AudioReverbPreset.Arena, new AudioReverbProperties()
                {
                    DryLevel = 0f,
                    Room = -1000f,
                    RoomHF = -698f,
                    RoomLF = 0f,
                    DecayTime = 7.24f,
                    DecayHFRatio = 0.33f,
                    ReflectionsLevel = -1166f,
                    ReflectionsDelay = 0f,
                    ReverbLevel = 16f,
                    ReverbDelay = 0.03f,
                    HFReference = 5000f,
                    LFReference = 250f,
                    Diffusion = 100f,
                    Density = 100f
                }
            },
            {
                AudioReverbPreset.Hangar, new AudioReverbProperties()
                {
                    DryLevel = 0f,
                    Room = -1000f,
                    RoomHF = -1000f,
                    RoomLF = 0f,
                    DecayTime = 10.05f,
                    DecayHFRatio = 0.23f,
                    ReflectionsLevel = -602f,
                    ReflectionsDelay = 0f,
                    ReverbLevel = 198f,
                    ReverbDelay = 0.03f,
                    HFReference = 5000f,
                    LFReference = 250f,
                    Diffusion = 100f,
                    Density = 100f
                }
            },
            {
                AudioReverbPreset.CarpetedHallway, new AudioReverbProperties()
                {
                    DryLevel = 0f,
                    Room = -1000f,
                    RoomHF = -4000f,
                    RoomLF = 0f,
                    DecayTime = 0.3f,
                    DecayHFRatio = 0.1f,
                    ReflectionsLevel = -1831f,
                    ReflectionsDelay = 0f,
                    ReverbLevel = -1630f,
                    ReverbDelay = 0.03f,
                    HFReference = 5000f,
                    LFReference = 250f,
                    Diffusion = 100f,
                    Density = 100f
                }
            },
            {
                AudioReverbPreset.Hallway, new AudioReverbProperties()
                {
                    DryLevel = 0f,
                    Room = -1000f,
                    RoomHF = -300f,
                    RoomLF = 0f,
                    DecayTime = 1.49f,
                    DecayHFRatio = 0.59f,
                    ReflectionsLevel = -1219f,
                    ReflectionsDelay = 0f,
                    ReverbLevel = 441f,
                    ReverbDelay = 0.011f,
                    HFReference = 5000f,
                    LFReference = 250f,
                    Diffusion = 100f,
                    Density = 100f
                }
            },
            {
                AudioReverbPreset.StoneCorridor, new AudioReverbProperties()
                {
                    DryLevel = 0f,
                    Room = -1000f,
                    RoomHF = -237f,
                    RoomLF = 0f,
                    DecayTime = 2.7f,
                    DecayHFRatio = 0.79f,
                    ReflectionsLevel = -1214f,
                    ReflectionsDelay = 0f,
                    ReverbLevel = 395f,
                    ReverbDelay = 0.02f,
                    HFReference = 5000f,
                    LFReference = 250f,
                    Diffusion = 100f,
                    Density = 100f
                }
            },
            {
                AudioReverbPreset.Alley, new AudioReverbProperties()
                {
                    DryLevel = 0f,
                    Room = -1000f,
                    RoomHF = -270f,
                    RoomLF = 0f,
                    DecayTime = 1.49f,
                    DecayHFRatio = 0.86f,
                    ReflectionsLevel = -1204f,
                    ReflectionsDelay = 0f,
                    ReverbLevel = -4f,
                    ReverbDelay = 0.011f,
                    HFReference = 5000f,
                    LFReference = 250f,
                    Diffusion = 100f,
                    Density = 100f
                }
            },
            {
                AudioReverbPreset.Forest, new AudioReverbProperties()
                {
                    DryLevel = 0f,
                    Room = -1000f,
                    RoomHF = -3300f,
                    RoomLF = 0f,
                    DecayTime = 1.49f,
                    DecayHFRatio = 0.54f,
                    ReflectionsLevel = -2560f,
                    ReflectionsDelay = 0f,
                    ReverbLevel = -229f,
                    ReverbDelay = 0.088f,
                    HFReference = 5000f,
                    LFReference = 250f,
                    Diffusion = 79f,
                    Density = 100f
                }
            },
            {
                AudioReverbPreset.City, new AudioReverbProperties()
                {
                    DryLevel = 0f,
                    Room = -1000f,
                    RoomHF = -800f,
                    RoomLF = 0f,
                    DecayTime = 1.49f,
                    DecayHFRatio = 0.67f,
                    ReflectionsLevel = -2273f,
                    ReflectionsDelay = 0f,
                    ReverbLevel = -1691f,
                    ReverbDelay = 0.011f,
                    HFReference = 5000f,
                    LFReference = 250f,
                    Diffusion = 50f,
                    Density = 100f
                }
            },
            {
                AudioReverbPreset.Mountains, new AudioReverbProperties()
                {
                    DryLevel = 0f,
                    Room = -1000f,
                    RoomHF = -2500f,
                    RoomLF = 0f,
                    DecayTime = 1.49f,
                    DecayHFRatio = 0.21f,
                    ReflectionsLevel = -2780f,
                    ReflectionsDelay = 0f,
                    ReverbLevel = -1434f,
                    ReverbDelay = 0.1f,
                    HFReference = 5000f,
                    LFReference = 250f,
                    Diffusion = 27f,
                    Density = 100f
                }
            },
            {
                AudioReverbPreset.Quarry, new AudioReverbProperties()
                {
                    DryLevel = 0f,
                    Room = -1000f,
                    RoomHF = -1000f,
                    RoomLF = 0f,
                    DecayTime = 1.49f,
                    DecayHFRatio = 0.83f,
                    ReflectionsLevel = -10000f,
                    ReflectionsDelay = 0f,
                    ReverbLevel = 500f,
                    ReverbDelay = 0.025f,
                    HFReference = 5000f,
                    LFReference = 250f,
                    Diffusion = 100f,
                    Density = 100f
                }
            },
            {
                AudioReverbPreset.Plain, new AudioReverbProperties()
                {
                    DryLevel = 0f,
                    Room = -1000f,
                    RoomHF = -2000f,
                    RoomLF = 0f,
                    DecayTime = 1.49f,
                    DecayHFRatio = 0.5f,
                    ReflectionsLevel = -2466f,
                    ReflectionsDelay = 0f,
                    ReverbLevel = -1926f,
                    ReverbDelay = 0.1f,
                    HFReference = 5000f,
                    LFReference = 250f,
                    Diffusion = 21f,
                    Density = 100f
                }
            },
            {
                AudioReverbPreset.ParkingLot, new AudioReverbProperties()
                {
                    DryLevel = 0f,
                    Room = -1000f,
                    RoomHF = 0f,
                    RoomLF = 0f,
                    DecayTime = 1.65f,
                    DecayHFRatio = 1.5f,
                    ReflectionsLevel = -1363f,
                    ReflectionsDelay = 0f,
                    ReverbLevel = -1153f,
                    ReverbDelay = 0.012f,
                    HFReference = 5000f,
                    LFReference = 250f,
                    Diffusion = 100f,
                    Density = 100f
                }
            },
            {
                AudioReverbPreset.SewerPipe, new AudioReverbProperties()
                {
                    DryLevel = 0f,
                    Room = -1000f,
                    RoomHF = -1000f,
                    RoomLF = 0f,
                    DecayTime = 2.81f,
                    DecayHFRatio = 0.14f,
                    ReflectionsLevel = 429f,
                    ReflectionsDelay = 0f,
                    ReverbLevel = 1023f,
                    ReverbDelay = 0.021f,
                    HFReference = 5000f,
                    LFReference = 250f,
                    Diffusion = 80f,
                    Density = 60f
                }
            },
            {
                AudioReverbPreset.Underwater, new AudioReverbProperties()
                {
                    DryLevel = 0f,
                    Room = -1000f,
                    RoomHF = -4000f,
                    RoomLF = 0f,
                    DecayTime = 1.49f,
                    DecayHFRatio = 0.1f,
                    ReflectionsLevel = -449f,
                    ReflectionsDelay = 0f,
                    ReverbLevel = 1700f,
                    ReverbDelay = 0.011f,
                    HFReference = 5000f,
                    LFReference = 250f,
                    Diffusion = 100f,
                    Density = 100f
                }
            },
            {
                AudioReverbPreset.Drugged, new AudioReverbProperties()
                {
                    DryLevel = 0f,
                    Room = -1000f,
                    RoomHF = 0f,
                    RoomLF = 0f,
                    DecayTime = 8.39f,
                    DecayHFRatio = 1.39f,
                    ReflectionsLevel = -115f,
                    ReflectionsDelay = 0f,
                    ReverbLevel = 985f,
                    ReverbDelay = 0.03f,
                    HFReference = 5000f,
                    LFReference = 250f,
                    Diffusion = 100f,
                    Density = 100f
                }
            },
            {
                AudioReverbPreset.Dizzy, new AudioReverbProperties()
                {
                    DryLevel = 0f,
                    Room = -1000f,
                    RoomHF = -400f,
                    RoomLF = 0f,
                    DecayTime = 17.23f,
                    DecayHFRatio = 0.56f,
                    ReflectionsLevel = -1713f,
                    ReflectionsDelay = 0f,
                    ReverbLevel = -613f,
                    ReverbDelay = 0.03f,
                    HFReference = 5000f,
                    LFReference = 250f,
                    Diffusion = 100f,
                    Density = 100f
                }
            },
            {
                AudioReverbPreset.Psychotic, new AudioReverbProperties()
                {
                    DryLevel = 0f,
                    Room = -1000f,
                    RoomHF = -151f,
                    RoomLF = 0f,
                    DecayTime = 7.56f,
                    DecayHFRatio = 0.91f,
                    ReflectionsLevel = -626f,
                    ReflectionsDelay = 0f,
                    ReverbLevel = 774f,
                    ReverbDelay = 0.03f,
                    HFReference = 5000f,
                    LFReference = 250f,
                    Diffusion = 100f,
                    Density = 100f
                }
            }
        };
    }
    public enum AudioZoneSurface
    {
        None,
        Generic,
        Wood,
        RoughStone,
        SmoothStone,
        Metal,
        Carpet,
        Dirt,
        Flesh,
    }
    [System.Serializable]
    public struct AudioReverbProperties
    {
        public float DryLevel;
        public float Room;
        public float RoomHF;
        public float RoomLF;
        public float DecayTime;
        public float DecayHFRatio;
        public float ReflectionsLevel;
        public float ReflectionsDelay;
        public float ReverbLevel;
        public float ReverbDelay;
        public float HFReference;
        public float LFReference;
        public float Diffusion;
        public float Density;

        public static AudioReverbProperties operator +(AudioReverbProperties left, AudioReverbProperties right)
        {
            return new AudioReverbProperties
            {
                DryLevel = left.DryLevel + right.DryLevel,
                Room = left.Room + right.Room,
                RoomHF = left.RoomHF + right.RoomHF,
                RoomLF = left.RoomLF + right.RoomLF,
                DecayTime = left.DecayTime + right.DecayTime,
                DecayHFRatio = left.DecayHFRatio + right.DecayHFRatio,
                ReflectionsLevel = left.ReflectionsLevel + right.ReflectionsLevel,
                ReflectionsDelay = left.ReflectionsDelay + right.ReflectionsDelay,
                ReverbLevel = left.ReverbLevel + right.ReverbLevel,
                ReverbDelay = left.ReverbDelay + right.ReverbDelay,
                HFReference = left.HFReference + right.HFReference,
                LFReference = left.LFReference + right.LFReference,
                Diffusion = left.Diffusion + right.Diffusion,
                Density = left.Density + right.Density,
            };
        }
        public static AudioReverbProperties operator *(AudioReverbProperties left, float right)
        {
            return new AudioReverbProperties
            {
                DryLevel = left.DryLevel * right,
                Room = left.Room * right,
                RoomHF = left.RoomHF * right,
                RoomLF = left.RoomLF * right,
                DecayTime = left.DecayTime * right,
                DecayHFRatio = left.DecayHFRatio * right,
                ReflectionsLevel = left.ReflectionsLevel * right,
                ReflectionsDelay = left.ReflectionsDelay * right,
                ReverbLevel = left.ReverbLevel * right,
                ReverbDelay = left.ReverbDelay * right,
                HFReference = left.HFReference * right,
                LFReference = left.LFReference * right,
                Diffusion = left.Diffusion * right,
                Density = left.Density * right,
            };
        }
        public static AudioReverbProperties operator /(AudioReverbProperties left, float right)
        {
            return new AudioReverbProperties
            {
                DryLevel = left.DryLevel / right,
                Room = left.Room / right,
                RoomHF = left.RoomHF / right,
                RoomLF = left.RoomLF / right,
                DecayTime = left.DecayTime / right,
                DecayHFRatio = left.DecayHFRatio / right,
                ReflectionsLevel = left.ReflectionsLevel / right,
                ReflectionsDelay = left.ReflectionsDelay / right,
                ReverbLevel = left.ReverbLevel / right,
                ReverbDelay = left.ReverbDelay / right,
                HFReference = left.HFReference / right,
                LFReference = left.LFReference / right,
                Diffusion = left.Diffusion / right,
                Density = left.Density / right,
            };
        }
        public static AudioReverbProperties Zero
        {
            get => new AudioReverbProperties()
            {
                DryLevel = 0f,
                Room = 0,
                RoomHF = 0,
                RoomLF = 0,
                DecayTime = 0,
                DecayHFRatio = 0,
                ReflectionsLevel = 0,
                ReflectionsDelay = 0,
                ReverbLevel = 0,
                ReverbDelay = 0,
                HFReference = 0f,
                LFReference = 0f,
                Diffusion = 0,
                Density = 0,
            };
        }
    }
    public struct AudioZoneBlend
    {
        // public AudioReverbProperties Reverb;
        public float AbsorptionCoefficient;
        public float ReverbCoefficient;
        public float Size;
        public float Blend;
        public int Priority;
    }
    public static class AudioExtensions
    {
        public static AudioReverbProperties Lerp(this AudioReverbProperties self, AudioReverbProperties other, float amount)
        {
            if (amount == 1) return other;
            else if (amount == 0) return self;

            return new AudioReverbProperties
            {
                DryLevel = Mathf.Lerp(self.DryLevel, other.DryLevel, amount),
                Room = Mathf.Lerp(self.Room, other.Room, amount),
                RoomHF = Mathf.Lerp(self.RoomHF, other.RoomHF, amount),
                RoomLF = Mathf.Lerp(self.RoomLF, other.RoomLF, amount),
                DecayTime = Mathf.Lerp(self.DecayTime, other.DecayTime, amount),
                DecayHFRatio = Mathf.Lerp(self.DecayHFRatio, other.DecayHFRatio, amount),
                ReflectionsLevel = Mathf.Lerp(self.ReflectionsLevel, other.ReflectionsLevel, amount),
                ReflectionsDelay = Mathf.Lerp(self.ReflectionsDelay, other.ReflectionsDelay, amount),
                ReverbLevel = Mathf.Lerp(self.ReverbLevel, other.ReverbLevel, amount),
                ReverbDelay = Mathf.Lerp(self.ReverbDelay, other.ReverbDelay, amount),
                HFReference = Mathf.Lerp(self.HFReference, other.HFReference, amount),
                LFReference = Mathf.Lerp(self.LFReference, other.LFReference, amount),
                Diffusion = Mathf.Lerp(self.Diffusion, other.Diffusion, amount),
                Density = Mathf.Lerp(self.Density, other.Density, amount)
            };
        }
    }
}