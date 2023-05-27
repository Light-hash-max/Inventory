using LinkedSquad.PlayerControls;
using LinkedSquad.SavingSystem.Data;
using LinkedSquad.Utility;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LinkedSquad.InventorySystem
{
    public class InventoryUI : SaveableMonobehaviour
    {
        public static InventoryUI Instance;

        public event Action<PointerEventData> OnBeginAiming;
        public event Action<PointerEventData> OnAiming;
        public event Action<PointerEventData> OnEndAiming;

        [SerializeField] private Transform itemsRoot;
        [SerializeField] private FP_Input input;
        [SerializeField] private TrajectoryRenderer _throwTrajectory;

        [SerializeField] private Color SelectedItemColor;
        [SerializeField] private Color NormalItemColor;
        [SerializeField] private float _throwTimer = 1f;
        [SerializeField] private float _itemDisplayDuration = 2f;
        public DynamicButton scriptDynamicButton;

        private ItemUI[] _items;
        private Inventory _inventory;
        [SerializeField, HideInNormalInspector] 
        private SaveableInt _selectedItem = new SaveableInt() { Value = 0 };
        private bool _isAiming;

        public float ThrowTimer => _throwTimer;
        public int SelectedSlot => _selectedItem.Value;
        public string SelectedItemId => _items[_selectedItem.Value].ItemID;




        private void Awake()
        {
            Instance = this;
            _inventory = GetComponent<Inventory>();
            _items = itemsRoot.GetComponentsInChildren<ItemUI>();
        }

        private void Start()
        {
            for (int i = 0; i < _items.Length; i++)
            {
                var index = i;

                _items[i].InventorySlotIndex = index;
                _items[i].InventoryArea = itemsRoot.gameObject;
            }

            HandleItemSelected(_items[_selectedItem.Value]);
        }

        private void Update()
        {
            if (!input.UseMobileInput)
            {
                var scroll = -Input.mouseScrollDelta.y;

                if (Mathf.Abs(scroll) > 0.1f)
                {
                    _selectedItem.Value += (int)scroll;
                    
                    if(_selectedItem >= _items.Length)
                    {
                        _selectedItem.Value = 0;
                    }
                    else if (_selectedItem < 0)
                    {
                        _selectedItem.Value = _items.Length - 1;
                    }

                    HandleItemSelected(_items[_selectedItem]);
                }

                for (int i = 0; i < 9; i++)
                {
                    if(Input.GetKeyDown((KeyCode)i + 49))
                    {
                        if (i >= _items.Length || i < 0)
                            return;

                        _selectedItem.Value = i;
                        HandleItemSelected(_items[_selectedItem]);
                    }
                }
            }

            if (_isAiming && _inventory.Items[_selectedItem] != null)
            {
                var origin = _inventory.ItemThrowRoot.position;
                var direction = _inventory.ItemThrowRoot.forward;
                var power = _inventory.AimingThrowPower;
                var mass = _inventory.Items[_selectedItem].ItemRigidBody.mass;

                _throwTrajectory.Draw(origin, direction, power, mass, out var hit);
            }
        }

        private void OnEnable()
        {
            _inventory.OnInventoryUpdated += HandleInventoryUpdated;
            _inventory.OnItemFailedPickUp += HandleItemFailedPickUp;

            foreach (var item in _items)
            {
                item.OnSelected     += HandleItemSelected;
                item.OnMove         += HandleItemMove;
                item.OnThrow        += HandleItemThrow;
                item.OnBeginAiming  += HandleBeginAiming;
                item.OnAiming       += HandleAiming;
                item.OnEndAiming    += HandleEndAiming;
            }
        }

        private void OnDisable()
        {
            _inventory.OnInventoryUpdated -= HandleInventoryUpdated;
            _inventory.OnItemFailedPickUp -= HandleItemFailedPickUp;

            foreach (var item in _items)
            {
                item.OnSelected     -= HandleItemSelected;
                item.OnMove         -= HandleItemMove;
                item.OnThrow        -= HandleItemThrow;
                item.OnBeginAiming  -= HandleBeginAiming;
                item.OnAiming       -= HandleAiming;
                item.OnEndAiming    -= HandleEndAiming;
            }
        }

        #region Event Handlers

        private void HandleInventoryUpdated(Item[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                this._items[i].ItemID = items[i] == null ? string.Empty : items[i].id;

                var iconImage = this._items[i].GetComponent<Image>();

                if(items[i] == null)
                {
                    iconImage.color = Color.clear;
                }
                else
                {
                    var iconSprite = ItemDisplayManager.GetItemIcon(items[i].id);

                    iconImage.sprite = iconSprite;
                    iconImage.color = Color.white;
                }
            }
            scriptDynamicButton.EnableButtons(); // Вызываем метод в скрипте DynamicButton чтобы отобразить кнопки
        }

        private void HandleItemSelected(ItemUI item)
        {
            foreach (var i in _items)
            {
                i.frame.color = NormalItemColor;
            }

            if(item != null)
            {
                _selectedItem.Value = item.InventorySlotIndex;
                item.frame.color = SelectedItemColor;

                var text = (!string.IsNullOrEmpty(item.ItemID))
                    ? ItemDisplayManager.GetItem(item.ItemID).itemName
                    : string.Empty;

                InventoryMessageController.Instance.DisplayMessage(text, _itemDisplayDuration);
            }
            else
            {
                InventoryMessageController.Instance.DisplayMessage(string.Empty, _itemDisplayDuration);
            }
            scriptDynamicButton.EnableButtons(); // Вызываем метод в скрипте DynamicButton чтобы отобразить кнопки
        }

        private void HandleItemMove(ItemUI item)
        {
            if(_inventory.MoveItem(item.InventorySlotIndex, ItemUI.HoveredItemIndex))
                HandleItemSelected(_items[ItemUI.HoveredItemIndex]);
        }

        private void HandleItemThrow(ItemUI item)
        {
            _inventory.RemoveItem(item.InventorySlotIndex, true);

            HandleItemSelected(item);
        }

        private void HandleItemFailedPickUp()
        {
            InventoryMessageController.Instance.DisplayMessage(GetLocalizedFullInventoryString(), _itemDisplayDuration);
        }

        private void HandleBeginAiming(PointerEventData data)
        {
            _isAiming = true;

            var origin = _inventory.ItemThrowRoot.position;
            var direction = _inventory.ItemThrowRoot.forward;
            var power = _inventory.AimingThrowPower;
            var mass = _inventory.Items[_selectedItem].ItemRigidBody.mass;

            _throwTrajectory.Draw(origin, direction, power, mass, out var hit);
            _throwTrajectory.Show();

            OnBeginAiming?.Invoke(data);
        }

        private void HandleAiming(PointerEventData data)
        {
            OnAiming?.Invoke(data);
        }

        private void HandleEndAiming(PointerEventData data)
        {
            _throwTrajectory.Hide();
            OnEndAiming?.Invoke(data);
        }

        #endregion

        private string GetLocalizedFullInventoryString()
        {
            // TODO: replace this with localized string
            return "Not enough place in the inventory";
        }
    }
}