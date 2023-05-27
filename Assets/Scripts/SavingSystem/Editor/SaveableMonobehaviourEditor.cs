using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Linq;
using Dioinecail.MonoBehaviourEditor.Editors;
using System.Collections.Generic;

namespace LinkedSquad.SavingSystem.Data.Editors
{
    public class SaveableField
    {
        public FieldInfo field;
        public object parent;
    }

    public class SaveableMonobehaviourEditor : ICustomEditor
    {
        public EditorType Type => EditorType.Before;

        private List<SaveableField> m_guidFields;
        private bool m_guidsRequireRegenerating => m_guidFields != null && m_guidFields.Count > 0;
        private SerializedObject m_target;



        public void OnEnable(SerializedObject target)
        {
            m_target = target;
            m_guidFields = CheckGuids();

            if (m_guidsRequireRegenerating)
            {
                GenerateGuids();

                m_guidFields = CheckGuids();
            }
        }

        public void OnInspectorGUI(SerializedObject target)
        {
            if (m_guidsRequireRegenerating)
            {
                if (GUILayout.Button("Found saveable fields with no Guid. Press to Generate"))
                {
                    Undo.RecordObject(target.targetObject, "Generate GUIDs");

                    GenerateGuids();
                    m_guidFields = CheckGuids();
                }
            }
        }

        private List<SaveableField> CheckGuids()
        {
            var foundFields = new List<SaveableField>();

            var targetObject = m_target.targetObject;
            var fields = targetObject.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                try
                {
                    var fieldValue = field.GetValue(targetObject);

                    var recursiveFields = CheckGuidsRecursive(field, fieldValue, 0);

                    if (recursiveFields != null)
                        foundFields.AddRange(recursiveFields);
                }
                catch (TargetException)
                {
                    Debug.LogError($"[{nameof(SaveableMonobehaviourEditor)}.CheckGuids] Trying to get a guid from null field. ParentField:'{m_target.targetObject.name}.{field.Name}'");
                }
            }

            return foundFields;
        }

        private List<SaveableField> CheckGuidsRecursive(FieldInfo parentField, object parentFieldValue, int recursiveIndex)
        {
            if (recursiveIndex > 7)
                return null;

            var foundFields = new List<SaveableField>();

            var guidField = parentField.FieldType.GetField("Guid", BindingFlags.Instance | BindingFlags.Public);

            if(guidField != null)
            {
                try
                {
                    var guid = (string)guidField.GetValue(parentFieldValue);

                    if (string.IsNullOrEmpty(guid) ||
                        guid.Equals(Guid.Empty.ToString()))
                    {
                        foundFields.Add(new SaveableField()
                        {
                            parent = parentFieldValue,
                            field = guidField
                        });
                    }
                }
                catch(TargetException)
                {
                    Debug.LogError($"[{nameof(SaveableMonobehaviourEditor)}.CheckGuidsRecursive] Trying to get a guid from null field. ParentField:'{m_target.targetObject.name}.{parentField.Name}'");
                }
            }

            var fields = parentField.FieldType
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if(fields.Count() > 0)
            {
                foreach (var field in fields)
                {
                    if(parentFieldValue != null)
                    {
                        var fieldValue = field.GetValue(parentFieldValue);

                        var recursiveFields = CheckGuidsRecursive(field, fieldValue, recursiveIndex + 1);

                        if (recursiveFields != null)
                            foundFields.AddRange(recursiveFields);
                    }
                }
            }

            return foundFields;
        }

        public static List<SaveableField> GetMissingGuidFields(UnityEngine.Object target)
        {
            var foundFields = new List<SaveableField>();

            var fields = target.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                try
                {
                    var fieldValue = field.GetValue(target);

                    var recursiveFields = GetMissingGuidFieldsRecursive(target, field, fieldValue, 0);

                    if (recursiveFields != null)
                        foundFields.AddRange(recursiveFields);
                }
                catch (TargetException)
                {
                    Debug.LogError($"[{nameof(SaveableMonobehaviourEditor)}.CheckGuids] Trying to get a guid from null field. ParentField:'{target.name}.{field.Name}'");
                }
            }

            return foundFields;
        }

        private static List<SaveableField> GetMissingGuidFieldsRecursive(UnityEngine.Object target, FieldInfo parentField, object parentFieldValue, int recursiveIndex)
        {
            if (recursiveIndex > 7)
                return null;

            var foundFields = new List<SaveableField>();

            var guidField = parentField.FieldType.GetField("Guid", BindingFlags.Instance | BindingFlags.Public);

            if (guidField != null)
            {
                try
                {
                    var guid = (string)guidField.GetValue(parentFieldValue);

                    if (string.IsNullOrEmpty(guid))
                    {
                        foundFields.Add(new SaveableField()
                        {
                            parent = parentFieldValue,
                            field = guidField
                        });
                    }
                }
                catch (TargetException)
                {
                    Debug.LogError($"[{nameof(SaveableMonobehaviourEditor)}.CheckGuidsRecursive] Trying to get a guid from null field. ParentField:'{target.name}.{parentField.Name}'");
                }
            }

            var fields = parentField.FieldType
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (fields.Count() > 0)
            {
                foreach (var field in fields)
                {
                    if (parentFieldValue != null)
                    {
                        var fieldValue = field.GetValue(parentFieldValue);

                        var recursiveFields = GetMissingGuidFieldsRecursive(target, field, fieldValue, recursiveIndex + 1);

                        if (recursiveFields != null)
                            foundFields.AddRange(recursiveFields);
                    }
                }
            }

            return foundFields;
        }

        private void GenerateGuids()
        {
            foreach (var field in m_guidFields)
            {
                field.field.SetValue(field.parent, Guid.NewGuid().ToString());
            }

            EditorUtility.SetDirty(m_target.targetObject);
        }

        #region Static Access

        public static void GenerateGuids(UnityEngine.Object target)
        {
            var missingGuids = GetMissingGuidFields(target);

            if(missingGuids != null && missingGuids.Count > 0)
            {
                foreach (var guidField in missingGuids)
                {
                    var field = guidField.field;
                    var parent = guidField.parent;

                    field.SetValue(parent, Guid.NewGuid().ToString());
                }
            }

            EditorUtility.SetDirty(target);
        }

        public static void ClearGuids(UnityEngine.Object target)
        {
            var foundGuids = GetMissingGuidFields(target);

            foreach (var guid in foundGuids)
            {
                var field = guid.field;
                var parent = guid.parent;

                field.SetValue(parent, string.Empty);
            }

            EditorUtility.SetDirty(target);
        }

        #endregion
    }
}