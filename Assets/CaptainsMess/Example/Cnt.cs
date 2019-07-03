using UnityEngine;
using UnityEngine.Networking;

public class Cnt : NetworkBehaviour{

    [SyncVar]
    public int a = 100;

    [SyncVar]
    public string Name;

    [Command]
    public void CmdTakeDamage(int damage)
    {
        //   if (isLocalPlayer)
        {
            a -= damage;

            if (a <= 0)
            {
                a = 0;
                Debug.Log("Dead!");
            }
        }
    }

    void Update()
    {
        if (isServer)
            Name = gameObject.name;
        if (gameObject.name == "Cnt(Clone)" || gameObject.name == "" || gameObject.name == null)
            gameObject.name = Name;
    }
}