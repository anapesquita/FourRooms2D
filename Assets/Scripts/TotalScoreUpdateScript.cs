using UnityEngine;
using UnityEngine.UI;

public class TotalScoreUpdateScript : MonoBehaviour
{

    public Text TotalScore;
    //public Text TotalScoreMessage;
    private GameController gameController;
    private int travelDistance_int;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    void Update()
    {
        if (gameController.experimentVersion == "micro2D_debug_portal")
        {
            // Display total travel distance instead of score
            //TotalScoreMessage.text = "Total Steps";
            float travelDistance = gameController.totalTravelDistance;
            travelDistance_int= Mathf.RoundToInt(travelDistance);
            TotalScore.text = travelDistance_int.ToString() + " steps";
        }
        else
        {
            // Display the score as normal
            //TotalScoreMessage.text = "Total Score";
            int currentTotalScore = gameController.totalScore;
            TotalScore.text = currentTotalScore.ToString();
        }

        TotalScore.fontSize = 108;
    }

}