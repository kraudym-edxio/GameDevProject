using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    private void OnDisable()
        {
            Invoke("Activate", 10);
        }
    
    private void Activate()
        {
            gameObject.SetActive(true);
        }
}
