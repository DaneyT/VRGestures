using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyScript : MonoBehaviour {

    [SerializeField]
    private GameObject m_cVirtualRealityObject;
    float acceleration = 0.0f;
   // private WandController m_cWandcontroller;
    bool m_bTriggerClicked = false;


    // Use this for initialization
    void Start () {
        //m_cWandcontroller.TriggerClicked += M_cWandcontroller_TriggerClicked;
        //m_cWandcontroller.TriggerUnclicked += M_cWandcontroller_TriggerUnclicked;
        SubscribeToVrControls();
    }

    private void SubscribeToVrControls()
    {
        if (m_cVirtualRealityObject != null)
        {
            var vRControllerLeft = m_cVirtualRealityObject.transform.GetChild(0);
            var vRControllerRight = m_cVirtualRealityObject.transform.GetChild(1);

            //Triggers are universal and can be subscribed to by any controller
            if (SteamVR.instance.hmd_ModelNumber != "" || SteamVR.instance.hmd_TrackingSystemName != "")
            {
                vRControllerRight.GetComponent<WandController>().TriggerClicked += M_cWandcontroller_TriggerClicked;
                vRControllerRight.GetComponent<WandController>().TriggerUnclicked += M_cWandcontroller_TriggerUnclicked;

                vRControllerLeft.GetComponent<WandController>().TriggerClicked += M_cWandcontroller_TriggerClicked;
                vRControllerLeft.GetComponent<WandController>().TriggerUnclicked += M_cWandcontroller_TriggerUnclicked;

            }
        }
    }

    private void M_cWandcontroller_TriggerUnclicked(object sender, ClickedEventArgs e)
    {
        m_bTriggerClicked = false;
    }

    private void M_cWandcontroller_TriggerClicked(object sender, ClickedEventArgs e)
    {
        Debug.Log("Triggggggggerd");
        m_bTriggerClicked = true;
    }

    // Update is called once per frame
    void Update ()
    {
        if(m_bTriggerClicked)
        MoveCamera();
	}

    private void MoveCamera()
    {
        if (acceleration < 5.0f)
        {
            acceleration += 0.1f;
        }

        //if (acceleration > 0.0f)
        //{
        //    acceleration -= 0.1f;
        //}

        //Compensate for floating point imprecision.
        //If the player is not supposed to be moving, explicitly tell him so.
        if (acceleration > -0.05f && acceleration < 0.05f)
        {
            acceleration = 0.0f;
        }
        Vector3 originalPosition = m_cVirtualRealityObject.transform.GetChild(2).transform.position;
        Vector3 newPostion;
        Transform transformCamera = m_cVirtualRealityObject.transform.GetChild(2).transform;
        transformCamera.Translate(transform.forward * acceleration * Time.deltaTime, Space.World);

        newPostion = transformCamera.position;
        transformCamera.position = originalPosition;
       
        m_cVirtualRealityObject.transform.position = newPostion;

        Debug.Log("BK" + m_cVirtualRealityObject.name + m_cVirtualRealityObject.transform.GetChild(2).transform);
        Debug.Log("BK");

        //if(m_cVirtualRealityObject.transform.GetChild(0).GetComponent<Collider>().bounds.Intersects())
    }
}
