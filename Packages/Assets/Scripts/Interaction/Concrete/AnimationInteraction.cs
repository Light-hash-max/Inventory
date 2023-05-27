using LinkedSquad.SavingSystem;
using LinkedSquad.SavingSystem.Data;
using System;
using System.Collections;
using UnityEngine;

namespace LinkedSquad.Interactions.Animations
{
    [RequireComponent(typeof(Animation))]
    public class AnimationInteraction : SaveableMonobehaviour
    {
        public event Action OnCloseBegin;
        public event Action OnCloseEnd;

        public event Action OnOpenBegin;
        public event Action OnOpenEnd;

        [Header("Animation")]
        [SerializeField] protected string _animationOpen = string.Empty;
        [SerializeField] protected string _animationClose = string.Empty;
        [SerializeField] protected string _animationLocked = string.Empty;
        [SerializeField] protected string _animationUnlock = string.Empty;

        [Tooltip("Если нужно использовать отдельный GameObject для анимации")]
        [SerializeField, Name("Animation (Component)")] protected Animation _targetAnimation;

        protected Animation _animationComponent;
        protected Interactable _targetInteraction;



        public override void SubscribeForSaving(ISaveSystem saveSystem)
        {
            base.SubscribeForSaving(saveSystem);

            saveSystem.OnDataLoaded += HandleDataLoaded;
        }

        private void Awake()
        {
            _targetInteraction = GetComponent<Interactable>();
            _animationComponent = (_targetAnimation == null)
                ? GetComponent<Animation>()
                : _targetAnimation;

            var clip = _animationComponent.clip;
            var clipLength = _targetInteraction.IsOpened ? clip.length : 0f;
            var animationTarget = _animationComponent.gameObject != null
                ? _animationComponent.gameObject
                : gameObject;

            clip.SampleAnimation(animationTarget, clipLength);
        }

        private void OnEnable()
        {
            _targetInteraction.OnInteract += OnInteract;
            _targetInteraction.OnOpen += OnOpen;
            _targetInteraction.OnClose += OnClose;
            _targetInteraction.OnUnlock += OnUnlock;
        }

        private void OnDisable()
        {
            _targetInteraction.OnInteract -= OnInteract;
            _targetInteraction.OnOpen -= OnOpen;
            _targetInteraction.OnClose -= OnClose;
            _targetInteraction.OnUnlock -= OnUnlock;
        }

        private void OnInteract()
        {
            if (!_targetInteraction.IsLocked || string.IsNullOrEmpty(_animationLocked))
                return;

            var go = _targetAnimation != null ? _targetAnimation.gameObject : gameObject;
            var ac = _animationComponent;
            var clip = _animationComponent.GetClip(_animationLocked);

            StartCoroutine(CoroutineAnimation(go, ac, clip, false));
        }

        private void OnOpen()
        {
            var go = _targetAnimation != null ? _targetAnimation.gameObject : gameObject;
            var ac = _animationComponent;
            var clip = _animationComponent.GetClip(_animationOpen);

            StartCoroutine(CoroutineAnimation(go, ac, clip, false, OnOpenBegin, OnOpenEnd));
        }

        private void OnClose()
        {
            var go = _targetAnimation != null ? _targetAnimation.gameObject : gameObject;
            var ac = _animationComponent;
            var clip = _animationComponent.GetClip(string.IsNullOrEmpty(_animationClose)
                ? _animationOpen
                : _animationClose);

            var reverse = string.IsNullOrEmpty(_animationClose);

            StartCoroutine(CoroutineAnimation(go, ac, clip, reverse, OnCloseBegin, OnCloseEnd));
        }

        private void OnUnlock()
        {
            if (string.IsNullOrEmpty(_animationUnlock))
                return;

            var go = _targetAnimation != null ? _targetAnimation.gameObject : gameObject;
            var ac = _animationComponent;
            var clip = _animationComponent.GetClip(_animationUnlock);

            StartCoroutine(CoroutineAnimation(go, ac, clip, false));
        }

        private IEnumerator CoroutineAnimation(
            GameObject target,
            Animation anim,
            AnimationClip clip,
            bool reverse,
            Action callbackBegin = null,
            Action callbackEnd = null)
        {
            callbackBegin?.Invoke();
            _targetInteraction.SetInteractionState(false);

            if (clip == null)
                clip = anim.clip;

            var clipLength = clip.length;
            var timer = 0f;

            while (timer < clipLength)
            {
                timer += Time.deltaTime;

                var time = reverse
                    ? clipLength - timer
                    : timer;

                clip.SampleAnimation(target, time);

                yield return null;
            }

            var endTimer = reverse ? 0f : clipLength;

            clip.SampleAnimation(target, endTimer);

            _targetInteraction.SetInteractionState(true);
            callbackEnd?.Invoke();
        }

        private void HandleDataLoaded()
        {
            var clip = _animationComponent.clip;
            var clipLength = _targetInteraction.IsOpened ? clip.length : 0f;
            var animationTarget = _animationComponent.gameObject != null
                ? _animationComponent.gameObject
                : gameObject;

            clip.SampleAnimation(animationTarget, clipLength);
        }
    }
}