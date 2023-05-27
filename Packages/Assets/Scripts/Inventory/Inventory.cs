using LinkedSquad.SavingSystem.Data;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LinkedSquad.InventorySystem
{
    [System.Serializable]
    public class ItemState
    {
        public string Id;
        public string ItemGuid;
        public int UsageCount;

        public static ItemState Empty => new ItemState() { Id = string.Empty, UsageCount = 0, ItemGuid = string.Empty };

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Id))
                return "Empty Item";

            return $"itemGuid: '{ItemGuid}', id: '{Id}', usageCount: '{UsageCount}'";
        }
    }

    public class Inventory : SaveableMonobehaviour
    {
        public static Inventory Instance;

        public event Action<Item[]> OnInventoryUpdated;
        public event Action OnItemFailedPickUp;

        [SerializeField] private Transform itemsUIRoot;
        [SerializeField] private Transform itemContainersRoot;
        [SerializeField] private Transform itemThrowRoot;
        [SerializeField] private ItemCombinationConfig combinationConfig;
        [SerializeField] private AudioSource _audioSource;

        [SerializeField] private float throwPower = 3f;
        [SerializeField] private float _aimingThrowPower = 6f;

        private Item[] _items;
        [SerializeField, HideInNormalInspector] 
        private SaveableArray<ItemState> _savedItems = new SaveableArray<ItemState>();
        private int _itemCount;
        private bool _isAiming;

        public Item[] Items => _items;
        public Transform ItemThrowRoot => itemThrowRoot;
        public float AimingThrowPower => _aimingThrowPower;



        private void Awake()
        {
            Instance = this;

            var items = itemsUIRoot.GetComponentsInChildren<ItemUI>(true);

            _itemCount = items.Length;
            _items = new Item[_itemCount];

            _savedItems.Value = new ItemState[_itemCount];
            _savedItems.OnBeforeDataSaved += HandleItemsSaved;
            _savedItems.OnDataLoaded += HandleItemsLoaded;

            var startingItems = itemContainersRoot.GetComponentsInChildren<Item>(true);

            if (startingItems.Length > 0)
            {
                foreach (var sItem in startingItems)
                {
                    AddItem(sItem);
                }
            }
            else
            {
                OnInventoryUpdated?.Invoke(Items);
            }
        }

        public bool AddItem(Item item)
        {
            for (int i = 0; i < _itemCount; i++)
            {
                if (Items[i] == null)
                {
                    Items[i] = item;

                    item.transform.SetParent(itemContainersRoot);
                    item.transform.localPosition = Vector3.zero;
                    item.gameObject.SetActive(false);

                    InventoryMessageController.Instance.DisplayMessage(item.itemName, 2f);

                    OnInventoryUpdated?.Invoke(Items);
                    return true;
                }
            }

            OnItemFailedPickUp?.Invoke();
            return false;
        }

        public void RemoveItem(int slotIndex, bool isThrow)
        {
            var item = Items[slotIndex];

            Items[slotIndex] = null;

            if (isThrow)
            {
                var throwPosition = itemThrowRoot.position;
                var throwDirection = itemThrowRoot.forward.normalized;

                item.transform.position = throwPosition;
                item.transform.rotation = itemThrowRoot.rotation;
                item.transform.SetParent(null);
                item.gameObject.SetActive(true);

                var throwingPower = _isAiming ? _aimingThrowPower : throwPower;
                var itemRB = item.GetComponent<Rigidbody>();
                var force = throwDirection * (throwingPower / itemRB.mass);

                itemRB.WakeUp();
                itemRB.angularVelocity = Vector3.zero;
                itemRB.velocity = force;
            }

            OnInventoryUpdated?.Invoke(Items);
        }

        public void ClearInventory()
        {
            for (int i = 0; i < Items.Length; i++)
            {
                Items[i] = null;
            }
            OnInventoryUpdated?.Invoke(Items);
        }

        public bool MoveItem(int slotFrom, int slotTo)
        {
            var itemFrom = Items[slotFrom];
            var itemTo = Items[slotTo];

            if(itemFrom != null && itemTo != null)
            {
                // this is a combination attempt
                // try combining items

                var c = combinationConfig.configs;

                if (!combinationConfig.configs.Any(config => Compare(config, itemFrom.id, itemTo.id)))
                {
                    return false;
                }

                // found combinations
                var results = combinationConfig.configs
                    .First(config => Compare(config, itemFrom.id, itemTo.id))
                    .results;

                var combineSound = combinationConfig.configs
                 .First(config => Compare(config, itemFrom.id, itemTo.id))
                 .combineSound;

                RemoveItem(slotFrom, false);
                RemoveItem(slotTo, false);

                _audioSource.PlayOneShot(combineSound);

                foreach (var itemId in results)
                {
                    var item = Instantiate(ItemDisplayManager.GetItem(itemId));

                    AddItem(item);
                }

                return true;
            }

            var tmp = Items[slotTo];
            Items[slotTo] = Items[slotFrom];
            Items[slotFrom] = tmp;

            OnInventoryUpdated?.Invoke(Items);

            return true;
        }

        private bool Compare(CombinationConfig config, string itemA, string itemB)
        {
            var normal = config.itemA == itemA && config.itemB == itemB;
            var reverse = config.itemA == itemB && config.itemB == itemA;

            return normal || reverse;
        }

        private void OnEnable()
        {
            InventoryUI.Instance.OnBeginAiming  += HandleBeginAiming;
            InventoryUI.Instance.OnEndAiming    += HandleEndAiming;
        }

        private void OnDisable()
        {
            InventoryUI.Instance.OnBeginAiming  -= HandleBeginAiming;
            InventoryUI.Instance.OnEndAiming    -= HandleEndAiming;
        }

        private void HandleBeginAiming(PointerEventData eventData)
        {
            _isAiming = true;
        }

        private void HandleEndAiming(PointerEventData eventData)
        {
            _isAiming = false;
        }

        #region Saving Handler

        private void HandleItemsSaved()
        {
            for (int i = 0; i < _itemCount; i++)
            {
                _savedItems.Value[i] = Items[i] != null
                    ? new ItemState() {
                        Id = Items[i].id,
                        UsageCount = Items[i].usageCount,
                        ItemGuid = Items[i].SaveableObject.ObjectGuid
                    }
                    : ItemState.Empty;
            }
        }

        private void HandleItemsLoaded(ItemState[] savedItems)
        {
            var saveableItems = FindObjectsOfType<Item>(true);

            for (int i = 0; i < Items.Length; i++)
            {
                Items[i] = GetItem(savedItems[i], saveableItems);
            }

            OnInventoryUpdated?.Invoke(Items);
        }

        private Item GetItem(ItemState savedItem, Item[] saveableItems)
        {
            if (savedItem.Id == string.Empty)
                return null;

            if(saveableItems.Any(item => savedItem.ItemGuid.Equals(item.SaveableObject.ObjectGuid)))
            {
                var item = saveableItems.First(item => savedItem.ItemGuid.Equals(item.SaveableObject.ObjectGuid));

                item.usageCount.Value = savedItem.UsageCount;
                item.gameObject.SetActive(false);
                item.transform.SetParent(itemContainersRoot);   

                return item;
            }

            var itemInstance = Instantiate(ItemDisplayManager.GetItem(savedItem.Id), itemContainersRoot);
            itemInstance.gameObject.SetActive(false);
            itemInstance.usageCount.Value = savedItem.UsageCount;

            return itemInstance;
        }

        #endregion
    }
}