using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.XR.iOS;

public enum CrironaState
{
    Offline,
    Connecting,
    Lobby,
    Countdown,
    WaitForLocationSync,
    Game,
    GameOver
}

public class CrironaSession : NetworkBehaviour
{
    public Text gameStateField;

    public static CrironaSession instance;

    CrironaExampleListener networkListener;
    List<CrironaPlayer> players;
    private CrironaNetworkTransmitter _networkTransmitter;
    private ExampleARSessionManager _arSessionManager;

    [SyncVar]
    public CrironaState gameState;

    [SyncVar]
    private string message;

    public void OnDestroy()
    {
        if (gameStateField != null)
        {
            gameStateField.text = "";
            gameStateField.gameObject.SetActive(false);
        }
    }

    [Server]
    public override void OnStartServer()
    {
        networkListener = FindObjectOfType<CrironaExampleListener>();
        _arSessionManager = FindObjectOfType<ExampleARSessionManager>();
        gameState = CrironaState.Connecting;
    }

    [Server]
    public void OnStartGame(List<CrironaPlayer> aStartingPlayers)
    {
        players = aStartingPlayers.Select(p => p as CrironaPlayer).ToList();

        RpcOnStartedGame();

        foreach (CrironaPlayer p in players)
        {
            p.locationSynced = false;
        }

        StartCoroutine(RunGame());
    }

    [Client]
    public override void OnStartClient()
    {
        if (instance)
            Debug.LogError("ERROR: Another GameSession");

        instance = this;

        networkListener = FindObjectOfType<CrironaExampleListener>();
        networkListener.gameSession = this;

        _networkTransmitter = GetComponent<CrironaNetworkTransmitter>();

        if (gameState != CrironaState.Lobby)
            gameState = CrironaState.Lobby;
    }

    [Command]
    public void CmdSendWorldMap()
    {
        ARWorldMap arWorldMap = _arSessionManager.GetSavedWorldMap();
        StartCoroutine(_networkTransmitter.SendBytesToClientsRoutine(0, arWorldMap.SerializeToByteArray()));
    }

    [Client]
    private void OnDataComepletelyReceived(int transmissionId, byte[] data)
    {
        CrironaNet networkManager = NetworkManager.singleton as CrironaNet;
        CrironaPlayer p = networkManager.localPlayer as CrironaPlayer;

        if (p != null)
            //deserialize data and relocalize
            StartCoroutine(p.RelocateDevice(data));
    }

    //on clients this will be called every time a chunk (fragment of complete data) has been received
    [Client]
    private void OnDataFragmentReceived(int transmissionId, byte[] data)
    {
        //update a progress bar or do something else with the information
    }


    public void OnJoinedLobby()
    {
        gameState = CrironaState.Lobby;
    }

    public void OnLeftLobby()
    {
        gameState = CrironaState.Offline;
    }

    public void OnCountdownStarted()
    {
        gameState = CrironaState.Countdown;
    }

    public void OnCountdownCancelled()
    {
        gameState = CrironaState.Lobby;
    }
    public void OnGame()
    {
        gameState = CrironaState.Game;
    }

    [Server]
    IEnumerator RunGame()
    {
        gameState = CrironaState.WaitForLocationSync;

        while (AllPlayersHaveSyncedLocation() == false)
        {
            yield return null;
        }

        while (true)
        {
            gameState = CrironaState.Game;
            yield return null;
        }
    }

    [Server]
    bool AllPlayersHaveSyncedLocation()
    {
        return players.All(p => p.locationSynced);
    }

    [Server]
    public void PlayAgain()
    {
        StartCoroutine(RunGame());
    }

    void Update()
    {
        if (isServer)
            if (gameState == CrironaState.Lobby)
                message = "Lobby";
            else if (gameState == CrironaState.Countdown)
                message = "Game Starting in " + Mathf.Ceil(networkListener.mess.CountdownTimer()) + "...";
            else if (gameState == CrironaState.WaitForLocationSync)
                message = "Location Sync...";
            else if (gameState == CrironaState.Game)
                message = null;

        gameStateField.text = message;
    }

    [ClientRpc]
    public void RpcOnStartedGame()
    {
        _networkTransmitter = GetComponent<CrironaNetworkTransmitter>();
        _networkTransmitter.OnDataCompletelyReceived += OnDataComepletelyReceived;
        _networkTransmitter.OnDataFragmentReceived += OnDataFragmentReceived;
    }
}
