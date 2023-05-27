using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RequiredAttribute))]
public class RequiredAttributePropertyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!property.boolValue)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        var target = property.serializedObject.targetObject as MonoBehaviour;
        var required = attribute as RequiredAttribute;

        var hasComponent = target.GetComponent(required.RequiredType) != null;

        if(hasComponent)
            return EditorGUI.GetPropertyHeight(property, label, true);
        else
        {
            return EditorGUI.GetPropertyHeight(property, label, true) + 20f;
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if(!property.boolValue)
        {
            EditorGUI.PropertyField(position, property, label, true);
            return;
        }

        var target = property.serializedObject.targetObject as MonoBehaviour;
        var required = attribute as RequiredAttribute;

        var hasComponent = target.GetComponent(required.RequiredType) != null;

        if (hasComponent)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
        else
        {
            position.height -= 20f;

            var pos1 = position;
            var pos2 = position;
            pos2.y += 20f;

            var errorStyle = new GUIStyle(EditorStyles.boldLabel);
            errorStyle.normal.textColor = Color.red;

            EditorGUI.LabelField(position, new GUIContent($"Для этой переменной нужен компонент '{required.RequiredType.Name}'"), errorStyle);
            EditorGUI.PropertyField(pos2, property, label, true);
        }
    }
}
