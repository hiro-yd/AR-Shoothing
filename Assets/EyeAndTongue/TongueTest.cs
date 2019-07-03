using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.iOS;

public class TongueTest : MonoBehaviour
{

    Dictionary<string, float> test;
    bool a = false;
    bool tongue = false;

    [SerializeField]
    Text text;
    [SerializeField]
    float Distance;

    [SerializeField]
    GameObject Cube;

    void Start()
    {
        UnityARSessionNativeInterface session = UnityARSessionNativeInterface.GetARSessionNativeInterface();

        Application.targetFrameRate = 60;
        ARKitFaceTrackingConfiguration config = new ARKitFaceTrackingConfiguration();
        config.alignment = UnityARAlignment.UnityARAlignmentGravity;
        config.enableLightEstimation = true;

        if (!config.IsSupported)
            return;

        session.RunWithConfig(config);


        UnityARSessionNativeInterface.ARFaceAnchorAddedEvent += FaceAdde;
        UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent += FaceUpdate;
        UnityARSessionNativeInterface.ARFaceAnchorRemovedEvent += FaceRemove;
    }

    private void Update()
    {
        if (a)
            //testにTongueOutが含まれているかどうか
            if (test.ContainsKey(ARBlendShapeLocation.TongueOut))
            {
                //Tongueout(舌)が0.5f以上出てたらtrue
                tongue = (test[ARBlendShapeLocation.TongueOut] > 0.5f);

                text.text = test[ARBlendShapeLocation.TongueOut].ToString();
            }

        if (tongue)
            StartCoroutine("MakeSphere");
    }

    void FaceAdde(ARFaceAnchor anchorDate)
    {
        a = true;
        test = anchorDate.blendShapes;
    }

    void FaceUpdate(ARFaceAnchor anchorDate)
    {
        gameObject.transform.localPosition = UnityARMatrixOps.GetPosition(anchorDate.transform);
        gameObject.transform.localRotation = UnityARMatrixOps.GetRotation(anchorDate.transform);
        test = anchorDate.blendShapes;
    }

    void FaceRemove(ARFaceAnchor anchorDate)
    {
        a = false;
    }
    IEnumerator MakeSphere()
    {
        Instantiate(Cube, new Vector3(transform.position.x, transform.position.y + Distance, transform.position.z), Quaternion.identity);
        yield return null;
    }
}
