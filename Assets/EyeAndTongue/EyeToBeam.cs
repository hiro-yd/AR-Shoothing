using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class EyeToBeam : MonoBehaviour
{

    Dictionary<string, float> TongueID;

    [SerializeField] GameObject BasePosition;
    [SerializeField] GameObject Beam;
    GameObject RightEye;
    GameObject LeftEye;

    bool isScreenIn = false;
    bool Tongue = false;

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

        UnityARSessionNativeInterface.ARFaceAnchorAddedEvent += FaceAdd;
        UnityARSessionNativeInterface.ARFaceAnchorUpdatedEvent += FaceUpdate;
        UnityARSessionNativeInterface.ARFaceAnchorRemovedEvent += FaceRemove;

        RightEye = Instantiate(BasePosition);
        LeftEye = Instantiate(BasePosition);

        LeftEye.SetActive(false);
        RightEye.SetActive(false);
    }

    void Update()
    {
        if (isScreenIn)
            if (TongueID.ContainsKey(ARBlendShapeLocation.TongueOut))
                Tongue = TongueID[ARBlendShapeLocation.TongueOut] > 0.5f;

        if (Tongue)
            BeamShot();
    }

    void BeamShot()
    {
        Instantiate(Beam, LeftEye.transform.position, gameObject.transform.rotation);
        Instantiate(Beam, RightEye.transform.position, gameObject.transform.rotation);
    }

    void FaceAdd(ARFaceAnchor AnchorDate)
    {
        isScreenIn = true;
        TongueID = AnchorDate.blendShapes;

        LeftEye.SetActive(true);
        RightEye.SetActive(true);
    }

    void FaceUpdate(ARFaceAnchor AnchorDate)
    {
        gameObject.transform.localPosition = UnityARMatrixOps.GetPosition(AnchorDate.transform);
        gameObject.transform.localRotation = UnityARMatrixOps.GetRotation(AnchorDate.transform);

        TongueID = AnchorDate.blendShapes;

        LeftEye.transform.position = AnchorDate.leftEyePose.position;
        LeftEye.transform.rotation = AnchorDate.leftEyePose.rotation;

        RightEye.transform.position = AnchorDate.rightEyePose.position;
        RightEye.transform.rotation = AnchorDate.rightEyePose.rotation;
    }

    void FaceRemove(ARFaceAnchor AnchorData)
    {
        isScreenIn = false;

        LeftEye.SetActive(false);
        RightEye.SetActive(false);
    }
}
