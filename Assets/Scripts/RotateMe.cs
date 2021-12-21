using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMe : MonoBehaviour
{
    Vector3 rotation;

    // Start is called before the first frame update
    void Start() 
    { 
        rotation = new Vector3(30, 15, 45);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotation * Time.deltaTime);
    }
    
}