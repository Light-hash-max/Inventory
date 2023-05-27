using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(NameAttribute))]
public class NameAttributePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attr = attribute as NameAttribute;
        var customName = new GUIContent(label);
        customName.text = attr.Name;

        EditorGUI.PropertyField(position, property, customName, true);
    }
}
