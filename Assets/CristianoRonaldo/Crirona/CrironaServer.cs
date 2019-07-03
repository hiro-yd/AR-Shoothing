using System;
using UnityEngine;
using UnityEngine.Networking;

public class CrironaServer : NetworkDiscovery
{
    public CrironaNet networkManager;
    public BroadcastData broadcastDataObject;
    public bool isOpen { get { return broadcastDataObject.isOpen; } set { broadcastDataObject.isOpen = value; } }
    public int numPlayers { get { return broadcastDataObject.numPlayers; } set { broadcastDataObject.numPlayers = value; } }
    public int serverScore { get { return broadcastDataObject.serverScore; } set { broadcastDataObject.serverScore = value; } }
    public string privateTeamKey { get { return broadcastDataObject.privateTeamKey; } set { broadcastDataObject.privateTeamKey = value; } }

    void Start()
    {
        showGUI = false;
        useNetworkManager = false;
    }

    public void Setup(CrironaNet aNetworkManager)
    {
        networkManager = aNetworkManager;
        broadcastKey = Mathf.Abs(aNetworkManager.broadcastIdentifier.GetHashCode()); // Make sure broadcastKey matches client
        isOpen = false;
        numPlayers = 0;

        broadcastDataObject = new BroadcastData();
        broadcastDataObject.peerId = networkManager.peerId;
        UpdateBroadcastData();
    }

    public void UpdateBroadcastData()
    {
        broadcastData = broadcastDataObject.ToString();
    }

    public void Reset()
    {
        isOpen = false;
        numPlayers = 0;
        UpdateBroadcastData();
    }

    public void RestartBroadcast()
    {
        if (running)
            StopBroadcast();

        // Delay briefly to let things settle down
        CancelInvoke("RestartBroadcastInternal");
        Invoke("RestartBroadcastInternal", 0.5f);
    }

    private void RestartBroadcastInternal()
    {
        UpdateBroadcastData();

        if (networkManager.verboseLogging)
            Debug.Log("Restarting server with data: " + broadcastData);

        if (Initialize() == false)
            Debug.LogError("Network port is unavailable");

        if (StartAsServer() == false)
        {
            Debug.LogError("Unable to broadcast");

            if (hostId != -1)
            {
                if (isServer)
                    NetworkTransport.StopBroadcastDiscovery();

                NetworkTransport.RemoveHost(hostId);
                hostId = -1;
            }
        }
    }
}
