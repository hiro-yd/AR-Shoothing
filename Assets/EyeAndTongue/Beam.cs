using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{

    [SerializeField] float Speed;
    float time;

    void Update()
    {
        transform.position += transform.forward * -Speed;
        time += Time.deltaTime;
        if (time >= 5)
            Destroy(gameObject);
    }
}
