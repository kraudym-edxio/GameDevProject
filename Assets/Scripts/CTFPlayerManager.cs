using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CTFPlayerManager : NetworkBehaviour
{

    [SyncVar]
    public Team playerTeam;

    void Start() {
        DontDestroyOnLoad(gameObject);

        if (playerTeam == Team.None) {

            // check if teams are unbalanced: if so balance by choosing team with lowest player count
            int redCount = 0;
            int blueCount = 0;
            foreach(var g in GameObject.FindGameObjectsWithTag("Player")) {
                var otherTeam = g.GetComponent<CTFPlayerManager>().playerTeam;
                switch(otherTeam) {
                    case Team.Red:
                        redCount++;
                        break;
                    case Team.Blue:
                        blueCount++;
                        break;
                    default:    // if unassigned
                        break;
                }
            }
            Debug.Log($"red count {redCount}, blue count: {blueCount}");

            // if balanced, choose randomly
            if (redCount == blueCount) {
                int numChoice = Random.Range(0,2);
                playerTeam = numChoice == 0 ? Team.Red : Team.Blue;
            } else if (redCount > blueCount) {
                playerTeam = Team.Blue;
            } else {
                playerTeam = Team.Red;
            }

        }

        switch (playerTeam) {
            case Team.Red:
                transform.Find("RedChicken").gameObject.SetActive(true);
                transform.Find("BlueChicken").gameObject.SetActive(false);
                break;
            case Team.Blue:
                transform.Find("RedChicken").gameObject.SetActive(false);
                transform.Find("BlueChicken").gameObject.SetActive(true);
                break;
        }
        transform.position = GameObject.Find("/NetworkManager").GetComponent<CTFManager>().GetRandomSpawnLocation().position;
        Debug.Log(transform.position);
    }
}
