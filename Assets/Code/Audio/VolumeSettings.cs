using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


namespace HocaInk.InteractiveWall
{
    public class VolumeSettings : MonoBehaviour
    {
        [SerializeField] private AudioMixer _mixer;


        const string MIXER_MUSIC = "MusicVolume";
        const string MIXER_SFX = "SFXVolume";

        private static float _musicVolume = 0.5f;
        private static float _sfxVolume = 0.5f;

        public static bool isMusicOn = true;
        public static bool isSfxOn = true;

        //private GameSettingsData _gameSettings;
        private AudioManager _audioManager;


        private void Start()
        {
            InitializeVolume();
        }

        public void InitializeVolume()
        {
            //_gameSettings = DataManager.Instance.gameSettings;
            _audioManager = AudioManager.instance;
            /*SetMusicVolume(_gameSettings.musicVolume);
            SetSFXVolume(_gameSettings.sfxVolume);
            _audioManager.musicSource.mute = !_gameSettings.isMusicOn;
            _audioManager.sfxSource.mute = !_gameSettings.isSfxOn;*/
            SetMusicVolume(0.5f);
            SetSFXVolume(0.5f);
            _audioManager.musicSource.mute = false;
            _audioManager.sfxSource.mute = false;
        }

        public void SetMusicVolume(float volume)
        {
            _mixer.SetFloat(MIXER_MUSIC,  Mathf.Log10(volume) * 20);
        }

        public void SetSFXVolume(float volume)
        {
            _mixer.SetFloat(MIXER_SFX, Mathf.Log10(volume) * 20);
        }
    

        public void SwitchOnMusic(bool isOn)
        {
            _audioManager.musicSource.mute = !isOn;
            //_gameSettings.isMusicOn = isOn;
        }
    
        public void SwitchOnSfx(bool isOn)
        {
            _audioManager.sfxSource.mute = !isOn;
            //_gameSettings.isSfxOn = isOn;
        }
    }
}
