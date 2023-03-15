using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;
using System.Net.Http;

public class RobotMovement : MonoBehaviour
{
    string website = "http://34.23.107.56/";
    static string robot_motion;
    static string next_motion;

    UnityEvent button_pressed = new UnityEvent();
    bool previously_pressed = false;
    
    static bool useProxy = false;

    static HttpClientHandler hch = new HttpClientHandler 
    {
        UseProxy = useProxy,
    };

    static readonly HttpClient client = new HttpClient(hch);

    // Start is called before the first frame update
    async void Start()
    {
        button_pressed.AddListener(ButtonPress);

        robot_motion = await client.GetStringAsync(website); // Calls the cloud website to get the updated IP address of the Raspberry Pi
        robot_motion = "http://" + robot_motion.Replace("\n", "").Replace("\r", "") + ":50000/motion/"; // Adds the trail to the IP address to access the motion API commands
        Debug.Log(robot_motion);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Makes the call to ButtonPress if any button is pressed
        // Needs to be changed to only take one input in at a time
        // Include a timer so that headset doesn't get bogged down with inputs
        if (Input.anyKeyDown && !previously_pressed)
        {
            button_pressed.Invoke();
        }
        previously_pressed = Input.anyKeyDown;

    }

    static async Task APICall(string ipAddress)
    {
        string responseBody = await client.GetStringAsync(ipAddress);
    }

    // Called whenever a key is pressed
    // If the key is one of the movement keys then it makes the respective API call
    void ButtonPress()
    {
        // Walk left/right
        if (Input.GetAxis("LeftJoystickHorizontal") == 1 || Input.GetKey("a"))
        {
            // next_motion = robot_motion + "walk_left";
            APICall(robot_motion + "walk_left");  // String concatenation in function argument needs testing again
        }
        else if (Input.GetAxis("LeftJoystickHorizontal") == -1) 
        {
            APICall(robot_motion + "walk_right");
        }
        // Walk forward
        else if (Input.GetAxis("LeftJoystickVertical") == 1)
        {
            APICall(robot_motion + "walk_forward_short");
        }
        // Rotate left/right
        else if (Input.GetAxis("RightJoystickHorizontal") == -1)
        {
            APICall(robot_motion + "turn_left");
        }
        else if (Input.GetAxis("RightJoystickHorizontal") == 1)
        {
            APICall(robot_motion + "turn_right");
        }
    }
}

