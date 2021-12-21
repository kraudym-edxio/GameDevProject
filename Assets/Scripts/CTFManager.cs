using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team {
    Red,
    Blue
}

public class CTFManager : MonoBehaviour
{
    // Start is called before the first frame update

    public static HashSet<int> chosenSpawnPoints = new HashSet<int>();
    public static Transform GetRandomSpawnLocation() {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        int randIndex = Random.Range(0, spawnPoints.Length);
        
        // find spawn point that hasn't been used after picking randomly. 
        int temp = randIndex;
        while (chosenSpawnPoints.Contains(temp)) {
            temp++;
            if (temp == randIndex) {
                break;
            } else if (temp == spawnPoints.Length) {
                temp = 0;
            }
        }
        randIndex = temp;
        chosenSpawnPoints.Add(randIndex);
        return spawnPoints[randIndex].transform;
    }

    // entry point for capture the flag. 
    // required data for CTF
    public GameObject lobby;
    public GameObject[] levels;     // level prefabs to instantiate
    public int currLevelIndex = 0;

    // method to call that starts ctf
    public static void StartCTF() {
        Debug.Log("starting capture the flag match...");
    }
}
