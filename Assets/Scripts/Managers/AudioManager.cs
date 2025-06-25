using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _sfxAudioSource;
    [SerializeField] private AudioClip _laserClip;

    public void PlayLaserSFX()
    {
        _sfxAudioSource.clip = _laserClip;
        _sfxAudioSource.Play();
    }

}
