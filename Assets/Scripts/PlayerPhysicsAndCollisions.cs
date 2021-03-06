using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CompositeCollider2D))]

public class PlayerPhysicsAndCollisions : MonoBehaviour
{
    public BoxCollider2D colliders;
    private PlayerController playerController;


    // Start is called before the first frame update
    void Start()
    {
        //colliders = GetComponent<BoxCollider2D>();
        playerController = GetComponentInParent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if collided with ground. Find a better way of doing this
        if(collision.gameObject.layer == 7 && collision.otherCollider.gameObject == colliders.gameObject)
        {
            playerController.Landed();
        }
    }
}
