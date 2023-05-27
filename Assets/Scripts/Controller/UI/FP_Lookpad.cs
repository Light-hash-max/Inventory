using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
using LinkedSquad.InventorySystem;

namespace LinkedSquad.PlayerControls
{
    [RequireComponent(typeof(EventTrigger))]
    [RequireComponent(typeof(CanvasGroup))]
    public class FP_Lookpad : MonoBehaviour
    {
        public event Action<PointerEventData> OnPointerDownEvent;
        public event Action<PointerEventData> OnPointerEvent;
        public event Action<PointerEventData, float> OnPointerUpEvent;

        private Vector2 touchInput, prevDelta, dragInput;
        private bool isPressed;
        private EventTrigger eventTrigger;
        private CanvasGroup canvasGroup;

        private int _pointerId = -1;
        private float _heldTime = 0f;
        [SerializeField] Slider SensitivitySlider;
        private float Sensitivity = 2f;



        void Start()
        {
            SetupListeners();
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;

        }

        // Update is called once per frame
        void Update()
        {
            touchInput = (dragInput - prevDelta) / Time.deltaTime;
            prevDelta = dragInput;

            _heldTime += Time.deltaTime;
        }

        //Setup events;
        void SetupListeners()
        {
            eventTrigger = gameObject.GetComponent<EventTrigger>();

            var pointerDownEvent = new EventTrigger.TriggerEvent();
            pointerDownEvent.AddListener(OnPointerDown);

            eventTrigger.triggers.Add(new EventTrigger.Entry { callback = pointerDownEvent, eventID = EventTriggerType.PointerDown });


            var pointerEvent = new EventTrigger.TriggerEvent();
            pointerEvent.AddListener(OnPointer);

            eventTrigger.triggers.Add(new EventTrigger.Entry { callback = pointerEvent, eventID = EventTriggerType.Drag });


            var pointerUpEvent = new EventTrigger.TriggerEvent();
            pointerUpEvent.AddListener(OnPointerUp);

            eventTrigger.triggers.Add(new EventTrigger.Entry { callback = pointerUpEvent, eventID = EventTriggerType.PointerUp });

            InventoryUI.Instance.OnBeginAiming += OnPointerDown;
            InventoryUI.Instance.OnAiming += OnPointer;
            InventoryUI.Instance.OnEndAiming += OnPointerUp;
        }

        public void OnPointerDown(BaseEventData data)
        {
            var evData = (PointerEventData)data;
            data.Use();

            OnPointerDownEvent?.Invoke(evData);

            if (_pointerId > -1)
                return;

            _heldTime = 0f;
            _pointerId = evData.pointerId;

            isPressed = true;
            prevDelta = dragInput = evData.position;
        }

        public void OnPointer(BaseEventData data)
        {
            var evData = (PointerEventData)data;
            evData.Use();
            OnPointerEvent?.Invoke(evData);

            if (_pointerId != evData.pointerId)
                return;

            dragInput = evData.position;
        }

        public void OnPointerUp(BaseEventData data)
        {
            var evData = (PointerEventData)data;
            OnPointerUpEvent?.Invoke(evData, _heldTime);

            if (_pointerId != evData.pointerId)
                return;

            _pointerId = -1;

            touchInput = Vector2.zero;
            isPressed = false;
        }

        //Returns drag vector;
        public Vector2 LookInput()
        {
            return touchInput * Time.deltaTime * Sensitivity;
        }

        //Returns whether or not button is pressed
        public bool IsPressed()
        {
            return isPressed;
        }


        public void ChangeSensitivity(float NewSensitivity)
        {
            Sensitivity = NewSensitivity;
            PlayerPrefs.SetFloat("Sensitivity", NewSensitivity);
        }
    }
}