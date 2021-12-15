using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField]
    public float speed = 50f;
    private float timeToDestroy = 3f;
    public int dmg;
    public Vector3 target;

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
        if (other.collider.CompareTag("Enemy"))
        {
            // do damage to enemy
        }
        Destroy(gameObject);
    }

}
