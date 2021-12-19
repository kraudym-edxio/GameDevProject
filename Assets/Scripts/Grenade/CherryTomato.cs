using UnityEngine;

public class CherryTomato : MonoBehaviour
{

    /*Variables*/
    //Time for fuse
    public float fuseDelay = 3f;
    //Radius of explosion
    public float radius = 5f;
    //Force at which the "grenade" is thrown
    public float force = 700f;

    //Explosion countdown
    float countdown;
    bool hasExploded = false;

    //Add explosion effect from the editor, used when "grenade" explodes
    public GameObject explosionEffect;

    //Sounds
    private AudioSource audioPlayer;
    [SerializeField]
    private AudioClip explodeClip;

    // Set the fuse timer on Awake
    void Awake()
    {
        countdown = fuseDelay;
        audioPlayer = GetComponent<AudioSource>();

    }

    // 
    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f && !hasExploded) {

            audioPlayer.PlayOneShot(explodeClip);
            Debug.Log("Explode sound played");
            Explode();
            hasExploded = true;
        }
    }


    void Explode() {
        Debug.Log("Boooom!!!!!!");
        

        //Show Effect
        Instantiate(explosionEffect, transform.position, transform.rotation);

        //USED TO SWITCH PREFAB WITH Destructable CLASS
        //Used with the Destructable.cs Script to destroy each object (that is destructable) in the radius,
        //and Destructable.cs will load the broke prefab for the Destroyed Object
        /*START
        Collider[] collidersToDestroy = Physics.OverlapSphere(transform.position, radius);

        foreach(Collider nearbyObject in collidersToDestroy)
        {

            Destructable dest = nearbyObject.GetComponent<Destructable>();
            if (dest != null)
            {
                dest.Destroy();
            }
        }
        END*/

        Collider[] collidersToMove = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider nearbyObject in collidersToMove)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Debug.Log("Name:"+ rb.name);
                rb.AddExplosionForce(force, transform.position, radius);
            }

        }

        // Remove "Grenade"
        Destroy(gameObject);
    }
}
