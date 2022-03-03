using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //speed in units/second
    public float maxSpeed = 2f;
    private Rigidbody2D _mRB;
    public float jumpForce = 8f;
    public float dashForce = 4f;
    
    public JumpingState jumpingState = JumpingState.Grounded;
    
    Vector2 _movementSinceLastPhysUpdate = new Vector2(0,0);
    bool jumpinPhysicsUpdate = false;

    // Start is called before the first frame update
    void Awake()
    {
        _mRB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //PlayerInput();
    }
    private void FixedUpdate()
    {
        _mRB.position += _movementSinceLastPhysUpdate;

        _mRB.velocity.Set(Mathf.Min(_mRB.velocity.x, maxSpeed),_mRB.velocity.y);

        _movementSinceLastPhysUpdate = new Vector2(0, 0);

        if(jumpinPhysicsUpdate)
        {
            PhysicsUpdateJump();
            jumpinPhysicsUpdate = false;
        }
    }


    /// <summary>
    /// Human Player input
    /// </summary>
    public void PlayerInput(in ActionBuffers actionsOut)
    {
        /* float horizontal = Input.GetAxis("Horizontal");
         float vertical = Input.GetAxis("Vertical");

         Vector2 distanceMoved = Instruct_Movement(horizontal, vertical);

         _movementSinceLastPhysUpdate += distanceMoved;
         if (Input.GetKeyUp("space"))
         {
             Instruct_Jump();
         }
         if (Input.GetKeyUp("s"))
         {
             Instruct_Stomp();
         }
         if (Input.GetKeyUp("left shift"))
         {
             Instruct_Dash();
         }*/

        //This allows access to the otherwise readonly discreet actions.
        //If funtionality is not as it should be this seems like a fragile piece of code. 
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        float horizontal = Input.GetAxis("Horizontal");

        continuousActions[0] = (int) horizontal;

        if (Input.GetKeyUp("space"))
        {
            discreteActions[1] = 1;
        }
        if (Input.GetKeyUp("s"))
        {
            discreteActions[2] = 1;
        }
        if (Input.GetKeyUp("left shift"))
        {
           discreteActions[3] = 1;
        }

    }

    public void aiCallMovement(float horizontal, float vertical)
    {

        _movementSinceLastPhysUpdate += Instruct_Movement(horizontal, vertical);

    }


    public Vector2 Instruct_Movement(float horizontal, float vertical)
    {
        Vector2 distanceMoved;
        Movement_Horizontal(horizontal, vertical, out distanceMoved);
        return distanceMoved;
    }

    private void Movement_Horizontal(float horizontal, float vertical, out Vector2 distanceMoved)
    {
        Vector2 movement = new Vector2(horizontal, vertical);
        //Debug.Log($"Movement executed. {movement}");

        //Speed not dependant on framerate
        distanceMoved = movement * Time.deltaTime * maxSpeed;

    }

    public  void Instruct_Stomp()
    {
        if ((jumpingState != JumpingState.Grounded && jumpingState != JumpingState.Stomping))
        {
            Movement_Stomp();
        }
    }

    public void Instruct_Dash()
    {
        if ((jumpingState != JumpingState.Grounded) && (jumpingState != JumpingState.Stomping))
        {
            Movement_Dash();
        }
    }

    public void Instruct_Jump()
    {
        jumpinPhysicsUpdate = true;
    }

    private void PhysicsUpdateJump()
    {
        if (jumpingState < JumpingState.DoubleJump)
        {
            Movement_Jump();
        }
    }

    private void Movement_Jump()
    {
        float extraForce = 0;
        //when Falling add more force
        if (_mRB.velocity.y < 0)
        {
            float currentVel = _mRB.velocity.y;

            //ForceMode.Impulse handles physics of object
            extraForce = -currentVel;
            //Debug.Log("Extra force = " + extraForce);
            //F=ma 
            extraForce *= _mRB.mass;
        }


        _mRB.AddForce(Vector2.up * (jumpForce + extraForce), ForceMode2D.Impulse);
        jumpingState++;
        Debug.Log("jumping state = " + jumpingState +" at frame "+Time.frameCount);
    }

    private void Movement_Dash()
    {
        jumpingState = JumpingState.Dash;
        _mRB.AddForce(_mRB.velocity * dashForce, ForceMode2D.Impulse);

        //Debug.Log("Jumping State = " + jumpingState);
    }

    private void Movement_Stomp()
    {
        jumpingState = JumpingState.Stomping;
        _mRB.AddForce(Vector2.down * jumpForce, ForceMode2D.Impulse);

       // Debug.Log("Jumping state = " + jumpingState);
    }

    public void Landed()
    {
        jumpingState = JumpingState.Grounded;
        Debug.Log("Jumping state = Grounded at frame "+ Time.frameCount);
    }
}
