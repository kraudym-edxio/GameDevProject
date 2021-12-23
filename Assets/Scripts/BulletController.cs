using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class BulletController : NetworkBehaviour
{
    [SerializeField]
    public float speed = 50f;
    private float timeToDestroy = 3f;
    public int dmg;
    public Vector3 target;
    public Team enemy;

    private void OnEnable()
    {
        Destroy(gameObject, timeToDestroy);
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // if we hit an enemy, do damage to the enemy
        if (other.CompareTag("Player") && other.GetComponent<CTFPlayerManager>().playerTeam == enemy)
        {
            // do damage to enemy
            other.GetComponent<PlayerControllerNetworking>().TakeDmg(dmg);
        }
        Destroy(gameObject);
    }

}