using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{

    bool a = false;

    void Start()
    {

    }
    void Update()
    {
        if (Input.anyKeyDown)
            a = !a;

        //if (a)
            //transform.LookAt(new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z));
    }
}
