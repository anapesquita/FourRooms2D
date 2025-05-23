﻿using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

// Updated for 2D collision detection by HRS (14/05/2019)

public class RewardHitScript : MonoBehaviour
{
    private Timer rewardTimer;
    private bool rewardHit = false;
    private bool rewardUncovered = false;
    private int rewardIndex;
    private Vector3 rewardPosition;
    private Vector3[] presentPositions;
    private int presentCoveringIndex = 0;
    private CircleCollider2D rewardCollider;

    // ********************************************************************** //

    void Start()
    {
        rewardIndex = GetComponent<SpawnRewardLocation>().rewardIndex;
        rewardCollider = GetComponent<CircleCollider2D>();

        rewardTimer = new Timer();
        rewardTimer.Reset();
        rewardPosition = GameController.control.rewardSpawnLocations[rewardIndex];
        presentPositions = GameController.control.presentPositions;

        for (int i=0; i < presentPositions.Length; i++) 
        { 
            if (presentPositions[i] == rewardPosition) 
            {
                presentCoveringIndex = i;
            }
        }
        Debug.Log("the present covering reward " + rewardIndex + "is present no. " + presentCoveringIndex);
    }

    // ********************************************************************** //

    void Update()
    {

        if (!rewardUncovered) // start the counter once reward uncovered, so that we dont provide the nice ding sound too early
        {
            rewardUncovered = (GameController.control.giftWrapState[presentCoveringIndex] == 0) ? true : false;  // has the covering boulder been removed?

            if (rewardUncovered) 
            {
                rewardTimer.Reset();   // record entry time
                rewardHit = true;
            }
        }

        if ((rewardTimer.ElapsedSeconds() > GameController.control.minDwellAtReward) && (rewardHit))
        {
            rewardCollider.enabled = false;  // disabled the reward collidor so we dont accidentally trigger it twice.
            GameController.control.StarFound();
            //GameController.control.AnimateRewardOnHit(rewardIndex);  // for making reward animate at location of reward hit

            if ( (rewardTimer.ElapsedSeconds() > (GameController.control.minDwellAtReward + GameController.control.animationTime)) && (rewardHit) ) 
            {
                rewardHit = false;   // Note: be careful that the reward doesnt grow in a way that causes it to trigger as two collected rewards prior to it disabling.
                GameController.control.DisableRewardByIndex(rewardIndex);
            }
        }
    }
    // ********************************************************************** //

}
