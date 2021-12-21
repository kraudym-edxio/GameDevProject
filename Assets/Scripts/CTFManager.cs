using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public int currLevelIndex = 1;

    // method to call that starts ctf
    public void StartCTF() {
        Debug.Log("starting capture the flag match...");
        SceneManager.LoadScene($"Level_0{currLevelIndex}");
        currLevelIndex++;
        if (currLevelIndex==4) currLevelIndex=1;
    }
}
