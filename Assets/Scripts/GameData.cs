﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public string experimentVersion;
    public string participantID;
    public string participantAge;
    public int participantGender;
    public string confirmationCode;
    public string participantFeedback;
    public bool transferCounterbalance;
    public bool intermingledTrials;
    public int totalTrials;
    public float dataRecordFrequency;
    public float restbreakDuration;
    public float getReadyDuration;
    public float totalExperimentTime;

    public List<int> trialList = new List<int>();
    public List<float> scannerTriggerTimes = new List<float>();

    public TrialData[] allTrialData;


    // ********************************************************************** //
    // Use a constructor
    public GameData(int trials)
    {
        //  initialize array of trials, and instantiate each trial in the array
        allTrialData = new TrialData[trials];
        for (int i = 0; i < allTrialData.Length; i++)
        {
            allTrialData[i] = new TrialData();
        }
    }
}
