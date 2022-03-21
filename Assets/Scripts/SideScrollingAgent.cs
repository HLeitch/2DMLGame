using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Sensors.Reflection; //for using the Observable attribute
using Unity.MLAgents.Actuators;

public class SideScrollingAgent : Agent
{
    public GameObject instanceBase;
    PlayerController playerController;
    LevelCreator levelCreator;
    
    Rigidbody2D rigidbody2D;

    [Observable]
    JumpingState jumpingState { get { return playerController.jumpingState; } }

    // Start is called before the first frame update
    void Start()
    {
        if(instanceBase is null)
        {
            Debug.LogError($"ATTACH A GAMEOBJECT TO INSTANCE BASE IN {this}");
        }
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


    public override void OnEpisodeBegin()
    {
        levelCreator.GenerateLevel();

        this.rigidbody2D.transform.localPosition = levelCreator.startFlag.transform.localPosition;
        this.rigidbody2D.velocity = Vector3.zero;
        
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

        if(normalisedDistanceToGoal() < 0.05f)
        {
            AddReward(1.0f);
            EndEpisode();
        }
        //If agent has fallen off
        if(this.transform.localPosition.y <-10)
        {
            //Large negative reward for falling behind the start position
            if (normalisedDistanceToGoal() > 1)
            {
                AddReward(-1.0f);
                EndEpisode();
            }
            //Smaller negative reward for falling throughout the level
            else
            {
                AddReward(-0.2f);
                EndEpisode();
            }

        }
        
        else
        {
            //add a small penalty for not finishing the level
            //gives a time limit of 100 seconds (hard coded) 
            //AddReward(-0.01f * Time.deltaTime);

            AddReward(0.01f * Time.deltaTime * (1 - normalisedDistanceToGoal()));

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
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        playerController.PlayerInput(in actionsOut);



    }

}
