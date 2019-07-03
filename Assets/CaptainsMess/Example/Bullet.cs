using UnityEngine;
using UnityEngine.Networking;

public class Bullet : MonoBehaviour{

    [SerializeField] float Speed;
    float time;

    void Update()
    {
        transform.position += transform.forward * Speed;
        time += Time.deltaTime;
        if (time >= 5)
            Destroy(gameObject);
    }
}
