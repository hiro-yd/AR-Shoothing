using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour
{

    void Start()
    {
        if (isLocalPlayer)
            GetComponent<Player>().enabled = true;
        else if (isClient == false)
        {
            transform.localPosition = new Vector3(0, 0, 0);
        }
        enabled = false;
    }

    void Update()
    {
        transform.localPosition = new Vector3(0, 0, 0);
    }

    public override void OnStartLocalPlayer()
    {
        if (isServer)
        {
            GetComponent<Player>().enabled = true;
        }
        else if (isServer == false)
        {
            //transform.SetParent(GameObject.Find("Camera").transform);
            transform.localPosition = new Vector3(0, 0, 0);
        }
        GetComponent<NetworkTransform>().enabled = true;
        GetComponent<ARLocationSync>().enabled = true;
        //GameObject.Find("Camera").transform.parent = null;
    }
}
