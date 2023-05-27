using LinkedSquad.Audio;
using LinkedSquad.InventorySystem;
using LinkedSquad.SavingSystem.Data;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace LinkedSquad.Interactions
{
    [Serializable]
    public class LockCondition
    {
        public Interactable Target;
        public bool Invert;
    }

    public class Interactable : SaveableMonobehaviour, IInteractable
    {
        public event Action OnInteract;
        public event Action OnOpen;
        public event Action OnClose;
        public event Action OnUnlock;

        [Header("Interaction Parameters")]
        [SerializeField] protected bool _openOnce;
        [SerializeField] protected bool _openAtStart;
        [SerializeField] protected bool _openAfterUnlocking;
        [SerializeField] protected bool _lockedAtStart;
        [SerializeField] protected bool _unlockOnce;
        [SerializeField] protected float _delayAfterUnlock = 0;
        [SerializeField] protected ItemId _itemIdRequiredToUnlock;
        [SerializeField] protected LockCondition[] _requiredInteractionsToUnlock;

        public bool IsOpened => _isOpen.Value; // is it opened
        public bool IsLocked => _isLocked.Value;
        public bool IsUnlockable => GetIsUnlockable();

        protected bool _isInteractable = true; // can we interact with this

        [SerializeField, HideInNormalInspector] protected SaveableBoolean _isOpen = new SaveableBoolean() { Value = false }; // state that is applied on save loading
        [SerializeField, HideInNormalInspector] protected SaveableBoolean _isLocked = new SaveableBoolean() { Value = false };
        [SerializeField, HideInNormalInspector] protected SaveableBoolean _hasOpened = new SaveableBoolean() { Value = false };
        [SerializeField, HideInNormalInspector] protected SaveableBoolean _hasUnlocked = new SaveableBoolean() { Value = false };

        private bool _isUnlockedByManiac;

#if UNITY_EDITOR
        [SerializeField, ReadOnly] private bool DebugUnlocked;
        [SerializeField, ReadOnly] private bool DebugOpened;
#endif



        public virtual void Interact()
        {
            if (_isInteractable)
            {
                if (_isUnlockedByManiac)
                {
                    _isOpen.Value = !_isOpen;

                    if (_isOpen.Value)
                    {
                        OnOpen?.Invoke();
                    }
                    else
                    {
                        OnClose?.Invoke();
                    }

                    return;
                }
                if (IsLocked)
                {
                    if (IsUnlockable)
                    {
                        if (_delayAfterUnlock != 0)
                        {
                            StartCoroutine(DisableInteractForTime(_delayAfterUnlock));
                        }
                        
                        _isLocked.Value = false;

                        OnUnlock?.Invoke();

                        TryUseItem();

                        if (_openAfterUnlocking && !_hasUnlocked.Value)
                        {
                            _hasUnlocked.Value = _unlockOnce;
                            _hasOpened.Value = _openOnce;
                            _isOpen.Value = true;

                            OnOpen?.Invoke();
                        }
                    }
                }
                else
                {
                    if (_openOnce && _hasOpened.Value)
                        return;

                    _hasOpened.Value = _openOnce;
                    _isOpen.Value = !_isOpen;

                    if (_isOpen.Value)
                    {
                        OnOpen?.Invoke();
                    }
                    else
                    {
                        OnClose?.Invoke();
                    }
                }

                OnInteract?.Invoke();
            }
        }

        private IEnumerator DisableInteractForTime(float time)
        {
            _isInteractable = false;
            yield return new WaitForSeconds(time);
            _isInteractable = true;
        }

        public void ForceOpen()
        {
            _isUnlockedByManiac = true;

            if (_isOpen.Value)
                return;

            _isOpen.Value = true;
            OnOpen?.Invoke();
        }

        public void ForceClose()
        {
            _isUnlockedByManiac = false;

            if (!_isOpen.Value)
                return;

            _isOpen.Value = false;
            OnClose?.Invoke();
        }

        public void SetInteractionState(bool state)
        {
            _isInteractable = state;
        }

        public void ClearLockedState()
        {
            _itemIdRequiredToUnlock = string.Empty;
            _requiredInteractionsToUnlock = new LockCondition[0];
        }

        protected virtual void Awake()
        {
            if (!_isOpen.IsLoaded)
                _isOpen.Value = _openAtStart;

            if (!_isLocked.IsLoaded)
                _isLocked.Value = _lockedAtStart;

#if UNITY_EDITOR
            DebugUnlocked = !IsLocked;
            DebugOpened = IsOpened;
#endif
        }

        protected virtual void OnEnable()
        {
            OnOpen += TryPickUpItem;

            if (_requiredInteractionsToUnlock != null)
            {
                foreach (var interaction in _requiredInteractionsToUnlock.Select(i => i.Target))
                {
                    interaction.OnOpen += UpdateUnlockedState;
                    interaction.OnClose += UpdateUnlockedState;
                }
            }
        }

        protected virtual void OnDisable()
        {
            OnOpen -= TryPickUpItem;

            if (_requiredInteractionsToUnlock != null)
            {
                foreach (var interaction in _requiredInteractionsToUnlock.Select(i => i.Target))
                {
                    interaction.OnOpen -= UpdateUnlockedState;
                    interaction.OnClose -= UpdateUnlockedState;
                }
            }
        }

        private void OnValidate()
        {
            // internal unity event
#if UNITY_EDITOR
            DebugUnlocked = !IsLocked;
            DebugOpened = IsOpened;
#endif
        }

        private bool GetIsUnlockable()
        {
            // check player inventory
            if (!string.IsNullOrEmpty(_itemIdRequiredToUnlock) && InventorySystem.Inventory.Instance != null)
            {
                var inventory = InventorySystem.Inventory.Instance;

                if (!inventory.Items.Any(item => item != null && _itemIdRequiredToUnlock.Equals(item.id)))
                    return false;

                var inventoryUI = InventoryUI.Instance;

                if (!_itemIdRequiredToUnlock.Equals(inventoryUI.SelectedItemId))
                    return false;
            }

            if (_requiredInteractionsToUnlock != null)
            {
                // check other interactions
                foreach (var condition in _requiredInteractionsToUnlock)
                {
                    if (condition.Invert && condition.Target.IsOpened)
                    {
                        return false;
                    }
                    else if (!condition.Invert && !condition.Target.IsOpened)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void UpdateUnlockedState()
        {
            var unlocked = GetIsUnlockable();

            if (_isLocked.Value && unlocked)
            {
                OnUnlock?.Invoke();

                TryUseItem();

                if (_openAfterUnlocking && !_hasUnlocked.Value)
                {
                    _hasUnlocked.Value = _unlockOnce;
                    _hasOpened.Value = _openOnce;
                    _isOpen.Value = true;

                    OnOpen?.Invoke();
                }
            }

            _isLocked.Value = !unlocked;
        }

        private void TryPickUpItem()
        {
            if (TryGetComponent<InventorySystem.Item>(out var item))
            {
                _isOpen.Value = false;

                // because item gets disabled
                // after being picked up 
                // so any OnInteract event
                // gets removed 
                OnInteract?.Invoke();

                InventorySystem.Inventory.Instance.AddItem(item);
            }
        }

        private void TryUseItem()
        {
            var inventory = InventorySystem.Inventory.Instance;

            if (!inventory.Items.Any(item => item != null && _itemIdRequiredToUnlock.Equals(item.id)))
                return;

            var item = inventory.Items.First(item => item != null && _itemIdRequiredToUnlock.Equals(item.id));

            if (item.Use())
            {
                inventory.RemoveItem(InventoryUI.Instance.SelectedSlot, false);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (_requiredInteractionsToUnlock != null)
            {
                foreach (var interaction in _requiredInteractionsToUnlock)
                {
                    if (interaction.Target == null)
                        continue;

                    var pos = interaction.Target.transform.position;
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(transform.position, pos);
                }
            }
        }
    }
}