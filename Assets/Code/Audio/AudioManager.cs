using System;
using UnityEngine;


namespace HocaInk.InteractiveWall
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;

        public AudioSource musicSource;
        public AudioSource sfxSource;
        public Sound[] sounds;
        public Sound[] tracks;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        public void PlayMusic(string trackName)
        {
            Sound track = Array.Find(tracks, tracks => tracks.Name == trackName);
            musicSource.clip = track.clip;
            musicSource.Play();
        }

        public void PlaySound(string soundName)
        {
            Sound sound = Array.Find(sounds, sounds => sounds.Name == soundName);
            sfxSource.PlayOneShot(sound.clip, sound.volume);
        }

        public void PlayRandomSound(string nameStatrts)
        {
            Sound[] soundsArray = Array.FindAll(sounds, sounds => sounds.Name.StartsWith(nameStatrts));
            int randomIndex = UnityEngine.Random.Range(0, soundsArray.Length);
            sfxSource.PlayOneShot(soundsArray[randomIndex].clip, soundsArray[randomIndex].volume);
        }
    }
}
