using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public Transform player;

    //Uses LateUpdate to update minimap after the player has moved
    private void LateUpdate()
    {
        //Follow the player
        Vector3 newPostion = player.position;
        newPostion.y = transform.position.y;
        transform.position = newPostion;

        //Used for rotation of the minimap camera
        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
    }

}
