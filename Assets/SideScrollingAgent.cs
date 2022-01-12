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
        
        base.OnEpisodeBegin();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(this.rigidbody2D.velocity);
    }

  

    public override void OnActionReceived(ActionBuffers actions)
    {
       // Debug.Log($"normalized distance to goal = {normalisedDistanceToGoal()}");

       

        if(normalisedDistanceToGoal() < 0.05f)
        {
            AddReward(1.0f);
            EndEpisode();
        }
        if(this.transform.localPosition.y <-10)
        {
            AddReward(-1.0f);
            EndEpisode();

        }
        //add a small penalty for not finishing the level
        //gives a time limit of 100 seconds (hard coded)
        else
        {
            AddReward(-0.01f * Time.deltaTime);

            int movementControl = actions.DiscreteActions[0];

            int jumpControl = actions.DiscreteActions[1];

            playerController.Instruct_Movement(movementControl,0);

            if (jumpControl > 0.9)
            {
                playerController.Instruct_Jump();
            }

        }
        Vector2 controlSignal = Vector2.zero;
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        playerController.PlayerInput();



    }

}
