using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Changes the audio volume based on game flow (paused / unpaused).
    public void SetVolumeState(bool isPaused)
    {
        AudioSource[] audioSources;
        audioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audioSource in audioSources)
        {
            // Ignore audio coming from UI.
            if (!audioSource.gameObject.CompareTag("Canvas"))
            {
                if (isPaused)
                {
                    audioSource.Pause();
                }
                else
                {
                    audioSource.UnPause();
                }
            }
        }
    }
}
