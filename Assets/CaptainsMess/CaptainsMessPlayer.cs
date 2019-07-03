using System;
using UnityEngine;
using UnityEngine.Networking;

public class CaptainsMessPlayer : NetworkBehaviour
{
    public static int VERSION = 1;

    public static bool StaticReady;

    [SyncVar]
    public int version;

    [SyncVar]
    public string deviceName;
    [SyncVar]
    public string deviceId;
    [SyncVar]
    public string peerId;
    [SyncVar]
    public int playerIndex;

    [SyncVar(hook = "OnReadyChanged")]
    public bool ready;

    [SyncVar]
    public byte slot;
    

    protected CaptainsMessNetworkManager networkManager;

    public override void OnStartClient()
    {
        networkManager = NetworkManager.singleton as CaptainsMessNetworkManager;
        if (networkManager)
        {
            networkManager.lobbySlots[slot] = this;
            OnClientEnterLobby();
        }
        else
            Debug.LogError("CaptainsMessPlayer could not find a CaptainsMessNetworkManager.");
    }

    private void Update()
    {
        if (ready)
            StaticReady = true;   

        else if(ready == false)
            StaticReady = false;
    }

    public override void OnStartLocalPlayer()
    {
        networkManager = NetworkManager.singleton as CaptainsMessNetworkManager;
        networkManager.localPlayer = this;

#if UNITY_ANDROID
			deviceName = SystemInfo.deviceModel;
#else
        deviceName = SystemInfo.deviceName;
#endif

        version = VERSION;
        deviceId = networkManager.deviceId;
        peerId = networkManager.peerId;
        playerIndex = slot;
        ready = false;
        
        CmdSetBasePlayerInfo(version, deviceName, deviceId, peerId, playerIndex);
    }

    [Command]
    public virtual void CmdSetBasePlayerInfo(int aVersion, string aDeviceName, string aDeviceId, string aPeerId, int aPlayerIndex)
    {
        version = aVersion;
        deviceName = aDeviceName;
        deviceId = aDeviceId;
        peerId = aPeerId;
        playerIndex = aPlayerIndex;
    }

    [Command]
    public void CmdSetReady(bool r)
    {
        ready = r;
    }

    public bool IsReady()
    {
        return ready;
    }

    void OnReadyChanged(bool newValue)
    {
        ready = newValue;
        OnClientReady(ready);

        if (ready)
            networkManager.CheckReadyToBegin();
    }

    public void SendReadyToBeginMessage()
    {
        CmdSetReady(true);
    }

    public void SendNotReadyToBeginMessage()
    {
        CmdSetReady(false);
    }

    public virtual void OnClientEnterLobby()
    {
    }

    public virtual void OnClientExitLobby()
    {
    }

    public virtual void OnClientReady(bool readyState)
    {
    }
    public override void OnDeserialize(NetworkReader reader, bool initialState)
    {
        base.OnDeserialize(reader, initialState);
    }
}