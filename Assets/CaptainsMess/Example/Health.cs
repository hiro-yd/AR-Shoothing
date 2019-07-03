using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Health : NetworkBehaviour
{
    [SyncVar]
    public string PlayerName;

    [SerializeField]
    RectTransform healthBar;

    public GameObject cnt;

    public static int a = 0;

    ExamplePlayerScript player;

    private void Start()
    {
        player = GetComponent<ExamplePlayerScript>();
    }

    private void Update()
    {
        if (isServer)
        {
            if (player.slot == 0)
                PlayerName = "Player1";
            else if (player.slot == 1)
                PlayerName = "Player2";
            else if (player.slot == 2)
                PlayerName = "Player3";
            else if (player.slot == 3)
                PlayerName = "Player4";
        }

        name = PlayerName;

        if (cnt == null)
        {
            if (isServer)
            {
                if (player.slot == 0)
                    cnt = GameObject.Find("Cnt1");
                else if (player.slot == 1)
                    cnt = GameObject.Find("Cnt2");
            }
            else if (!isServer)
            {
                if (player.version == 0)
                    cnt = GameObject.Find("Cnt1");
                else if (player.version == 1)
                    cnt = GameObject.Find("Cnt2");
            }
            return;
        }
        healthBar.sizeDelta = new Vector2(cnt.GetComponent<Cnt>().a, healthBar.sizeDelta.y);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "qwertyuiop")
        {
            cnt.GetComponent<Cnt>().CmdTakeDamage(10);
            NetworkServer.Destroy(collision.gameObject);
        }
    }
}