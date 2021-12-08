using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JumpingState
{
    Grounded = 0,
    SingleJump = 1,
    DoubleJump = 2,
    Stomping = 3,
    Falling = 9
}

public class PlayerController : MonoBehaviour
{
    //speed in units/second
    public float maxSpeed = 2;
    Rigidbody2D _mRB;
    public float jumpForce = 8;
    
    JumpingState jumpingState = JumpingState.Grounded;
    
    
    bool _jumping = false;

    Vector2 _movementSinceLastPhysUpdate = new Vector2(0,0);

    // Start is called before the first frame update
    void Start()
    {
        _mRB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
    }
    private void FixedUpdate()
    {
        _mRB.position += _movementSinceLastPhysUpdate;
        _movementSinceLastPhysUpdate = new Vector2(0, 0);
    }

    void PlayerInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        //Speed not dependant on framerate
        Vector2 distanceMoved = new Vector2(horizontal, 0.0f)* Time.deltaTime * maxSpeed;

        _movementSinceLastPhysUpdate += distanceMoved;

        if (Input.GetKeyUp("space")&& jumpingState < JumpingState.DoubleJump)
        {
            float extraForce = 0;
            //when Falling add more force
            if(_mRB.velocity.y < 0)
            {
                float currentVel = _mRB.velocity.y;

                //ForceMode.Impulse handles physics of object
                extraForce = -currentVel;
                Debug.Log("Extra force = " + extraForce);
                //F=ma 
                extraForce *= _mRB.mass;
            }


            _mRB.AddForce(Vector2.up * (jumpForce+extraForce),ForceMode2D.Impulse);
            jumpingState++;
            Debug.Log("jumping state = "+ jumpingState);
        }

        if(Input.GetKeyUp("s") && (jumpingState != JumpingState.Grounded && jumpingState != JumpingState.Stomping))
        {
            jumpingState = JumpingState.Stomping;
            _mRB.AddForce(Vector2.down* jumpForce, ForceMode2D.Impulse);

            Debug.Log("Jumping state = " + jumpingState);
        }

    }

    public void Landed()
    {
        jumpingState = JumpingState.Grounded;
        Debug.Log("Landed called");
    }
}
