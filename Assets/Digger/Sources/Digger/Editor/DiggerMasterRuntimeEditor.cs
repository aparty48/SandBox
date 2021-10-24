using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Digger
{
    #region DiggerPRO

    [CustomEditor(typeof(DiggerMasterRuntime))]
    public class DiggerMasterRuntimeEditor : Editor
    {
        private DiggerMasterRuntime diggerMasterRuntime;

        public void OnEnable()
        {
            diggerMasterRuntime = (DiggerMasterRuntime) target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.HelpBox("This script allows Digger to work at runtime, for real-time / in-game digging.\n\n" +
                                    "It has a public method named 'Modify' that you must call from your scripts to edit the terrain.\n\n" +
                                    "See 'DiggerRuntimeUsageExample.cs' in Assets/Digger/Demo for a working example.", MessageType.Info);

            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Runtime Options", EditorStyles.boldLabel);

            diggerMasterRuntime.BufferSize = EditorGUILayout.IntField("Buffer size", diggerMasterRuntime.BufferSize);
            EditorGUILayout.HelpBox("The buffer size is the maximum number of asynchronous modifications that can be pending. This is used " +
                                    "when you call ModifyAsyncBuffured method of DiggerMasterRuntime. It is recommended to keep it low, otherwise " +
                                    "modifications may happen a long time after the player actually did it, leading to bad gameplay. In most cases, a buffer " +
                                    "size of 1 is the best option.", MessageType.Info);

            EditorGUILayout.Space();
            
            diggerMasterRuntime.EnablePersistence = EditorGUILayout.ToggleLeft("Enable persistence features at runtime", diggerMasterRuntime.EnablePersistence);
            if (diggerMasterRuntime.EnablePersistence) {
                EditorGUILayout.HelpBox("Persistence at runtime is enabled.\n\n" +
                                        "You will be able to use the DiggerMasterRuntime.Persist method.\n\n" +
                                        "Note: this may have a (very) little impact on performance to keep track of what needs to be persisted.", MessageType.Info);
            } else {
                EditorGUILayout.HelpBox("Persistence at runtime is disabled.\n\n" +
                                        "You will NOT be able to use the DiggerMasterRuntime.Persist method (it will do nothing).", MessageType.Warning);
            }

            if (EditorGUI.EndChangeCheck()) {
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
        }
    }

    #endregion
}