using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Net.Http;
using UnityEngine.XR;

public class BaseMovement : MonoBehaviour
{
    string website = "http://34.23.107.56/";
    static string robot_motion;

    static bool useProxy = false;

    static HttpClientHandler hch = new HttpClientHandler 
    {
        UseProxy = useProxy,
    };

    static readonly HttpClient client = new HttpClient(hch);

    private InputDevice rightController, leftController;

    bool crouched = false;

    // Start is called before the first frame update
    async void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDeviceCharacteristics rcChara = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDeviceCharacteristics lcChara = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(rcChara, devices);

        if (devices.Count > 0 ) 
        {
            rightController = devices[0];
        }

        InputDevices.GetDevicesWithCharacteristics(lcChara, devices);

        if (devices.Count > 0)
        {
            leftController = devices[0];
        }

        robot_motion = await client.GetStringAsync(website); // Calls the cloud website to get the updated IP address of the Raspberry Pi
        robot_motion = "http://" + robot_motion.Replace("\n", "").Replace("\r", "") + ":50000/motion/"; // Adds the trail to the IP address to access the motion API commands
        Debug.Log(robot_motion);  // Outputs the Pi IP to console if needed
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.anyKeyDown)
        {
            // Timer ++
        }

        // Put all commands in here
        rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rightVec);
        leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 leftVec);

        // Walk left/right movement
        if (leftVec.x < -0.5)
        {
            APICall(robot_motion + "walk_left");
        }
        else if (leftVec.x > 0.5)
        {
            APICall(robot_motion + "walk_right");
        }

        // Walk forward movement
        else if (leftVec.y > 0.5)
        {
            APICall(robot_motion + "walk_forward_short");
        }

        // Turn left/right movement
        else if (rightVec.x > 0.5)
        {
            APICall(robot_motion + "turn_right");
        }
        else if (rightVec.x < -.5)
        {
            APICall(robot_motion + "turn_left");
        }

        /* 
         * Crouch occurs if pressed 3 times
         * Needs work as it basically shuts the robot down and won't stand back up
         * This includes the head and arm motors
         * Need these motors to still be responsive as the user will need to crouch and pick stuff up
         */
        else if (rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool Bbutton) && Bbutton)
        {
            // APICall(robot_motion + "")
            if (!crouched) 
            {
                APICall(robot_motion + "sit_down");
                crouched = true;
            }
            else 
            { 
                APICall(robot_motion + "basic_motion");
                crouched = false;
            }
        }
        
        // For fun :)
        else if (leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool Ybutton) && Ybutton)
        {
            APICall(robot_motion + "dance_gangnamstyle");
        }

        // Put the robot back into intial standing position
        else if (leftController.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool LJClick) && LJClick)
        {
            APICall(robot_motion + "reset");
        }
    }

    static async Task APICall(string ipAddress)
    {
        string responseBody = await client.GetStringAsync(ipAddress);
    }
}

