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

    private void OnCollisionEnter(Collision other)
    {
        // if we hit an enemy, do damage to the enemy
        if (other.gameObject.GetComponent<CTFPlayerManager>().playerTeam == enemy)
        {
            // do damage to enemy
            other.gameObject.GetComponent<PlayerControllerNetworking>().TakeDmg(dmg);
        }
        Destroy(gameObject);
    }

}