using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour{

    GameObject Camera;

    void Start()
    {
        Camera = GameObject.Find("Camera");
    }
    private void Update()
    {
        transform.position = Camera.transform.position;
    }
}
