using UnityEngine;

namespace LinkedSquad.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class GlobalAudioSource : MonoBehaviour
    {
        public static AudioSource Instance;

        private void Awake()
        {
            Instance = GetComponent<AudioSource>();
        }
    }
}