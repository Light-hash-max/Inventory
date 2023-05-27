using LinkedSquad.Interactions.Animations;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace LinkedSquad.Interactions
{
    public class EventInteraction : MonoBehaviour
    {
        [Header("Эвенты Анимации Открытия")]
        public UnityEvent onOpenBeginEvent;
        public UnityEvent onOpenEndEvent;

        [Space]
        [Header("Эвенты Анимации Закрытия")]
        public UnityEvent onCloseBeginEvent;
        public UnityEvent onCloseEndEvent;

        [Header("Эвенты Заблокированного интеракшена")]
        public UnityEvent onLockedEvent;
        public UnityEvent onUnlockEvent;

        private Interactable _targetInteraction;
        private AnimationInteraction _targetAnimation;



        private void Awake()
        {
            _targetInteraction  = GetComponent<Interactable>();
            _targetAnimation    = GetComponent<AnimationInteraction>();
        }

        private void OnEnable()
        {
            _targetInteraction.OnInteract   += OnInteract;
            _targetInteraction.OnUnlock     += onUnlockEvent.Invoke;
            _targetInteraction.OnOpen       += onOpenBeginEvent.Invoke;
            _targetInteraction.OnClose      += onCloseBeginEvent.Invoke;

            if (_targetAnimation != null)
            {
                _targetAnimation.OnOpenEnd += onOpenEndEvent.Invoke;
                _targetAnimation.OnCloseEnd += onCloseEndEvent.Invoke;
            }
        }

        private void OnDisable()
        {
            _targetInteraction.OnInteract   -= OnInteract;
            _targetInteraction.OnUnlock     -= onUnlockEvent.Invoke;
            _targetInteraction.OnOpen       -= onOpenBeginEvent.Invoke;
            _targetInteraction.OnClose      -= onCloseBeginEvent.Invoke;

            if (_targetAnimation != null)
            {
                _targetAnimation.OnOpenEnd -= onOpenEndEvent.Invoke;
                _targetAnimation.OnCloseEnd -= onCloseEndEvent.Invoke;
            }
        }

        private void OnInteract()
        {
            if (_targetInteraction.IsLocked)
            {
                onLockedEvent?.Invoke();
            }
        }
    }
}