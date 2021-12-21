using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTFPlayerManager : MonoBehaviour
{
    public static List<Team> ChosenTeams = new List<Team>();

    public Team playerTeam;

    void Start() {
        // check if teams are unbalanced: if so balance by choosing team with lowest player count
        int redCount = 0;
        int blueCount = 0;
        for (int i = 0; i < ChosenTeams.Count; i++) {
            switch(ChosenTeams[i]) {
                case Team.Red:
                    redCount++;
                    break;
                case Team.Blue:
                    blueCount++;
                    break;
            }
        }
        // if balanced, choose randomly
        if (redCount == blueCount) {
            int numChoice = Random.Range(0,2);
            playerTeam = numChoice == 0 ? Team.Red : Team.Blue;
        } else if (redCount > blueCount) {
            playerTeam = Team.Blue;
        } else {
            playerTeam = Team.Red;
        }
        ChosenTeams.Add(playerTeam);

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
        transform.position = CTFManager.GetRandomSpawnLocation().position;
    }
}
