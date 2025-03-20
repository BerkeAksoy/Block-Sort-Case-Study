using UnityEngine;

public class AudioManager : BaseSingleton<AudioManager>
{
    AudioSource _audioSource;
    [SerializeField] AudioClip _legoSnapClip;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayPieceSnap()
    {
        _audioSource.PlayOneShot(_legoSnapClip);
    }
}
