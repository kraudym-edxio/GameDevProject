using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HealthBoost : MonoBehaviour
{
    public int pickupCount = 0;

    public void Start()
    {
        
    }

    public void OnTriggerEnter(Collider Col)
    {
        if (Col.gameObject.tag == "health")
        {
            pickupCount++;
            
            
            Col.gameObject.SetActive(false);
            Destroy(Col.gameObject);
        }
        
    }
    
    private void OnDestroy() {
        int n = PlayerPrefs.GetInt("pickupCount", 0);
        PlayerPrefs.SetInt("pickupCount", pickupCount+n);
    }

}
