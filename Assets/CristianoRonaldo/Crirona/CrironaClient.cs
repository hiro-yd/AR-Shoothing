using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CrironaClient : NetworkDiscovery
{
    public CrironaNet networkManager;
    public Dictionary<string, DiscoveredServer> discoveredServers;
    public const float ServerKeepAliveTime = 5.0f;
    public bool autoJoin;

    public Queue<string> receivedBroadcastLog;

    private const int maxLogLines = 4;
    private const string broadcastLogTokens = "-.";
    private int broadcastLogCounter = 0;

    void Start()
    {
        discoveredServers = new Dictionary<string, DiscoveredServer>();
        receivedBroadcastLog = new Queue<string>();
        showGUI = false;

        InvokeRepeating("CleanServerList", 3, 1);
    }

    public void Setup(CrironaNet aNetworkManager)
    {
        networkManager = aNetworkManager;
        broadcastKey = Mathf.Abs(aNetworkManager.broadcastIdentifier.GetHashCode()); // Make sure broadcastKey matches server
    }

    public void Reset()
    {
        discoveredServers.Clear();
        receivedBroadcastLog.Clear();
        autoJoin = false;
    }

    public void StartJoining()
    {
        Reset();
        if (Initialize() == false)
        {
            Debug.LogError("#CaptainsMess# Network port is unavailable");
        }
        if (StartAsClient() == false)
        {
            Debug.LogError("#CaptainsMess# Unable to listen");

            // Clean up some data that Unity seems not to
            if (hostId != -1)
            {
                NetworkTransport.RemoveHost(hostId);
                hostId = -1;
            }
        }
        autoJoin = true;
    }

    public void CleanServerList()
    {
        var toRemove = new List<string>();
        foreach (var kvp in discoveredServers)
        {
            float timeSinceLastBroadcast = Time.time - kvp.Value.timestamp;
            if (timeSinceLastBroadcast > ServerKeepAliveTime)
            {
                toRemove.Add(kvp.Key);
            }
        }

        foreach (string server in toRemove)
        {
            discoveredServers.Remove(server);
        }
    }

    public override void OnReceivedBroadcast(string aFromAddress, string aRawData)
    {
        BroadcastData data = new BroadcastData();
        data.FromString(aRawData);

        // DEBUG LOG
        broadcastLogCounter += 1;
        receivedBroadcastLog.Enqueue(broadcastLogTokens[broadcastLogCounter % broadcastLogTokens.Length] + " " + aRawData);
        if (receivedBroadcastLog.Count > maxLogLines)
        {
            receivedBroadcastLog.Dequeue();
        }

        var server = new DiscoveredServer(data);
        server.rawData = aRawData;
        server.ipAddress = aFromAddress;
        server.timestamp = Time.time;

        bool newData = false;
        if (discoveredServers.ContainsKey(aFromAddress) == false)
        {
            // New server
            discoveredServers.Add(aFromAddress, server);
            newData = true;
        }
        else
        {
            if (discoveredServers[aFromAddress].rawData != aRawData)
            {
                // Old server with new info
                discoveredServers[aFromAddress] = server;
                newData = true;
            }
            else
            {
                // Just update the timestamp
                discoveredServers[aFromAddress].timestamp = Time.time;
                newData = false;
            }
        }

        networkManager.OnReceivedBroadcast(aFromAddress, aRawData);

        if (newData)
        {
            networkManager.OnDiscoveredServer(server);
        }
    }
}
