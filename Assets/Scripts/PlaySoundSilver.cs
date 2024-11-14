using UnityEngine;
using UnityEngine.UI;

public class PlaySoundSilver : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip goldSound;
    private float volumeMultiplier = 1f;

    // Reference to the button component
    private Button button;

    private void Awake()
    {
        // Get the Button component
        button = GetComponent<Button>();

        // Get or add AudioSource
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                // Configure AudioSource defaults
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 0f; // Make the sound 2D
            }
        }

        // Add click listener
        button.onClick.AddListener(PlayClickSound);
    }

    private void OnDestroy()
    {
        // Clean up listener when object is destroyed
        if (button != null)
        {
            button.onClick.RemoveListener(PlayClickSound);
        }
    }

    public void PlayClickSound()
    {
        if (button.interactable && goldSound != null)
        {
            audioSource.PlayOneShot(goldSound, volumeMultiplier);
        }
    }

    // Method to adjust volume
    public void SetVolume(float volume)
    {
        volumeMultiplier = Mathf.Clamp(volume, 0f, 1f);
    }
}