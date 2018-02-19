using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DigitalRuby.SoundManagerNamespace
{
	public class MenuMusic : MonoBehaviour {
        
        private AudioSource[] musicSources;
        private AudioSource[] audioSources;

        void Start() 
		{
            AudioSource music1 = GameObject.Find("music1").GetComponent<AudioSource>();
            AudioSource audio1 = GameObject.Find("audio1").GetComponent<AudioSource>();

            musicSources = new AudioSource[1];
            audioSources = new AudioSource[1];

            musicSources[0] = music1;
            audioSources[0] = audio1;

            musicSources[0].PlayLoopingMusicManaged(1.0f, 1.0f, false);
		}

		public void PlayMusic(int index)
		{
			if (index == 1) {
                musicSources[0].PlayLoopingMusicManaged (1.0f, 1.0f, false);
			}
		}

		public void StopMusic(int index)
		{
			if (index == 1) {
                musicSources[0].StopLoopingMusicManaged ();
			}
		}

		public void MuteMusic(int index)
		{
			SoundManager.MusicVolume = 0;
		}

		public void PlaySoundEfffect(int index)
		{
			if (index == 1) {
                audioSources[0].PlayOneShotSoundManaged(audioSources[0].clip);
			}
		}

		public void SoundVolumeChanged()
		{
			Slider sefx_slider = GameObject.Find("sefxSlider").GetComponent<Slider>();
			SoundManager.SoundVolume = sefx_slider.value;
		}

		public void MusicVolumeChanged()
		{
			Slider music_slider = GameObject.Find("musicSlider").GetComponent<Slider>();
			SoundManager.MusicVolume = music_slider.value;
		}

		public void PlayRandomSound()
		{
			int randInt = (int)Random.Range(0.0f, 100.0f);
			AudioSource coinSound = GameObject.Find("coinAudio").GetComponent<AudioSource>();
			AudioSource boomSound = GameObject.Find("boomAudio").GetComponent<AudioSource>();
			AudioSource confettiSound = GameObject.Find("confettiAudio").GetComponent<AudioSource>();
			AudioSource popSound = GameObject.Find("popAudio").GetComponent<AudioSource>();

			if (randInt <= 50)
			{
				boomSound.PlayOneShotSoundManaged(boomSound.clip);
			}
			else if (randInt > 50 && randInt <= 80)
			{
				popSound.PlayOneShotSoundManaged(popSound.clip);
			}
			else if (randInt > 80 && randInt <= 90)
			{
				coinSound.PlayOneShotSoundManaged(coinSound.clip);
			}
			else if (randInt > 90 && randInt <= 100)
			{
				confettiSound.PlayOneShotSoundManaged(confettiSound.clip);
			}
		}
	}
}