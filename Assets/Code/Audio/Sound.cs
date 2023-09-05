using UnityEngine.Audio;
using UnityEngine;


namespace HocaInk.InteractiveWall
{
    [System.Serializable]
    public class Sound
    {
        public string Name;
        public AudioClip clip;

        [Range(0, 1f)]
        public float volume;
        /*[Range(0.1f, 3f)]
        public float pitch;*/

        /*[HideInInspector]
        public AudioSource source;*/

    }
}
