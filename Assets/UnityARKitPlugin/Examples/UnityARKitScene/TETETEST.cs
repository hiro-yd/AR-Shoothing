using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class TETETEST : MonoBehaviour
{

    [SerializeField]
    GameObject Cube;

    void CreateCube(ARPoint point)
    {
        List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface()
            .HitTest(point, ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent);

        if (hitResults.Count > 0)
        {
            var CubeObject = Instantiate(Cube);

            CubeObject.transform.position = UnityARMatrixOps.GetPosition(hitResults[0].worldTransform);
            CubeObject.transform.rotation = UnityARMatrixOps.GetRotation(hitResults[0].worldTransform);
        }
    }

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            var ScreenPos = Camera.main.ScreenToViewportPoint(Input.GetTouch(0).position);
            ARPoint point = new ARPoint { x = ScreenPos.x, y = ScreenPos.y };

            CreateCube(point);
        }
    }
}
