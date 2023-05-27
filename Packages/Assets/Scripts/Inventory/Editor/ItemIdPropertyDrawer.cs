using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace LinkedSquad.InventorySystem.Editors
{
    public class IdArgument
    {
        public SerializedProperty Property;
        public string Id;
    }

    [CustomPropertyDrawer(typeof(ItemId))]
    public class ItemIdPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var idProp = property.FindPropertyRelative("Id");
            var prefixedPosition = EditorGUI.PrefixLabel(position, new GUIContent(label));

            var dropDownPos = prefixedPosition;
            dropDownPos.width = prefixedPosition.width * 0.2f;

            var valuePos = dropDownPos;
            valuePos.x = dropDownPos.x + dropDownPos.width;
            valuePos.width = prefixedPosition.width - dropDownPos.width;

            if (EditorGUI.DropdownButton(prefixedPosition, new GUIContent(idProp.stringValue), FocusType.Keyboard))
            {
                DisplayItemsMenu(idProp);
            }
        }

        private IEnumerable<string> LoadItemIds()
        {
            var items = Resources.LoadAll<Item>(ItemDisplayManager.ITEMS_PATH);

            return items.Select(i => i.id);
        }

        private void DisplayItemsMenu(SerializedProperty property)
        {
            var items = LoadItemIds();

            GenericMenu menu = new GenericMenu();

            var currentId = property.stringValue;

            {
                var itemIdArg = new IdArgument()
                {
                    Property = property,
                    Id = string.Empty
                };

                menu.AddItem(new GUIContent("Empty String"), string.Equals(currentId, string.Empty), OnItemSelected, itemIdArg);
            }

            foreach (var item in items)
            {
                var itemIdArg = new IdArgument()
                {
                    Property = property,
                    Id = item
                };

                menu.AddItem(new GUIContent(item), string.Equals(currentId, item), OnItemSelected, itemIdArg);
            }

            menu.ShowAsContext();
        }

        private void OnItemSelected(object item)
        {
            if (item == null)
                return;

            var itemIdProp = (IdArgument)item;

            itemIdProp.Property.stringValue = itemIdProp.Id;
            itemIdProp.Property.serializedObject.ApplyModifiedProperties();
        }
    }
}