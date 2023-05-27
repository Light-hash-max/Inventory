using UnityEngine;
using System.Collections.Generic;

namespace LinkedSquad.PlayerControls
{
    public class FP_FootSteps : MonoBehaviour
    {
        public AudioClip jumpSound, landSound;
        public float crouchVolumeKoef = 0.33f;
        public List<Footsteps> footsteps = new List<Footsteps>();
        private FP_Controller playerController;

        private AudioSource audioSource;
        private int randomStep;

        private float defaultStepVolume;

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                // we automatically add an audiosource, if one has not been manually added.
                // (if you want to control the rolloff or other audio settings, add an audiosource manually)
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            defaultStepVolume = audioSource.volume;
        }

        void Start()
        {
            playerController = GetComponent<FP_Controller>();
        }

        public void PlayFootstepSounds()
        {
            for (int i = 0; i < footsteps.Count; i++)
            {
                if (footsteps[i].SurfaceTag == playerController.SurfaceTag())
                {
                    // pick & play a random footstep sound from the array,
                    // excluding sound at index 0
                    randomStep = Random.Range(1, footsteps[i].stepSounds.Length);
                    audioSource.clip = footsteps[i].stepSounds[randomStep];
                    audioSource.volume = playerController.IsCrouching() ? defaultStepVolume * crouchVolumeKoef : defaultStepVolume;
                    audioSource.Play();

                    // move picked sound to index 0 so it's not picked next time
                    footsteps[i].stepSounds[randomStep] = footsteps[i].stepSounds[0];
                    footsteps[i].stepSounds[0] = audioSource.clip;
                }
            }
        }

        public void ResetFootstepSounds()
        {
            for (int i = 0; i < footsteps.Count; i++)
            {
                if (footsteps[i].SurfaceTag == playerController.SurfaceTag())
                {
                    audioSource.clip = footsteps[i].stepSounds[0];
                    audioSource.volume = playerController.IsCrouching() ? defaultStepVolume * crouchVolumeKoef : defaultStepVolume;
                    audioSource.Play();
                }
            }
        }
    }
}