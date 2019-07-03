using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ExampleMessage : MessageBase
{
    public byte[] byteBuffer;
}

public class ExamplePlayerScript : CaptainsMessPlayer
{

    [SyncVar]
    public Color myColour;

    [SyncVar]
    public bool locationSynced;

    public GameObject spherePrefab;
    public GameObject Cnt;

    private bool locationSent;

    private ARLocationSync _arLocationSync;
    private ExampleARSessionManager _exampleARSessionManager;

    public static bool a = false;

    [SyncVar]
    public int abc = 0;
    public static int def;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        CmdMakeCnt(transform.position, transform.rotation);
        
        _arLocationSync = GetComponent<ARLocationSync>();
        _exampleARSessionManager = FindObjectOfType<ExampleARSessionManager>();


        // カスタムプレーヤーの情報を送信する
        // 必要な情報を追加 (例 色, プレイたーの画像, 個人設定, etc...)

        myColour = UnityEngine.Random.ColorHSV(0, 1, 1, 1, 1, 1);
        CmdSetCustomPlayerInfo(myColour);
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

    [Command]
    public void CmdSetCustomPlayerInfo(Color aColour)
    {
        myColour = aColour;
    }

    [Command]
    public void CmdMakeCnt(Vector3 position, Quaternion rotation)
    {
        abc = def;
        var cnt = Instantiate(Cnt, position, rotation);
        abc += 1;
        cnt.name = "Cnt" + abc;
        NetworkServer.Spawn(cnt);
        def = abc;
    }

    [Command]
    public void CmdMakeSphere(Vector3 position, Quaternion rotation)
    {
        var sphere = Instantiate(spherePrefab, position, rotation);
        NetworkServer.Spawn(sphere);
        RpcSetSphereColor(sphere, myColour.r, myColour.g, myColour.b);
    }

    [Command]
    public void CmdPlayAgain()
    {
        ExampleGameSession.instance.PlayAgain();
    }

    [ClientRpc]
    public void RpcSetSphereColor(GameObject sphere, float r, float g, float b)
    {
        sphere.GetComponent<Renderer>().material.color = new Color(r, g, b);
    }

    private void Update()
    {
        if (a == false)
            RpcSetSphereColor(gameObject, myColour.r, myColour.g, myColour.b);
    }

    void OnGUI()
    {
        if (isLocalPlayer)
        {
            GUILayout.BeginArea(new Rect(0, Screen.height * 0.7f, Screen.width, 100));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            ExampleGameSession gameSession = ExampleGameSession.instance;
            if (gameSession)
            {
                if (gameSession.gameState == GameState.Lobby || gameSession.gameState == GameState.Countdown)
                {
                    if (GUILayout.Button(IsReady() ? "Not ready" : "Ready", GUILayout.Width(Screen.width * 0.3f), GUILayout.Height(100)))
                        if (IsReady())
                            SendNotReadyToBeginMessage();
                        else
                        {
                            SendReadyToBeginMessage();
                            a = true;
                        }
                }

                else if (gameSession.gameState == GameState.WaitForLocationSync)
                {

                    if (isServer && locationSent == false)
                    {
                        gameSession.CmdSendWorldMap();
                        locationSent = true;
                    }

                }
                else if (gameSession.gameState == GameState.Game)
                {
                    a = true;
                    if (GUILayout.Button("Make Sphere", GUILayout.Width(Screen.width * 0.4f), GUILayout.Height(300)))
                    {
                        Transform camTransform = _exampleARSessionManager.CameraTransform();
                        Vector3 spherePosition = camTransform.position + (camTransform.forward.normalized * 0.2f); //place sphere 2cm in front of device
                        CmdMakeSphere(spherePosition, camTransform.rotation);
                    }
                }
                else if (gameSession.gameState == GameState.GameOver)
                {
                    if (isServer)
                        if (GUILayout.Button("Play Again", GUILayout.Width(Screen.width * 0.3f), GUILayout.Height(100)))
                            CmdPlayAgain();
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
}
