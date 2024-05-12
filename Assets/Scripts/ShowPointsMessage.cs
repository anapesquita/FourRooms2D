using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShowPointsMessage : MonoBehaviour
{
    public Text PointsMessage;

    // ********************************************************************** //

    void Update()
    {

        // When the total score updates make it flash cyan
        if (GameController.control.flashPoints)
        {
            //AP CelebratoryMessage.fontSize = 36;
            //AP --
            PointsMessage.fontSize = 36 * 3;
            //--AP
            PointsMessage.color = Color.white;
            Debug.Log("About to show TEST");
            PointsMessage.text = "You gained " + GameController.control.trialScore + " points"; //+ GameController.control.trialScore;
        }
        else
        {
            PointsMessage.text = "";
        }
    }
}
