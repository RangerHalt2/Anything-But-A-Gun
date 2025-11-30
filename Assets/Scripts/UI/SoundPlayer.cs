using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public AudioSource buttonAudioSource;

    public void PlayButtonSound()
    {
        if (buttonAudioSource != null)
        {
            buttonAudioSource.Play();
        }
    }
}