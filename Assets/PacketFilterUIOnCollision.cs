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

    void OnCollisionEnter(Collision collision)
    {
        //Detect Collsions
        //Debug.Log(collision.gameObject.name + " Collided with: " + transform.gameObject.name + " with parent: " + collision.transform.parent.name);

        //Bring up UI
        if (collision.gameObject.name == "PlayerArmature" && transform.gameObject.name == "MaliciousCube")
        {
            NewGameMgr.inst.OnAttackableDestinationClicked(destination);
        }
        else
        {
            //Debug.Log("Ash did not collide with a destination building");
        }
    }
}
