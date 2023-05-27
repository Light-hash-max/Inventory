using LinkedSquad.FileSystem;
using LinkedSquad.SavingSystem.Data;
using LinkedSquad.SavingSystem.Data.Editors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace LinkedSquad.SavingSystem.Debugger.Editors
{
    public class SaveDebuggerEditor : EditorWindow
    {
        private class GuidTarget
        {
            public UnityEngine.Object UnityObject;
            public FieldInfo Field;
            public object Target;
        }

        private event Action onSearchValueChanged;

        private Dictionary<string, SaveData> m_Data;
        private static SaveDebuggerEditor m_EditorWindow;
        private Vector2 m_EditorScroll;

        private static float Width => m_EditorWindow.position.width;
        private static float HalfWidth => Width / 2f;

        private string m_SearchValue = string.Empty;
        private string SearchValue
        {
            get => m_SearchValue;
            set
            {
                if (string.Equals(m_SearchValue, value))
                    return;

                m_SearchValue = value;
                onSearchValueChanged?.Invoke();
            }
        }

        private List<GuidTarget> m_guidFields;
        private bool m_guidsRequireRegenerating => m_guidFields != null && m_guidFields.Count > 0;



        [MenuItem("Window/Save Debugger")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            m_EditorWindow = (SaveDebuggerEditor)EditorWindow.GetWindow(typeof(SaveDebuggerEditor));
            m_EditorWindow.titleContent = new GUIContent("Save Debugger");
            m_EditorWindow.position = new Rect(50, 50, 900, 600);
            m_EditorWindow.Show();
        }

        void OnGUI()
        {
            if(GUILayout.Button("Check Objects for missing GUIDs"))
            {
                var sceneObjects = UnityEngine.Object.FindObjectsOfType<SaveableMonobehaviour>(true);
                var foundGuids = new List<GuidTarget>();

                foreach (var obj in sceneObjects)
                {
                    var guids = SaveableMonobehaviourEditor.GetMissingGuidFields(obj);

                    if (guids != null && guids.Count > 0)
                    {
                        foreach (var guid in guids)
                        {
                            foundGuids.Add(new GuidTarget()
                            {
                                UnityObject = obj,
                                Field = guid.field,
                                Target = guid.parent
                            });
                        }
                    }
                }

                m_guidFields = foundGuids;
            }

            if (m_guidsRequireRegenerating)
            {
                if (GUILayout.Button("Generate missing GUIDs"))
                {
                    Undo.RecordObjects(m_guidFields.Select(g => g.UnityObject).ToArray(), "Generate Guids");

                    foreach (var guid in m_guidFields)
                    {
                        var field = guid.Field;
                        var target = guid.Target;

                        field.SetValue(target, System.Guid.NewGuid().ToString());

                        EditorUtility.SetDirty(guid.UnityObject);
                    }
                }
            }

            if (GUILayout.Button("Load Save"))
            {
                var fileManager = new StandaloneFileManager();
                m_Data = (Dictionary<string, SaveData>)fileManager.LoadFile(SaveSystem.SaveFilePath);
            }

            if(GUILayout.Button("Open SaveFile location"))
            {
                var path = Directory.GetParent(SaveSystem.SaveFilePath).FullName;

                path = path.Replace(@"/", @"\");

                System.Diagnostics.Process.Start("explorer.exe", path);
            }

            EditorGUILayout.Separator();

            DrawSearchBar();

            if (m_Data == null)
                return;

            var displayData = m_Data.Where(kvp => kvp.Value.FieldName.ToLower().Contains(m_SearchValue));

            m_EditorScroll = EditorGUILayout.BeginScrollView(m_EditorScroll);

            foreach (var d in displayData)
            {
                GUILayout.BeginVertical(GUILayout.Width(Width - 25f));

                GUILayout.BeginHorizontal(GUILayout.Width(Width - 25f));

                GUILayout.TextField("Guid:", GUILayout.Width(HalfWidth));
                GUILayout.TextField(d.Key);

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                if (d.Value.Data == null)
                    d.Value.Data = new object();

                if(d.Value.Data.GetType() == typeof(float[]))
                {
                    var array = (float[])d.Value.Data;

                    switch (array.Length)
                    {
                        case 2:
                            var vec2 = new Vector2(array[0], array[1]);
                            GUILayout.TextField(d.Value.FieldName, GUILayout.Width(HalfWidth));
                            GUILayout.TextField(vec2.ToString());
                            break;
                        case 3:
                            var vec3 = new Vector3(array[0], array[1], array[2]);
                            GUILayout.TextField(d.Value.FieldName, GUILayout.Width(HalfWidth));
                            GUILayout.TextField(vec3.ToString());
                            break;
                        case 4:
                            var quat = new Quaternion(array[0], array[1], array[2], array[3]);
                            GUILayout.TextField(d.Value.FieldName, GUILayout.Width(HalfWidth));
                            GUILayout.TextField(quat.ToString());
                            break;
                        default:
                            break;
                    }
                }
                else if (d.Value.Data.GetType().IsArray)
                {
                    EditorGUILayout.BeginVertical();

                    var type = d.Value.Data.GetType();
                    var elementType = type.GetElementType();
                    var lengthField = type.GetProperty("Length", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                    var length = (int)lengthField.GetValue(d.Value.Data);

                    var getValueMethod = type
                        .GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
                        .First(m => m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType.Equals(typeof(int)));

                    for (int i = 0; i < length; i++)
                    {
                        var value = getValueMethod.Invoke(d.Value.Data, new object[1] { i });

                        GUILayout.TextField($"{d.Value.FieldName}[{i}]: '{value.ToString()}'");
                    }

                    EditorGUILayout.EndVertical();
                }
                else
                {
                    GUILayout.TextField(d.Value.FieldName, GUILayout.Width(HalfWidth));
                    GUILayout.TextField(d.Value.Data.ToString());
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();

                EditorGUILayout.Separator();
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawSearchBar()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(new GUIContent("Search: "), GUILayout.Width(45f));

            SearchValue = EditorGUILayout.TextField(SearchValue);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
        }
    }
}