using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team {
    Red,
    Blue
}

public class CTFManager : MonoBehaviour
{
    public GameObject redTeamChicken;
    public GameObject blueTeamChicken;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static Transform GetRandomSpawnLocation() {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        int randIndex = Random.Range(0, spawnPoints.Length);

        return spawnPoints[randIndex].transform;
    }
}
