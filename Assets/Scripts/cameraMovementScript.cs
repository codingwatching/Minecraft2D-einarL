using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMovementScript : MonoBehaviour
{

    public CinemachineVirtualCamera vCam;
    private int minOthoSize = 2;
    private int maxOthoSize = 20;

    private bool isZoomTipActive = false;

    private IDataService dataService = JsonDataService.Instance;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!dataService.exists("inventory.json")) isZoomTipActive = true;
	}

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.X)) // if holding X
        {
            if (isZoomTipActive)
            {
                if (Input.mouseScrollDelta.y != 0)
                {
                    GameObject.Find("Canvas").GetComponent<TipScript>()?.removeZoomTip();
                    isZoomTipActive = false;
                }
            }
            vCam.m_Lens.OrthographicSize -= Input.mouseScrollDelta.y; // scroll mouse wheel to change the size of the camera
            vCam.m_Lens.OrthographicSize = Math.Max(minOthoSize, vCam.m_Lens.OrthographicSize);
		    vCam.m_Lens.OrthographicSize = Math.Min(maxOthoSize, vCam.m_Lens.OrthographicSize);
		}
	}
}
