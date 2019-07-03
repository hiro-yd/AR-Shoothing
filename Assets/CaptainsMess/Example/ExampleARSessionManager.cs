using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExampleARSessionManager : MonoBehaviour
{

    public Camera m_camera;
    private UnityARSessionNativeInterface m_session;
    private ARMapArea arMapArea;

    [Header("AR Config Options")]
    public UnityARAlignment startAlignment = UnityARAlignment.UnityARAlignmentGravity;
    public UnityARPlaneDetection planeDetection = UnityARPlaneDetection.Horizontal;
    public ARReferenceImagesSet detectionImages = null;
    public bool getPointCloud = true;
    public bool enableLightEstimation = true;
    public bool enableAutoFocus = true;
    private bool sessionStarted = false;

    [SerializeField]
    private GameObject choiceLayout;

    void Start()
    {
        arMapArea = GetComponent<ARMapArea>();
        choiceLayout.SetActive(true);
        DontDestroyOnLoad(gameObject);
        StartSession();

        if (m_camera == null)
            m_camera = Camera.main;

        DontDestroyOnLoad(m_camera.gameObject);
        MapArea();
    }

    public void StartSession(ARWorldMap arWorldMap = null)
    {
        m_session = UnityARSessionNativeInterface.GetARSessionNativeInterface();

        Application.targetFrameRate = 60;
        ARKitWorldTrackingSessionConfiguration config = new ARKitWorldTrackingSessionConfiguration
        {
            planeDetection = planeDetection,
            alignment = startAlignment,
            getPointCloudData = getPointCloud,
            enableLightEstimation = enableLightEstimation,
            enableAutoFocus = enableAutoFocus,
            worldMap = arWorldMap
        };

        if (detectionImages != null)
            config.referenceImagesGroupName = detectionImages.resourceGroupName;

        if (config.IsSupported)
        {
            m_session.RunWithConfig(config);
            UnityARSessionNativeInterface.ARFrameUpdatedEvent += FirstFrameUpdate;
        }


    }

    void FirstFrameUpdate(UnityARCamera cam)
    {
        sessionStarted = true;
        UnityARSessionNativeInterface.ARFrameUpdatedEvent -= FirstFrameUpdate;
    }


    public void GoToNextScene()
    {
        choiceLayout.SetActive(false);

        SceneManager.LoadScene(1);
    }

    public void MapArea()
    {

        choiceLayout.SetActive(false);

        StartCoroutine(MappingCoroutine());
    }

    IEnumerator MappingCoroutine()
    {
        yield return arMapArea.MapArea();

        while (arMapArea.worldMapSaved == false)
        {
            yield return null;
        }

        GoToNextScene();
    }

    public ARWorldMap GetSavedWorldMap()
    {
        if (arMapArea.worldMapSaved)
            return arMapArea.mappedWorld;
        else
            return null;
    }

    public Transform CameraTransform()
    {
        return m_camera.transform;
    }

    void Update()
    {
        m_camera.gameObject.SetActive(true);
        if (m_camera != null && sessionStarted)
        {
            // JUST WORKS
            Matrix4x4 matrix = m_session.GetCameraPose();
            m_camera.transform.localPosition = UnityARMatrixOps.GetPosition(matrix);
            m_camera.transform.localRotation = UnityARMatrixOps.GetRotation(matrix);
            m_camera.projectionMatrix = m_session.GetCameraProjection();
        }

    }

}
