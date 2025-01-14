using UnityEngine;
using UnityEngine.UI;

public class TotalScoreUpdateScript : MonoBehaviour
{

    public Text TotalScore;
    private GameController gameController;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    void Update()
    {
        if (gameController.experimentVersion == "micro2D_debug_portal")
        {
            // Display total travel distance instead of score
            float travelDistance = gameController.totalTravelDistance;
            TotalScore.text = travelDistance.ToString("F2") + " m";
        }
        else
        {
            // Display the score as normal
            int currentTotalScore = gameController.totalScore;
            TotalScore.text = currentTotalScore.ToString();
        }

        TotalScore.fontSize = 108;
    }

}