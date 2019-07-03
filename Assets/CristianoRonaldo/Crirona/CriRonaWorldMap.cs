using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using UnityEngine.Networking;

public class CriRonaWorldMap : NetworkBehaviour{

    private ExampleARSessionManager _arSessionManager;
    private NetworkTransmitter _networkTransmitter;

    void Start()
    {

    }

    void Update()
    {

    }

    [Command]
    public void CmdSendWorldMap()
    {
        ARWorldMap arWorldMap = _arSessionManager.GetSavedWorldMap();
        StartCoroutine(_networkTransmitter.SendBytesToClientsRoutine(0, arWorldMap.SerializeToByteArray()));
    }
}