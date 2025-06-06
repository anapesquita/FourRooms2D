﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;


public class ExperimentConfig
{
    /// <summary>
    /// This script contains all the experiment configuration details
    /// e.g. experiment type, trial numbers, ordering and randomisation, trial 
    /// start and end locations. 
    /// Notes:  variables should eventually be turned private. Some currently public for ease of communication with DataController.
    /// Author: Hannah Sheahan, sheahan.hannah@gmail.com
    /// Date: 08/11/2018
    /// </summary>


    // Scenes/mazes
    private const int setupAndCloseTrials = 7;     // Note: there must be 8 extra trials in trial list to account for Persistent, InformationScreen, BeforeStartingScreen, ConsentScreen, StartScreen, Instructions, QuestionnaireScreen and Exit 'trials'.
    private const int postTrials = 1;
    private const int restbreakOffset = 1;         // Note: makes specifying restbreaks more intuitive
    private const int getReadyTrial = 1;           // Note: this is the get ready screen after the practice
    private const int setupTrials = setupAndCloseTrials - postTrials;
    private int totalTrials;
    private int practiceTrials;
    private int nDebreifQuestions;
    private int restFrequency;
    private int nbreaks;
    private string[] trialMazes;
    private string[] possibleMazes;                // the existing mazes/scenes that can be selected from
    private int sceneCount;
    private int roomSize;
    private float playerZposition;
    private float rewardZposition;
    public bool[][] bridgeStates;                   // whether the 4 different bridges are ON (active) or OFF (a hole in the floor)

    // Control state ordering (human/computer)
    public string[][] controlStateOrder;
    public bool[] computerAgentCorrect;

    // Positions and orientations
    private Vector3 mazeCentre;
    private Vector3[] possiblePlayerPositions;
    private string[] playerStartRooms;
    private string[] star1Rooms;
    private string[] star2Rooms;
    private Vector3[] playerStartPositions;
    private Vector3[] playerStartOrientations;
    private Vector3 spawnOrientation;

    private Vector3[] possibleRewardPositions;
    private bool[] presentPositionHistory1;
    private bool[] presentPositionHistory2;
    private Vector3[][] rewardPositions;

    private Vector3[] blueRoomPositions;
    private Vector3[] redRoomPositions;
    private Vector3[] yellowRoomPositions;
    private Vector3[] greenRoomPositions;
    private Vector3[] spawnedPresentPositions;

    private Vector3[] blueRoomStartPositions;
    private Vector3[] redRoomStartPositions;
    private Vector3[] yellowRoomStartPositions;
    private Vector3[] greenRoomStartPositions;


    private Vector3[] bluePresentPositions;
    private Vector3[] redPresentPositions;
    private Vector3[] yellowPresentPositions;
    private Vector3[] greenPresentPositions;
    public Vector3[][] presentPositions;

    // Counterbalancing
    public bool transferCounterbalance = false;  // False = (cheese and peanut have the same covariance); True = (cheese and martinis have the same covariance)
    public bool wackyColours = false;            // False = (red, blue, green, yellow); True = (turquoise, pink, white, orange)
    public bool intermingledTrials = false;      // False = trial sequence is blocked by reward context. True = randomly intermingled rewards.

    // Rewards
    private bool[] doubleRewardTask;         // if there are two stars to collect: true, else false
    private bool[] freeForage;               // array specifying whether each trial was free foraging or not i.e. many rewards or just 2
    private const int ONE_STAR = 0;
    private const int TWO_STARS = 1;
    private string[] possibleRewardTypes; 
    private string[] rewardTypes;             // diamond or gold? (martini or beer)
    public int numberPresentsPerRoom;

    // Timer variables (public since fewer things go wrong if these are changed externally, since this will be tracked in the data, but please don't...)
    public float[] maxMovementTime;
    public float preDisplayCueTime;
    public float[][] goalHitPauseTime;
    public float finalGoalHitPauseTime;
    public float displayCueTime;
    public float goCueDelay;
    public float minDwellAtReward;
    public float displayMessageTime;
    public float errorDwellTime;
    public float restbreakDuration;
    public float getReadyDuration;
    public float[][] hallwayFreezeTime;      // jittered random per door per trial
    public float preFreezeTime;
    private float dataRecordFrequency;       // NOTE: this frequency is referred to in TrackingScript.cs for player data and here for state data
    public float oneSquareMoveTime;
    public float minTimeBetweenMoves;
    public float[] blankTime;
    public float animationTime;
    public float preRewardAppearTime;

    // Debriefing question and answer data
    public QuestionData[] debriefQuestions;                              // final order of questions we WILL include
    public List<QuestionData> allQuestions = new List<QuestionData>();    // all possible questions that we could include

    // Randomisation of trial sequence
    public System.Random rand = new System.Random();

    // Preset experiments
    public string experimentVersion;
    private int nExecutedTrials;            // to be used in micro_debug mode only

    // ********************************************************************** //
    // Use a constructor to set this up
    public ExperimentConfig() 
    {
        // Experiments with training blocked by context

        
        //experimentVersion = "nav2D_reversal_2cues";
        //experimentVersion = "nav2D_probablistic";
        experimentVersion = "micro2D_debug_portal";
        //experimentVersion = "nav2D_separation";

        //experimentVersion = "mturk2D_cheesewatermelon";     // ***HRS note that if you do wacky colours youll have to change the debrief question text which mentions room colours
        //experimentVersion = "mturk2D_day3_intermingled";
        //experimentVersion = "mturk2D_peanutmartini";
        //experimentVersion = "mturk2D_cheesewatermelon_wackycolours";
        //experimentVersion = "mturk2D_peanutmartini_wackycolours";
        //experimentVersion = "scannertask_cheese";   // be careful with adding extra practice trials between scan runs though (dont have extra practice)
        //experimentVersion = "scannertask_peanut";   // HRS used for training on day1, so do not use for testing in scanner
        //experimentVersion = "scannertask_banana";
        //experimentVersion = "scannertask_avocado";
        //experimentVersion = "mapping_practice";
        // ------------------------------------------

        // Set these variables to define your experiment:
        switch (experimentVersion)
        {

            case "mapping_practice":         // ---- a 5 min practice during brain mapping ---- //
                nDebreifQuestions = 0;
                practiceTrials = 8 + getReadyTrial;
                totalTrials = 0 + setupAndCloseTrials + practiceTrials + nDebreifQuestions;        // accounts for the Persistent, StartScreen and Exit 'trials'
                restFrequency = 16 + restbreakOffset;                               // Take a rest after this many normal trials
                restbreakDuration = 30.0f;                                          // how long are the imposed rest breaks?

                break;

            case "scannertask_cheese":       // ---- The fMRI scanning task: 32 trial run A ----//
                nDebreifQuestions = 0;
                practiceTrials = 0 + getReadyTrial;
                totalTrials = 32 + setupAndCloseTrials + practiceTrials + nDebreifQuestions;        // accounts for the Persistent, StartScreen and Exit 'trials'
                restFrequency = 16 + restbreakOffset;                               // Take a rest after this many normal trials
                restbreakDuration = 30.0f;                                          // how long are the imposed rest breaks?
                break;

            case "scannertask_peanut":       // ---- The fMRI scanning task: 32 trial run B ----//
                nDebreifQuestions = 0;
                practiceTrials = 0 + getReadyTrial;
                totalTrials = 32 + setupAndCloseTrials + practiceTrials + nDebreifQuestions;        // accounts for the Persistent, StartScreen and Exit 'trials'
                restFrequency = 16 + restbreakOffset;                               // Take a rest after this many normal trials
                restbreakDuration = 30.0f;                                          // how long are the imposed rest breaks?
                break;

            case "scannertask_banana":       // ---- The fMRI scanning task: 32 trial run B ----//
                nDebreifQuestions = 0;
                practiceTrials = 0 + getReadyTrial;
                totalTrials = 32 + setupAndCloseTrials + practiceTrials + nDebreifQuestions;        // accounts for the Persistent, StartScreen and Exit 'trials'
                restFrequency = 16 + restbreakOffset;                               // Take a rest after this many normal trials
                restbreakDuration = 30.0f;                                          // how long are the imposed rest breaks?
                break;

            case "scannertask_avocado":       // ---- The fMRI scanning task: 32 trial run A ----//
                nDebreifQuestions = 0;
                practiceTrials = 0 + getReadyTrial;
                totalTrials = 32 + setupAndCloseTrials + practiceTrials + nDebreifQuestions;        // accounts for the Persistent, StartScreen and Exit 'trials'
                restFrequency = 16 + restbreakOffset;                               // Take a rest after this many normal trials
                restbreakDuration = 30.0f;                                          // how long are the imposed rest breaks?
                break;

            case "mturk2D_cheesewatermelon":       // ----Full 4 block learning experiment day 1-----
                nDebreifQuestions = 0; 
                practiceTrials = 2 + getReadyTrial;
                totalTrials = 16 * 4 + setupAndCloseTrials + practiceTrials + nDebreifQuestions;        // accounts for the Persistent, StartScreen and Exit 'trials'
                restFrequency = 16 + restbreakOffset;                               // Take a rest after this many normal trials
                restbreakDuration = 30.0f;                                          // how long are the imposed rest breaks?
                transferCounterbalance = false;                                     // this does nothing
                break;
            
            case "mturk2D_cheesewatermelon_wackycolours":       // ----Full 4 block learning experiment-----
                nDebreifQuestions = 0; 
                practiceTrials = 2 + getReadyTrial;
                totalTrials = 16 * 4 + setupAndCloseTrials + practiceTrials + nDebreifQuestions;        // accounts for the Persistent, StartScreen and Exit 'trials'
                restFrequency = 16 + restbreakOffset;                               // Take a rest after this many normal trials
                restbreakDuration = 30.0f;                                          // how long are the imposed rest breaks?
                transferCounterbalance = false;                                     // this does nothing
                wackyColours = true;                                                // use different colours to the peanut/martini case
                break;

            case "mturk2D_peanutmartini":       // ----Full 4 block learning experiment day 2-----
                nDebreifQuestions = 0;
                practiceTrials = 2 + getReadyTrial;
                totalTrials = 16 * 4 + setupAndCloseTrials + practiceTrials + nDebreifQuestions;        // accounts for the Persistent, StartScreen and Exit 'trials'
                restFrequency = 16 + restbreakOffset;                               // Take a rest after this many normal trials
                restbreakDuration = 30.0f;                                          // how long are the imposed rest breaks?
                transferCounterbalance = false;
                break;

            case "nav2D_separation":       // ----Full 4 block learning experiment day 2-----
                nDebreifQuestions = 0;
                practiceTrials = 2 + getReadyTrial;
                totalTrials = 8 * 4 *  2 + setupAndCloseTrials + practiceTrials + nDebreifQuestions;        // accounts for the Persistent, StartScreen and Exit 'trials'
                restFrequency = 32 + restbreakOffset;                               // Take a rest after this many normal trials
                restbreakDuration = 30.0f;                                          // how long are the imposed rest breaks?
                transferCounterbalance = false;
                break;

            case "nav2D_probablistic":       // ----Full 4 block learning experiment day 2-----
                nDebreifQuestions = 0;
                practiceTrials = 2 + getReadyTrial;
                totalTrials = 16 * 3 * 2 + setupAndCloseTrials + practiceTrials + nDebreifQuestions;        // accounts for the Persistent, StartScreen and Exit 'trials'
                restFrequency = 16 * 3 + restbreakOffset;                               // Take a rest after this many normal trials
                restbreakDuration = 30.0f;                                          // how long are the imposed rest breaks?
                transferCounterbalance = false;
                break;

            case "nav2D_reversal_2cues":       // ----Full 4 block learning experiment day 2-----

                nDebreifQuestions = 0;
                practiceTrials = 2 + getReadyTrial;
                totalTrials = 160 + setupAndCloseTrials + practiceTrials + nDebreifQuestions;        // accounts for the Persistent, StartScreen and Exit 'trials'
                restFrequency = 32 + restbreakOffset;                               // Take a rest after this many normal trials
                restbreakDuration = 30.0f;                                          // how long are the imposed rest breaks?
                break;


                //break;
            //AP----
            case "mturk2D_day3_intermingled":
                nDebreifQuestions = 0;
                practiceTrials = 2 + getReadyTrial;
                totalTrials = 16 * 4 + setupAndCloseTrials + practiceTrials + nDebreifQuestions;        // accounts for the Persistent, StartScreen and Exit 'trials'
                restFrequency = 16 + restbreakOffset;                               // Take a rest after this many normal trials
                restbreakDuration = 30.0f;                                          // how long are the imposed rest breaks?
                transferCounterbalance = true;
                break;
//---- AP
            case "mturk2D_peanutmartini_wackycolours":       // ----Full 4 block learning experiment-----
                nDebreifQuestions = 0;
                practiceTrials = 2 + getReadyTrial;
                totalTrials = 16 * 4 + setupAndCloseTrials + practiceTrials + nDebreifQuestions;        // accounts for the Persistent, StartScreen and Exit 'trials'
                restFrequency = 16 + restbreakOffset;                               // Take a rest after this many normal trials
                restbreakDuration = 30.0f;                                          // how long are the imposed rest breaks?
                transferCounterbalance = false;
                wackyColours = true;
                break;

            case "micro2D_debug_portal":            // ----Mini debugging test experiment-----
                nDebreifQuestions = 0;
                practiceTrials = 0 + getReadyTrial;
                nExecutedTrials = 64;                                         // note that this is only used for the micro_debug version
                totalTrials = nExecutedTrials + setupAndCloseTrials + practiceTrials + nDebreifQuestions;        // accounts for the Persistent, StartScreen and Exit 'trials'
                restFrequency = 64 + restbreakOffset;                            // Take a rest after this many normal trials
                restbreakDuration = 5.0f;                                       // how long are the imposed rest breaks?
                transferCounterbalance = false;
                break;

            default:
                Debug.Log("Warning: defining an untested trial sequence");
                break;
        }

        // Figure out how many rest breaks we will have and add them to the trial list
        nbreaks = Math.Max( (int)((totalTrials - setupAndCloseTrials - practiceTrials - nDebreifQuestions) / restFrequency), 0 );  // round down to whole integer
        totalTrials = totalTrials + nbreaks;
      
        // Timer variables (measured in seconds) - these can later be changed to be different per trial for jitter etc
        dataRecordFrequency = 0.04f;
        //AP getReadyDuration = 3.0f;    // how long do we have to 'get ready' after the practice, before main experiment begins?
        //AP --
        getReadyDuration = 5.0f;
        //--AP


        // Note that when used, jitters ADD to these values - hence they are minimums
        //maxMovementTime        = 60.0f;   // changed to be a function of trial number. Time allowed to collect both rewards, incl. wait after hitting first one
        preDisplayCueTime = 0.2f;   // 2.5f;    //  Decode representation of room prior to cue here
        displayCueTime = 1.5f;       //1.5f;
        goCueDelay = 0.1f; //1.0f;    //
        //goalHitPauseTime       = 1.0f;      // This will also be the amount of time between computer vs human control handovers (+ minDwellAtReward + preRewardAppearTime)
        //AP finalGoalHitPauseTime  = 1f;
        finalGoalHitPauseTime  = 1.0f; //5f;
        minDwellAtReward       = 0.1f;
        preRewardAppearTime    = 0.2f;      // I think this needs to be jittered to separate neural signals for same room diff states under a consistent policy
        displayMessageTime = 1.0f; //1.5f;     
        errorDwellTime = 1.1f; //1.5f;    // Note: should be at least as long as displayMessageTime
        // hallwayFreezeTime      = 4.0f;    // amount of time player is stuck in place with each hallway traversal. This will now be exponential-jittered
        preFreezeTime = 0.2f; //0.3f;    // should be about the same, maybe slightly longer than oneSquareMoveTime
        //blankTime              = 2.0f;    // Note: ***HRS should be jittered (blank screen time prior to trial starting)
        animationTime          = 1.0f;    // how long the reward grows for when it appears (mainly for visuals)
        numberPresentsPerRoom  = 1;       //

        // physical movement times
        oneSquareMoveTime = 0.1f; //0.2f;        // Time it will take player to move from one square to next (sec) for animation
        minTimeBetweenMoves = 0.2f;//0.4f;      // How much time between each allowable move (from movement trigger) (sec) (must be >> than oneSquareMoveTime or position moves off board and path planned execution doesnt work - weird exception)

        // These variables define the environment (are less likely to be played with)
        roomSize = 4;              // rooms are each 4x4 grids. If this changes, you will need to change this code

        playerZposition = 0f;      
        rewardZposition   = 0f;
        mazeCentre      = new Vector3(0f, 0f, playerZposition);


        // Define a maze, start and goal positions, and reward type for each trial
        trialMazes = new string[totalTrials];
        playerStartRooms = new string[totalTrials];
        star1Rooms = new string[totalTrials];
        star2Rooms = new string[totalTrials];
        playerStartPositions = new Vector3[totalTrials];
        playerStartOrientations = new Vector3[totalTrials];
        rewardPositions = new Vector3[totalTrials][];
        doubleRewardTask = new bool[totalTrials];
        freeForage = new bool[totalTrials];
        rewardTypes = new string[totalTrials];
        presentPositions = new Vector3[totalTrials][];
        maxMovementTime = new float[totalTrials];
        bridgeStates = new bool[totalTrials][];
        controlStateOrder = new string[totalTrials][];
        computerAgentCorrect = new bool[totalTrials];
        hallwayFreezeTime = new float[totalTrials][];
        goalHitPauseTime = new float[totalTrials][];
        blankTime = new float[totalTrials];

        // make space for the debriefing questions and answers at the end
        debriefQuestions = new QuestionData[totalTrials];
        for (int i = 0; i < totalTrials; i++)
        {
            debriefQuestions[i] = new QuestionData(0);
        }

        // Generate a list of all the possible (player or star) spawn locations
        GeneratePossibleSettings();

        // Define the start up menu and exit trials.   Note:  the other variables take their default values on these trials
        trialMazes[0] = "Persistent";
        //AP --
        trialMazes[2] = "InformationScreen";
        trialMazes[1] = "BeforeStartingScreen";
        //--AP
        trialMazes[3] = "ConsentScreen";
        trialMazes[4] = "StartScreen";
        trialMazes[5] = "InstructionsScreen";
        trialMazes[setupTrials + practiceTrials-1] = "GetReady";
        trialMazes[totalTrials - 1] = "Exit";


        // Generate the trial randomisation/list that we want.   Note: Ensure this is aligned with the total number of trials
        int nextTrial = System.Array.IndexOf(trialMazes, null);

        // Add in the practice trials
        AddPracticeTrials(nextTrial, practiceTrials-1);

        // Find the next trial that hasnt been specified yet to start defining the rest of our trial sequence
        nextTrial = System.Array.IndexOf(trialMazes, null);

        // Define the full trial sequence
        switch (experimentVersion)
        {
            case "nav2D_probablistic":

                nextTrial = AddRevLearnBlock_v1(nextTrial);
                nextTrial = RestBreakHere(nextTrial);

                nextTrial = AddRevLearnBlock_v1(nextTrial);
              
                break;

            case "nav2D_reversal_2cues":   //----To be performed day after learning experiment: 4 block transfer experiment (1hr)-----
                                                 //----Transfer block 1
                Debug.Log("Inside the experimentVersion");

                //----Training block 1
                nextTrial = AddTrainingBlock_v2(nextTrial);

                //---- training block 2
                nextTrial = AddTrainingBlock_v2(nextTrial);
                nextTrial = RestBreakHere(nextTrial);

                ////---- testing block 1
                nextTrial = AddTestingBlock_v2(nextTrial);

                ////---- testing block 2
                nextTrial = AddTestingBlock_v2_switch(nextTrial);

                ////---- testing block 3
                nextTrial = AddTestingBlock_v2(nextTrial);

                ////---- testing block 4
                nextTrial = AddTestingBlock_v2_switch(nextTrial);
                nextTrial = RestBreakHere(nextTrial);

                ////---- testing block 3
                nextTrial = AddTestingBlock_v2(nextTrial);

                ////---- testing block 4
                nextTrial = AddTestingBlock_v2_switch(nextTrial);



                break;

            case "scannertask_cheese":

                // shuffled contexts for 2 runs
                nextTrial = AddTwoScannerRuns(nextTrial, "cheese", "watermelon");
                break;

            case "scannertask_peanut":
                // NOTE *HRS this context is used for day1 training so do not use it for testing
                // shuffled contexts for 2 runs
                nextTrial = AddTwoScannerRuns(nextTrial, "peanut", "martini");
                break;

            case "scannertask_banana":

                // shuffled contexts for 2 runs
                nextTrial = AddTwoScannerRuns(nextTrial, "mushroom", "banana");
                break;

            case "scannertask_avocado":

                // shuffled contexts for 2 runs
                nextTrial = AddTwoScannerRuns(nextTrial, "pineapple", "avocado");
                break;

            case "mturk2D_cheesewatermelon":// ----Full 4 block learning experiment-----
                //---- training block 1
                nextTrial = AddTrainingBlock(nextTrial);
                nextTrial = RestBreakHere(nextTrial);

                //---- training block 2
                nextTrial = AddTrainingBlock(nextTrial);
                nextTrial = RestBreakHere(nextTrial);

                //---- training block 3
                nextTrial = AddTrainingBlock(nextTrial);
                nextTrial = RestBreakHere(nextTrial);

                //---- training block 4
                nextTrial = AddTrainingBlock(nextTrial);

                break;
            case "mturk2D_cheesewatermelon_wackycolours":  
                   
                //---- training block 1
                nextTrial = AddTrainingBlock(nextTrial);
                nextTrial = RestBreakHere(nextTrial);                  

                //---- training block 2
                nextTrial = AddTrainingBlock(nextTrial);
                nextTrial = RestBreakHere(nextTrial);                   

                //---- training block 3
                nextTrial = AddTrainingBlock(nextTrial);
                nextTrial = RestBreakHere(nextTrial);                   

                //---- training block 4
                nextTrial = AddTrainingBlock(nextTrial);

                break;

            case "mturk2D_peanutmartini":  // ----To be performed day after learning experiment: 4 block transfer experiment (1hr)-----
                                           //---- transfer block 1
                                           //---- training block 1
                nextTrial = AddTrainingBlockDay2(nextTrial);
                nextTrial = RestBreakHere(nextTrial);

                //---- training block 2
                //nextTrial = AddTrainingBlockDay2(nextTrial);
                //nextTrial = RestBreakHere(nextTrial);

                //---- training block 3
                //nextTrial = AddTrainingBlockDay2(nextTrial);
                //nextTrial = RestBreakHere(nextTrial);

                //---- training block 4
                //nextTrial = AddTrainingBlockDay2(nextTrial);

                break;
            //AP----

            case "nav2D_separation":  // ----To be performed day after learning experiment: 4 block transfer experiment (1hr)-----
                                           //---- transfer block 1
                                           //---- training block 1
                nextTrial = AddTrainingBlock_separation(nextTrial);
                nextTrial = RestBreakHere(nextTrial);

                nextTrial = AddTestingBlock_separation(nextTrial);

                //---- training block 2
                //nextTrial = AddTrainingBlockDay2(nextTrial);
                //nextTrial = RestBreakHere(nextTrial);

                //---- training block 3
                //nextTrial = AddTrainingBlockDay2(nextTrial);
                //nextTrial = RestBreakHere(nextTrial);

                //---- training block 4
                //nextTrial = AddTrainingBlockDay2(nextTrial);

                break;
            //AP----
            case "mturk2D_day3_intermingled":

                if (rand.Next(2) == 0)   // randomise whether the watermelon or cheese sub-block happens first
                {
                    //---- transfer block 1
                    nextTrial = AddIntermTransferBlock_A(nextTrial);
                    nextTrial = RestBreakHere(nextTrial);

                    //---- transfer block 2
                    nextTrial = AddIntermTransferBlock_A(nextTrial);
                    nextTrial = RestBreakHere(nextTrial);

                    //---- transfer block 3
                    nextTrial = AddIntermTransferBlock_B(nextTrial);
                    nextTrial = RestBreakHere(nextTrial);

                    //---- transfer block 4
                    nextTrial = AddIntermTransferBlock_B(nextTrial);
                }
                else
                {
                    //---- transfer block 1
                    nextTrial = AddIntermTransferBlock_B(nextTrial);
                    nextTrial = RestBreakHere(nextTrial);

                    //---- transfer block 2
                    nextTrial = AddIntermTransferBlock_B(nextTrial);
                    nextTrial = RestBreakHere(nextTrial);

                    //---- transfer block 3
                    nextTrial = AddIntermTransferBlock_A(nextTrial);
                    nextTrial = RestBreakHere(nextTrial);

                    //---- transfer block 4
                    nextTrial = AddIntermTransferBlock_A(nextTrial);
                }
  
                break;

                //----AP
            case "mturk2D_peanutmartini_wackycolours":  

                //---- transfer block 1
                nextTrial = AddTransferBlock(nextTrial);
                nextTrial = RestBreakHere(nextTrial);

                //---- transfer block 2
                nextTrial = AddTransferBlock(nextTrial);
                nextTrial = RestBreakHere(nextTrial);

                //---- transfer block 3
                nextTrial = AddTransferBlock(nextTrial);
                nextTrial = RestBreakHere(nextTrial);

                //---- transfer block 4
                nextTrial = AddTransferBlock(nextTrial);

                break;

            case "micro2D_debug_portal":            // ----Mini debugging test experiment-----

                nextTrial = AddTrainingBlock_micro(nextTrial, nExecutedTrials);
                break;

            default:
                Debug.Log("Warning: defining either entirely practice trials, or an untested trial sequence");
                break;
        }

        AddDebriefQuestions(nextTrial);

        // For debugging: print out the final trial sequence in readable text to check it looks ok
        PrintTrialSequence();

    }

    public string GetExperimentVersion()
    {
        return experimentVersion;
    }

    // ********************************************************************** //

    private int AddTwoScannerRuns(int nextTrial, string contextA, string contextB) 
    {
        // vertical then horizontal

        //---- test context A1
        int firstTrial = nextTrial;
        nextTrial = AddfMRITrainingBlock(nextTrial, contextA);
        nextTrial = RestBreakHere(nextTrial);

        //---- test context A2
        nextTrial = AddfMRITrainingBlock(nextTrial, contextB);
        Debug.Log("firstTrial: " + firstTrial);
        Debug.Log("nextTrial: " + nextTrial);
        // Reshuffle the order of the trials for these two contexts, keeping counterbalancing
        ReshuffleTrialOrder(firstTrial, nextTrial - firstTrial);

        return nextTrial;
    }

    // ********************************************************************** //

    private void PrintTrialSequence()
    {
        // This function is for debugging/checking the final trial sequence by printing to console
        for (int trial = 0; trial < totalTrials; trial++)
        {
            Debug.Log("Trial " + trial + ", Maze: " + trialMazes[trial] + ", Reward type: " + rewardTypes[trial]);
            Debug.Log("Start room: " + playerStartRooms[trial] + ", First reward room: " + star1Rooms[trial] + ", Second reward room: " + star2Rooms[trial]);
            Debug.Log("--------");
        }
    }

    // ********************************************************************** //

    private void AddPracticeTrials(int nextTrial, int numPracticeTrials)
    {
        if (numPracticeTrials > 0) 
        { 
            bool freeForageFLAG = false;
            SingleContextfMRIPracticeBlock(nextTrial, numPracticeTrials, "banana", 0, freeForageFLAG);

            for (int trial = nextTrial; trial < numPracticeTrials + nextTrial; trial++) 
            {
                Debug.Log("Adding another practice trial");
                trialMazes[trial] = "Practice";   // reset the maze for a practice trial (good for marking in our datafile too)
            }
        }
    }

    // ********************************************************************** //

    private void AddDebriefQuestions(int nextTrial) 
    {
        // Set up a list of all debreifing questions you want to ask and mark which are actually correct 
        // based on counterbalancing and which reward types were shown.

        int answerOrder;
        int nPossibleAnswers = 2;
        QuestionData oneQuestion;
        string[] rewards = new string[2];
        string[] rooms = new string[4];

        if (experimentVersion.Contains("peanut"))
        {
            rewards[0] = "peanut";
            rewards[1] = "martini";
        }
        else
        {
            rewards[0] = "cheese";
            rewards[1] = "watermelon";
        }

        if (wackyColours) 
        {
            rooms[0] = "lavender";
            rooms[1] = "pink";
            rooms[2] = "turquoise";
            rooms[3] = "orange";
        }
        else 
        {
            rooms[0] = "yellow";
            rooms[1] = "green";
            rooms[2] = "red";
            rooms[3] = "blue";
        }


        // ---- Question 1 ---
        QuestionData questiondata = new QuestionData(nPossibleAnswers);

        answerOrder = rand.Next(nPossibleAnswers);
        questiondata.questionText = "You have just found a " + rewards[0] + " in the " + rooms[1] + " room.\n Which room will the other " + rewards[0] + " be in?";
        questiondata.stimulus = "";
        questiondata.answers[answerOrder].answerText = (!transferCounterbalance) ? rooms[2]: rooms[0];       // correct
        questiondata.answers[1 - answerOrder].answerText = (!transferCounterbalance) ? rooms[0] : rooms[2];  // incorrect
        questiondata.answers[answerOrder].isCorrect = true;
        allQuestions.Add(questiondata);


        // ---- Question 2 ---
        questiondata = new QuestionData(nPossibleAnswers);

        answerOrder = rand.Next(nPossibleAnswers);
        questiondata.questionText = "You have just found a " + rewards[0] + " in the " + rooms[0] + " room.\n Which room will the other " + rewards[0] + " be in?";
        questiondata.stimulus = "";
        questiondata.answers[answerOrder].answerText = (!transferCounterbalance) ? rooms[3] : rooms[1];       // correct
        questiondata.answers[1 - answerOrder].answerText = (!transferCounterbalance) ? rooms[1] : rooms[3];  // incorrect
        questiondata.answers[answerOrder].isCorrect = true;
        allQuestions.Add(questiondata);

        // ---- Question 3 ---
        questiondata = new QuestionData(nPossibleAnswers);

        answerOrder = rand.Next(nPossibleAnswers);
        questiondata.questionText = "You have just found a " + rewards[1] + " in the " + rooms[2] + " room.\n Which room will the other " + rewards[1] + " be in?";
        questiondata.stimulus = "";
        questiondata.answers[answerOrder].answerText = (!transferCounterbalance) ? rooms[3] : rooms[1];       // correct
        questiondata.answers[1 - answerOrder].answerText = (!transferCounterbalance) ? rooms[1] : rooms[3];  // incorrect
        questiondata.answers[answerOrder].isCorrect = true;
        allQuestions.Add(questiondata);

        // ---- Question 4 ---
        questiondata = new QuestionData(nPossibleAnswers);

        answerOrder = rand.Next(nPossibleAnswers);
        questiondata.questionText = "You have just found a " + rewards[1] + " in the " + rooms[3] + " room.\n Which room will the other " + rewards[1] + " be in?";
        questiondata.stimulus = "";
        questiondata.answers[answerOrder].answerText = (!transferCounterbalance) ? rooms[2] : rooms[0];       // correct
        questiondata.answers[1 - answerOrder].answerText = (!transferCounterbalance) ? rooms[0] : rooms[2];  // incorrect
        questiondata.answers[answerOrder].isCorrect = true;
        allQuestions.Add(questiondata);

        // ---- Question 5 ---
        questiondata = new QuestionData(nPossibleAnswers);

        answerOrder = rand.Next(nPossibleAnswers);
        questiondata.questionText = "You were looking for a " + rewards[0] + " and did NOT find one in the " + rooms[3] + " room.\n Which room should you go to next?";
        questiondata.stimulus = "";
        questiondata.answers[answerOrder].answerText = (!transferCounterbalance) ? rooms[2] : rooms[0];       // correct
        questiondata.answers[1 - answerOrder].answerText = (!transferCounterbalance) ? rooms[0] : rooms[2];  // incorrect
        questiondata.answers[answerOrder].isCorrect = true;
        allQuestions.Add(questiondata);

        // ---- Question 6 ---
        questiondata = new QuestionData(nPossibleAnswers);

        answerOrder = rand.Next(nPossibleAnswers);
        questiondata.questionText = "You were looking for a " + rewards[0] + " and did NOT find one in the " + rooms[2] + " room.\n Which room should you go to next?";
        questiondata.stimulus = "";
        questiondata.answers[answerOrder].answerText = (!transferCounterbalance) ? rooms[3] : rooms[1];       // correct
        questiondata.answers[1 - answerOrder].answerText = (!transferCounterbalance) ? rooms[1] : rooms[3];  // incorrect
        questiondata.answers[answerOrder].isCorrect = true;
        allQuestions.Add(questiondata);

        // ---- Question 7 ---
        questiondata = new QuestionData(nPossibleAnswers);

        answerOrder = rand.Next(nPossibleAnswers);
        questiondata.questionText = "You were looking for a " + rewards[1] + " and did NOT find one in the " + rooms[0] + " room.\n Which room should you go to next?";
        questiondata.stimulus = "";
        questiondata.answers[answerOrder].answerText = (!transferCounterbalance) ? rooms[3] : rooms[1];       // correct
        questiondata.answers[1 - answerOrder].answerText = (!transferCounterbalance) ? rooms[1] : rooms[3];  // incorrect
        questiondata.answers[answerOrder].isCorrect = true;
        allQuestions.Add(questiondata);

        // ---- Question 8 ---
        questiondata = new QuestionData(nPossibleAnswers);

        answerOrder = rand.Next(nPossibleAnswers);
        questiondata.questionText = "You were looking for a " + rewards[1] + " and did NOT find one in the " + rooms[1] + " room.\n Which room should you go to next?";
        questiondata.stimulus = "";
        questiondata.answers[answerOrder].answerText = (!transferCounterbalance) ? rooms[2] : rooms[0];       // correct
        questiondata.answers[1 - answerOrder].answerText = (!transferCounterbalance) ? rooms[0] : rooms[2];  // incorrect
        questiondata.answers[answerOrder].isCorrect = true;
        allQuestions.Add(questiondata);

        // ----------------------------------




        // Shuffle the question order
        int n = allQuestions.Count;

        // Perform the Fisher-Yates algorithm for shuffling array elements in place 
        for (int i = 0; i < n; i++)
        {
            int k = i + rand.Next(n - i); // select random index in array, less than n-i

            // shuffle questions to ask, keeping their associated data together
            oneQuestion = allQuestions[k];
            allQuestions[k] = allQuestions[i];
            allQuestions[i] = oneQuestion;
        }

        // Store the randomised trial order (reuse random trials if we haven't specified enough unique ones)
        for (int i = 0; i < nDebreifQuestions; i++)
        {
            oneQuestion = (i < n) ? allQuestions[i] : allQuestions[rand.Next(allQuestions.Count)];
            debriefQuestions[i + nextTrial] = oneQuestion;
            trialMazes[i + nextTrial] = "QuestionTime";
        }
    }

    // ********************************************************************** //

    private string ChooseRandomRoom()
    {
        // Choose a random room of the four rooms
        string[] fourRooms = { "blue", "yellow", "red", "green" };
        int n = fourRooms.Length;
        int ind = rand.Next(n);   // Note: for some reason c# wants this stored to do randomisation, not directly input to fourRooms[rand.Next(n)]

        return fourRooms[ind]; 
    }

    // ********************************************************************** //

    private Vector3[] ChooseNRandomPresentPositions( int nPresents, Vector3[] roomPositions )
    {
        Vector3[] positionsInRoom = new Vector3[nPresents];
        bool collisionInSpawnLocations;
        int iterationCounter = 0;
        // generate a random set of N present positions
        for (int i = 0; i < nPresents; i++)
        {
            collisionInSpawnLocations = true;
            iterationCounter = 0;
            // make sure the rewards dont spawn on top of each other
            while (collisionInSpawnLocations)
            {
                iterationCounter++;
                collisionInSpawnLocations = false;   // benefit of the doubt
                positionsInRoom[i] = roomPositions[UnityEngine.Random.Range(0, roomPositions.Length - 1)];

                for (int j = 0; j < i; j++)  // just compare to the present positions already generated
                {
                    if (positionsInRoom[i] == positionsInRoom[j])
                    {
                        collisionInSpawnLocations = true;   // respawn the present location
                    }
                }

                // implement a catchment check for the while loop
                if (iterationCounter > 40) 
                {
                    Debug.Log("There was a while loop error: D");
                    break;
                }
            }
        }
        return positionsInRoom;
    }

    // ********************************************************************** //

    private Vector3[] ChooseSingleCornerPresentPosition(Vector3[] roomPositions)
    {
        Vector3[] positionInRoom = new Vector3[1];
        Vector3 possiblePosition = new Vector3();

        // generate a single present position from the vector (the one in the furthest corner)
        possiblePosition = roomPositions[0];
        for (int i=0; i < roomPositions.Length; i++) {

            // the way our room positions are designed we always want the square with x,y = [4|-4,4|-4]
            if (possiblePosition.x < Math.Abs(roomPositions[i].x))
            {
                possiblePosition.x = roomPositions[i].x;
            }
            if (possiblePosition.y < Math.Abs(roomPositions[i].y))
            {
                possiblePosition.y = roomPositions[i].y;
            }
        }
        positionInRoom[0] = possiblePosition;
        return positionInRoom;
    }

    // ********************************************************************** //

    private Vector3[] ChooseSingleCornerStartPosition(Vector3[] roomPositions)
    {
        Vector3[] positionInRoom = new Vector3[1];
        Vector3 possiblePosition = new Vector3();

        // generate a single start position from the vector (the one in the centre-most corner)
        possiblePosition = roomPositions[0];
        for (int i = 0; i < roomPositions.Length; i++)
        {

            // the way our room positions are designed we always want the square with x,y = [1|-1,1|-1]
            if (possiblePosition.x > Math.Abs(roomPositions[i].x))
            {
                possiblePosition.x = roomPositions[i].x;
            }
            if (possiblePosition.y > Math.Abs(roomPositions[i].y))
            {
                possiblePosition.y = roomPositions[i].y;
            }
        }
        positionInRoom[0] = possiblePosition;
        return positionInRoom;
    }

    // ********************************************************************** //

    private Vector3[] ChooseNUnoccupiedPresentPositions(int trial, int nPresents, Vector3[] roomPositions)
    {
        Vector3[] positionsInRoom = new Vector3[nPresents];
        Vector3 positionInRoom = new Vector3();
        List<Vector3> spawnableRoomPositions = new List<Vector3>();
        List<Vector3> withinTrialUnsedPresentPositions = new List<Vector3>();
        bool[] positionsUsedThisTrial; 
        int index;
        int desiredPositionIndex;

        positionsUsedThisTrial = new bool[possibleRewardPositions.Length];
        for (int i = 0; i < positionsUsedThisTrial.Length; i++)
        {
            positionsUsedThisTrial[i] = false;
        }

        // generate a random set of N present positions in this room
        for (int k = 0; k < nPresents; k++)
        {
            // find the places in the room where we haven't spawned yet this block and turn them into a list
            spawnableRoomPositions.Clear();

            for (int j = 0; j < roomPositions.Length; j++)
            {
                index = Array.IndexOf(possibleRewardPositions, roomPositions[j]);

                if (!positionsUsedThisTrial[index]) 
                {
                    if (!presentPositionHistory1[index])  // (we fill this first)
                    {
                        // add to a list of unoccupied positions that can be sampled from (avoids rejection sampling)
                        spawnableRoomPositions.Add(roomPositions[j]); 
                    }
                }
            }

            // make sure the reward doesn't spawn in a place that's been occupied previously this block
            bool noValidPositions = !spawnableRoomPositions.Any();
            if (noValidPositions) 
            {
                // check the second presentPositionHistory i.e. we are filling these spots for the second time
                for (int j = 0; j < roomPositions.Length; j++)
                {
                    index = Array.IndexOf(possibleRewardPositions, roomPositions[j]);

                    if (!positionsUsedThisTrial[index])
                    {
                        if (!presentPositionHistory2[index]) // (we fill this second)
                        {
                            // add to a list of unoccupied positions that can be sampled from (avoids rejection sampling)
                            spawnableRoomPositions.Add(roomPositions[j]);
                        }
                    }
                }

                // if there are still no valid positions, something's gone wrong
                noValidPositions = !spawnableRoomPositions.Any();
                if (noValidPositions) 
                { 
                    Debug.Log("Something has gone wrong. In a 4x4 grid this should never happen. This is trial " + trial);
                }
                else 
                {
                    // sample a position that has only been used once
                    desiredPositionIndex = rand.Next(spawnableRoomPositions.Count);
                    positionInRoom = spawnableRoomPositions[desiredPositionIndex];

                    // update the history of spawn positions
                    index = Array.IndexOf(possibleRewardPositions, positionInRoom);
                    presentPositionHistory2[index] = true;
                    positionsUsedThisTrial[index] = true;
                }
            }
            else 
            {   
                // sample an unused position
                desiredPositionIndex = rand.Next(spawnableRoomPositions.Count);
                positionInRoom = spawnableRoomPositions[desiredPositionIndex];

                // update the history of spawn positions
                index = Array.IndexOf(possibleRewardPositions, positionInRoom);
                presentPositionHistory1[index] = true;
                positionsUsedThisTrial[index] = true;
            }

            positionsInRoom[k] = positionInRoom;

        }

        return positionsInRoom;
    }

    // ********************************************************************** //

    private void GeneratePresentPositions(int trial, int trialInBlock, bool freeForageFLAG)
    {
        // - If the is a 2 reward covariance trial, spawn the presents in random positions within each room.
        // - Make sure that every single square within each room have a present on it 2x within the block of 8 trials

        // presents can be at any position in the room now
        presentPositions[trial] = new Vector3[numberPresentsPerRoom * 4];
        rewardPositions[trial] = new Vector3[numberPresentsPerRoom * 4];

           
        // constrain the randomised locations for the presents to spawn in different places to before
        // Note: each index of presentPositionHistory specifies a different square in the maze. True means the square has had a present on it, False means it hasnt

        // reset the presentPositionHistory tracker
        if (trialInBlock == 0) 
        {
            presentPositionHistory1 = new bool[possibleRewardPositions.Length];  // fill this first when spawning
            presentPositionHistory2 = new bool[possibleRewardPositions.Length];  // fill this second when spawning (prevents leaving 2 slot on top of each other in final trial)
            for (int i = 0; i < presentPositionHistory1.Length; i++) 
            {
                presentPositionHistory1[i] = false;
                presentPositionHistory2[i] = false;
            }
        }
        // select reward positions based on ones that have not yet been occupied
        // ...but if there isn't a space in the room that hasnt been occupied, just spawn wherever in the room
        /*
        greenPresentPositions = ChooseNUnoccupiedPresentPositions(trial, numberPresentsPerRoom, greenRoomPositions);
        redPresentPositions = ChooseNUnoccupiedPresentPositions(trial, numberPresentsPerRoom, redRoomPositions);
        yellowPresentPositions = ChooseNUnoccupiedPresentPositions(trial, numberPresentsPerRoom, yellowRoomPositions);
        bluePresentPositions = ChooseNUnoccupiedPresentPositions(trial, numberPresentsPerRoom, blueRoomPositions);
        */

        // HRS for having a single consistent present position per room
        greenPresentPositions = ChooseSingleCornerPresentPosition(greenRoomPositions);
        redPresentPositions = ChooseSingleCornerPresentPosition(redRoomPositions);
        yellowPresentPositions = ChooseSingleCornerPresentPosition(yellowRoomPositions);
        bluePresentPositions = ChooseSingleCornerPresentPosition(blueRoomPositions);

        // HRS for having a single consistent start position in each room (in the corner opposite the present)
        greenRoomStartPositions = ChooseSingleCornerStartPosition(greenRoomPositions);
        redRoomStartPositions = ChooseSingleCornerStartPosition(redRoomPositions);
        yellowRoomStartPositions = ChooseSingleCornerStartPosition(yellowRoomPositions);
        blueRoomStartPositions = ChooseSingleCornerStartPosition(blueRoomPositions);

        // concatenate all the positions of generated presents 
        greenPresentPositions.CopyTo(presentPositions[trial], 0);
        redPresentPositions.CopyTo(presentPositions[trial], greenPresentPositions.Length);
        yellowPresentPositions.CopyTo(presentPositions[trial], greenPresentPositions.Length + redPresentPositions.Length);
        bluePresentPositions.CopyTo(presentPositions[trial], greenPresentPositions.Length + redPresentPositions.Length + yellowPresentPositions.Length);

    }

    // ********************************************************************** //

    private void GeneratePossibleSettings()
    {
        // Generate all possible spawn locations for player and stars
        possiblePlayerPositions = new Vector3[roomSize * roomSize * 4]; // we are working with 4 square rooms
        possibleRewardPositions = new Vector3[roomSize * roomSize * 4];
        blueRoomPositions = new Vector3[roomSize * roomSize];
        redRoomPositions = new Vector3[roomSize * roomSize];
        yellowRoomPositions = new Vector3[roomSize * roomSize];
        greenRoomPositions = new Vector3[roomSize * roomSize];


        // Version 2D 4x4 room positions  ***HRS later should really use this to create loop for specifying positions

        // Blue room
        int startind = 0;
        float[] XPositionsblue = { 1f, 2f, 3f, 4f };
        float[] YPositionsblue = { 1f, 2f, 3f, 4f };

        AddPossibleLocations(possiblePlayerPositions, startind, XPositionsblue, YPositionsblue, rewardZposition);
        AddPossibleLocations(possibleRewardPositions, startind, XPositionsblue, YPositionsblue, rewardZposition);
        startind = startind + roomSize * roomSize;

        // Red room
        float[] XPositionsred = { 1f, 2f, 3f, 4f };
        float[] YPositionsred = { -1f, -2f, -3f, -4f };

        AddPossibleLocations(possiblePlayerPositions, startind, XPositionsred,  YPositionsred, rewardZposition);
        AddPossibleLocations(possibleRewardPositions, startind, XPositionsred, YPositionsred, rewardZposition);
        startind = startind + roomSize * roomSize;

        // Green room
        float[] XPositionsgreen = { -1f, -2f, -3f, -4f };
        float[] YPositionsgreen = { -1f, -2f, -3f, -4f };

        AddPossibleLocations(possiblePlayerPositions, startind, XPositionsgreen,  YPositionsgreen, rewardZposition);
        AddPossibleLocations(possibleRewardPositions, startind, XPositionsgreen,  YPositionsgreen, rewardZposition);
        startind = startind + roomSize * roomSize;

        // Yellow room
        float[] XPositionsyellow = { -1f, -2f, -3f, -4f };
        float[] YPositionsyellow = { 1f, 2f, 3f, 4f };

        AddPossibleLocations(possiblePlayerPositions, startind, XPositionsyellow, YPositionsyellow, rewardZposition);
        AddPossibleLocations(possibleRewardPositions, startind, XPositionsyellow, YPositionsyellow, rewardZposition);


        // Add position arrays for locations in particular rooms
        startind = 0;
        AddPossibleLocations(blueRoomPositions, startind, XPositionsblue, YPositionsblue, rewardZposition);
        AddPossibleLocations(redRoomPositions, startind, XPositionsred, YPositionsred, rewardZposition);
        AddPossibleLocations(greenRoomPositions, startind, XPositionsgreen, YPositionsgreen, rewardZposition);
        AddPossibleLocations(yellowRoomPositions, startind, XPositionsyellow, YPositionsyellow, rewardZposition);

        // Get all the possible mazes/scenes in the build that we can work with
        sceneCount = SceneManager.sceneCountInBuildSettings;
        possibleMazes = new string[sceneCount];
        for (int i = 0; i < sceneCount; i++)
        {
            possibleMazes[i] = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
        }

        // Possible reward types
        possibleRewardTypes = new string[] { "watermelon", "cheese", "banana", "peanut" };  // ***HRS dont think this gets used
    }

    // ********************************************************************** //

    void AddPossibleLocations(Vector3[] locationVar, int startind, float[] xpositions, float[] ypositions, float zposition)
    {
        int ind = startind;
        for (int i = 0; i < roomSize; i++)
        {
            for (int j = 0; j < roomSize; j++)
            {
                locationVar[ind] = new Vector3(xpositions[i], ypositions[j], zposition);
                ind++;
            }
        }
    }

    // ********************************************************************** //

    private void GenerateControlOrder(int trial)
    {
        // HRS function currently obsolete, we are now randomising/balancing this variable

        // For now set this to be human-human control. Later we will have:
        // H-H
        // C-C
        // H-C
        // C-H
        // And this will be balanced appropriately across trials
        // AP controlStateOrder[trial] = new string[2] { "Computer", "Human" };
        // AP --
        controlStateOrder[trial] = new string[2] { "Human", "Human" };
        // -- AP

        // whether the agent chooses the closest correct or incorrect box (after searching in the first room)
        computerAgentCorrect[trial] = true;

    }

    // ********************************************************************** //

    private Vector3 findStartOrientation(Vector3 position)
    {
        /*
        // Generate a starting orientation that always makes the player look towards the centre of the environment
        Vector3 lookVector = new Vector3();
        lookVector = mazeCentre - position;

        float angle = (float)Math.Atan2(lookVector.z, lookVector.x);   // angle of the vector connecting centre and spawn location
        angle = 90 - angle * (float)(180 / Math.PI);                   // correct for where angles are measured from

        if (angle<0)   // put the view angle in the range 0 to 360 degrees
        {
            angle = 360 + angle;
        }
        spawnOrientation = new Vector3(0.0f, angle, 0.0f);
        */
        spawnOrientation = new Vector3(0f, 0f, 0f);  // ***HRS not currently used and will probs not be used for 2D version
        return spawnOrientation;
    }

    // ********************************************************************** //

    private int RestBreakHere(int firstTrial)
    {
        // Insert a rest break here and move to the next trial in the sequence

        trialMazes[firstTrial] = "RestBreak";
        return firstTrial + 1;
    }

    // ********************************************************************** //

    private int AddfMRITrainingBlock(int nextTrial, string context)
    {
        // For the fMRI task we want blocks to be 16 trials in length, and a single context at a time. 
        // Within these 16 trials we counterbalance across whether human/computer starts first

        bool freeForageFLAG = false;
        nextTrial = SingleContextfMRIBlock(nextTrial, context, freeForageFLAG);

        return nextTrial;
    }

    // ********************************************************************** //

    private int AddTrainingBlock(int nextTrial)
    {
        // Add a 16 trial training block to the trial list. Trials are randomised within each context, but not between contexts 
        bool freeForageFLAG = false;

        if (rand.Next(2) == 0)   // randomise whether the watermelon or cheese sub-block happens first
        {
            nextTrial = SingleContextDoubleRewardBlock(nextTrial, "cheese", 0, freeForageFLAG);
            nextTrial = SingleContextDoubleRewardBlock(nextTrial, "watermelon", 0, freeForageFLAG);
        }
        else
        {
            nextTrial = SingleContextDoubleRewardBlock(nextTrial, "watermelon",0, freeForageFLAG);
            nextTrial = SingleContextDoubleRewardBlock(nextTrial, "cheese", 0, freeForageFLAG);
        }
        return nextTrial;
    }

    private int AddTrainingBlock_separation(int nextTrial)
    {
        // Add a 16 trial training block to the trial list. Trials are randomised within each context, but not between contexts 
        bool freeForageFLAG = false;
        //int firstTrial = nextTrial;

        nextTrial = SingleContextDoubleRewardBlock(nextTrial, "pineapple", 1, freeForageFLAG);
        nextTrial = SingleContextDoubleRewardBlock(nextTrial, "banana", 0, freeForageFLAG);
        nextTrial = SingleContextDoubleRewardBlock(nextTrial, "mushroom", 1, freeForageFLAG);
        nextTrial = SingleContextDoubleRewardBlock(nextTrial, "avocado", 0, freeForageFLAG);

        return nextTrial;
    }

    private int AddTestingBlock_separation(int nextTrial)
    {
        // Add a 16 trial training block to the trial list. Trials are randomised within each context, but not between contexts 
        bool freeForageFLAG = false;
        int firstTrial = nextTrial;

        nextTrial = SingleContextDoubleRewardBlock(nextTrial, "pineapple", 1, freeForageFLAG);
        nextTrial = SingleContextDoubleRewardBlock(nextTrial, "banana", 0, freeForageFLAG);
        nextTrial = SingleContextDoubleRewardBlock(nextTrial, "mushroom", 1, freeForageFLAG);
        nextTrial = SingleContextDoubleRewardBlock(nextTrial, "avocado", 0, freeForageFLAG);

        ReshuffleTrialOrder(firstTrial, nextTrial - firstTrial);

        return nextTrial;
    }

    private int AddTrainingBlockDay2(int nextTrial)
    {
        // Add a 16 trial training block to the trial list. Trials are randomised within each context, but not between contexts 
        bool freeForageFLAG = false;
        //int firstTrial = nextTrial;
        if (rand.Next(2) == 0)   // randomise whether the watermelon or cheese sub-block happens first
        {
            nextTrial = SingleContextDoubleRewardBlock(nextTrial, "peanut", 0, freeForageFLAG);
            nextTrial = SingleContextDoubleRewardBlock(nextTrial, "martini", 1, freeForageFLAG);
            nextTrial = SingleContextDoubleRewardBlock(nextTrial, "peanut", 0, freeForageFLAG);
            nextTrial = SingleContextDoubleRewardBlock(nextTrial, "martini", 1, freeForageFLAG);
        }
        else
        {
            nextTrial = SingleContextDoubleRewardBlock(nextTrial, "martini", 0, freeForageFLAG);
            nextTrial = SingleContextDoubleRewardBlock(nextTrial, "peanut", 0, freeForageFLAG);
        }

        return nextTrial;
    }

    private int AddRevLearnBlock_v1(int nextTrial)
    {
        // Add a 16 trial training block to the trial list
        bool freeForageFLAG = false;

        // Create array of covariance values and shuffle them
        int[] covarianceOrder = new int[] { 0, 1, 2 };

        // Fisher-Yates shuffle algorithm
        for (int i = covarianceOrder.Length - 1; i > 0; i--)
        {
            int randomIndex = rand.Next(0, i + 1);
            // Swap
            int temp = covarianceOrder[i];
            covarianceOrder[i] = covarianceOrder[randomIndex];
            covarianceOrder[randomIndex] = temp;
        }

        // Apply the randomized covariance values in sequence
        nextTrial = SingleContextDoubleRewardBlock_rev(nextTrial, "cheese", covarianceOrder[0], freeForageFLAG);
        nextTrial = SingleContextDoubleRewardBlock_rev(nextTrial, "cheese", covarianceOrder[1], freeForageFLAG);
        nextTrial = SingleContextDoubleRewardBlock_rev(nextTrial, "cheese", covarianceOrder[2], freeForageFLAG);

        return nextTrial;
    }

    private int AddTrainingBlock_v2(int nextTrial)
    {
        // Add a 16 trial training block to the trial list. Trials are randomised within each context, but not between contexts 
        bool freeForageFLAG = false;
        //int firstTrial = nextTrial;
        if (rand.Next(2) == 0)   // randomise whether the watermelon or cheese sub-block happens first
        {
            nextTrial = SingleContextDoubleRewardBlock(nextTrial, "peanut", 0, freeForageFLAG);
            nextTrial = SingleContextDoubleRewardBlock(nextTrial, "martini", 0, freeForageFLAG);
            nextTrial = SingleContextDoubleRewardBlock(nextTrial, "cheese", 0, freeForageFLAG);
            nextTrial = SingleContextDoubleRewardBlock(nextTrial, "watermelon", 0, freeForageFLAG);
        }
        else
        {
            nextTrial = SingleContextDoubleRewardBlock(nextTrial, "peanut", 0, freeForageFLAG);
            nextTrial = SingleContextDoubleRewardBlock(nextTrial, "martini", 0, freeForageFLAG);
            nextTrial = SingleContextDoubleRewardBlock(nextTrial, "cheese", 0, freeForageFLAG);
            nextTrial = SingleContextDoubleRewardBlock(nextTrial, "watermelon", 0, freeForageFLAG);
        }

        return nextTrial;
    }

    private int AddTestingBlock_v2(int nextTrial)
    {
        // Add a 16 trial training block to the trial list. Trials are randomised within each context, but not between contexts 
        bool freeForageFLAG = false;
        int firstTrial = nextTrial;
        if (rand.Next(2) == 0)   // randomise whether the watermelon or cheese sub-block happens first
        {
            nextTrial = SingleContextDoubleRewardBlock_small(nextTrial, "peanut", 0, freeForageFLAG);
            nextTrial = SingleContextDoubleRewardBlock_small(nextTrial, "martini", 0, freeForageFLAG);
            nextTrial = SingleContextDoubleRewardBlock_small(nextTrial, "cheese", 0, freeForageFLAG);
            nextTrial = SingleContextDoubleRewardBlock_small(nextTrial, "watermelon", 0, freeForageFLAG);
        }
        else
        {
            nextTrial = SingleContextDoubleRewardBlock_small(nextTrial, "peanut", 0, freeForageFLAG);
            nextTrial = SingleContextDoubleRewardBlock_small(nextTrial, "martini", 0, freeForageFLAG);
            nextTrial = SingleContextDoubleRewardBlock_small(nextTrial, "cheese", 0, freeForageFLAG);
            nextTrial = SingleContextDoubleRewardBlock_small(nextTrial, "watermelon", 0, freeForageFLAG);
        }
        ReshuffleTrialOrder(firstTrial, nextTrial - firstTrial);

        return nextTrial;
    }

    private int AddTestingBlock_v2_switch(int nextTrial)
    {
        // Add a 16 trial training block to the trial list. Trials are randomised within each context, but not between contexts 
        bool freeForageFLAG = false;
        int firstTrial = nextTrial;

        if (rand.Next(2) == 0)   // randomise whether the watermelon or cheese sub-block happens first
        {
            nextTrial = SingleContextDoubleRewardBlock_small(nextTrial, "peanut", 1, freeForageFLAG);
            nextTrial = SingleContextDoubleRewardBlock_small(nextTrial, "martini", 1, freeForageFLAG);
            nextTrial = SingleContextDoubleRewardBlock_small(nextTrial, "cheese", 1, freeForageFLAG);
            nextTrial = SingleContextDoubleRewardBlock_small(nextTrial, "watermelon", 1, freeForageFLAG);
        }
        else
        {
            nextTrial = SingleContextDoubleRewardBlock_small(nextTrial, "peanut", 1, freeForageFLAG);
            nextTrial = SingleContextDoubleRewardBlock_small(nextTrial, "martini", 1, freeForageFLAG);
            nextTrial = SingleContextDoubleRewardBlock_small(nextTrial, "cheese", 1, freeForageFLAG);
            nextTrial = SingleContextDoubleRewardBlock_small(nextTrial, "watermelon", 1, freeForageFLAG);
        }
        ReshuffleTrialOrder(firstTrial, nextTrial - firstTrial);

        return nextTrial;
    }

    // ********************************************************************** //

    private int AddIntermTrainingBlock(int nextTrial)
    {
        // Add a 16 trial training block to the trial list. Trials are randomised within each context, but not between contexts 
        int firstTrial = nextTrial;
        bool freeForageFLAG = false;
        nextTrial = SingleContextDoubleRewardBlock(nextTrial, "cheese", 0, freeForageFLAG);
        nextTrial = SingleContextDoubleRewardBlock(nextTrial, "watermelon", 0, freeForageFLAG);

        // reshuffle the trial ordering so they are intermingled but preserve the previous arrangement of things
        ReshuffleTrialOrder(firstTrial, nextTrial-firstTrial );

        return nextTrial;
    }

    // ********************************************************************** //

    private int AddTransferBlock(int nextTrial)
    {
        // Add a 16 trial training block to the trial list. Trials are randomised within each context, but not between contexts 
        bool freeForageFLAG = false;

        if (rand.Next(2) == 0)   // randomise whether the watermelon or cheese sub-block happens first
        {
            nextTrial = SingleContextDoubleRewardBlock(nextTrial, "martini", 0, freeForageFLAG);
            nextTrial = SingleContextDoubleRewardBlock(nextTrial, "peanut", 0, freeForageFLAG);
        }
        else
        {
            nextTrial = SingleContextDoubleRewardBlock(nextTrial, "peanut", 0,freeForageFLAG);
            nextTrial = SingleContextDoubleRewardBlock(nextTrial, "martini", 0, freeForageFLAG);
        }
        return nextTrial;
    }

    // ********************************************************************** //

    private int AddIntermTransferBlock_A(int nextTrial)
    {
        // Add a 16 trial training block to the trial list. Trials are randomised within each context, but not between contexts 
        int firstTrial = nextTrial;
        bool freeForageFLAG = false;
        nextTrial = SingleContextDoubleRewardBlock(nextTrial, "pineapple", 0,freeForageFLAG);
        nextTrial = SingleContextDoubleRewardBlock(nextTrial, "avocado", 0,freeForageFLAG);

        // reshuffle the trial ordering so they are intermingled but preserve the previous arrangement of things
        ReshuffleTrialOrder(firstTrial, nextTrial - firstTrial);

        return nextTrial;
    }

    private int AddIntermTransferBlock_B(int nextTrial)
    {
        // Add a 16 trial training block to the trial list. Trials are randomised within each context, but not between contexts 
        int firstTrial = nextTrial;
        bool freeForageFLAG = false;
        nextTrial = SingleContextDoubleRewardBlock(nextTrial, "mushroom", 0,freeForageFLAG);
        nextTrial = SingleContextDoubleRewardBlock(nextTrial, "banana", 0,freeForageFLAG);

        // reshuffle the trial ordering so they are intermingled but preserve the previous arrangement of things
        ReshuffleTrialOrder(firstTrial, nextTrial - firstTrial);

        return nextTrial;
    }
    // ********************************************************************** //

    private int AddFreeForageBlock(int nextTrial, string rewardSet)
    {
        // Add a 16 trial free-foraging block in which all boxes are rewarded, to the trial list. Trials are randomised within each context, but not between contexts. 
        bool freeForageFLAG = true;

        if (rewardSet == "cheeseandwatermelon") 
        { 
            if (rand.Next(2) == 0)   // randomise whether the watermelon or cheese sub-block happens first
            {
                nextTrial = SingleContextDoubleRewardBlock(nextTrial, "watermelon", 0,freeForageFLAG);
                nextTrial = SingleContextDoubleRewardBlock(nextTrial, "cheese", 0,freeForageFLAG);
            }
            else
            {
                nextTrial = SingleContextDoubleRewardBlock(nextTrial, "cheese", 0,freeForageFLAG);
                nextTrial = SingleContextDoubleRewardBlock(nextTrial, "watermelon", 0,freeForageFLAG);
            }
        }

        return nextTrial;
    }
    // ********************************************************************** //

    private int AddTrainingBlock_micro(int nextTrial, int numberOfTrials)
    {
        if (experimentVersion == "micro2D_debug_portal")
        {
            GeneratePortalTrialSequence(nextTrial, 64); // Generate 64 trials with new spawn system
            return nextTrial + 64;
        }
        else
        {
            return SingleRewardBlock_micro(nextTrial, "mushroom", 0, numberOfTrials);
        }
    }
    // ********************************************************************** //

    private int SingleContextfMRIPracticeBlock(int firstTrial, int blockLength, string context, int covariance, bool freeForageFLAG)
    {
        // This function specifies a set of semi-balanced trials for practicing on. 
        // This will be the familiarisation session for participants to get used to the new experiment pace, switching control etc of the test sessions, 
        // and we will take MRI brain map during this time. 

        string startRoom;
        int contextSide;
        bool controlCorrect;

        string[] arrayContexts = new string[blockLength];
        string[] arrayStartRooms = new string[blockLength];
        int[] arrayContextSides = new int[blockLength];
        string[][] arrayControlType = new string[blockLength][];
        bool[] arrayControlCorrect = new bool[blockLength];

        for (int i = 0; i < blockLength; i++)
        {
            // use a different start location for each trial
            switch (i % 4)
            {
                case 0:
                    startRoom = "yellow";
                    break;
                case 1:
                    startRoom = "green";
                    break;
                case 2:
                    startRoom = "red";
                    break;
                case 3:
                    startRoom = "blue";
                    break;
                default:
                    startRoom = "error";
                    Debug.Log("Start room specified incorrectly");
                    break;
            }

            // switch the side of the room the rewards are located on for each context
            if (blockLength % 2 != 0)
            {
                Debug.Log("Error: Odd number of trials specified per block. Specify even number for proper counterbalancing");
            }

            // Note that the contextSide is important for the context training blocks, but irrelevant for the free-foraging blocks
            if ((i % 8) < 4)
            {
                contextSide = 1;
            }
            else
            {
                contextSide = 2;
            }

            // Now mark whether human or computer control first for this trial
            string[] controlType = new string[2];       // allocate new memory (HRS this is important for randomisation, otherwise we just have a pointer)
            if (blockLength % 2 == 0)    // ***HRS note that for these practice trials this wont be perfectly balanced, but they will get some experience with computer v human starting first
            {
                controlType[0] = "Human";
                // AP controlType[1] = "Computer";
                // AP --
                controlType[1] = "Human";
                // -- AP
            }
            else
            {
                // AP --
                controlType[0] = "Human";
                // -- AP
                // AP controlType[0] = "Computer";
                controlType[1] = "Human";
            }
            // Mark whether the computer control moves the agent to the correct or incorrect boulder
            controlCorrect = false;   // always have agent move to incorrect location to get good coverage (for now ***HRS)


            // Store trial setup in array, for later randomisation
            arrayContexts[i] = context;
            arrayStartRooms[i] = startRoom;
            arrayContextSides[i] = contextSide;
            arrayControlType[i] = controlType;
            arrayControlCorrect[i] = controlCorrect;
        }

        // Randomise the trial order and save it
        ShuffleTrialOrderAndStoreBlock(firstTrial, blockLength, arrayContexts, covariance, arrayStartRooms, arrayContextSides, arrayControlType, arrayControlCorrect, freeForageFLAG);

        return firstTrial + blockLength;
    }

    // ********************************************************************** //

    private int SingleContextfMRIBlock(int firstTrial, string context, bool freeForageFLAG)
    {
        // This function specifies the required trials in the block for a 16 trial single context fMRI block
        // returns the next trial after this block

        string startRoom;
        int contextSide;
        bool controlCorrect;
        int blockLength = 16; // Specify the next 16 trials
        int covariance = 0;

        string[] arrayContexts = new string[blockLength];
        string[] arrayStartRooms = new string[blockLength];
        int[] arrayContextSides = new int[blockLength];
        string[][] arrayControlType = new string[blockLength][];
        bool[] arrayControlCorrect = new bool[blockLength];

        for (int i = 0; i < blockLength; i++)
        {
            // use a different start location for each trial
            switch (i % 4)
            {
                case 0:
                    startRoom = "yellow";
                    break;
                case 1:
                    startRoom = "green";
                    break;
                case 2:
                    startRoom = "red";
                    break;
                case 3:
                    startRoom = "blue";
                    break;
                default:
                    startRoom = "error";
                    Debug.Log("Start room specified incorrectly");
                    break;
            }

            // switch the side of the room the rewards are located on for each context
            if (blockLength % 2 != 0)
            {
                Debug.Log("Error: Odd number of trials specified per block. Specify even number for proper counterbalancing");
            }

            // Note that the contextSide is important for the context training blocks, but irrelevant for the free-foraging blocks
            if ((i % 8)< 4)
            {
                contextSide = 1;
            }
            else
            {
                contextSide = 2;
            }

            // Now mark whether human or computer control first for this trial
            string[] controlType = new string[2];       // allocate new memory (HRS this is important for randomisation, otherwise we just have a pointer)
            if (i < blockLength / 2) 
            {
                controlType[0] = "Human";
                // AP --
                controlType[1] = "Human";
                // -- AP
                //controlType[1] = "Computer";
            }
            else 
            {
                // AP --
                controlType[0] = "Human";
                // -- AP
                // controlType[0] = "Computer";
                controlType[1] = "Human";
            }
            // Mark whether the computer control moves the agent to the correct or incorrect boulder
            controlCorrect = false;   // always have agent move to incorrect location to get good coverage (for now ***HRS)


            // Store trial setup in array, for later randomisation
            arrayContexts[i] = context;
            arrayStartRooms[i] = startRoom;
            arrayContextSides[i] = contextSide;
            arrayControlType[i] = controlType;
            arrayControlCorrect[i] = controlCorrect;
        }

        // Randomise the trial order and save it
        ShuffleTrialOrderAndStoreBlock(firstTrial, blockLength, arrayContexts, covariance, arrayStartRooms, arrayContextSides, arrayControlType, arrayControlCorrect, freeForageFLAG);

        return firstTrial + blockLength;
    }

    // ********************************************************************** //

    private int SingleContextDoubleRewardBlock_small(int firstTrial, string context, int covariance, bool freeForageFLAG)
    {
        // This function specifies the required trials in the block, and returns the next trial after this block
        // NOTE: Use this function if you want to 'block' by reward type

        string startRoom;
        int contextSide;
        int blockLength = 4; // Specify the next 8 trials
        string[] controlType = new string[2] { "Human", "Human" };  // default: control remains human the whole time
        bool controlCorrect = true;                                // default: static

        string[] arrayContexts = new string[blockLength];
        string[] arrayStartRooms = new string[blockLength];
        int[] arrayContextSides = new int[blockLength];
        string[][] arrayControlType = new string[blockLength][];
        bool[] arrayControlCorrect = new bool[blockLength];

        for (int i = 0; i < blockLength; i++)
        {
            // use a different start location for each trial
            switch (i % 4)
            {
                case 0:
                    startRoom = "yellow";
                    break;
                case 1:
                    startRoom = "green";
                    break;
                case 2:
                    startRoom = "red";
                    break;
                case 3:
                    startRoom = "blue";
                    break;
                default:
                    startRoom = "error";
                    Debug.Log("Start room specified incorrectly");
                    break;
            }

            // switch the side of the room the rewards are located on for each context
            if (blockLength % 2 != 0)
            {
                Debug.Log("Error: Odd number of trials specified per block. Specify even number for proper counterbalancing");
            }

            // Note that the contextSide is important for the context training blocks, but irrelevant for the free-foraging blocks
            if (i < (blockLength / 2))
            {
                contextSide = 1;
            }
            else
            {
                contextSide = 2;
            }

            // Store trial setup in array, for later randomisation
            arrayContexts[i] = context;
            arrayStartRooms[i] = startRoom;
            arrayContextSides[i] = contextSide;
            arrayControlType[i] = controlType;
            arrayControlCorrect[i] = controlCorrect;
        }


        ShuffleTrialOrderAndStoreBlock(firstTrial, blockLength, arrayContexts, covariance, arrayStartRooms, arrayContextSides, arrayControlType, arrayControlCorrect, freeForageFLAG);

        return firstTrial + blockLength;
    }


    private int SingleContextDoubleRewardBlock(int firstTrial, string context, int covariance, bool freeForageFLAG)
    {
        // This function specifies the required trials in the block, and returns the next trial after this block
        // NOTE: Use this function if you want to 'block' by reward type

        string startRoom;
        int contextSide;
        int blockLength = 8; // Specify the next 8 trials
        string[] controlType = new string[2] { "Human", "Human"};  // default: control remains human the whole time
        bool controlCorrect = true;                                // default: static

        string[] arrayContexts = new string[blockLength];
        string[] arrayStartRooms = new string[blockLength];
        int[] arrayContextSides = new int[blockLength];
        string[][] arrayControlType = new string[blockLength][];
        bool[] arrayControlCorrect = new bool[blockLength];

        for (int i = 0; i < blockLength; i++)
        {
            // use a different start location for each trial
            switch (i % 4)
            {
                case 0:
                    startRoom = "yellow";
                    break;
                case 1:
                    startRoom = "green";
                    break;
                case 2:
                    startRoom = "red";
                    break;
                case 3:
                    startRoom = "blue";
                    break;
                default:
                    startRoom = "error";
                    Debug.Log("Start room specified incorrectly");
                    break;
            }

            // switch the side of the room the rewards are located on for each context
            if (blockLength % 2 !=0)
            {
                Debug.Log("Error: Odd number of trials specified per block. Specify even number for proper counterbalancing");
            }

            // Note that the contextSide is important for the context training blocks, but irrelevant for the free-foraging blocks
            if (i < (blockLength/2)) 
            {
                contextSide = 1;
            }
            else
            {
                contextSide = 2;
            }

            // Store trial setup in array, for later randomisation
            arrayContexts[i] = context;
            arrayStartRooms[i] = startRoom;
            arrayContextSides[i] = contextSide;
            arrayControlType[i] = controlType;
            arrayControlCorrect[i] = controlCorrect;
        }


        ShuffleTrialOrderAndStoreBlock(firstTrial, blockLength, arrayContexts, covariance, arrayStartRooms, arrayContextSides, arrayControlType, arrayControlCorrect, freeForageFLAG);

        return firstTrial + blockLength;
    }

    private int SingleContextDoubleRewardBlock_rev(int firstTrial, string context, int covariance, bool freeForageFLAG)
    {
        // This function specifies the required trials in the block, and returns the next trial after this block
        string startRoom;
        int contextSide;
        int blockLength = 16; // Specify the next 16 trials
        string[] controlType = new string[2] { "Human", "Human" };  // default: control remains human the whole time
        bool controlCorrect = true;                                // default: static
        string[] arrayContexts = new string[blockLength];
        string[] arrayStartRooms = new string[blockLength];
        int[] arrayContextSides = new int[blockLength];
        string[][] arrayControlType = new string[blockLength][];
        bool[] arrayControlCorrect = new bool[blockLength];
        int[] arrayCovariance = new int[blockLength];

        // Initialize random number generator
        System.Random random = new System.Random();

        // Choose two different random indices
        int randomIndex1 = random.Next(0, blockLength);
        int randomIndex2;
        do
        {
            randomIndex2 = random.Next(0, blockLength);
        } while (randomIndex2 == randomIndex1);  // Ensure we get a different second index

        // Determine new covariance values for both random trials
        int newCovariance1 = GetNewCovariance(covariance, random);
        int newCovariance2;
        do
        {
            newCovariance2 = GetNewCovariance(covariance, random);
        } while (newCovariance2 == newCovariance1); // Ensure different covariance values

        for (int i = 0; i < blockLength; i++)
        {
            // use a different start location for each trial
            switch (i % 4)
            {
                case 0:
                    startRoom = "yellow";
                    break;
                case 1:
                    startRoom = "green";
                    break;
                case 2:
                    startRoom = "red";
                    break;
                case 3:
                    startRoom = "blue";
                    break;
                default:
                    startRoom = "error";
                    Debug.Log("Start room specified incorrectly");
                    break;
            }

            // switch the side of the room the rewards are located on for each context
            if (blockLength % 2 != 0)
            {
                Debug.Log("Error: Odd number of trials specified per block. Specify even number for proper counterbalancing");
            }

            // Note that the contextSide is important for the context training blocks, but irrelevant for the free-foraging blocks
            if (i < (blockLength / 2))
            {
                contextSide = 1;
            }
            else
            {
                contextSide = 2;
            }

            // Store trial setup in array, for later randomisation
            arrayContexts[i] = context;
            arrayStartRooms[i] = startRoom;
            arrayContextSides[i] = contextSide;
            arrayControlType[i] = controlType;
            arrayControlCorrect[i] = controlCorrect;

            // Set covariance for this trial - assign new values to the two random indices
            if (i == randomIndex1)
                arrayCovariance[i] = newCovariance1;
            else if (i == randomIndex2)
                arrayCovariance[i] = newCovariance2;
            else
                arrayCovariance[i] = covariance;
        }

        // Pass the array of covariances to ShuffleTrialOrderAndStoreBlock
        ShuffleTrialOrderAndStoreBlock_rev(firstTrial, blockLength, arrayContexts, arrayCovariance, arrayStartRooms, arrayContextSides, arrayControlType, arrayControlCorrect, freeForageFLAG);
        return firstTrial + blockLength;
    }

    // Helper method to get a new covariance value different from the current one
    private int GetNewCovariance(int currentCovariance, System.Random random)
    {
        switch (currentCovariance)
        {
            case 0:
                // If original is 0, randomly choose between 1 and 2
                return random.Next(0, 2) == 0 ? 1 : 2;
            case 1:
                // If original is 1, randomly choose between 0 and 2
                return random.Next(0, 2) == 0 ? 0 : 2;
            case 2:
                // If original is 2, randomly choose between 0 and 1
                return random.Next(0, 2);
            default:
                return currentCovariance;
        }
    }


    // ********************************************************************** //

    private int DoubleRewardBlock_micro(int firstTrial, string context, int covariance, int blockLength)
    {
        // This is for use during testing and debugging only - it DOES NOT specify a full counterbalanced trial sequence
        // This function specifies the required trials in the block, and returns the next trial after this block

        string startRoom;
        int contextSide;
        string[] controlType = new string[2] { "Human", "Human" };  // default: control remains human the whole time
        bool controlCorrect = true;                                 // default: static

        string[] arrayContexts = new string[blockLength];
        string[] arrayStartRooms = new string[blockLength];
        int[] arrayContextSides = new int[blockLength];
        string[][] arrayControlType = new string[blockLength][];
        bool[] arrayControlCorrect = new bool[blockLength];

        for (int i = 0; i < blockLength; i++)
        {
            // use a different start location for each trial
            switch (i % 4)
            {
                case 0:
                    startRoom = "yellow";
                    break;
                case 1:
                    startRoom = "green";
                    break;
                case 2:
                    startRoom = "red";
                    break;
                case 3:
                    startRoom = "blue";
                    break;
                default:
                    startRoom = "error";
                    Debug.Log("Start room specified incorrectly");
                    break;
            }

            // switch the side of the room the rewards are located on for each context
            if (blockLength % 2 != 0)
            {
                Debug.Log("Error: Odd number of trials specified per block. Specify even number for proper counterbalancing");
            }

            if (i < (blockLength / 2))
            {
                contextSide = 1;
            }
            else
            {
                contextSide = 2;
            }

            // Store trial setup in array, for later randomisation
            arrayContexts[i] = context;
            arrayStartRooms[i] = startRoom;
            arrayContextSides[i] = contextSide;
            arrayControlType[i] = controlType;
            arrayControlCorrect[i] = controlCorrect;
        }

        // Randomise the trial order and save it
        bool freeForageFLAG = false;
        ShuffleTrialOrderAndStoreBlock(firstTrial, blockLength, arrayContexts, covariance, arrayStartRooms, arrayContextSides, arrayControlType, arrayControlCorrect, freeForageFLAG);

        return firstTrial + blockLength;
    }

    // ********************************************************************** //

    // ********************************************************************** //

    private int SingleRewardBlock_micro(int firstTrial, string context, int covariance, int blockLength)
    {
        // This is for use during testing and debugging only - it DOES NOT specify a full counterbalanced trial sequence
        // This function specifies the required trials in the block, and returns the next trial after this block

        string startRoom;
        int contextSide;
        string[] controlType = new string[2] { "Human", "Human" };  // default: control remains human the whole time
        bool controlCorrect = true;                                 // default: static

        string[] arrayContexts = new string[blockLength];
        string[] arrayStartRooms = new string[blockLength];
        int[] arrayContextSides = new int[blockLength];
        string[][] arrayControlType = new string[blockLength][];
        bool[] arrayControlCorrect = new bool[blockLength];

        for (int i = 0; i < blockLength; i++)
        {
            // use a different start location for each trial
            switch (i % 4)
            {
                case 0:
                    startRoom = "yellow";
                    break;
                case 1:
                    startRoom = "green";
                    break;
                case 2:
                    startRoom = "red";
                    break;
                case 3:
                    startRoom = "blue";
                    break;
                default:
                    startRoom = "error";
                    Debug.Log("Start room specified incorrectly");
                    break;
            }

            // switch the side of the room the rewards are located on for each context
            if (blockLength % 2 != 0)
            {
                Debug.Log("Error: Odd number of trials specified per block. Specify even number for proper counterbalancing");
            }

            if (i < (blockLength / 2))
            {
                contextSide = 1;
            }
            else
            {
                contextSide = 2;
            }

            // Store trial setup in array, for later randomisation
            arrayContexts[i] = context;
            arrayStartRooms[i] = startRoom;
            arrayContextSides[i] = contextSide;
            arrayControlType[i] = controlType;
            arrayControlCorrect[i] = controlCorrect;
        }

        // Randomise the trial order and save it
        bool freeForageFLAG = false;
        ShuffleTrialOrderAndStoreBlock_Single(firstTrial, blockLength, arrayContexts, covariance, arrayStartRooms, arrayContextSides, arrayControlType, arrayControlCorrect, freeForageFLAG);

        return firstTrial + blockLength;
    }

    private void SetTrialInContext_Single(int trial, int trialInBlock, string startRoom, string context, int covariance, int contextSide, string[] controlType, bool controlCorrect, bool freeForageFLAG)
    {
        // This function specifies the reward covariance

        // Note the variable 'contextSide' specifies whether the two rooms containing the reward will be located on the left or right of the environment
        // e.g. if cheese context: the y/b side, vs the g/r side. if watermelon context: the y/g side, vs the b/r side.
        // When the trial is a free foraging trial however, the 'contextSide' variable is used to specify which of the bridges is blocked, to control CW and CCW turns from the start room (since rewards are in all rooms).


        bool trialSetCorrectly = false;
        SetSingleRewardTrial(trial, trialInBlock, context, startRoom, "yellow", "red", contextSide, controlType, controlCorrect, freeForageFLAG);
        trialSetCorrectly = true;

            if (!trialSetCorrectly)
        {
            Debug.Log("Something went wrong specifying the rooms affiliated with each context!");
        }
    }



    // ********************************************************************** //


    // ********************************************************************** //

    private void SetTrialInContext(int trial, int trialInBlock, string startRoom, string context, int covariance, int contextSide, string[] controlType, bool controlCorrect, bool freeForageFLAG)
    {
        // This function specifies the reward covariance

        // Note the variable 'contextSide' specifies whether the two rooms containing the reward will be located on the left or right of the environment
        // e.g. if cheese context: the y/b side, vs the g/r side. if watermelon context: the y/g side, vs the b/r side.
        // When the trial is a free foraging trial however, the 'contextSide' variable is used to specify which of the bridges is blocked, to control CW and CCW turns from the start room (since rewards are in all rooms).


        bool trialSetCorrectly = false;
        if (covariance == 2)
        {
            switch (context)
            {
                case "cheese":
                    if (contextSide == 1)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "yellow", "red", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    else if (contextSide == 2)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "green", "blue", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    break;
            }
        } else if (covariance == 0)
        {
            switch (context)
            {
                case "cheese":   // vertical

                    if (contextSide == 1)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "yellow", "blue", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    else if (contextSide == 2)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "green", "red", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    break;

                case "watermelon":   // horizontal

                    if (contextSide == 1)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "yellow", "green", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    else if (contextSide == 2)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "blue", "red", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    break;

                case "peanut":
                    //AP ---
                    if (contextSide == 1)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "yellow", "blue", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    else if (contextSide == 2)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "green", "red", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    break;
                //--AP

                case "peanut_switch":
                    //AP ---
                    if (contextSide == 1)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "yellow", "green", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    else if (contextSide == 2)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "blue", "red", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    break;
                //--AP

                case "mushroom":

                    if (contextSide == 1)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "yellow", "blue", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    else if (contextSide == 2)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "green", "red", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    break;
                case "pineapple":

                    if (contextSide == 1)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "yellow", "blue", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    else if (contextSide == 2)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "green", "red", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    break;
                case "banana":

                    if (contextSide == 1)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "yellow", "green", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    else if (contextSide == 2)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "blue", "red", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    break;

                case "martini":
                    //AP ---
                    if (contextSide == 1)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "yellow", "green", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    else if (contextSide == 2)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "blue", "red", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    break;
                //AP ---
                case "martini_switch":
                    if (contextSide == 1)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "yellow", "blue", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    else if (contextSide == 2)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "green", "red", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    break;
                //AP ---
                case "avocado":

                    if (contextSide == 1)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "yellow", "green", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    else if (contextSide == 2)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "blue", "red", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    break;

                default:
                    break;
            }
        }
        else if (covariance == 1)

        {
            switch (context)
            {
                case "cheese":   // vertical

                    if (contextSide == 1)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "yellow", "green", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    else if (contextSide == 2)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "blue", "red", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    break;

                case "watermelon":   // horizontal

                    if (contextSide == 1)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "yellow", "blue", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    else if (contextSide == 2)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "green", "red", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    break;


                //--AP

                case "peanut":
                    //AP ---
                    if (contextSide == 1)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "yellow", "green", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    else if (contextSide == 2)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "blue", "red", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    break;
                //--AP

                case "mushroom":

                    if (contextSide == 1)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "yellow", "blue", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    else if (contextSide == 2)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "green", "red", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    break;
                case "pineapple":

                    if (contextSide == 1)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "yellow", "blue", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    else if (contextSide == 2)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "green", "red", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    break;
                case "banana":

                    if (contextSide == 1)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "yellow", "green", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    else if (contextSide == 2)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "blue", "red", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    break;

                case "martini":
                    if (contextSide == 1)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "yellow", "blue", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    else if (contextSide == 2)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "green", "red", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    break;
                //AP ---
                case "avocado":

                    if (contextSide == 1)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "yellow", "green", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    else if (contextSide == 2)
                    {
                        SetDoubleRewardTrial(trial, trialInBlock, context, startRoom, "blue", "red", contextSide, controlType, controlCorrect, freeForageFLAG);
                        trialSetCorrectly = true;
                    }
                    break;

                default:
                    break;
            }
        }

            if (!trialSetCorrectly)
        {
            Debug.Log("Something went wrong specifying the rooms affiliated with each context!");
        }
    }



    // ********************************************************************** //

    private void SetSingleRewardTrial(int trial, int trialInBlock, string context, string startRoom, string rewardRoom1, string rewardRoom2, int contextSide, string[] controlType, bool controlCorrect, bool freeForageFLAG)
    {
        bool collisionInSpawnLocations = true;
        int iterationCounter = 0;
        int nrooms = 4;
        bridgeStates[trial] = new bool[4];                  // there are 4 bridges

        // Check that we've inputted a valid trial number
        if (trial <= (setupTrials - 1))
        {
            Debug.Log("Trial randomisation failed: invalid trial number input writing to.");
        }
        else
        {
            // Write the trial according to context and room/start locations
            rewardTypes[trial] = context;

            if (experimentVersion == "micro2D_debug_portal")
            {
                // Debug mode - single reward setup
                doubleRewardTask[trial] = false;
                rewardPositions[trial] = new Vector3[1];
                rewardPositions[trial][0] = new Vector3(3f, 1f, 0f);
                star1Rooms[trial] = rewardRoom1;
                star2Rooms[trial] = rewardRoom1;  // Same as first room since single reward
            }
            else
            {
                doubleRewardTask[trial] = true;
            }

            // Generate present positions (keep this for boulder positioning)
            GeneratePresentPositions(trial, trialInBlock, freeForageFLAG);

            if (!experimentVersion.Equals("micro2D_debug_portal"))
            {
                // Original reward positioning logic for non-debug mode
                if (freeForageFLAG)
                {
                    // rewards are positioned in all boxes
                    trialMazes[trial] = "PrePostForage_" + rewardTypes[trial];
                    freeForage[trial] = true;
                    maxMovementTime[trial] = 120.0f;
                    blankTime[trial] = ExponentialJitter(0.5f, 0.75f, 1f);
                    hallwayFreezeTime[trial] = new float[4];
                    goalHitPauseTime[trial] = new float[4];
                    for (int i = 0; i < nrooms; i++)
                    {
                        hallwayFreezeTime[trial][i] = ExponentialJitter(2f, 1.5f, 7f);
                        goalHitPauseTime[trial][i] = ExponentialJitter(2f, 1f, 5f);
                    }

                    star1Rooms[trial] = "";
                    star2Rooms[trial] = "";

                    for (int i = 0; i < presentPositions[trial].Length; i++)
                    {
                        rewardPositions[trial][i] = presentPositions[trial][i];
                    }
                }
                else
                {
                    if (wackyColours)
                    {
                        trialMazes[trial] = "FourRooms_wackycolours_" + rewardTypes[trial];
                    }
                    else
                    {
                        trialMazes[trial] = "FourRooms_" + rewardTypes[trial];
                    }
                    freeForage[trial] = false;
                    maxMovementTime[trial] = 60.0f;
                    blankTime[trial] = ExponentialJitter(2.5f, 1.5f, 7f);
                    hallwayFreezeTime[trial] = new float[4];
                    goalHitPauseTime[trial] = new float[4];
                    for (int i = 0; i < nrooms; i++)
                    {
                        hallwayFreezeTime[trial][i] = ExponentialJitter(2f, 1.5f, 7f);
                        goalHitPauseTime[trial][i] = ExponentialJitter(2f, 1f, 5f);
                    }

                    star1Rooms[trial] = rewardRoom1;
                    star2Rooms[trial] = rewardRoom2;

                    rewardPositions[trial][0] = RandomPresentInRoom(rewardRoom1);
                    rewardPositions[trial][1] = RandomPresentInRoom(rewardRoom2);
                }
            }
            else
            {
                // Debug mode - set up basic trial parameters
                trialMazes[trial] = "FourRooms_" + rewardTypes[trial];
                freeForage[trial] = false;
                maxMovementTime[trial] = 60.0f;
                blankTime[trial] = ExponentialJitter(2.5f, 1.5f, 7f);
                hallwayFreezeTime[trial] = new float[4];
                goalHitPauseTime[trial] = new float[4];
                for (int i = 0; i < nrooms; i++)
                {
                    hallwayFreezeTime[trial][i] = ExponentialJitter(2f, 1.5f, 7f);
                    goalHitPauseTime[trial][i] = ExponentialJitter(2f, 1f, 5f);
                }
            }

            // Common setup for both modes
            controlStateOrder[trial] = controlType;
            computerAgentCorrect[trial] = controlCorrect;

            for (int i = 0; i < bridgeStates[trial].Length; i++)
            {
                bridgeStates[trial][i] = true;
            }

            // Player start position setup
            playerStartRooms[trial] = startRoom;
            playerStartPositions[trial] = RandomStartPositionInRoom(startRoom);
            iterationCounter = 0;

            while (collisionInSpawnLocations)
            {
                iterationCounter++;
                collisionInSpawnLocations = false;
                playerStartPositions[trial] = RandomStartPositionInRoom(startRoom);

                for (int k = 0; k < presentPositions[trial].Length; k++)
                {
                    if (playerStartPositions[trial] == presentPositions[trial][k])
                    {
                        collisionInSpawnLocations = true;
                    }
                }

                if (iterationCounter > 40)
                {
                    Debug.Log("There was a while loop error: C");
                    break;
                }
            }

            playerStartOrientations[trial] = findStartOrientation(playerStartPositions[trial]);
        }
    }

    // ********************************************************************** //

    private void SetDoubleRewardTrial(int trial, int trialInBlock, string context, string startRoom, string rewardRoom1, string rewardRoom2, int contextSide, string[] controlType, bool controlCorrect, bool freeForageFLAG)
    {
        // This function writes the trial number indicated by the input variable 'trial'.
        // Note: use this function within another that modulates context such that e.g. for 'cheese', the rooms for room1 and room2 reward are set

        bool collisionInSpawnLocations = true;
        int iterationCounter = 0;
        int nrooms = 4;
        bridgeStates[trial] = new bool[4];                  // there are 4 bridges

        // Check that we've inputted a valid trial number
        if ( trial <= (setupTrials - 1) )
        {
            Debug.Log("Trial randomisation failed: invalid trial number input writing to.");
        }
        else
        {
            // Write the trial according to context and room/start locations
            rewardTypes[trial] = context;
            doubleRewardTask[trial] = true;

            // generate the random locations for the presents in each room
            GeneratePresentPositions(trial, trialInBlock, freeForageFLAG);


            if (freeForageFLAG) 
            {
                // rewards are positioned in all boxes
                trialMazes[trial] = "PrePostForage_" + rewardTypes[trial];
                freeForage[trial] = true;
                maxMovementTime[trial] = 120.0f;       // 2 mins to collect all rewards on freeforaging trials
                //AP --
                //maxMovementTime[trial] = 300.0f;            // 5 mins to collect all rewards on freeforaging trials
                //--AP
                blankTime[trial] = ExponentialJitter(0.5f, 0.75f, 1f);
                //blankTime[trial] = ExponentialJitter(2.5f, 1.5f, 7f);
                hallwayFreezeTime[trial] = new float[4];
                goalHitPauseTime[trial] = new float[4];
                for (int i = 0; i < nrooms; i++)
                {
                    hallwayFreezeTime[trial][i] = ExponentialJitter(2f, 1.5f, 7f);   // jitter times: mean, min, max, 
                    goalHitPauseTime[trial][i] = ExponentialJitter(2f, 1f, 5f);
                }

                // select random locations in rooms 1 and 2 for the two rewards (one in each)
                star1Rooms[trial] = "";
                star2Rooms[trial] = "";

                // define the control type: switching order between human and computer agent avatar control
                controlStateOrder[trial] = controlType;
                computerAgentCorrect[trial] = controlCorrect;

                // Specific reward locations within each room for all rewards
                for (int i = 0; i < presentPositions[trial].Length; i++)
                {
                    rewardPositions[trial][i] = presentPositions[trial][i];
                }

                // all the bridges that are available for walking over...
                for (int i = 0; i < bridgeStates[trial].Length; i++)
                {
                    bridgeStates[trial][i] = true;
                }

                // determine which bridge to disable, to control CW vs CCW turns
                // Note: contextSide==1 means they have to turn CW, contextSide==2 means they have to turn CCW
                switch (startRoom) 
                {
                    case "blue":
                        if (contextSide==1)
                        {
                            bridgeStates[trial][2] = false; // bridge 3
                        }
                        else
                        {
                            bridgeStates[trial][3] = false; // bridge 4
                        }
                        break;

                    case "red":
                        if (contextSide == 1)
                        {
                            bridgeStates[trial][1] = false; // bridge 2
                        }
                        else
                        {
                            bridgeStates[trial][2] = false; // bridge 3
                        }
                        break;

                    case "yellow":
                        if (contextSide == 1)
                        {
                            bridgeStates[trial][3] = false; // bridge 4
                        }
                        else
                        {
                            bridgeStates[trial][0] = false; // bridge 1
                        }
                        break;

                    case "green":
                        if (contextSide == 1)
                        {
                            bridgeStates[trial][0] = false; // bridge 1
                        }
                        else
                        {
                            bridgeStates[trial][1] = false; // bridge 2
                        }
                        break;

                    default:
                        Debug.Log("Warning: invalid room specified, trial sequence will not be properly counterbalanced.");
                        break;               
                }

            }
            else
            {
                // this is a two-reward trial
                if (wackyColours) 
                {
                    trialMazes[trial] = "FourRooms_wackycolours_" + rewardTypes[trial];
                }
                else
                { 
                    trialMazes[trial] = "FourRooms_" + rewardTypes[trial];
                }
                freeForage[trial] = false;
                //AP maxMovementTime[trial] = 50.0f;        // 1 min to collect just the 2 rewards on covariance trials ***HRS changed from 60 on 4/06/2019
                maxMovementTime[trial] = 60.0f;
                blankTime[trial] = ExponentialJitter(2.5f, 1.5f, 7f);
                hallwayFreezeTime[trial] = new float[4];
                goalHitPauseTime[trial] = new float[4];
                for (int i=0; i < nrooms; i++)
                {
                    hallwayFreezeTime[trial][i] = ExponentialJitter(2f, 1.5f, 7f);   // jitter times: mean, min, max, 
                    goalHitPauseTime[trial][i] = ExponentialJitter(2f, 1f, 5f);
                }


                // select random locations in rooms 1 and 2 for the two rewards (one in each)
                star1Rooms[trial] = rewardRoom1;
                star2Rooms[trial] = rewardRoom2;

                // Specific reward locations within each room for all rewards
                rewardPositions[trial][0] = RandomPresentInRoom(rewardRoom1);
                rewardPositions[trial][1] = RandomPresentInRoom(rewardRoom2);

                // define the control type: switching order between human and computer agent avatar control
                controlStateOrder[trial] = controlType;
                computerAgentCorrect[trial] = controlCorrect;

                // all the bridges are available for walking over
                for (int i = 0; i < bridgeStates[trial].Length; i++) 
                { 
                    bridgeStates[trial][i] = true;
                }
            }

            // select start location as random position in given room
            playerStartRooms[trial] = startRoom;
            playerStartPositions[trial] = RandomStartPositionInRoom(startRoom);
            iterationCounter = 0;

            // make sure the player doesn't spawn on one of the rewards
            while ( collisionInSpawnLocations )
            {
                iterationCounter++;
                collisionInSpawnLocations = false;   // benefit of the doubt
                playerStartPositions[trial] = RandomStartPositionInRoom(startRoom);
               
                // make sure player doesnt spawn on a present box
                for (int k = 0; k < presentPositions[trial].Length; k++)
                {
                    if (playerStartPositions[trial] == presentPositions[trial][k])
                    {
                        collisionInSpawnLocations = true;   // respawn the player location
                    }
                    // Note: at one point there was an attempt here to stop spawning adjacent to a present box, but for several present arrangements this is impossible and results in an infinite while loop
                }
                // implement a catchment check for the while loop
                if (iterationCounter > 40) 
                {
                    Debug.Log("There was a while loop error: C");
                    break;
                }
            }
            // orient player towards the centre of the environment (will be maximally informative of location in environment)
            playerStartOrientations[trial] = findStartOrientation(playerStartPositions[trial]); 
        }
    }

    // ********************************************************************** //

    private Vector3 RandomPositionInRoom(string roomColour)
    {
        // select a random position in a room of a given colour
        switch (roomColour)
        {
            case "blue":
                return blueRoomPositions[UnityEngine.Random.Range(0, blueRoomPositions.Length - 1)];

            case "red":
                return redRoomPositions[UnityEngine.Random.Range(0, redRoomPositions.Length - 1)];

            case "green":
                return greenRoomPositions[UnityEngine.Random.Range(0, greenRoomPositions.Length - 1)];
            
            case "yellow":
                return yellowRoomPositions[UnityEngine.Random.Range(0, yellowRoomPositions.Length - 1)];
            
            default:
                return new Vector3(0.0f, 0.0f, 0.0f);  // this should never happen
        }
    }

    // ********************************************************************** //

    private Vector3 RandomStartPositionInRoom(string roomColour)
    {
        // We will use this to restrict the start positions to be in the corner opposite each boulder, in each room
        switch (roomColour)
        {
            case "blue":
                return blueRoomStartPositions[UnityEngine.Random.Range(0, blueRoomStartPositions.Length - 1)];

            case "red":
                return redRoomStartPositions[UnityEngine.Random.Range(0, redRoomStartPositions.Length - 1)];

            case "green":
                return greenRoomStartPositions[UnityEngine.Random.Range(0, greenRoomStartPositions.Length - 1)];

            case "yellow":
                return yellowRoomStartPositions[UnityEngine.Random.Range(0, yellowRoomStartPositions.Length - 1)];

            default:
                return new Vector3(0.0f, 0.0f, 0.0f);  // this should never happen
        }
    }

    // ********************************************************************** //

    private Vector3 RandomPresentInRoom( string roomColour)
    {
        // select a random present in a room of a given colour to put the reward in
        switch (roomColour)
        {
            case "blue":
                return bluePresentPositions[UnityEngine.Random.Range(0, bluePresentPositions.Length - 1)];

            case "red":
                return redPresentPositions[UnityEngine.Random.Range(0, redPresentPositions.Length - 1)];

            case "green":
                return greenPresentPositions[UnityEngine.Random.Range(0, greenPresentPositions.Length - 1)];

            case "yellow":
                return yellowPresentPositions[UnityEngine.Random.Range(0, yellowPresentPositions.Length - 1)];

            default:
                return new Vector3(0.0f, 0.0f, 0.0f);  // this should never happen
        }
    }

    // ********************************************************************** //

    public void ShuffleTrialOrderAndStoreBlock_rev(int firstTrial, int blockLength, string[] arrayContexts, int[] arraycovariance, string[] arrayStartRooms, int[] arrayContextSides, string[][] arrayControlType, bool[] arrayControlCorrect, bool freeForageFLAG)
    {
        // This function shuffles the prospective trials from firstTrial to firstTrial+blockLength and stores them.
        // This has been checked and works correctly :)

        string startRoom;
        string context;
        int contextSide;
        int covariance;
        bool controlCorrect;
        string[] controlType = new string[2];
         
        bool randomiseOrder = true;
        int n = arrayContexts.Length;

        if (randomiseOrder)
        {
            // Perform the Fisher-Yates algorithm for shuffling array elements in place 
            // (use same sample for each of the 3 arrays to keep order aligned across arrays)
            for (int i = 0; i < n; i++)
            {
                int k = i + rand.Next(n - i); // select random index in array, less than n-i

                // shuffle contexts
                string tempContext = arrayContexts[k];
                arrayContexts[k] = arrayContexts[i];
                arrayContexts[i] = tempContext;

                // shuffle start room
                string tempRoom = arrayStartRooms[k];
                arrayStartRooms[k] = arrayStartRooms[i];
                arrayStartRooms[i] = tempRoom;

                // shuffle context side
                int tempContextSide = arrayContextSides[k];
                arrayContextSides[k] = arrayContextSides[i];
                arrayContextSides[i] = tempContextSide;

                // shuffle control type
                string[] tempControlType = arrayControlType[k];
                arrayControlType[k] = arrayControlType[i];
                arrayControlType[i] = tempControlType;

                // shuffle whether computer control correct or not
                bool tempControlCorrect = arrayControlCorrect[k];
                arrayControlCorrect[k] = arrayControlCorrect[i];
                arrayControlCorrect[i] = tempControlCorrect;

            }
        }
        // Store the randomised trial order
        for (int i = 0; i < n; i++)
        {
            startRoom = arrayStartRooms[i];
            context = arrayContexts[i];
            contextSide = arrayContextSides[i];
            controlType = arrayControlType[i];
            controlCorrect = arrayControlCorrect[i];
            covariance = arraycovariance[i];
            SetTrialInContext(i + firstTrial, i, startRoom, context, covariance, contextSide, controlType, controlCorrect, freeForageFLAG);
        }
    }

    public void ShuffleTrialOrderAndStoreBlock(int firstTrial, int blockLength, string[] arrayContexts, int covariance, string[] arrayStartRooms, int[] arrayContextSides, string[][] arrayControlType, bool[] arrayControlCorrect, bool freeForageFLAG)
    {
        // This function shuffles the prospective trials from firstTrial to firstTrial+blockLength and stores them.
        // This has been checked and works correctly :)

        string startRoom;
        string context;
        int contextSide;
        bool controlCorrect;
        string[] controlType = new string[2];

        bool randomiseOrder = true;
        int n = arrayContexts.Length;

        if (randomiseOrder)
        {
            // Perform the Fisher-Yates algorithm for shuffling array elements in place 
            // (use same sample for each of the 3 arrays to keep order aligned across arrays)
            for (int i = 0; i < n; i++)
            {
                int k = i + rand.Next(n - i); // select random index in array, less than n-i

                // shuffle contexts
                string tempContext = arrayContexts[k];
                arrayContexts[k] = arrayContexts[i];
                arrayContexts[i] = tempContext;

                // shuffle start room
                string tempRoom = arrayStartRooms[k];
                arrayStartRooms[k] = arrayStartRooms[i];
                arrayStartRooms[i] = tempRoom;

                // shuffle context side
                int tempContextSide = arrayContextSides[k];
                arrayContextSides[k] = arrayContextSides[i];
                arrayContextSides[i] = tempContextSide;

                // shuffle control type
                string[] tempControlType = arrayControlType[k];
                arrayControlType[k] = arrayControlType[i];
                arrayControlType[i] = tempControlType;

                // shuffle whether computer control correct or not
                bool tempControlCorrect = arrayControlCorrect[k];
                arrayControlCorrect[k] = arrayControlCorrect[i];
                arrayControlCorrect[i] = tempControlCorrect;

            }
        }
        // Store the randomised trial order
        for (int i = 0; i < n; i++)
        {
            startRoom = arrayStartRooms[i];
            context = arrayContexts[i];
            contextSide = arrayContextSides[i];
            controlType = arrayControlType[i];
            controlCorrect = arrayControlCorrect[i];
          
            SetTrialInContext(i + firstTrial, i, startRoom, context, covariance, contextSide, controlType, controlCorrect, freeForageFLAG);
        }
    }



    // ********************************************************************** //

    public void ShuffleTrialOrderAndStoreBlock_Single(int firstTrial, int blockLength, string[] arrayContexts, int covariance, string[] arrayStartRooms, int[] arrayContextSides, string[][] arrayControlType, bool[] arrayControlCorrect, bool freeForageFLAG)
    {
        // This function shuffles the prospective trials from firstTrial to firstTrial+blockLength and stores them.
        // This has been checked and works correctly :)

        string startRoom;
        string context;
        int contextSide;
        bool controlCorrect;
        string[] controlType = new string[2];

        bool randomiseOrder = true;
        int n = arrayContexts.Length;

        if (randomiseOrder)
        {
            // Perform the Fisher-Yates algorithm for shuffling array elements in place 
            // (use same sample for each of the 3 arrays to keep order aligned across arrays)
            for (int i = 0; i < n; i++)
            {
                int k = i + rand.Next(n - i); // select random index in array, less than n-i

                // shuffle contexts
                string tempContext = arrayContexts[k];
                arrayContexts[k] = arrayContexts[i];
                arrayContexts[i] = tempContext;

                // shuffle start room
                string tempRoom = arrayStartRooms[k];
                arrayStartRooms[k] = arrayStartRooms[i];
                arrayStartRooms[i] = tempRoom;

                // shuffle context side
                int tempContextSide = arrayContextSides[k];
                arrayContextSides[k] = arrayContextSides[i];
                arrayContextSides[i] = tempContextSide;

                // shuffle control type
                string[] tempControlType = arrayControlType[k];
                arrayControlType[k] = arrayControlType[i];
                arrayControlType[i] = tempControlType;

                // shuffle whether computer control correct or not
                bool tempControlCorrect = arrayControlCorrect[k];
                arrayControlCorrect[k] = arrayControlCorrect[i];
                arrayControlCorrect[i] = tempControlCorrect;

            }
        }
        // Store the randomised trial order
        for (int i = 0; i < n; i++)
        {
            startRoom = arrayStartRooms[i];
            context = arrayContexts[i];
            contextSide = arrayContextSides[i];
            controlType = arrayControlType[i];
            controlCorrect = arrayControlCorrect[i];

            SetTrialInContext_Single(i + firstTrial, i, startRoom, context, covariance, contextSide, controlType, controlCorrect, freeForageFLAG);
        }
    }



    // ********************************************************************** //


    public void ReshuffleTrialOrder(int firstTrial, int blockLength)
    {
        // This function reshuffles the set prospective trials from firstTrial to firstTrial+blockLength and stores them.
        // Bit ugly but ok for now (could have a function with a different or flexible return type that does this for each var)

        int n = blockLength;

        // Identify any rest breaks already scheduled, we will preserve their positions
        List<int> preservedRestIndices = new List<int>();
        for (int trial = firstTrial; trial < (blockLength + firstTrial); trial++) 
        { 
            if (trialMazes[trial] == "RestBreak") 
            {
                preservedRestIndices.Add(trial);
            }
        }


        // Perform the Fisher-Yates algorithm for shuffling array elements in place, and shuffle all trials between firstTrial and firstTrial+blockLength 
        // (use same sample for each of the 3 arrays to keep order aligned across arrays)
        for (int i = 0; i < n; i++)
        {
            int k = i + rand.Next(n - i); // select random index in array, less than n-i
            SwapTrials(k + firstTrial, i + firstTrial);  // swap the contents of these trials at indices k+firstTrial, and i+firstTrial
        }

        // Now find where the restbreak trials have gone to, and swap them back with their correct original positions
        int nRequiredSwaps = preservedRestIndices.Count;
        int nSwaps = 0;
        int checkedFlag = -1;
        int[] uncheckedTrials = Enumerable.Range(firstTrial, blockLength).ToArray();
        for (int i = 0; i < n; i++)
        {
            int trial = firstTrial+i;

            if (uncheckedTrials[i] != checkedFlag) 
            { 
                if (trialMazes[trial] == "RestBreak")
                {
                    if (nRequiredSwaps > 0) 
                    {
                        //Debug.Log("this happened");
                        SwapTrials(trial, preservedRestIndices[nSwaps]);
                        //Debug.Log("seemed to go ok");
                        uncheckedTrials[preservedRestIndices[nSwaps]-firstTrial] = checkedFlag; // make sure we dont swap this element again!
                        nSwaps++;
                    }
                }
            }
        }
    }

    // ********************************************************************** //

    private void SwapTrials(int indexA, int indexB) 
    {
        // This function serves to swap the scheduled trial contents of trials k and i
        // shuffle contexts / reward types
        string tempContext = rewardTypes[indexA];
        rewardTypes[indexA] = rewardTypes[indexB];
        rewardTypes[indexB] = tempContext;

        // shuffle start room
        string tempRoom = playerStartRooms[indexA];
        playerStartRooms[indexA] = playerStartRooms[indexB];
        playerStartRooms[indexB] = tempRoom;

        // shuffle start position
        Vector3 tempStartPosition = playerStartPositions[indexA];
        playerStartPositions[indexA] = playerStartPositions[indexB];
        playerStartPositions[indexB] = tempStartPosition;

        // shuffle start orientation
        Vector3 tempStartOrientation = playerStartOrientations[indexA];
        playerStartOrientations[indexA] = playerStartOrientations[indexB];
        playerStartOrientations[indexB] = tempStartOrientation;

        // shuffle reward positions
        Vector3[] tempRewardPosition = rewardPositions[indexA];
        rewardPositions[indexA] = rewardPositions[indexB];
        rewardPositions[indexB] = tempRewardPosition;

        // shuffle present positions
        Vector3[] tempPresentPositions = presentPositions[indexA];
        presentPositions[indexA] = presentPositions[indexB];
        presentPositions[indexB] = tempPresentPositions;

        // reward room 1
        string tempRewardRoom = star1Rooms[indexA];
        star1Rooms[indexA] = star1Rooms[indexB];
        star1Rooms[indexB] = tempRewardRoom;

        // reward room 2
        tempRewardRoom = star2Rooms[indexA];
        star2Rooms[indexA] = star2Rooms[indexB];
        star2Rooms[indexB] = tempRewardRoom;

        // shuffle control type
        string[] tempControlType = controlStateOrder[indexA];
        controlStateOrder[indexA] = controlStateOrder[indexB];
        controlStateOrder[indexB] = tempControlType;

        // shuffle whether computer control correct or not
        bool tempControlCorrect = computerAgentCorrect[indexA];
        computerAgentCorrect[indexA] = computerAgentCorrect[indexB];
        computerAgentCorrect[indexB] = tempControlCorrect;

        // movement time
        float tempMoveTime = maxMovementTime[indexA];
        maxMovementTime[indexA] = maxMovementTime[indexB];
        maxMovementTime[indexB] = tempMoveTime;

        // ITI times
        float tempblankTime = blankTime[indexA];
        blankTime[indexA] = blankTime[indexB];
        blankTime[indexB] = tempblankTime;

        // hallwayfreeze times
        float[] tempFreezeTimes = hallwayFreezeTime[indexA];
        hallwayFreezeTime[indexA] = hallwayFreezeTime[indexB];
        hallwayFreezeTime[indexB] = tempFreezeTimes;

        // prereward appear times
        float[] tempgoalHitPauseTime = goalHitPauseTime[indexA];
        goalHitPauseTime[indexA] = goalHitPauseTime[indexB];
        goalHitPauseTime[indexB] = tempgoalHitPauseTime;

        // free forage flag
        bool tempForage = freeForage[indexA];
        freeForage[indexA] = freeForage[indexB];
        freeForage[indexB] = tempForage;

        // shuffle trialMazes
        string tempTrialMazes = trialMazes[indexA];
        trialMazes[indexA] = trialMazes[indexB];
        trialMazes[indexB] = tempTrialMazes;

        // shuffle whether double reward
        bool tempDoubleReward = doubleRewardTask[indexA];
        doubleRewardTask[indexA] = doubleRewardTask[indexB];
        doubleRewardTask[indexB] = tempDoubleReward;

        // shuffle bridge states
        bool[] tempBridgeStates = bridgeStates[indexA];
        bridgeStates[indexA] = bridgeStates[indexB];
        bridgeStates[indexB] = tempBridgeStates;

    }

    // ********************************************************************** //

    private void GenerateRandomTrialPositions(int trial)
    {
        // HRS not currently used
        int iterationCounter = 0;

        // Generate a trial that randomly positions the player and reward/s
        playerStartRooms[trial] = ChooseRandomRoom();
        playerStartPositions[trial] = RandomPositionInRoom(playerStartRooms[trial]); // random start position
        playerStartOrientations[trial] = findStartOrientation(playerStartPositions[trial]);   // orient player towards the centre of the environment

        // adapted for array of reward positions
        star1Rooms[trial] = ChooseRandomRoom();
        star2Rooms[trial] = ChooseRandomRoom();
        rewardPositions[trial][0] = RandomPositionInRoom(star1Rooms[trial]);          // random star1 position in random room

        // ensure reward doesnt spawn on the player position (later this will be pre-determined)
        while (playerStartPositions[trial] == rewardPositions[trial][0])
        {
            iterationCounter++;
            rewardPositions[trial][0] = RandomPositionInRoom(star1Rooms[trial]);

            // implement a catchment check for the while loop
            if (iterationCounter > 40)
            {
                Debug.Log("There was a while loop error:  A");
                break;
            }
        }

        // One star, or two?
        if (doubleRewardTask[trial])
        {   // generate another position for star2
            rewardPositions[trial][1] = RandomPositionInRoom(star2Rooms[trial]);      // random star2 position in random room
            iterationCounter = 0;
            // ensure rewards do not spawn on top of each other, or on top of player position
            while ((playerStartPositions[trial] == rewardPositions[trial][1]) || (rewardPositions[trial][0] == rewardPositions[trial][1]))
            {
                iterationCounter++;
                rewardPositions[trial][1] = RandomPositionInRoom(star2Rooms[trial]);

                // implement a catchment check for the while loop
                if (iterationCounter > 40) 
                {
                    Debug.Log("There was a while loop error: B");
                    break;
                }
            }
        }
        else
        {   // single star to be collected
            rewardPositions[trial][1] = rewardPositions[trial][0];
        }

    }

    // ********************************************************************** //

    private void RandomPlayerAndRewardPositions()
    {
        // This script is used for debugging purposes, to run the experiment without imposing a particular training scheme

        // This function generates trial content that randomly positions the player and reward/s in the different rooms
        int n = possibleRewardTypes.Length;
        int rewardInd;
        for (int trial = setupTrials + practiceTrials; trial < totalTrials - postTrials; trial++)
        {
            // Deal with restbreaks and regular trials
            if ((trial - setupTrials - practiceTrials + 1) % restFrequency == 0)  // Time for a rest break
            {
                trialMazes[trial] = "RestBreak";
            }
            else                                    // It's a regular trial
            {
                rewardInd = rand.Next(n);           // select a random reward type
                rewardTypes[trial] = possibleRewardTypes[rewardInd];
                trialMazes[trial] = "FourRooms_" + rewardTypes[trial];
                doubleRewardTask[trial] = true;
                GenerateRandomTrialPositions(trial);   // randomly position player start and reward/s locations
            }
        }
    }

    // ********************************************************************** //

    public float ExponentialJitter(float mean, float min, float max)
    {
        // sample a jittered time from a truncated exponential with mean = 1/lamba, where lamba is the rate of exponential.
        // use the inverse transform sampling method
        // Inputs: mean - desired mean time for the distribution the sample is jittered from ( in final output coordinates )
        //         min  - lower limit on output value
        //         max  - upper limit on output value
         
        float rate;
        float y;
        float sample = max-min;

        // adopt default values if range not specified appropriately
        if (max <= min) 
        {
            Debug.Log("ERROR: Problem specifying range of values for jitterin times. Max <= min. Using default values.");
            min = 1f;   // take some default values
            max = 7f;
        }

        rate = 1f / (mean-min);           // later we will shift the distribution so all values are greater than min, so need to remap the mean too

        // throw out all values above max and resample
        while (sample >= max-min) 
        {
            y = (float)rand.NextDouble();     // uniform random sample in x
            sample = (-1f/rate) * (float)Math.Log(1f - y);   // un-truncated exponential sample
        }

        // remap our sampled value so that our exponential lies in the appropriate new range
        sample += min;
        return sample;
    }

    // ********************************************************************** //
    // Get() and Set() Methods
    // ********************************************************************** //

    public int GetTotalTrials()
    {
        return totalTrials;
    }

    // ********************************************************************** //

    public float GetDataFrequency()
    {
        return dataRecordFrequency;
    }

    // ********************************************************************** //

    public string GetTrialMaze(int trial)
    {
        return trialMazes[trial];
    }

  // ********************************************************************** //

    public Vector3 GetPlayerStartPosition(int trial)
    {
        return playerStartPositions[trial];
    }

    // ********************************************************************** //

    public Vector3 GetPlayerStartOrientation(int trial)
    {
        return playerStartOrientations[trial];
    }

    // ********************************************************************** //

    public Vector3[] GetRewardStartPositions(int trial)
    {
        return rewardPositions[trial];
    }

    // ********************************************************************** //

    public string GetRewardType(int trial)
    {
        return rewardTypes[trial];
    }

    // ********************************************************************** //

    public bool GetIsDoubleReward(int trial)
    {
        return doubleRewardTask[trial];
    }

    // ********************************************************************** //

    public bool GetIsFreeForaging(int trial)
    {
        return freeForage[trial];
    }

    // ********************************************************************** //

    private List<Vector3> GenerateCornerRoomSpawnPositions()
    {
        List<Vector3> spawnPositions = new List<Vector3>();

        // Top-left room (yellow) positions
        for (int x = -4; x <= -1; x++)
        {
            for (int y = 1; y <= 4; y++)
            {
                spawnPositions.Add(new Vector3(x, y, playerZposition));
            }
        }

        // Bottom-right room (red) positions
        for (int x = 1; x <= 4; x++)
        {
            for (int y = -4; y <= -1; y++)
            {
                spawnPositions.Add(new Vector3(x, y, playerZposition));
            }
        }

        return spawnPositions;
    }

    private void GeneratePortalTrialSequence(int firstTrial, int numberOfTrials)
    {
        // Get all possible spawn positions
        List<Vector3> allSpawnPositions = GenerateCornerRoomSpawnPositions();

        // Shuffle the positions
        for (int i = allSpawnPositions.Count - 1; i > 0; i--)
        {
            int j = rand.Next(i + 1);
            Vector3 temp = allSpawnPositions[i];
            allSpawnPositions[i] = allSpawnPositions[j];
            allSpawnPositions[j] = temp;
        }

        // Set up trials
        for (int i = 0; i < numberOfTrials; i++)
        {
            int trial = firstTrial + i;

            // Basic trial setup
            trialMazes[trial] = "FourRooms_mushroom";
            doubleRewardTask[trial] = false;
            freeForage[trial] = false;
            maxMovementTime[trial] = 60.0f;
            blankTime[trial] = ExponentialJitter(2.5f, 1.5f, 7f);

            // Set spawn position from shuffled list
            Vector3 spawnPos = allSpawnPositions[i % allSpawnPositions.Count];
            playerStartPositions[trial] = spawnPos;

            // Set spawn room based on position
            if (spawnPos.x < 0)
            {
                playerStartRooms[trial] = "yellow"; // top-left
            }
            else
            {
                playerStartRooms[trial] = "red"; // bottom-right
            }

            // Set fixed reward position
            rewardPositions[trial] = new Vector3[1];
            rewardPositions[trial][0] = new Vector3(3f, 1f, 0f);

            // Other necessary trial setup
            controlStateOrder[trial] = new string[2] { "Human", "Human" };
            computerAgentCorrect[trial] = true;
            hallwayFreezeTime[trial] = new float[4];
            goalHitPauseTime[trial] = new float[4];
            bridgeStates[trial] = new bool[4] { true, true, true, true };

            for (int j = 0; j < 4; j++)
            {
                hallwayFreezeTime[trial][j] = ExponentialJitter(2f, 1.5f, 7f);
                goalHitPauseTime[trial][j] = ExponentialJitter(2f, 1f, 5f);
            }
        }
    }

}