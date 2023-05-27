using System.Linq;
using UnityEngine;

namespace LinkedSquad.InventorySystem
{
    public static class ItemDisplayManager
    {
        private static readonly Item[] _items = Resources.LoadAll<Item>(ITEMS_PATH);

        public const string ITEMS_PATH = "Database/";

        public static Sprite GetItemIcon(string id)
        {
            var sprite = _items.First(item => string.Equals(id, item.id)).inventoryIcon;
            return sprite;
        }

        public static Item GetItem(string id)
        {
            var targetItem = _items.First(item => string.Equals(id, item.id));
            return targetItem;
        }
    }
}