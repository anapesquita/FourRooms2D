using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TotalScoreUpdateScript : MonoBehaviour {

    public Text TotalScore;
    public Text ScoreUpdate;
    private int currentTotalScore;
    private int trialScore;

    // ********************************************************************** //

    void Update () 
    {
        currentTotalScore = GameController.control.totalScore;
        TotalScore.text = currentTotalScore.ToString();
        Debug.Log("-----UpdatedTotalScore:" + currentTotalScore);
        ScoreUpdate.fontSize = 108;
        TotalScore.fontSize = 108;

        // When the total score updates make it flash cyan
        if (GameController.control.flashTotalScore)
        {
            // show how much the score will be updated by
            ScoreUpdate.color = Color.white;
            // AP ScoreUpdate.fontSize = 24;
            // AP --
            ScoreUpdate.fontSize = 108;
            // -- AP

            trialScore = GameController.control.trialScore;
            if (trialScore>0)
            {
                //AP ScoreUpdate.text = "+" + trialScore.ToString();
                ScoreUpdate.text = "";
                TotalScore.color = Color.cyan;  // flash cyan since +ve update
                // AP TotalScore.fontSize = 50;
                // AP --
                TotalScore.fontSize = 108;
                // -- AP
            }
            else
            {
                //AP ScoreUpdate.text = trialScore.ToString();
                ScoreUpdate.text = "";

                TotalScore.color = Color.red;  // flash red since -ve update
                //AP TotalScore.fontSize = 50;
                // AP --
                TotalScore.fontSize = 108;
                // -- AP
            }

        }
        else
        {
            TotalScore.color = Color.white;
            //AP TotalScore.fontSize = 36;
            // AP --
            TotalScore.fontSize = 108;
            // -- AP
            ScoreUpdate.text = "";
        }
    }

}
