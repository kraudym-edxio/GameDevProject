using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public enum Team {
    None,
    Red,
    Blue,
    Error
}

public class CTFManager : NetworkBehaviour 
{
    public HashSet<int> chosenSpawnPoints = new HashSet<int>();

    // needed prefabs
    public GameObject redFlag;
    public GameObject blueFlag;

    // entry point for capture the flag. 
    // required data for CTF
    public int currLevelIndex = 2;  // 0=menu, 1=lobby, 2=level1, 3=level2, 4=level3

    // amount of wins by each team: 10 declares victory
    [SyncVar]
    public bool inGame = false;

    [SyncVar]
	public int redWins = 0;

    [SyncVar]
    public int blueWins = 0;

    public int winLimit = 1;

    // method to call that starts ctf
    public void StartCTF() {
        Debug.Log("starting capture the flag match...");
        chosenSpawnPoints = new HashSet<int>();     // erase old spawn points
        foreach(var g in GameObject.FindGameObjectsWithTag("hasAudio")) {
            g.GetComponent<AudioSource>().mute = true;
        }
        if (!inGame) {
            SceneManager.LoadScene(currLevelIndex);
            inGame = true;
            // coroutine waits for scene to load before getting objects in scene
            StartCoroutine("startCTFOnceSceneLoads");
        } else {
            Debug.Log("game already started... wait for game to end. ");
        }
    }

    public void EndCTF() {
        SceneManager.LoadScene("Lobby");
        inGame = false;
        redWins = 0;
        blueWins = 0;
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

            var nmh = GameObject.Find("/NetworkManager").GetComponent<NetworkManagerHUD>();
            nmh.useLobbyGUI = false;
            nmh.SendMessage("SetHUD");
            
            SetFlags();
        }

    }

    public void SetFlags() {
        var redFlagSpawnLocations = GameObject.FindGameObjectsWithTag($"RedFlagSpawn");
        var blueFlagSpawnLocations = GameObject.FindGameObjectsWithTag($"BlueFlagSpawn");

        if (redFlagSpawnLocations.Length > 0 && blueFlagSpawnLocations.Length > 0) {
            var redChoice = Random.Range(0, redFlagSpawnLocations.Length);
            var blueChoice = Random.Range(0, blueFlagSpawnLocations.Length);

            Debug.Log(redChoice);

            var r = Instantiate(redFlag, redFlagSpawnLocations[redChoice].transform);
            var b = Instantiate(blueFlag, blueFlagSpawnLocations[blueChoice].transform);

            r.transform.parent = GameObject.Find("/RedArea").transform;
            b.transform.parent = GameObject.Find("/BlueArea").transform;
        }
    }
    
    // helper functions
    public Transform GetRandomSpawnLocation(bool useTeamSpawn=false, Team t=Team.Red) {
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


}
