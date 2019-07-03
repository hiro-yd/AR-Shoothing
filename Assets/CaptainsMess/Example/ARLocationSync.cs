using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.iOS;

public class ARLocationSync : MonoBehaviour 
{
	//public GameObject statusGO;
	//public Text statusText;

	ARTrackingState _arTrackingState;
	ARTrackingStateReason _arTrackingStateReason;

	void OnEnable () {
		//statusGO.SetActive (false);
		UnityARSessionNativeInterface.ARSessionTrackingChangedEvent += TrackingChanged;
	}

	void OnDisable()
	{
		UnityARSessionNativeInterface.ARSessionTrackingChangedEvent -= TrackingChanged;

	}

	void TrackingChanged(UnityARCamera cam)
	{
		_arTrackingState = cam.trackingState;
		_arTrackingStateReason = cam.trackingReason;
	}

	public IEnumerator Relocate(byte[] receivedBytes)
	{
		//start relocation
		ARWorldMap arWorldMap = ARWorldMap.SerializeFromByteArray(receivedBytes);

		//Use the AR Session manager to restart session with received world map to sync up
		ExampleARSessionManager easm = FindObjectOfType<ExampleARSessionManager>();
		easm.StartSession(arWorldMap);

		//check tracking state and update UI
		while (_arTrackingState != ARTrackingState.ARTrackingStateLimited || _arTrackingStateReason != ARTrackingStateReason.ARTrackingStateReasonRelocalizing) 
		{
			yield return null;  //wait until it starts relocalizing
		}

		while (_arTrackingState != ARTrackingState.ARTrackingStateNormal) 
		{
			yield return null;
		}
        
    }
}
