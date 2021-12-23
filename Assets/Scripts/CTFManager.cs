using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    public Text redCountGUI;
    public Text blueCountGUI;

    [SyncVar]
    public int redFlagChoice = -1;
    [SyncVar]
    public int blueFlagChoice = -1;

    void Update() {
        try {
            if (inGame && (redFlagChoice == -1 || blueFlagChoice == -1)) {
                var redFlagSpawnLocations = GameObject.FindGameObjectsWithTag($"RedFlagSpawn");
                var blueFlagSpawnLocations = GameObject.FindGameObjectsWithTag($"BlueFlagSpawn");
                redFlagChoice = Random.Range(0, redFlagSpawnLocations.Length);
                blueFlagChoice = Random.Range(0, blueFlagSpawnLocations.Length);

                var r = GameObject.FindGameObjectWithTag("RedFlag");
                var b = GameObject.FindGameObjectWithTag("BlueFlag");

                r.transform.position = redFlagSpawnLocations[redFlagChoice].transform.position;
                b.transform.position = blueFlagSpawnLocations[blueFlagChoice].transform.position;

                Debug.Log(r.transform.position);
            }
        } catch (System.Exception e){
            redFlagChoice = -1;
            blueFlagChoice = -1;
            //Debug.LogError(e);
        }
    }

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

            if (SceneManager.GetActiveScene().buildIndex != 1) {
                redCountGUI = GameObject.Find("/Canvas").transform.Find("ScoreGUI").Find("RedTeam").Find("ScoreText").GetComponent<Text>();
                blueCountGUI = GameObject.Find("/Canvas").transform.Find("ScoreGUI").Find("BlueTeam").Find("ScoreText").GetComponent<Text>();
                redCountGUI.text = $"Red Score: {redWins}";
                blueCountGUI.text = $"Blue Score: {blueWins}";
            }

            currLevelIndex++;
            if (currLevelIndex==5) currLevelIndex=2;

            foreach(var g in GameObject.FindGameObjectsWithTag("Player")) {
                g.GetComponent<PlayerControllerNetworking>().SetPauseMenu();
                g.GetComponent<PlayerControllerNetworking>().SetPosition();
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
            if (isServer) {
                var redFlagChoice = Random.Range(0, redFlagSpawnLocations.Length);
                var blueFlagChoice = Random.Range(0, blueFlagSpawnLocations.Length);
                var r = Instantiate(redFlag, redFlagSpawnLocations[redFlagChoice].transform);
                var b = Instantiate(blueFlag, blueFlagSpawnLocations[blueFlagChoice].transform);
                NetworkServer.Spawn(r);
                NetworkServer.Spawn(b);
            }
        }
    }
    
    // helper functions
    public Transform GetRandomSpawnLocation2(Team t=Team.Red) {
        if (SceneManager.GetActiveScene().buildIndex == 1) { // is lobby
            return GetRandomSpawnLocation(false, t);
        } 
        return GetRandomSpawnLocation(true, t);
    }
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
