﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CrironaExampleListener : CrironaListener
{
    public enum NetworkState
    {
        Init,
        Offline,
        Connecting,
        Connected,
        Disrupted
    };
    [HideInInspector]
    public NetworkState networkState = NetworkState.Init;

    public GameObject CrironaSessionPrefab;
    public CrironaSession gameSession;

    public void Start()
    {
        networkState = NetworkState.Offline;

        ClientScene.RegisterPrefab(CrironaSessionPrefab);
    }

    public override void OnStartConnecting()
    {
        networkState = NetworkState.Connecting;
    }

    public override void OnStopConnecting()
    {
        networkState = NetworkState.Offline;
    }

    public override void OnServerCreated()
    {
        // Create game session
        CrironaSession oldSession = FindObjectOfType<CrironaSession>();
        if (oldSession == null)
        {
            GameObject serverSession = Instantiate(CrironaSessionPrefab);
            NetworkServer.Spawn(serverSession);
        }
        else
        {
            Debug.LogError("GameSession already exists");
        }
    }

    public override void OnJoinedLobby()
    {
        networkState = NetworkState.Connected;

        gameSession = FindObjectOfType<CrironaSession>();
        if (gameSession)
            gameSession.OnJoinedLobby();
    }

    public override void OnLeftLobby()
    {
        networkState = NetworkState.Offline;

        gameSession.OnLeftLobby();
    }

    public override void OnCountdownStarted()
    {
        gameSession.OnCountdownStarted();
    }

    public override void OnGame()
    {
        gameSession.OnGame();
    }

    public override void OnCountdownCancelled()
    {
        gameSession.OnCountdownCancelled();
    }

    public override void OnStartGame(List<CrironaMessPlayer> aStartingPlayers)
    {
        Debug.Log("GO");
        //gameSession.OnStartGame(aStartingPlayers);
    }
}
