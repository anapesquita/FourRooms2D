using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenMessageScript : MonoBehaviour {

    public Text screenMessage;
	
	void Update () 
    {
        // update the text gameobject's message in-line with the FSM
        //AP screenMessage.fontSize = 30;
        //AP --
        screenMessage.fontSize = 30*3;
        //--AP
        screenMessage.text = GameController.control.textMessage;
    }
}
