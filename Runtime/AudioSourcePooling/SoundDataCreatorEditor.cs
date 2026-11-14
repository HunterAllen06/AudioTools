using UnityEngine;
#if UNITY_6000_0_OR_NEWER
using UnityEngine.Audio;
#endif
using System.Linq;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HunterAllen.AudioTools
{
    public static class SoundDataCreatorEditor
    {
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Create Sound Data")]
        static void CreateSoundData()
        {
            if (Selection.objects.Length == 0) return;
            var selectedAudio = Selection.objects;
#if UNITY_6000_0_OR_NEWER
            string path = AssetDatabase.GetAssetPath(selectedAudio[0].GetEntityId());
#else
            string path = AssetDatabase.GetAssetPath(selectedAudio[0].GetInstanceID());
#endif
            var nameLength = selectedAudio[0].name.Length + path.Split('.').Last().Length + 1;
            path = path.Remove(path.Length - nameLength, nameLength);

            foreach (var audio in selectedAudio)
            {
                var soundData = ScriptableObject.CreateInstance<SoundData>();
                var name = audio.name;

                if (name[0..2] == "S_")
                {
                    name = name.Remove(0, 2);
                }
                else if (name[0..4] == "ARC_")
                {
                    name = name.Remove(0, 4);
                }

                soundData.name = $"SD_{name}";
                soundData.Sound = new Sound()
                {
#if UNITY_6000_0_OR_NEWER
                    Audio = audio as AudioResource ?? audio as AudioClip,
#else
                    Audio = audio as AudioClip,
#endif
                    MaxCount = 2
                };
                AssetDatabase.CreateAsset(soundData, $"{path}SD_{name}.asset");
            }

            SetIds();
            AssetDatabase.SaveAssets();
            Selection.objects = selectedAudio;
            Debug.Log($"Created [{selectedAudio.Length}] Sound Data {(selectedAudio.Length > 1 ? "assets" : "asset")} at {path}. This action is not recorded to the undo stack.");
        }

        public static List<SoundData> SetIds()
        {
            var soundData = UnityEditor.AssetDatabase.FindAssets("t:SoundData")
                .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                .Select(UnityEditor.AssetDatabase.LoadAssetAtPath<SoundData>);

            short id = 0;
            foreach (var sound in soundData)
            {
                sound.SetId(id);
                id++;
            }

            AssetDatabase.SaveAssets();

            return soundData.ToList();
        }
#endif
    }
}
