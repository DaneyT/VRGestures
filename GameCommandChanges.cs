using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Linq;
using UnityEngine;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Valve.VR;
using UnityEngine.UI;

public class GameCommandChanges : MonoBehaviour
{
    //------Debug stuff------
    private const bool DebugLog = true;
    [SerializeField]
    private Canvas m_cDebugCanvas;
    //-----------------------

    [SerializeField]
    private GameObject m_cVirtualRealityCameraObject;
    [SerializeField]
    private Text m_cHMDText;
    private GameObject m_cFirstCubeOfStreak;
    private GameObject m_cCurrentAreaTouched;
    [SerializeField]
    private GameObject m_cHandProps;
    private GestureContainer m_lGestureContainer;
    private int primitiveCount;
    private int m_iGestureStreakNumber;
    private int m_iCubeFromStreakNumber;
    private int m_iGestureNamesRecordIndex;
    private float m_iTimePassed;
    private float m_iTimeToNextIntervalCheck = 0.05f;
    private bool m_bGripped;
    private bool m_bGestureTookToLong;
    private bool m_bDeserializeOnce;
    private bool m_bStreakStarted;
    private bool m_bFirstCubeOfStreakTouched;
    private bool m_bTriggerPressed;
    private bool m_bRecordMode;
    private bool m_bShowHideDebug;
    private List<GestureObject> m_lGesturePrimitiveStreakCheckList;     //This list will be filled and collision checked for a initiated gesture
    private List<GesturePoints> m_lController3DSpaceGesturePoint;
    private String[] GestureNames = new string[5] { "GrabPhone", "AimPistol", "MoveArmsUp", "MoveArmsDown", "Nothing!. All gestures are recorded!" };


    [SerializeField]
    [XmlArray("GestureObject")]
    [XmlArrayItem("GestureObject")]
    private List<GestureObject> m_lGesturePrimitiveStreak = new List<GestureObject>();

    private Dictionary<string, List<GestureObject>> m_lGestureStreak = new Dictionary<string, List<GestureObject>>();

    // Use this for initialization
    void Start()
    {
        //http://answers.unity3d.com/questions/400284/trying-to-create-a-custom-class-array.html
        m_bGripped = false;
        m_bGestureTookToLong = false;
        m_bTriggerPressed = false;
        m_cHMDText.enabled = false;
        m_bDeserializeOnce = true;
        m_bStreakStarted = false;
        m_bFirstCubeOfStreakTouched = false;
        m_bRecordMode = false;
        m_iCubeFromStreakNumber = 1;
        m_iGestureStreakNumber = 0;
        m_iTimePassed = 0;
        primitiveCount = 0;
        m_iGestureNamesRecordIndex = 0;
        m_lGestureContainer = new GestureContainer();
        m_lGesturePrimitiveStreakCheckList = new List<GestureObject>();
        m_lController3DSpaceGesturePoint = new List<GesturePoints>();
        subscribeToVRInputEvents();
    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            m_bRecordMode = true;
        }
        ////ClearList
        //if (Input.GetKeyDown(KeyCode.Alpha0))
        //{
        //    Debug.Log("Test " + m_lGesturePrimitiveStreak.Count);
        //    m_lGesturePrimitiveStreak = new List<GestureObject>();
        //    //m_lGesturePrimitiveStreak.Clear();
        //    Debug.Log("Checkafter");
        //}

        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            //Dictionary<string, List<GestureObject>> xmlDict = new Dictionary<string, List<GestureObject>>();
            //FileInfo fInfo = new FileInfo(@"D:\Unity Projects\VRGestures\StreamFiles\Gestures.xml");
            //if (fInfo != null && fInfo.Exists)
            //    File.Delete(@"D:\Unity Projects\VRGestures\StreamFiles\Gestures.xml");

            ////Get each dictionary entry
            //foreach (KeyValuePair<string, List<GestureObject>> entry in m_lGestureStreak)
            //{
            //    List<GestureObject> Go = new List<GestureObject>();

            //    Go = entry.Value;

            //    //Duplicates the list with gestures to GestureContainer class so we can write an XML file with the list data
            //    for (int i = 0; i < Go.Count; i++)
            //    {
            //        //Fill in the field of the class with object data needed for gestures
            //        Debug.Log("Processed entry" + i + " | of " + entry.Key);
            //        Go[i].m_vPosition = Go[i].m_cGestureObjectPrimitive.gameObject.transform.position;
            //        Go[i].m_vRotation = Go[i].m_cGestureObjectPrimitive.gameObject.transform.rotation.eulerAngles;
            //    }
            //    //Fill class with values
            //    m_lGestureContainer.Gestures = Go;
            //    //Write away the class and on to the next
            //    xmlDict.Add(entry.Key, m_lGestureContainer.Gestures);
            //}
            //m_lGestureContainer.SerializeObject(xmlDict, @"D:\Unity Projects\VRGestures\StreamFiles\Gestures.xml");
        }

        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            //var deserializedObject = m_lGestureContainer.DeSerializeObject(@"D:\Unity Projects\VRGestures\StreamFiles\Gestures.xml");
            ////Overwrite the List of gestureobjects with the new deserialized xml data
            //m_lGestureStreak = deserializedObject;
            //Debug.Log("Deserialized!");
            //m_bRecordMode = false;
        }

        //Debug function: Gesture starter. It will create gameobjects in the deserialized dictionary, give name and 
        //disable the collision and renderer except the first one
        if (Input.GetKeyDown(KeyCode.Alpha3) && m_bDeserializeOnce)
        {
            //foreach (KeyValuePair<string, List<GestureObject>> pair in m_lGestureStreak)
            //{
            //    GameObject go = new GameObject(pair.Key + "Streak");
            //    go.transform.SetParent(m_cVirtualRealityCameraObject.transform.GetChild(2));

            //    for (int i = 0; i < pair.Value.Count; i++)
            //    {
            //        GameObject cubeObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //        Quaternion cubeRotation = Quaternion.Euler(pair.Value[i].m_vRotation);

            //        pair.Value[i].m_cGestureObjectPrimitive = cubeObj;
            //        cubeObj.transform.position = pair.Value[i].m_vPosition;
            //        cubeObj.transform.rotation = cubeRotation;
            //        cubeObj.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
            //        cubeObj.gameObject.name = "Cube" + i;
            //        cubeObj.transform.SetParent(go.transform);
            //        if (i != 0)
            //        {
            //            cubeObj.GetComponent<Collider>().enabled = false;
            //            cubeObj.GetComponent<Renderer>().enabled = false;
            //        }
            //    }
            //    pair.Value[0].m_cGestureObjectPrimitive.gameObject.name = "Start";
            //    pair.Value[pair.Value.Count - 1].m_cGestureObjectPrimitive.gameObject.name = "End"; //-1 because the list starts at 0
            //}
            //m_bDeserializeOnce = false;
        }
    }

    private void FixedUpdate()
    {
        GetCurrentArea();
        if (Time.fixedTime >= m_iTimeToNextIntervalCheck)
        {

            m_iTimeToNextIntervalCheck = Time.fixedTime + 0.05f;
            SaveControllerSpacePoint();

        }

        if (m_bGripped && m_bRecordMode)
            CreateGestureStreak();

        if (!m_bStreakStarted && !m_bDeserializeOnce && m_bGripped && !m_bRecordMode) //controller is gripped, dict is filled with gestures, recordmode is on and deserialized -> check gestures
            GestureInitChecker();

        if (m_bFirstCubeOfStreakTouched)
            CheckGesture();
    }

    private void GestureInitChecker()
    {
        var GestureKeys = m_lGestureStreak.Keys.ToList();
        //Check if streak from dictionary will be activated and started
        for (int i = 0; i < m_lGestureStreak.Count; i++)
        {
            //GameObject firstCubeOfList = m_lGestureStreak.FirstOrDefault(x => x.Value[i].m_cGestureObjectPrimitive.gameObject.name == "Start").Value[0].m_cGestureObjectPrimitive.gameObject;
            GameObject firstCubeOfList = m_lGestureStreak[GestureKeys[i]][0].m_cGestureObjectPrimitive.gameObject;

            //Check every first gameobject for every dictionary gesture entry
            if (m_cVirtualRealityCameraObject.transform.GetChild(0).GetChild(0).GetComponent<Collider>().bounds.Intersects(
                firstCubeOfList.GetComponent<Collider>().bounds))
            {
                m_cFirstCubeOfStreak = firstCubeOfList;
                m_lGesturePrimitiveStreakCheckList = m_lGestureStreak[GestureKeys[i]];                  //Set the checklist with gameobjects to be collision checked
                m_bFirstCubeOfStreakTouched = true;                                                     //Set true to activate the CheckGesture() function

                for(int h =0; h < m_lGestureStreak.Count; h++)
                {
                    if (m_lGestureStreak[GestureKeys[h]][0].m_cGestureObjectPrimitive.gameObject != firstCubeOfList)
                    {
                        firstCubeOfList.GetComponent<Collider>().enabled = false;
                        firstCubeOfList.GetComponent<Renderer>().enabled = false;
                    }
                }
            }
        }
    }

    private void CheckGesture()
    {
        //Check if left "Start" cube (1)
        if (!m_cVirtualRealityCameraObject.transform.GetChild(0).GetChild(0).GetComponent<Collider>().bounds.Intersects(
            m_lGesturePrimitiveStreakCheckList[0].m_cGestureObjectPrimitive.gameObject.GetComponent<Collider>().bounds)
            && m_bFirstCubeOfStreakTouched && !m_bGestureTookToLong)
        {
            //Add miliseconds to the timer
            GestureTimer();

            //Streak is started/busy
            m_bStreakStarted = true;

            if (m_lGesturePrimitiveStreakCheckList.Count == 0)
                return;

            //Deactivate cube 1/0 because we left it and activate cube 2
            if (m_lGesturePrimitiveStreakCheckList[0].m_cGestureObjectPrimitive.gameObject.GetComponent<Collider>().enabled)
            {
                m_lGesturePrimitiveStreakCheckList[0].m_cGestureObjectPrimitive.gameObject.GetComponent<Collider>().enabled = false;
                m_lGesturePrimitiveStreakCheckList[0].m_cGestureObjectPrimitive.gameObject.GetComponent<Renderer>().enabled = false;
                //Activate cube 2 and stuff
                m_lGesturePrimitiveStreakCheckList[1].m_cGestureObjectPrimitive.gameObject.GetComponent<Collider>().enabled = true;
                m_lGesturePrimitiveStreakCheckList[1].m_cGestureObjectPrimitive.gameObject.GetComponent<Renderer>().enabled = true;
            }
            //If cube 2 is hit? deactivate 2 and activate 3!, if 3 is hit? deactivate 3 and activate 4! etc. etc.
            if (m_cVirtualRealityCameraObject.transform.GetChild(0).GetChild(0).GetComponent<Collider>().bounds.Intersects(  //m_cVirtualRealityCameraObject.transform.GetChild(0).GetChild(0)
            m_lGesturePrimitiveStreakCheckList[m_iCubeFromStreakNumber].m_cGestureObjectPrimitive.gameObject.GetComponent<Collider>().bounds))
            {
                //disable hit cube
                Debug.Log("Hit" + m_lGesturePrimitiveStreakCheckList[m_iCubeFromStreakNumber].m_cGestureObjectPrimitive.gameObject.name);
                m_lGesturePrimitiveStreakCheckList[m_iCubeFromStreakNumber].m_cGestureObjectPrimitive.gameObject.GetComponent<Collider>().enabled = false;
                m_lGesturePrimitiveStreakCheckList[m_iCubeFromStreakNumber].m_cGestureObjectPrimitive.gameObject.GetComponent<Renderer>().enabled = false;
                //activate next
                if (m_iCubeFromStreakNumber < m_lGesturePrimitiveStreakCheckList.Count - 1)
                {
                    m_lGesturePrimitiveStreakCheckList[m_iCubeFromStreakNumber + 1].m_cGestureObjectPrimitive.gameObject.GetComponent<Collider>().enabled = true;
                    m_lGesturePrimitiveStreakCheckList[m_iCubeFromStreakNumber + 1].m_cGestureObjectPrimitive.gameObject.GetComponent<Renderer>().enabled = true;
                    m_iCubeFromStreakNumber++;
                }
            }

            //Gesture succeeded, can only succeed if the last cube is active
            if (m_cVirtualRealityCameraObject.transform.GetChild(0).GetChild(0).GetComponent<Collider>().bounds.Intersects( //m_cVirtualRealityCameraObject.transform.GetChild(0).GetChild(0)
            m_lGesturePrimitiveStreakCheckList[m_lGesturePrimitiveStreakCheckList.Count - 1].m_cGestureObjectPrimitive.gameObject.GetComponent<Collider>().bounds)) //String comparison?
            {
                string GestureStreakName;

                foreach (KeyValuePair<string, List<GestureObject>> pair in m_lGestureStreak)
                {
                    if (pair.Value[0].m_cGestureObjectPrimitive.transform.position == m_cFirstCubeOfStreak.transform.position)
                    {
                        GestureStreakName = pair.Key;
                        Debug.Log("Succes! Completed: " + GestureStreakName);

                        int gestureNumber = Array.IndexOf(GestureNames, GestureStreakName);

                        m_cHMDText.text = "Succes! Completed: " + GestureStreakName;
                        StartCoroutine(DeactivateHandObject(gestureNumber));
                        m_cHMDText.enabled = true;
                        StartCoroutine(DeactivateText());
                        break;
                    }
                }
                //End cube is hit, succes! -> activate text succes
                StartCoroutine(DeactivateText());
                m_bGestureTookToLong = false;
                m_bStreakStarted = false;
                m_iTimePassed = 0.0f;
                m_bFirstCubeOfStreakTouched = false;
                return;
            }

            //Took to long to complete gesture (seconds)? -> return to next if code block and reset gesture
            if (m_iTimePassed >= m_lGesturePrimitiveStreakCheckList[0].m_iSpeed + 0.75f && m_bStreakStarted && !m_bGestureTookToLong)
            {
                m_bGestureTookToLong = true;
                Debug.LogError("Took to long!");
                m_bFirstCubeOfStreakTouched = false;
                m_bStreakStarted = false;
                m_bGestureTookToLong = false;
                ResetStreak();
                return;
            }
        }
    }
    private void CreateGestureStreak()
    {
        GestureTimer();
        Debug.Log("Pressed!");

        //if (!m_cHMDText.enabled)
        //{
            m_cHMDText.text = "Recording " + GestureNames[m_iGestureNamesRecordIndex] + "...";
            m_cHMDText.enabled = true;
        //}

        m_lGesturePrimitiveStreak.Add(new GestureObject());
        m_lGesturePrimitiveStreak[primitiveCount].m_iAcceleration = 1.00f;
        m_lGesturePrimitiveStreak[primitiveCount].m_cGestureObjectPrimitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
        m_lGesturePrimitiveStreak[primitiveCount].m_cGestureObjectPrimitive.gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        m_lGesturePrimitiveStreak[primitiveCount].m_cGestureObjectPrimitive.gameObject.transform.position = m_cVirtualRealityCameraObject.transform.GetChild(0).GetChild(0).position;
        m_lGesturePrimitiveStreak[primitiveCount].m_cGestureObjectPrimitive.gameObject.name = "Cube" + primitiveCount;
        m_lGesturePrimitiveStreak[primitiveCount].m_cGestureObjectPrimitive.gameObject.transform.SetParent(m_cVirtualRealityCameraObject.transform.GetChild(2).transform);
        primitiveCount++;
    }

    private void ResetStreak()
    {
        //Reset GesturePrimitiveStreak
        Debug.LogError("All Streak resetted!");
        foreach (KeyValuePair<string, List<GestureObject>> gestureDict in m_lGestureStreak)
        {
            gestureDict.Value[0].m_cGestureObjectPrimitive.gameObject.GetComponent<Collider>().enabled = true;
            gestureDict.Value[0].m_cGestureObjectPrimitive.gameObject.GetComponent<Renderer>().enabled = true;
            for (int i = 1; i < m_lGesturePrimitiveStreakCheckList.Count - 1; i++)
            {
                gestureDict.Value[i].m_cGestureObjectPrimitive.gameObject.GetComponent<Collider>().enabled = false;
                gestureDict.Value[i].m_cGestureObjectPrimitive.gameObject.GetComponent<Renderer>().enabled = false;
            }
        }

        m_iTimePassed = 0.0f;
    }

    private void SerializeGestures()
    {
        Dictionary<string, List<GestureObject>> xmlDict = new Dictionary<string, List<GestureObject>>();
        FileInfo fInfo = new FileInfo(@"D:\Unity Projects\VRGestures\StreamFiles\Gestures.xml");
        if (fInfo != null && fInfo.Exists)
            File.Delete(@"D:\Unity Projects\VRGestures\StreamFiles\Gestures.xml");

        //Get each dictionary entry
        foreach (KeyValuePair<string, List<GestureObject>> entry in m_lGestureStreak)
        {
            List<GestureObject> Go = new List<GestureObject>();

            Go = entry.Value;

            //Duplicates the list with gestures to GestureContainer class so we can write an XML file with the list data
            for (int i = 0; i < Go.Count; i++)
            {
                //Fill in the field of the class with object data needed for gestures
                Debug.Log("Processed entry" + i + " | of " + entry.Key);
                Go[i].m_vPosition = Go[i].m_cGestureObjectPrimitive.gameObject.transform.position;
                Go[i].m_vRotation = Go[i].m_cGestureObjectPrimitive.gameObject.transform.rotation.eulerAngles;
            }
            //Fill class with values
            m_lGestureContainer.Gestures = Go;
            //Write away the class and on to the next
            xmlDict.Add(entry.Key, m_lGestureContainer.Gestures);
        }
        m_lGestureContainer.SerializeObject(xmlDict, @"D:\Unity Projects\VRGestures\StreamFiles\Gestures.xml");
    }

    private void DeserializeGestures()
    {
        var deserializedObject = m_lGestureContainer.DeSerializeObject(@"D:\Unity Projects\VRGestures\StreamFiles\Gestures.xml");
        //Overwrite the List of gestureobjects with the new deserialized xml data
        m_lGestureStreak = deserializedObject;
        Debug.Log("Deserialized!");
        m_bRecordMode = false;
    }

    private void CreatePrimStreaks()
    {
        foreach (KeyValuePair<string, List<GestureObject>> pair in m_lGestureStreak)
        {
            GameObject go = new GameObject(pair.Key + "Streak");
            go.transform.SetParent(m_cVirtualRealityCameraObject.transform.GetChild(2));

            for (int i = 0; i < pair.Value.Count; i++)
            {
                GameObject cubeObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Quaternion cubeRotation = Quaternion.Euler(pair.Value[i].m_vRotation);

                pair.Value[i].m_cGestureObjectPrimitive = cubeObj;
                cubeObj.transform.position = pair.Value[i].m_vPosition;
                cubeObj.transform.rotation = cubeRotation;
                cubeObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                cubeObj.gameObject.name = "Cube" + i;
                cubeObj.transform.SetParent(go.transform);
                if (i != 0)
                {
                    cubeObj.GetComponent<Collider>().enabled = false;
                    cubeObj.GetComponent<Renderer>().enabled = false;
                }
            }
            pair.Value[0].m_cGestureObjectPrimitive.gameObject.name = "Start";
            pair.Value[pair.Value.Count - 1].m_cGestureObjectPrimitive.gameObject.name = "End"; //-1 because the list starts at 0
        }
        m_bDeserializeOnce = false;
    }
    #region VR Button Inputs

    private void subscribeToVRInputEvents()
    {
        Debug.Log("Subscribing....");
        Debug.Log(SteamVR.instance.hmd_ModelNumber);
        if (SteamVR.instance == null)
            return;

        if (m_cVirtualRealityCameraObject != null)
        {
            var vRControllerLeft = m_cVirtualRealityCameraObject.transform.GetChild(0);
            var vRControllerRight = m_cVirtualRealityCameraObject.transform.GetChild(1);

            //Triggers are universal and can be subscribed to by any controller
            if (SteamVR.instance.hmd_ModelNumber != "" || SteamVR.instance.hmd_TrackingSystemName != "")
            {
                vRControllerRight.GetComponent<WandController>().TriggerClicked += Controller_TriggerPressed;
                vRControllerRight.GetComponent<WandController>().TriggerUnclicked += Controller_TriggerUnpressed;

                vRControllerLeft.GetComponent<WandController>().TriggerClicked += Controller_TriggerPressed;
                vRControllerLeft.GetComponent<WandController>().TriggerUnclicked += Controller_TriggerUnpressed;

                vRControllerRight.GetComponent<WandController>().Gripped += Controller_Gripped;
                vRControllerRight.GetComponent<WandController>().Ungripped += Controller_Ungripped;

                vRControllerLeft.GetComponent<WandController>().Gripped += Controller_Gripped;
                vRControllerLeft.GetComponent<WandController>().Ungripped += Controller_Ungripped;

            }
            if (SteamVR.instance.hmd_ModelNumber == "Oculus Rift CV1" || SteamVR.instance.hmd_TrackingSystemName.Contains("oculus"))
            {
                if (DebugLog) Debug.Log("Oculus connected!");
                vRControllerRight.GetComponent<WandController>().APressed += Controller_TeleportClicked;
                vRControllerRight.GetComponent<WandController>().AUnpressed += Controller_TeleportUnclicked;

                vRControllerLeft.GetComponent<WandController>().APressed += Controller_TeleportClicked;
                vRControllerLeft.GetComponent<WandController>().AUnpressed += Controller_TeleportUnclicked;

                vRControllerRight.GetComponent<WandController>().YBPressed += Controller_CycleToolsClicked;
                vRControllerRight.GetComponent<WandController>().YBUnpressed += Controller_CycleToolsUnclicked;

                vRControllerLeft.GetComponent<WandController>().YBPressed += Controller_CycleToolsClicked;
                vRControllerLeft.GetComponent<WandController>().YBUnpressed += Controller_CycleToolsUnclicked;
            }
            else if (SteamVR.instance.hmd_ModelNumber == "Vive MV" || SteamVR.instance.hmd_TrackingSystemName.Contains("vive"))
            {
                if (DebugLog) Debug.Log("Vive detected!");
                vRControllerRight.GetComponent<WandController>().PadClicked += Controller_TeleportClicked;
                vRControllerRight.GetComponent<WandController>().PadUnclicked += Controller_TeleportUnclicked;

                vRControllerLeft.GetComponent<WandController>().PadClicked += Controller_TeleportClicked;
                vRControllerLeft.GetComponent<WandController>().PadUnclicked += Controller_TeleportUnclicked;

                vRControllerRight.GetComponent<WandController>().MenuButtonClicked += Controller_CycleToolsClicked;
                vRControllerRight.GetComponent<WandController>().MenuButtonUnclicked += Controller_CycleToolsUnclicked;

                vRControllerLeft.GetComponent<WandController>().MenuButtonClicked += Controller_CycleToolsClicked;
                vRControllerLeft.GetComponent<WandController>().MenuButtonUnclicked += Controller_CycleToolsUnclicked;
            }
            else
            {
                Debug.LogError("No VR input devices found! Please connect your VR controllers");
                return;
            }
        }
    }

    #region VR controller input functions
    private void Controller_TeleportClicked(object sender, ClickedEventArgs e)
    { }

    private void Controller_TeleportUnclicked(object sender, ClickedEventArgs e)
    { }


    private void Controller_CycleToolsClicked(object sender, ClickedEventArgs e)
    {
        SteamVR_TrackedController control = sender as SteamVR_TrackedController; //get the triggerd controller

        if (control.controllerIndex == SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost))
        {
            SerializeGestures();
            Debug.Log("Serialized!");
        }
        
        if (control.controllerIndex == SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost))
        {
            Debug.Log("Recreate streak!");
            StartCoroutine(ReCreateGestureStreak());
        }

    }

    private void Controller_CycleToolsUnclicked(object sender, ClickedEventArgs e)
    { }

    private void Controller_TriggerPressed(object sender, ClickedEventArgs e)
    {
        SteamVR_TrackedController control = sender as SteamVR_TrackedController; //get the triggerd controller


        if (control.controllerIndex == SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost))
        {
            m_bShowHideDebug = !m_bShowHideDebug;
        }

        if (control.controllerIndex == SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost))
        {
            m_bTriggerPressed = true;
            m_cHMDText.text = "Record " + GestureNames[m_iGestureNamesRecordIndex];
            m_cHMDText.enabled = true;
        }
    }

    private void Controller_TriggerUnpressed(object sender, ClickedEventArgs e)
    {
        Debug.LogError("TriggerPressed!");
        m_cHMDText.enabled = false;
        m_bTriggerPressed = false;
    }

    private void Controller_Gripped(object sender, ClickedEventArgs e)
    {
        SteamVR_TrackedController control = sender as SteamVR_TrackedController; //get the triggerd controller
        Debug.Log("Gripped ");
        if (control.controllerIndex == SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost))
        {
            Debug.Log("Gripped left!");
            m_bGripped = true;
        }
    }

    private void Controller_Ungripped(object sender, ClickedEventArgs e)
    {
        m_bGripped = false;

        //Add to dictionary if there are still gestures to be recorded
        if (m_iGestureNamesRecordIndex != GestureNames.Length - 1 && m_bRecordMode)
        {

            List<GestureObject> formattedList = new List<GestureObject>();

            foreach (GestureObject obj in m_lGesturePrimitiveStreak)
                obj.m_iSpeed = m_iTimePassed;

            //Cut down the objects in the list to max 10
            var takeObjectsAtPositions = Math.Round((double)m_lGesturePrimitiveStreak.Count / 10, 0);   //devide by 10, accuracy doesn't really matter here
            int ObjectAtListPos = (int)takeObjectsAtPositions;                                          //Convert to int
            int[] objectPositions = new int[10] { 0, ObjectAtListPos, ObjectAtListPos * 2,              //Write to array to fill list in next forloop (cut down list)
                ObjectAtListPos * 3, ObjectAtListPos * 4, ObjectAtListPos * 5,
                ObjectAtListPos * 6, ObjectAtListPos * 7, ObjectAtListPos * 8, m_lGesturePrimitiveStreak.Count -1 };

            for (int i = 0; i < 10; i++)
            {
                formattedList.Add(m_lGesturePrimitiveStreak[objectPositions[i]]);
            }

            m_lGestureStreak.Add(GestureNames[m_iGestureNamesRecordIndex], new List<GestureObject>(formattedList)); //OLD: new List<GestureObject>(m_lGesturePrimitiveStreak)
            m_iGestureStreakNumber++;
            Debug.Log(String.Format("Time passed {0} || speed in obj = {1}", m_iTimePassed, m_lGesturePrimitiveStreak[0].m_iSpeed));
            m_iTimePassed = 0.0f;
            primitiveCount = 0;
            m_iGestureNamesRecordIndex++; //Move the array(GestureNames) to the next string to record gesture by name

            //Clean objects from list and in scene
            //for(int i =0; i<m_lGesturePrimitiveStreak.Count; i++)
            //{
            //    GameObject.Destroy(m_lGesturePrimitiveStreak[i].m_cGestureObjectPrimitive);
            //}
            m_lGesturePrimitiveStreak = new List<GestureObject>();
        }
        else
            m_bRecordMode = false;
    }

    #endregion

    private void OnDeviceConnected(params object[] args)
    {
        var i = (int)args[0];
        SteamVR.connected[i] = (bool)args[1];
        if (SteamVR.connected[i] == true)
        {
            OnDeviceConnectedAction(args);
        }
        else if (SteamVR.connected[i] == false)
        {
            OnDeviceDisconnected(args); //TODO: handle stuff when controller has been disconnected (check if controller, unsubscribe events etc.)
        }
    }

    public void OnDeviceDisconnected(params object[] args)
    {
        SteamVR_TrackedController controller = args[1] as SteamVR_TrackedController;
        var index = (uint)(int)args[0];
        var system = OpenVR.System;
        if (system != null && system.GetTrackedDeviceClass(index) == ETrackedDeviceClass.Controller)
        {
            controller.TriggerClicked -= Controller_TeleportClicked;
        }
    }

    private void OnDeviceConnectedAction(object[] args)
    {
        var index = (uint)(int)args[0];
        var system = OpenVR.System;
        if (system != null && system.GetTrackedDeviceClass(index) == ETrackedDeviceClass.Controller)
        {
            int leftIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
            int rightIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
        }
    }

    #endregion

    private void GestureTimer()
    {
        m_iTimePassed = m_iTimePassed += Time.deltaTime;
    }

    private void GetCurrentArea()
    {
        GameObject Area = m_cVirtualRealityCameraObject.transform.GetChild(2).GetChild(2).gameObject;
        for (int i = 0; i < 3; i++)
        {
            
            //Debug.Log("BK");
            foreach (Transform obj in Area.transform.GetChild(i))
            {
                if (m_cVirtualRealityCameraObject.transform.GetChild(0).GetChild(0).GetComponent<Collider>().bounds.Intersects(
                    obj.GetComponent<Collider>().bounds))
                {
                    m_cCurrentAreaTouched = obj.gameObject;
                }
            }
        }
    }

    IEnumerator DeactivateText()
    {
        yield return new WaitForSeconds(1f);
        m_cHMDText.enabled = false;
    }

    IEnumerator DeactivateHandObject(int i)
    {
        m_cVirtualRealityCameraObject.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
        m_cHandProps.transform.GetChild(i).gameObject.SetActive(true);
        //m_cVirtualRealityCameraObject.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(i).gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        m_cHandProps.transform.GetChild(i).gameObject.SetActive(false);

        m_cVirtualRealityCameraObject.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
        //m_cVirtualRealityCameraObject.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(i).gameObject.SetActive(false);
    }

    IEnumerator ReCreateGestureStreak()
    {
        DeserializeGestures();
        yield return new WaitForSeconds(0.5f);
        CreatePrimStreaks();
    }
    /// <summary>
    /// Used to calcuate distance and angle between gesture points
    /// </summary>
    /// <returns></returns>
    private void SaveControllerSpacePoint()
    {
        //Debug.Log("Time passed in miliseconds = " + m_iTimeToNextIntervalCheck);

        //Adding the current position/last entry
        GesturePoints GPoint = new GesturePoints();
        if (m_lController3DSpaceGesturePoint.Count == 0)
        {
            GPoint.m_iDistanceToLast = 0;
            GPoint.m_iAngleToLast = 0;
            GPoint.m_vPosition = m_cVirtualRealityCameraObject.transform.GetChild(0).transform.position;
            //GPoint.m_cCurrentAreaPrimitive = .... TODO
        }
        else
        {
            //Round float to 5 decimals behind the 0 because that is already super accurate
            GPoint.m_iDistanceToLast = (float)Math.Round((decimal)Vector3.Angle(m_cVirtualRealityCameraObject.transform.GetChild(0).transform.position, m_lController3DSpaceGesturePoint[0].m_vPosition),5);
            GPoint.m_iAngleToLast = (float)Math.Round((decimal)Vector3.Distance(m_cVirtualRealityCameraObject.transform.GetChild(0).transform.position, m_lController3DSpaceGesturePoint[0].m_vPosition),5);
            GPoint.m_vPosition = m_cVirtualRealityCameraObject.transform.GetChild(0).transform.position;
            //GPoint.m_cCurrentAreaPrimitive = .... TODO
        }
        //Add GesturePoints to the first entry of the list and destroy the last entry
        m_lController3DSpaceGesturePoint.Insert(0, GPoint);
        if (m_lController3DSpaceGesturePoint != null && m_lController3DSpaceGesturePoint.Count == 11)
            m_lController3DSpaceGesturePoint.RemoveAt(10);

        UpdateDebugGesturePointList();
    }

    /// <summary>
    /// Debug controller data debug text displayfunction
    /// </summary>
    private void UpdateDebugGesturePointList()
    {
        for (int i = 0; i != 10; i++)
        {
            //Debug.Log(i + "BK Check" + m_cDebugCanvas.transform.GetChild(i).name);
            if (i == m_lController3DSpaceGesturePoint.Count)
                return;

            if (m_bShowHideDebug)
                m_cDebugCanvas.transform.GetChild(i).gameObject.SetActive(true);
            else
                m_cDebugCanvas.transform.GetChild(i).gameObject.SetActive(false);

            m_cDebugCanvas.transform.GetChild(i).GetComponent<Text>().text = String.Format(i + " Dist.ToLast: {0} | Ang.ToLast: {1}", m_lController3DSpaceGesturePoint[i].m_iDistanceToLast, m_lController3DSpaceGesturePoint[i].m_iAngleToLast);
        }
        if (m_cCurrentAreaTouched != null)
            //Debug.Log("Area touched: " + m_cCurrentAreaTouched.name);
            m_cDebugCanvas.transform.GetChild(10).GetComponent<Text>().text = m_cCurrentAreaTouched.name;
    }
}