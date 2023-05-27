using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LinkedSquad.InventorySystem
{
    // visual representation of an Item within the UI
    // i.e. Image containing a Sprite
    public class ItemUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action<ItemUI> OnSelected;
        public event Action<ItemUI> OnThrow;
        public event Action<ItemUI> OnMove;

        public event Action<PointerEventData> OnBeginAiming;
        public event Action<PointerEventData> OnAiming;
        public event Action<PointerEventData> OnEndAiming;

        public float throwTimer = 1f;
        public string ItemID { get; set; }
        public int InventorySlotIndex { get; set; }
        public GameObject InventoryArea { get; set; }
        public Image frame;

        private bool _isDragging;
        private float _currentThrowTimer = -2f;
        private Image _previewImage;
        private PointerEventData _currentPointerData;
        private bool _isAiming;

        public static ItemUI HoveredItem { get; private set; }
        public static int HoveredItemIndex { get; private set; }



        private void Update()
        {
            if (_currentThrowTimer < -1f)
                return;

            if(_currentThrowTimer > 0)
            {
                _currentThrowTimer -= Time.deltaTime;
            }
            else
            {
                _currentThrowTimer = -2f;

                _isAiming = true;
                OnBeginAiming?.Invoke(_currentPointerData);
            }
        }

        #region Handle Icon Dragging

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (string.IsNullOrEmpty(ItemID))
            {
                _currentThrowTimer = -2f;
                _isDragging = false;

                return;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (string.IsNullOrEmpty(ItemID))
                return;

            _currentPointerData = eventData;

            if (_isAiming)
                OnAiming?.Invoke(_currentPointerData); 
            
            if (!eventData.hovered.Any(t => t.Equals(InventoryArea))
                && _currentThrowTimer < -1f)
            {
                _currentThrowTimer = InventoryUI.Instance.ThrowTimer;
            }

            transform.position = eventData.position;
            _previewImage.transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.localPosition = Vector3.zero;
        }

        #endregion

        #region Handle Pointer Events

        public void OnPointerDown(PointerEventData eventData)
        {
            if(eventData.pointerId == -2)
            {
                OnThrow?.Invoke(this);

                return;
            }

            _previewImage = CreateClone();

            GetComponent<Image>().raycastTarget = false;

            OnSelected?.Invoke(this);

            if (string.IsNullOrEmpty(ItemID))
                return;

            _isDragging = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _currentThrowTimer = -2f;

            GetComponent<Image>().raycastTarget = true;

            if (_previewImage != null)
                Destroy(_previewImage.gameObject);

            if (!_isDragging)
                return;

            _isDragging = false;

            if(HoveredItem != null)
            {
                OnMove?.Invoke(this);
            }
            else if(!eventData.hovered.Any(t => t.Equals(InventoryArea)))
            {
                OnThrow?.Invoke(this);
            }

            if (_isAiming)
            {
                _isAiming = false;
                OnEndAiming?.Invoke(_currentPointerData);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            HoveredItem = this;
            HoveredItemIndex = InventorySlotIndex;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(HoveredItem != null 
                && _isDragging
                && !eventData.hovered.Any(t => t.Equals(InventoryArea)))
            {
                _currentThrowTimer = InventoryUI.Instance.ThrowTimer;
            }

            HoveredItem = null;
            HoveredItemIndex = -1;
        }

        #endregion

        private Image CreateClone()
        {
            var obj = new GameObject(gameObject.name);
            var rectTransform = obj.AddComponent<RectTransform>();
            var image = obj.AddComponent<Image>();
            image.raycastTarget = false;
            image.sprite = GetComponent<Image>().sprite;

            rectTransform.SetParent(transform.parent.parent);
            rectTransform.localScale = Vector3.one;
            rectTransform.sizeDelta = GetComponent<RectTransform>().sizeDelta;
            rectTransform.SetAsLastSibling();

            return image;
        }
    }
}