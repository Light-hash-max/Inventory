using UnityEngine;

namespace LinkedSquad.InventorySystem
{
    [System.Serializable]
    public class CombinationConfig
    {
        public ItemId itemA;
        public ItemId itemB;
        public AudioClip combineSound;

        public ItemId[] results;
    }

    [CreateAssetMenu]
    public class ItemCombinationConfig : ScriptableObject
    {
        public CombinationConfig[] configs;
    }
}