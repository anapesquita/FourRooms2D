﻿using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;


public class HideOrDisplayReward : MonoBehaviour {

	public GameObject reward;
    private int rewardIndex;

    private void Start()
    {
        rewardIndex = reward.GetComponent<SpawnRewardLocation>().rewardIndex;
    }


    private void Update()
    {
        //Debug.Log("rewardsVisible length is " + GameController.control.rewardsVisible.Length);
        reward.SetActive(GameController.control.rewardsVisible[rewardIndex]); 
    }
}
