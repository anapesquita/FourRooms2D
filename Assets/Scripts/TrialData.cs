﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TrialData
{
    /// <summary>
    /// This script contains all the data to be saved per trial
    /// Author: Hannah Sheahan
    /// Date: 31/10/2018
    /// </summary>

    // Static per trial data
    public int trialNumber = 0;
    public List<int> trialListIndex = new List<int>();      // which trial attempt

    //public int mapIndex;   // this causes some complications (because SceneManager only manages active/loaded scenes, so .buildIndex throws -1)
    public string mapName;
    public bool doubleRewardTask;
    public bool freeForage;
    public string rewardType;
    public bool[] bridgeStates;
    public QuestionData debriefQuestion;
    public string debriefResponse;
    public float debriefResponseTime;

    public Vector3 playerSpawnLocation;
    public Vector3 playerSpawnOrientation;
    public Vector3[] rewardPositions;
    public Vector3[] presentPositions;
    public string[] controlStateOrder;
    public bool computerAgentCorrect;

    // trial event times
    public float maxMovementTime;
    public float trialScore;
    public List<float> firstMovementTime = new List<float>();       // time until first star collected (kept as list to account for error trials)
    public List<float> totalMovementTime = new List<float>();       // time until second star collected


    // trial configuration times
    public float preDisplayCueTime;
    public float displayCueTime;
    public float goCueDelay;
    public float minDwellAtReward;
    public float preRewardAppearTime;
    public float displayMessageTime;
    public float errorDwellTime;
    public float[] goalHitPauseTime;
    public float finalGoalHitPauseTime;
    public float[] hallwayFreezeTime;     // jittered & different for each doorway
    public float preFreezeTime;
    public float oneSquareMoveTime;
    public float minTimeBetweenMoves;
    public float blankTime;               // jittered
    public float animationTime;

    // trial error flags
    public List<bool> FLAG_cliffFallError = new List<bool>();
    public List<bool> FLAG_trialTimeout = new List<bool>();
    public List<bool> FLAG_trialError = new List<bool>();
    public List<bool> FLAG_dataWritingError = new List<bool>();
    public List<bool> FLAG_fullScreenModeError = new List<bool>();
    public List<bool> FLAG_frameRateError = new List<bool>();

    // Tracking data
    public List<string> stateTransitions = new List<string>();
    public List<string> timeStepTrackingData = new List<string>();
    public List<string> giftWrapStateTransitions = new List<string>();
    public List<string> controlStateTransitions = new List<string>();

    // Key choice tracking
    public string keyChoice = "";  // Will store "silver" or "gold"
    public bool wasKeyChoiceSuccessful = false;  // Tracks if the chosen key resulted in doubled points

    public Vector3 characterSpawnLocation;     // Where the character actually spawned
    public bool portalUsedBeforeTarget;        // Whether any portal was used before reaching target
    public string portalUsedType = "none";     // Which portal was used ("none", "first", or "second")
    public float totalTravelDistance = 0f;     // Total distance traveled from spawn to target

}