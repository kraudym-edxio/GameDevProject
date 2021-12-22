using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public enum Team {
    Red,
    Blue
}

public class CTFManager : MonoBehaviour
{
    // Start is called before the first frame update

    public static HashSet<int> chosenSpawnPoints = new HashSet<int>();
    public static Transform GetRandomSpawnLocation(bool useTeamSpawn=false, Team t=Team.Red) {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        if (useTeamSpawn) {
            if (t == Team.Red) {
                Debug.Log("red");
                spawnPoints = GameObject.FindGameObjectsWithTag("RedSpawnPoint");
            } else if (t == Team.Blue) {
                Debug.Log("blue");
                spawnPoints = GameObject.FindGameObjectsWithTag("BlueSpawnPoint");
            }
        }

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
        Debug.Log($"randIndex: {randIndex}, temp: {temp}, spawnPoints.Length: {spawnPoints.Length}");
        randIndex = temp;
        chosenSpawnPoints.Add(randIndex);
        return spawnPoints[randIndex].transform;
    }

    // entry point for capture the flag. 
    // required data for CTF
    public int currLevelIndex = 2;  // 0=menu, 1=lobby, 2=level1, 3=level2, 4=level3

    // method to call that starts ctf
    public void StartCTF() {
        Debug.Log("starting capture the flag match...");
        chosenSpawnPoints = new HashSet<int>();     // erase old spawn points
        foreach(var g in GameObject.FindGameObjectsWithTag("hasAudio")) {
            g.GetComponent<AudioSource>().mute = true;
        }
        SceneManager.LoadScene(currLevelIndex);
        // coroutine waits for scene to load before getting objects in scene
        StartCoroutine("startCTFOnceSceneLoads");
    }

    private IEnumerator startCTFOnceSceneLoads() {
        while (SceneManager.GetActiveScene().buildIndex != currLevelIndex) {
            yield return null;
        }
        if (SceneManager.GetActiveScene().buildIndex == currLevelIndex) {

            currLevelIndex++;
            if (currLevelIndex==5) currLevelIndex=2;

            foreach(var g in GameObject.FindGameObjectsWithTag("Player")) {
                g.GetComponent<PlayerControllerNetworking>().SendMessage("SetPauseMenu");
                g.GetComponent<PlayerControllerNetworking>().SendMessage("SetPosition");
            }

            var nmh = gameObject.GetComponent<NetworkManagerHUD>();
            nmh.useLobbyGUI = false;
            nmh.SendMessage("SetHUD");
        }

    }

    public static void DestroyNM() {
        Debug.Log("destroying nm?");
        Destroy(GameObject.Find("NetworkManager"));
    }
}
