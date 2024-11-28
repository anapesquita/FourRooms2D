using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlaySoundSilver : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip positiveSound; // Sound for when points are doubled
    [SerializeField] private AudioClip negativeSound; // Sound for when points are lost
    private float volumeMultiplier = 1f;
    private bool isAvocadoOrMushroom = false;
    private bool isBananaOrPineapple = false;

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

        // Check scene type
        CheckSceneType();

        // Add click listener
        button.onClick.AddListener(PlayClickSound);
    }

    private void CheckSceneType()
    {
        string currentScene = SceneManager.GetActiveScene().name.ToLower();
        isAvocadoOrMushroom = currentScene.Contains("avocado") || currentScene.Contains("mushroom");
        isBananaOrPineapple = currentScene.Contains("banana") || currentScene.Contains("pineapple");
        Debug.Log($"Silver Button - Current scene: {currentScene}, IsAvocadoOrMushroom: {isAvocadoOrMushroom}, IsBananaOrPineapple: {isBananaOrPineapple}");
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
        if (!button.interactable) return;

        AudioClip soundToPlay = null;

        if (isAvocadoOrMushroom)
        {
            // Play positive sound for avocado/mushroom scenes
            soundToPlay = positiveSound;
        }
        else if (isBananaOrPineapple)
        {
            // Play negative sound for banana/pineapple scenes
            soundToPlay = negativeSound;
        }
        else
        {
            // Default to positive sound for other scenes
            soundToPlay = positiveSound;
        }

        if (soundToPlay != null)
        {
            audioSource.PlayOneShot(soundToPlay, volumeMultiplier);
        }
    }

    // Method to adjust volume
    public void SetVolume(float volume)
    {
        volumeMultiplier = Mathf.Clamp(volume, 0f, 1f);
    }
}