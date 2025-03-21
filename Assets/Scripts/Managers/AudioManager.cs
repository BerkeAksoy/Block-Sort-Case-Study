using UnityEngine;

public class AudioManager : BaseSingleton<AudioManager>
{
    AudioSource _audioSource;
    [SerializeField] AudioClip _legoSnapClip, _slotCompleteClip;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayPieceSnap()
    {
        _audioSource.PlayOneShot(_legoSnapClip);
    }

    public void PlaySlotComplete()
    {
        _audioSource.PlayOneShot(_slotCompleteClip, 0.2f);
    }
}
