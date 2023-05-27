using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LinkedSquad.Audio;

namespace LinkedSquad.Interactions.Audio
{
    public class AudioInteraction : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField, Name("Audio Source")] protected AudioSource _targetSource;
        [SerializeField] protected AudioClip _audioInteract;
        [SerializeField] protected AudioClip _audioOpen;
        [SerializeField] protected AudioClip _audioClose;
        [SerializeField] protected AudioClip _audioLocked;
        [SerializeField] protected AudioClip _audioUnlock;

        protected AudioSource _audioSource;
        protected Interactable _targetInteraction;



        private void Awake()
        {
            _targetInteraction = GetComponent<Interactable>();
            _audioSource = (_targetSource == null)
                ? GetComponent<AudioSource>()
                : _targetSource;
        }

        private void OnEnable()
        {
            _targetInteraction.OnInteract += OnInteractAudio;
            _targetInteraction.OnOpen += OnOpen;
            _targetInteraction.OnClose += OnClose;
            _targetInteraction.OnUnlock += OnUnlock;
        }

        private void OnDisable()
        {
            _targetInteraction.OnInteract -= OnInteractAudio;
            _targetInteraction.OnOpen -= OnOpen;
            _targetInteraction.OnClose -= OnClose;
            _targetInteraction.OnUnlock -= OnUnlock;
        }

        private void OnInteractAudio()
        {
            if (_targetInteraction.IsLocked)
            {
                PlayClip(_audioLocked);
            }
            else
            {
                PlayClipUsingGlobalAudio(_audioInteract);
            }
        }

        private void OnOpen() => PlayClip(_audioOpen);
        private void OnClose() => PlayClip(_audioClose);
        private void OnUnlock() => PlayClip(_audioUnlock);

        public void OnAudioPaused(bool isPaused)
        {
            if (isPaused)
                _audioSource.Pause();
            else
                _audioSource.UnPause();
        }

        private void PlayClip(AudioClip clip)
        {
            if (clip == null)
                return;

            _audioSource.Stop();
            _audioSource.PlayOneShot(clip);
        }

        private void PlayClipUsingGlobalAudio(AudioClip clip)
        {
            if (clip == null)
                return;

            GlobalAudioSource.Instance.PlayOneShot(clip);
        }
    }
}