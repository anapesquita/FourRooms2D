using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndPanelController : MonoBehaviour
{
    public GameObject endPanel;
    public TextMeshProUGUI scoreText;
    public Button keySilverButton;
    public Button keyGoldButton;
    private GameController gameController;
    private float delayBeforeHide = 3f;
    private int originalTrialScore = 0;
    private bool initialized = false;
    private bool messageSet = false;
    private bool isAvocadoOrMushroom = false;
    private bool isBananaOrPineapple = false;

    void Start()
    {
        ValidateComponents();
        CheckSceneType();
    }

    private void CheckSceneType()
    {
        string currentScene = SceneManager.GetActiveScene().name.ToLower();
        isAvocadoOrMushroom = currentScene.Contains("avocado") || currentScene.Contains("mushroom");
        isBananaOrPineapple = currentScene.Contains("banana") || currentScene.Contains("pineapple");
        Debug.Log($"Current scene: {currentScene}, IsAvocadoOrMushroom: {isAvocadoOrMushroom}, IsBananaOrPineapple: {isBananaOrPineapple}");
    }

    private void OnSilverKeyClicked()
    {
        Debug.Log("Silver Key clicked - starting process");
        keySilverButton.interactable = false;
        keyGoldButton.interactable = false;

        messageSet = true;

        if (isAvocadoOrMushroom)
        {
            // Double points for avocado/mushroom scenes
            scoreText.text = "Points doubled!";
            scoreText.color = Color.green;
            gameController.OnSilverKeyFromPanel();
        }
        else if (isBananaOrPineapple)
        {
            // Lose points for banana/pineapple scenes
            scoreText.text = "Points lost!";
            scoreText.color = Color.red;
            gameController.OnGoldKeyFromPanel();
        }
        else
        {
            // Default behavior for other scenes
            scoreText.text = "Points doubled!";
            scoreText.color = Color.green;
            gameController.OnSilverKeyFromPanel();
        }

        Debug.Log($"Set text to: {scoreText.text}");
        StartCoroutine(DelayedHideAndTransition());
    }

    private void OnGoldKeyClicked()
    {
        Debug.Log("Gold Key clicked - starting process");
        keySilverButton.interactable = false;
        keyGoldButton.interactable = false;

        messageSet = true;

        if (isAvocadoOrMushroom)
        {
            // Lose points for avocado/mushroom scenes
            scoreText.text = "Points lost!";
            scoreText.color = Color.red;
            gameController.OnGoldKeyFromPanel();
        }
        else if (isBananaOrPineapple)
        {
            // Double points for banana/pineapple scenes
            scoreText.text = "Points doubled!";
            scoreText.color = Color.green;
            gameController.OnSilverKeyFromPanel();
        }
        else
        {
            // Default behavior for other scenes
            scoreText.text = "Points lost!";
            scoreText.color = Color.red;
            gameController.OnGoldKeyFromPanel();
        }

        Debug.Log($"Set text to: {scoreText.text}");
        StartCoroutine(DelayedHideAndTransition());
    }

    public void ShowPanel()
    {
        Debug.Log("ShowPanel called on EndPanelController");
        if (endPanel != null)
        {
            endPanel.SetActive(true);
            if (!messageSet)
            {
                scoreText.text = "Choose a key!";
                Debug.Log("Set initial text: Choose a key!");
            }
        }
    }

    private void ValidateComponents()
    {
        bool isValid = true;

        if (endPanel == null)
        {
            Debug.LogError("EndPanel is not assigned!");
            isValid = false;
        }

        if (scoreText == null)
        {
            Debug.LogError("Score Text (TextMeshPro) is not assigned!");
            isValid = false;
        }

        if (keySilverButton == null)
        {
            Debug.LogError("Silver Key Button is not assigned!");
            isValid = false;
        }

        if (keyGoldButton == null)
        {
            Debug.LogError("Gold Key Button is not assigned!");
            isValid = false;
        }

        gameController = FindObjectOfType<GameController>();
        if (gameController == null)
        {
            Debug.LogError("GameController not found in scene!");
            isValid = false;
        }

        if (isValid)
        {
            Debug.Log("All components validated successfully!");
            InitializePanel();
        }
        else
        {
            Debug.LogError("Please assign all required components in the Inspector!");
        }
    }

    private void InitializePanel()
    {
        endPanel.SetActive(false);

        keySilverButton.onClick.AddListener(OnSilverKeyClicked);
        keyGoldButton.onClick.AddListener(OnGoldKeyClicked);

        gameController.RegisterEndPanelController(this);

        initialized = true;
        Debug.Log("EndPanelController initialized successfully");
    }

    public void HidePanel()
    {
        Debug.Log("HidePanel called on EndPanelController");
        if (endPanel != null)
        {
            endPanel.SetActive(false);
        }
    }

    private void UpdatePanelInfo(string context)
    {
        if (!initialized)
        {
            Debug.LogError("Trying to update panel before initialization!");
            return;
        }

        Debug.Log($"UpdatePanelInfo called. Context: {context}");

        string scoreMessage;

        if (context == "silver")
        {
            scoreMessage = isAvocadoOrMushroom ? "Points doubled!" : "Points lost!";
        }
        else if (context == "gold")
        {
            scoreMessage = isBananaOrPineapple ? "Points doubled!" : "Points lost!";
        }
        else
        {
            scoreMessage = "Choose a key!";
        }

        Debug.Log($"Setting score text to: {scoreMessage}");
        scoreText.text = scoreMessage;
    }

    private IEnumerator DelayedHideAndTransition()
    {
        Debug.Log($"Starting delay before hide. Current text: {scoreText.text}");
        yield return new WaitForSeconds(delayBeforeHide);
        Debug.Log("Delay complete, hiding panel");
        messageSet = false;
        HidePanel();
        gameController.StateNext(GameController.STATE_FINISH);
    }

    private void OnDestroy()
    {
        if (keySilverButton != null)
        {
            keySilverButton.onClick.RemoveListener(OnSilverKeyClicked);
        }
        if (keyGoldButton != null)
        {
            keyGoldButton.onClick.RemoveListener(OnGoldKeyClicked);
        }
    }
}