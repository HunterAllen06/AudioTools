using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace HunterAllen.AudioTools.Editor
{
    public static class AudioObjectCreateMenuButtons
    {
        [MenuItem("GameObject/Audio/Hunter Allen/Audio Zone Manager", priority = 1, secondaryPriority = 20)]
        static void CreateAudioZoneManager() => Create<AudioZoneManager>();

        [MenuItem("GameObject/Audio/Hunter Allen/Audio Zone", priority = 2, secondaryPriority = 20)]
        static void CreateAudioZone() => Create<AudioZone>();

        [MenuItem("GameObject/Audio/Hunter Allen/Audio Zone Member", priority = 3, secondaryPriority = 20)]
        static void CreateAudioZoneMember()
        {
            var sphere = Create<SphereCollider>(null, "AudioZoneMember");
            sphere.isTrigger = true;

            var rigidbody = sphere.gameObject.AddComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            
            var audioSource = sphere.gameObject.AddComponent<AudioSource>();

            var audioZoneMember = sphere.gameObject.AddComponent<AudioZoneMember>();

            UnityEditorInternal.ComponentUtility.MoveComponentUp(audioZoneMember);
            UnityEditorInternal.ComponentUtility.MoveComponentUp(audioZoneMember);
            UnityEditorInternal.ComponentUtility.MoveComponentUp(audioZoneMember);
            UnityEditorInternal.ComponentUtility.MoveComponentUp(audioZoneMember);
            UnityEditorInternal.ComponentUtility.MoveComponentUp(audioZoneMember);
            sphere.gameObject.layer = 31;
        }

        [MenuItem("GameObject/Audio/Hunter Allen/Sound Emitter", priority = 4, secondaryPriority = 20)]
        static void CreateSoundEmitter() => Create<SoundEmitter>();

        [MenuItem("GameObject/Audio/Hunter Allen/Sound Listener", priority = 5, secondaryPriority = 20)]
        static void CreateSoundListener() => Create<SoundListener>();

        [MenuItem("GameObject/Audio/Hunter Allen/Sound Player", priority = 6, secondaryPriority = 20)]
        static void CreateSoundPlayer() => Create<SoundPlayer>();

        static T Create<T>(GameObject owner = null, string name = "") where T : Component
        {
            bool hasOwner = owner != null;
            bool hasCustomName = !name.Equals(string.Empty);
            name = !hasCustomName ? typeof(T).Name : name;
            owner ??= new GameObject(name);
            T obj = owner.AddComponent<T>();
            if (!hasOwner) Undo.RegisterCreatedObjectUndo(owner, $"Create {name}");
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            return obj;
        }
    }
}