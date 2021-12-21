using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTFPlayerManager : MonoBehaviour
{
    public static List<Team> ChosenTeams = new List<Team>();

    public Team playerTeam;


    void Start() {
        int numChoice = Random.Range(0,2);
        playerTeam = numChoice == 0 ? Team.Red : Team.Blue;

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
