using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CelebrateBothRewardsHit : MonoBehaviour
{
    public Text CelebratoryMessage;

    // ********************************************************************** //

    void Update()
    {

        // When the total score updates make it flash cyan
        if (GameController.control.flashCongratulations)
        {
            //AP CelebratoryMessage.fontSize = 36;
            //AP --
            CelebratoryMessage.fontSize = 36*3;
            //--AP
            CelebratoryMessage.color = Color.white;
            //AP CelebratoryMessage.text = "Well done! You found both rewards!";
            CelebratoryMessage.text = "";
        }
        else
        {
            CelebratoryMessage.text = "";
        }
    }

}
