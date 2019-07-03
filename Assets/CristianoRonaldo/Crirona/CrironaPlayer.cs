using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CrironaMessage : MessageBase
{
    public byte[] byteBuffer;
}

public class CrironaPlayer : CrironaMessPlayer
{
    [SyncVar]
    public bool locationSynced;

    private bool locationSent;

    private ARLocationSync _arLocationSync;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        _arLocationSync = GetComponent<ARLocationSync>();

        // カスタムプレーヤーの情報を送信する
        // 必要な情報を追加 (例 色, プレイたーの画像, 個人設定, etc...)
        locationSent = false;
    }

    public IEnumerator RelocateDevice(byte[] receivedBytes)
    {
        yield return null;
        //actually sync up using arrelocator
        yield return _arLocationSync.Relocate(receivedBytes);
        CmdSetLocationSynced();
        yield return null;
    }

    [Command]
    public void CmdSetLocationSynced()
    {
        locationSynced = true;
    }

    void OnGUI()
    {
        if (isLocalPlayer)
        {
            GUILayout.BeginArea(new Rect(0, Screen.height * 0.7f, Screen.width, 100));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            CrironaSession gameSession = CrironaSession.instance;
            if (gameSession)
            {
                if (gameSession.gameState == CrironaState.Lobby)// || gameSession.gameState == CrironaState.Countdown)
                {
                    //if (GUILayout.Button(IsReady() ? "Not ready" : "Ready", GUILayout.Width(Screen.width * 0.3f), GUILayout.Height(100)))
                    //if (IsReady())
                    //SendNotReadyToBeginMessage();
                    //else
                    SendReadyToBeginMessage();
                }

                else if (gameSession.gameState == CrironaState.WaitForLocationSync)
                {

                    if (isServer && locationSent == false)
                    {
                        gameSession.CmdSendWorldMap();
                        locationSent = true;
                    }

                }
                else if (gameSession.gameState == CrironaState.Game)
                {

                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
}
