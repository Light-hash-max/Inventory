using LinkedSquad.SavingSystem.Data;
using LinkedSquad.SavingSystem.Data.Editors;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LinkedSquad.SavingSystem.Debugger
{
    // This script is needed because
    // when you duplicate a gameObject inside the Editor
    // our custom SaveSystem Guids
    // will be duplicated as well
    // so we need to regenerate Guids
    // after we duplicate any gameObject

    [InitializeOnLoad]
    public class SaveGuidsRegenerator
    {
        private static List<int> m_duplicateInstances;

        static SaveGuidsRegenerator()
        {
            EditorApplication.hierarchyWindowItemOnGUI += DebugDuplicateCommand;
        }

        ~SaveGuidsRegenerator()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= DebugDuplicateCommand;
        }

        private static void DebugDuplicateCommand(int instanceID, Rect selectionRect)
        {
            if (Event.current.commandName == "Duplicate")
            {
                m_duplicateInstances = new List<int>();
                 
                EditorApplication.update += EditorTick;
            }
        }

        private static void EditorTick()
        {
            EditorApplication.update -= EditorTick;

            var selection = Selection.gameObjects;

            foreach (var s in selection)
            {
                if (m_duplicateInstances.Contains(s.GetInstanceID()))
                    return;

                m_duplicateInstances.Add(s.GetInstanceID());

                if(s.TryGetComponent<SaveableMonobehaviour>(out var saveable))
                {
                    SaveableMonobehaviourEditor.ClearGuids(saveable);
                    SaveableMonobehaviourEditor.GenerateGuids(saveable);
                }

                Debug.Log($"Selection: {s.name}");
            }
        }
    }
}