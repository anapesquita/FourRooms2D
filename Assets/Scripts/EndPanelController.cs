using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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
    private bool messageSet = false; // Add this flag

    

    void Start()
    {
        ValidateComponents();
    }

    private void OnSilverKeyClicked()
    {
        Debug.Log("Silver Key clicked - starting process");
        keySilverButton.interactable = false;
        keyGoldButton.interactable = false;

        // Set text and ensure it persists
        messageSet = true;
        scoreText.text = "Points doubled!";
        Debug.Log("Set text to: Points doubled!");

        gameController.OnSilverKeyFromPanel();
        StartCoroutine(DelayedHideAndTransition());
    }

    private void OnGoldKeyClicked()
    {
        Debug.Log("Gold Key clicked - starting process");
        keySilverButton.interactable = false;
        keyGoldButton.interactable = false;

        // Set text and ensure it persists
        messageSet = true;
        scoreText.text = "Points lost!";
        Debug.Log("Set text to: Points lost!");

        gameController.OnGoldKeyFromPanel();
        StartCoroutine(DelayedHideAndTransition());
    }

    public void ShowPanel()
    {
        Debug.Log("ShowPanel called on EndPanelController");
        if (endPanel != null)
        {
            endPanel.SetActive(true);
            // Only set initial text if no message has been set by key clicks
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

        string scoreMessage;  // Declare without initial value

        if (context == "silver")
        {
            scoreMessage = "Points doubled!";
        }
        else if (context == "gold")
        {
            scoreMessage = "Points lost!";
        }
        else
        {
            // Initial message when panel opens
            scoreMessage = "Choose a key!";
        }

        Debug.Log($"Setting score text to: {scoreMessage}");
        scoreText.text = scoreMessage;
    }

    private IEnumerator DelayedHideAndTransition()
    {
        Debug.Log($"Starting delay before hide. Current text: {scoreText.text}");
        // Add a small delay to ensure the message is visible
        yield return new WaitForSeconds(delayBeforeHide);
        Debug.Log("Delay complete, hiding panel");
        messageSet = false; // Reset the flag
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