using UnityEngine;

public class CollisionTrigger : MonoBehaviour
{
    public void Start()
    {
        Debug.Log("START");
    }


    //Detect collisions between the GameObjects with Colliders attached
    void OnTriggerEnter(Collider collision)
    {
        Debug.Log(collision.gameObject.name);

        //Check for a match with the specified name on any GameObject that collides with your GameObject
        if (collision.gameObject.tag == "Player")
        {
            //If the GameObject's name matches the one you suggest, output this message in the console
            Debug.Log("PLAYER");
        }
        else {
            //If the GameObject's name matches the one you suggest, output this message in the console
            Debug.Log("NOT PLAYER");
        }


    }
}