using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PresentRevealScript : MonoBehaviour
{
    public GameObject present;
    public int presentIndex;

    void Start()
    {
        if (GameController.control.experimentVersion == "micro2D_debug_portal")
        {
            // In debug mode, only show boulder 0 at reward position
            if (presentIndex == 0)
            {
                // Place boulder at reward location (3,4)
                transform.position = new Vector3(3f, 4f, 0f);
            }
            else
            {
                // Hide all other boulders
                gameObject.SetActive(false);
            }
        }
        else
        {
            // Normal mode - use configured spawn locations
            transform.position = GameController.control.presentPositions[presentIndex];
        }
    }
}