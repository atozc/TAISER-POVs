using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketFilterUIOnCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        destination = transform.parent.GetComponentInChildren<TDestination>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public TDestination destination;
    

    //Detect collisions between the GameObjects with Colliders attached
    void OnCollisionEnter(Collision collision)
    {
        //Check for a match with the specified name on any GameObject that collides with your GameObject
        if (collision.gameObject.name == "Door")
        {
            //If the GameObject's name matches
            Debug.Log("Player collided with door");

            NewGameMgr.inst.OnAttackableDestinationClicked(destination);
        }
        else
        {
            Debug.Log("Player did not collide with door");
        }
    }
}
