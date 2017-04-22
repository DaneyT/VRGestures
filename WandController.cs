using UnityEngine;
using System.Collections;
using Valve.VR;

//This is a own custom class used as a wrapper for the Vive controller inputs
public class WandController : SteamVR_TrackedController
{
    //internal buttonmask class van maken
    public class ButtonMaskOverride : SteamVR_Controller.ButtonMask
    {
        public const ulong AButton = (1ul << (int)EVRButtonId.k_EButton_A);
        public const ulong YBButton = (1ul << (int)EVRButtonId.k_EButton_ApplicationMenu);
    }

    //these are the reallife velocity and angular velocity data
    public SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)controllerIndex); } }  //device
    public Vector3 velocity { get { return controller.velocity; } }                                                //velocity 
    public Vector3 angularVelocity { get { return controller.angularVelocity; } }                                  //angular velocity

    public bool aPressed = false;
    public bool ybPressed = false;


    public event ClickedEventHandler APressed;
    public event ClickedEventHandler AUnpressed;
    public event ClickedEventHandler YBPressed;
    public event ClickedEventHandler YBUnpressed;

    public override void Start()
    {
        base.Start();

    }

    public override void Update()
    {
        base.Update();

        //Oculus button inputs A & X
        ulong aButton = controllerState.ulButtonPressed & (1UL << ((int)EVRButtonId.k_EButton_A));
        if (aButton > 0L && !aPressed)
        {
            aPressed = true;
            ClickedEventArgs e;
            e.controllerIndex = controllerIndex;
            e.flags = (uint)controllerState.ulButtonPressed;
            e.padX = controllerState.rAxis0.x;
            e.padY = controllerState.rAxis0.y;
            OnAPressed(e);
        }
        else if (aButton == 0L && aPressed)
        {
            aPressed = false;
            ClickedEventArgs e;
            e.controllerIndex = controllerIndex;
            e.flags = (uint)controllerState.ulButtonPressed;
            e.padX = controllerState.rAxis0.x;
            e.padY = controllerState.rAxis0.y;
            OnAUnpressed(e);
        }

        ulong ybButton = controllerState.ulButtonPressed & (1UL << ((int)EVRButtonId.k_EButton_ApplicationMenu));
        if(ybButton > 0L && !ybPressed)
        {
            ybPressed = true;
            ClickedEventArgs e;
            e.controllerIndex = controllerIndex;
            e.flags = (uint)controllerState.ulButtonPressed;
            e.padX = controllerState.rAxis0.x;
            e.padY = controllerState.rAxis0.y;
            OnYBPressed(e);
        }
        else if(ybButton == 0L && ybPressed)
        {
            ybPressed = false;
            ClickedEventArgs e;
            e.controllerIndex = controllerIndex;
            e.flags = (uint)controllerState.ulButtonPressed;
            e.padX = controllerState.rAxis0.x;
            e.padY = controllerState.rAxis0.y;
            OnYBUnpressed(e);
        }


    }

    public override void OnTriggerClicked(ClickedEventArgs e)                                               //e.controllerIndex 3 & 4 zijn de controllers
    {
        base.OnTriggerClicked(e);
    }

    public override void OnTriggerUnclicked(ClickedEventArgs e)
    {
        base.OnTriggerUnclicked(e);
    }

    //***********************************************
    //------------Vive Specific Controls-------------
    //***********************************************

    #region Vive Inputs

    public override void OnMenuClicked(ClickedEventArgs e)
    {
        base.OnMenuClicked(e);
    }

    public override void OnMenuUnclicked(ClickedEventArgs e)
    {
        base.OnMenuUnclicked(e);
    }

    //public override void OnSteamClicked(ClickedEventArgs e)
    //{
    //    base.OnSteamClicked(e);
    //}

    public override void OnPadClicked(ClickedEventArgs e)
    {
        base.OnPadClicked(e);
    }

    public override void OnPadUnclicked(ClickedEventArgs e)
    {
        base.OnPadUnclicked(e);
    }

    //public override void OnPadTouched(ClickedEventArgs e)
    //{
    //    base.OnPadTouched(e);
    //}

    //public override void OnPadUntouched(ClickedEventArgs e)
    //{
    //    base.OnPadUntouched(e);
    //}

    public override void OnGripped(ClickedEventArgs e)
    {
        base.OnGripped(e);
    }

    public override void OnUngripped(ClickedEventArgs e)
    {
        base.OnUngripped(e);
    }

    #endregion

    //***********************************************
    //----------Oculus Specific Controls-------------
    //***********************************************

    #region Oculus Inputs
    public virtual void OnAPressed(ClickedEventArgs e)
    {
        if (APressed != null)
            APressed(this, e);

    }
    public virtual void OnAUnpressed(ClickedEventArgs e)
    {
        if (AUnpressed != null)
            AUnpressed(this, e);
    }

    public virtual void OnYBPressed(ClickedEventArgs e)
    {
        if (YBPressed != null)
            YBPressed(this, e);

    }
    public virtual void OnYBUnpressed(ClickedEventArgs e)
    {
        if (YBUnpressed != null)
            YBUnpressed(this, e);
    }
    #endregion


    public float GetTriggerAxis()
    {
        //if the controller isn't valid, return 0
        if (controller == null)
            return 0;

        //Use SteamVR_Controller.Device's GetAxis() method to get the trigger axis
        return controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis1).x;      //goes from 0(unpressed) to 1(fully pressed)

    }
}