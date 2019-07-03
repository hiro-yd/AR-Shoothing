using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.XR.iOS;

public enum GameState
{
    Offline,
    Connecting,
    Lobby,
    Countdown,
    WaitForLocationSync,
    Game,
    GameOver
}

public class ExampleGameSession : NetworkBehaviour
{
    public Text gameStateField;

    public static ExampleGameSession instance;

    ExampleListener networkListener;
    List<ExamplePlayerScript> players;
    private NetworkTransmitter _networkTransmitter;
    private ExampleARSessionManager _arSessionManager;

    [SyncVar]
    public GameState gameState;

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
        networkListener = FindObjectOfType<ExampleListener>();
        _arSessionManager = FindObjectOfType<ExampleARSessionManager>();
        gameState = GameState.Connecting;
    }

    [Server]
    public void OnStartGame(List<CaptainsMessPlayer> aStartingPlayers)
    {
        players = aStartingPlayers.Select(p => p as ExamplePlayerScript).ToList();

        RpcOnStartedGame();

        foreach (ExamplePlayerScript p in players)
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

        networkListener = FindObjectOfType<ExampleListener>();
        networkListener.gameSession = this;

        _networkTransmitter = GetComponent<NetworkTransmitter>();

        if (gameState != GameState.Lobby)
            gameState = GameState.Lobby;
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
        CaptainsMessNetworkManager networkManager = NetworkManager.singleton as CaptainsMessNetworkManager;
        ExamplePlayerScript p = networkManager.localPlayer as ExamplePlayerScript;

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
        gameState = GameState.Lobby;
    }

    public void OnLeftLobby()
    {
        gameState = GameState.Offline;
    }

    public void OnCountdownStarted()
    {
        gameState = GameState.Countdown;
    }

    public void OnCountdownCancelled()
    {
        gameState = GameState.Lobby;
    }
    public void OnGame()
    {
        gameState = GameState.Game;
    }

    [Server]
    IEnumerator RunGame()
    {
        gameState = GameState.WaitForLocationSync;

        while (AllPlayersHaveSyncedLocation() == false)
        {
            yield return null;
        }

        while (true)
        {
            gameState = GameState.Game;
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
            if (gameState == GameState.Lobby)
                message = "Lobby";
            else if (gameState == GameState.Countdown)
                message = "Game Starting in " + Mathf.Ceil(networkListener.mess.CountdownTimer()) + "...";
            else if (gameState == GameState.WaitForLocationSync)
                message = "Location Sync...";
            else if (gameState == GameState.Game)
                message = null;

        gameStateField.text = message;
    }

    [ClientRpc]
    public void RpcOnStartedGame()
    {
        _networkTransmitter = GetComponent<NetworkTransmitter>();
        _networkTransmitter.OnDataCompletelyReceived += OnDataComepletelyReceived;
        _networkTransmitter.OnDataFragmentReceived += OnDataFragmentReceived;
    }
}
