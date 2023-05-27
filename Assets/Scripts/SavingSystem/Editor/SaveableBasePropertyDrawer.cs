using UnityEngine;
using UnityEditor;

namespace LinkedSquad.SavingSystem.Data.Editors
{
    public class SaveableBasePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueProp = property.FindPropertyRelative("Value");
            var isSaveProp = property.FindPropertyRelative("IsSave");

            var valuePosition = position;
            valuePosition.width = position.width - 40f;

            var isSavePosition = position;
            isSavePosition.x += valuePosition.width + 5f;
            isSavePosition.width = 40f;

            EditorGUI.PropertyField(valuePosition, valueProp, label, false);

            var buttonStyle = new GUIStyle(EditorStyles.toolbarButton);

            if(isSaveProp.boolValue)
            {
                buttonStyle.normal.textColor = Color.green;
                buttonStyle.hover.textColor = new Color(0.5f, 0.8f, 0.6f, 1.0f);
            }
            else
            {
                buttonStyle.normal.textColor = Color.red;
                buttonStyle.hover.textColor = new Color(0.8f, 0.5f, 0.6f, 1.0f);
            }

            if (GUI.Button(isSavePosition, "Save", buttonStyle))
            {
                isSaveProp.boolValue = !isSaveProp.boolValue;
            }
        }
    }

    #region Base Classes and Structs

    [CustomPropertyDrawer(typeof(SaveableBase<float>), true)]
    public class FloatSaveablePropertyDrawer : SaveableBasePropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableBase<double>), true)]
    public class DoubleSaveablePropertyDrawer : SaveableBasePropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableBase<int>), true)]
    public class IntSaveablePropertyDrawer : SaveableBasePropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableBase<uint>), true)]
    public class UIntSaveablePropertyDrawer : SaveableBasePropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableBase<short>), true)]
    public class ShortSaveablePropertyDrawer : SaveableBasePropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableBase<long>), true)]
    public class LongSaveablePropertyDrawer : SaveableBasePropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableBase<ulong>), true)]
    public class ULongSaveablePropertyDrawer : SaveableBasePropertyDrawer
    {
        // because Unity is trash
        // you have to clamp ULong on your own
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueProp = property.FindPropertyRelative("Value");
            var isSaveProp = property.FindPropertyRelative("IsSave");

            var valuePosition = position;
            valuePosition.width = position.width - 40f;

            var isSavePosition = position;
            isSavePosition.x += valuePosition.width + 5f;
            isSavePosition.width = 40f;

            var value = EditorGUI.LongField(valuePosition, label, valueProp.longValue);
            valueProp.longValue = (long)Mathf.Clamp(value, ulong.MinValue, ulong.MaxValue);

            var buttonStyle = new GUIStyle(EditorStyles.toolbarButton);

            if (isSaveProp.boolValue)
            {
                buttonStyle.normal.textColor = Color.green;
                buttonStyle.hover.textColor = new Color(0.5f, 0.8f, 0.6f, 1.0f);
            }
            else
            {
                buttonStyle.normal.textColor = Color.red;
                buttonStyle.hover.textColor = new Color(0.8f, 0.5f, 0.6f, 1.0f);
            }

            if (GUI.Button(isSavePosition, "Save", buttonStyle))
            {
                isSaveProp.boolValue = !isSaveProp.boolValue;
            }
        }
    }

    [CustomPropertyDrawer(typeof(SaveableBase<bool>), true)]
    public class BoolSaveablePropertyDrawer : SaveableBasePropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableBase<char>), true)]
    public class CharSaveablePropertyDrawer : SaveableBasePropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableBase<string>), true)]
    public class StringSaveablePropertyDrawer : SaveableBasePropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableBase<Vector3>), true)]
    public class Vector3SaveablePropertyDrawer : SaveableBasePropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableBase<Vector2>), true)]
    public class Vector2SaveablePropertyDrawer : SaveableBasePropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableBase<Quaternion>), true)]
    public class QuaternionSaveablePropertyDrawer : SaveableBasePropertyDrawer { }

    #endregion

    #region Arrays

    public abstract class SaveableCollectionPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueProp = property.FindPropertyRelative("Value");
            var isSaveProp = property.FindPropertyRelative("IsSave");

            var valuePosition = position;
            valuePosition.width = position.width - 40f;

            var isSavePosition = position;
            isSavePosition.x += valuePosition.width + 5f;
            isSavePosition.width = 40f;

            EditorGUI.PropertyField(valuePosition, valueProp, label, true);

            var buttonStyle = new GUIStyle(EditorStyles.toolbarButton);

            if (isSaveProp.boolValue)
            {
                buttonStyle.normal.textColor = Color.green;
                buttonStyle.hover.textColor = new Color(0.5f, 0.8f, 0.6f, 1.0f);
            }
            else
            {
                buttonStyle.normal.textColor = Color.red;
                buttonStyle.hover.textColor = new Color(0.8f, 0.5f, 0.6f, 1.0f);
            }

            if (GUI.Button(isSavePosition, "Save", buttonStyle))
            {
                isSaveProp.boolValue = !isSaveProp.boolValue;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var valueProp = property.FindPropertyRelative("Value");
            var arraySize = valueProp.arraySize;

            if(valueProp.isExpanded)
            {
                var size = (arraySize > 0)
                    ? base.GetPropertyHeight(property, label) + (arraySize * 20f) + 20f
                    : base.GetPropertyHeight(property, label) + 40f;

                return size;
            }

            return base.GetPropertyHeight(property, label);
        }
    }

    [CustomPropertyDrawer(typeof(SaveableArray<float>))]
    public class SaveableSaveableFloatArrayPropertyDrawer : SaveableCollectionPropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableArray<double>), true)]
    public class SaveableDoubleArrayPropertyDrawer : SaveableCollectionPropertyDrawer { } 

    [CustomPropertyDrawer(typeof(SaveableArray<int>), true)]
    public class SaveableIntArrayPropertyDrawer : SaveableCollectionPropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableArray<uint>), true)]
    public class SaveableUIntArrayPropertyDrawer : SaveableCollectionPropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableArray<short>), true)]
    public class SaveableShortArrayPropertyDrawer : SaveableCollectionPropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableArray<long>), true)]
    public class SaveableLongArrayPropertyDrawer : SaveableCollectionPropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableArray<bool>), true)]
    public class SaveableBoolArrayPropertyDrawer : SaveableCollectionPropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableArray<char>), true)]
    public class SaveableCharArrayPropertyDrawer : SaveableCollectionPropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableArray<string>), true)]
    public class SaveableStringArrayPropertyDrawer : SaveableCollectionPropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableArray<Vector3>), true)]
    public class SaveableVector3ArrayPropertyDrawer : SaveableCollectionPropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableArray<Vector2>), true)]
    public class SaveableVector2ArrayPropertyDrawer : SaveableCollectionPropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableArray<Quaternion>), true)]
    public class SaveableQuaternionArrayPropertyDrawer : SaveableCollectionPropertyDrawer { }

    #endregion

    #region Lists

    [CustomPropertyDrawer(typeof(SaveableList<float>))]
    public class SaveableSaveableFloatListPropertyDrawer : SaveableCollectionPropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableArray<double>), true)]
    public class SaveableDoubleListPropertyDrawer : SaveableCollectionPropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableArray<int>), true)]
    public class SaveableIntListPropertyDrawer : SaveableCollectionPropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableArray<uint>), true)]
    public class SaveableUIntListPropertyDrawer : SaveableCollectionPropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableArray<short>), true)]
    public class SaveableShortListPropertyDrawer : SaveableCollectionPropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableArray<long>), true)]
    public class SaveableLongListPropertyDrawer : SaveableCollectionPropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableArray<bool>), true)]
    public class SaveableBoolListPropertyDrawer : SaveableCollectionPropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableArray<char>), true)]
    public class SaveableCharListPropertyDrawer : SaveableCollectionPropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableArray<string>), true)]
    public class SaveableStringListPropertyDrawer : SaveableCollectionPropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableArray<Vector3>), true)]
    public class SaveableVector3ListPropertyDrawer : SaveableCollectionPropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableArray<Vector2>), true)]
    public class SaveableVector2ListPropertyDrawer : SaveableCollectionPropertyDrawer { }

    [CustomPropertyDrawer(typeof(SaveableArray<Quaternion>), true)]
    public class SaveableQuaternionListPropertyDrawer : SaveableCollectionPropertyDrawer { }

    #endregion
}