using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDriver : MonoBehaviour
{
    public LevelManager levelManager;
    private PlayerController player;

    private float zPosCamera;
    //limit of height before camera follows player.
    public float yLimMove = 2.0f;

    //smoothness of camera tracking
    [SerializeField]
    private float smoothTime;
    private Vector3 _velocity;



    // Start is called before the first frame update
    void Awake()
    {
        //levelManager = GameObject.FindObjectOfType<LevelManager>();
        player = levelManager.player;
        zPosCamera = this.transform.position.z;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        moveCamera();
    }
    private void FixedUpdate()
    {


    }

    private void moveCamera()
    {
        Vector3 newPosition = this.transform.localPosition;
        float player_x = player.transform.localPosition.x;
        float player_y = player.transform.localPosition.y;
        //float ylim = player_y + yLimMove;

        if (player_x > levelManager.levelStart.x && player_x < levelManager.levelEnd.x)
        {
            newPosition.x = player_x;
            
        }
        if(player_y > yLimMove)
        {
            newPosition.y = player_y - yLimMove;
        }

        //into world space
        this.transform.localPosition = Vector3.SmoothDamp(transform.localPosition, newPosition, ref _velocity, smoothTime);
        
    }
}
