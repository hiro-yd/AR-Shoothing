using UnityEngine;
using UnityEngine.XR.iOS;

public class EyeTest : MonoBehaviour
{

    [SerializeField]
    GameObject EyePrefab;

    GameObject RightEye;
    GameObject LeftEye;

    void Start()
    {
        UnityARSessionNativeInterface.ARFaceAnchorAddedEvent += FaceAdd;
        UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent += FaceUpdat;
        UnityARSessionNativeInterface.ARFaceAnchorRemovedEvent += FaceRemove;

        LeftEye = Instantiate(EyePrefab);
        RightEye = Instantiate(EyePrefab);

        LeftEye.SetActive(false);
        RightEye.SetActive(false);
    }

    void FaceAdd(ARFaceAnchor AnchorDate)
    {
        //LeftEye.transform.position = AnchorDate.leftEyePose.position;
        //LeftEye.transform.rotation = AnchorDate.leftEyePose.rotation;

        //RightEye.transform.position = AnchorDate.rightEyePose.position;
        //RightEye.transform.rotation = AnchorDate.rightEyePose.rotation;

        LeftEye.SetActive(true);
        RightEye.SetActive(true);
    }

    void FaceUpdat(ARFaceAnchor AnchorDate)
    {
        LeftEye.transform.position = AnchorDate.leftEyePose.position;
        LeftEye.transform.rotation = AnchorDate.leftEyePose.rotation;

        RightEye.transform.position = AnchorDate.rightEyePose.position;
        RightEye.transform.rotation = AnchorDate.rightEyePose.rotation;
    }

    void FaceRemove(ARFaceAnchor AnchorData)
    {
        LeftEye.SetActive(false);
        RightEye.SetActive(false);
    }
}
