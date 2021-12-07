using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject characterPrefab;
    [SerializeField]
    private float respawnTime = 5f;

    private float nextSpawnTime;


    void Update()
    {
        if(Time.time >= nextSpawnTime)
        {
            nextSpawnTime = Time.time + respawnTime;

            GameObject.Instantiate(characterPrefab, transform.position, transform.rotation);
        }
        
    }
}
