using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Sensors.Reflection; //for using the Observable attribute
using Unity.MLAgents.Actuators;
using System;

public class SideScrollingAgent : Agent
{
    public GameObject instanceBase;
    PlayerController playerController;
    LevelCreator levelCreator;
    StatsRecorder agentStats;

    Rigidbody2D rigidbody2D;

    [SerializeField]
    private int distanceRewardTimestepGap = 50;
    private float minDistanceReached = 1.0f;


    public int levelSeriesSeed = 0000000;
    private int episodeNumber = 0;

   

    [Observable]
    JumpingState jumpingState { get { return playerController.jumpingState; } }

    // Start is called before the first frame update
    void Start()
    {
        if(instanceBase is null)
        {
            Debug.LogError($"ATTACH A GAMEOBJECT TO INSTANCE BASE IN {this}");
        }
        agentStats = Academy.Instance.StatsRecorder;
        playerController = GetComponent<PlayerController>();
        levelCreator = instanceBase.GetComponentInChildren<LevelCreator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        
    }

    float normalisedDistanceToGoal()
    {
        float length = levelCreator.endFlag.transform.localPosition.x - levelCreator.startFlag.transform.localPosition.x;
        float posAlongPath = levelCreator.endFlag.transform.localPosition.x - this.transform.localPosition.x;

        return (posAlongPath / length);
    }

    //This creates a random seed from a seed given. Allows episodes to have drastically different levels
    int newLevelSeed(int randSeed)
    {
        UnityEngine.Random.InitState(randSeed);
        float fVal = UnityEngine.Random.value;
        int val =(int)( fVal * 10000000);
        return val;

    }

    public override void OnEpisodeBegin()
    {
        episodeNumber++;

        int nextSeed = newLevelSeed(levelSeriesSeed + episodeNumber);
        levelCreator.GenerateLevel(nextSeed);
        //levelCreator.GenerateLevel();

        this.rigidbody2D.transform.localPosition = levelCreator.startFlag.transform.localPosition;
        this.rigidbody2D.velocity = Vector3.zero;
        minDistanceReached = 1.0f;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(this.rigidbody2D.velocity.x);
        sensor.AddObservation(this.rigidbody2D.velocity.y);
        sensor.AddObservation((int) jumpingState);

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Debug.Log($"normalized distance to goal = {normalisedDistanceToGoal()}");
        //Debug.Log($"Continuous buffers [0] = {actions.ContinuousActions.Array[0]}");
        string outputActions = actions.ContinuousActions.Array.ToString();
        //Debug.Log($"Continuous Buffer = {outputActions}");

        if(normalisedDistanceToGoal() < 0.02f)
        {
            agentStats.Add("Level Complete", 1);

            //Only recorded on successful attempts as the whole level will have been completed
            agentStats.Add("Jump Accuracy", playerController.jumpCounter / levelCreator.numJumps);
            playerController.jumpCounter = 0;
            AddReward(1.0f);
            EndEpisode();
        }
        //If agent has fallen off 
        if (this.transform.localPosition.y < -10)
        {
            //Large negative reward for falling behind the start position
            if (normalisedDistanceToGoal() > 1)
            {
                agentStats.Add("Level Complete", 0);
                AddReward(-1.0f);
                EndEpisode();
            }
            //Smaller negative reward for falling throughout the level
            else
            {
                AddReward(-0.2f);
                EndEpisode();
            }

            //If the agent is about to call the final step, record the episode as not completed
            if (StepCount == MaxStep - 1)
            {
                agentStats.Add("Level Complete", 0);
            }


        }

        else
        {
            //add a small penalty for not finishing the level
            //gives a time limit of 100 seconds (hard coded) 
            //AddReward(-0.01f * Time.deltaTime);

            //AddReward(0.01f * Time.deltaTime * (1 - normalisedDistanceToGoal()));
            
            

            //AddReward(0.001f * (1 - normalisedDistanceToGoal()));
            
            if(Academy.Instance.StepCount%distanceRewardTimestepGap==0)
            {
                _RewardDistanceTravelled();
            }

            //AddReward(0.001f * (1 - normalisedDistanceToGoal()));
            



            float movementControl = actions.ContinuousActions[0];

            float jumpControl = actions.DiscreteActions[0];

            float StompControl = actions.DiscreteActions[1];

            float dashControl = actions.DiscreteActions[2];

            playerController.aiCallMovement(movementControl,0);

            if (jumpControl > 0.9f)
            {
                playerController.Instruct_Jump();
            }
            if (StompControl > 0.9f)
            {
                playerController.Instruct_Stomp();
            }
            if(dashControl > 0.9f)
            {
                playerController.Instruct_Dash();
            }
        }
        Vector2 controlSignal = Vector2.zero;
    }

    private void _RewardDistanceTravelled()
    {
        //Agent is not as far as it's maximum or has stayed still within this episode
        if (normalisedDistanceToGoal() >= minDistanceReached)
        {
            //return nothing
        }

        //The agent has moved forward from the previous maximum record
        else
        {
            minDistanceReached = normalisedDistanceToGoal();
            AddReward(0.01f * normalisedDistanceToGoal()*((MaxStep-StepCount))/MaxStep);
            //Debug.Log("distance reward = " + 0.01f * normalisedDistanceToGoal() * ((MaxStep - StepCount)) / MaxStep);
        }
                
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        playerController.PlayerInput(in actionsOut);



    }

}
