using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    private bool alreadyTriggered;

    //Sounds
    private AudioSource audioPlayer;
    [SerializeField]
    private AudioClip audioClip;
    void Awake()
    {
        audioPlayer = GetComponent<AudioSource>();
        alreadyTriggered = false;

    }
    //Detect collisions between the GameObjects with Colliders attached
    void OnTriggerEnter(Collider collision)
    {
        Debug.Log(collision.gameObject.name);

        //Check for a match with the specified name on any GameObject that collides with your GameObject
        if (collision.gameObject.tag == "Player")
        {
            //If the GameObject's name matches the one you suggest, output this message in the console
            //If sound not played than play audio clip
            if (!alreadyTriggered){
                audioPlayer.PlayOneShot(audioClip);
                alreadyTriggered = true;
            }
        }

    }
}
