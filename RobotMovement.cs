using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using System.Threading.Tasks;
using System.Net.Http;

public class RobotMovement : MonoBehaviour
{
    string robot_motion = "http://10.100.54.50:50000/motion/";
    public static string textOutput;

    UnityEvent button_pressed = new UnityEvent();
    bool previously_pressed = false;

    static readonly HttpClient client = new HttpClient();

    // Start is called before the first frame update
    void Start()
    {
        button_pressed.AddListener(ButtonPress);
        textOutput = "the code is running";
    }

    // Update is called once per frame
    void Update()
    {
        // Makes the call to ButtonPress if any button is pressed
        if (Input.anyKeyDown && !previously_pressed)
        {
            button_pressed.Invoke();
            textOutput = "Some button was pressed";
        }
        previously_pressed = Input.anyKeyDown;

    }

    static async Task APICall(string uri)
    {
        string responseBody = await client.GetStringAsync(uri);

        textOutput = responseBody;
    }

    // Called whenever a key is pressed
    // If the key is one of the movement keys then it makes the respective API call
    void ButtonPress()
    {
        // Walk left/right
        if (Input.GetAxis("LeftJoystickHorizontal") > 0)
        {
            APICall(robot_motion + "walk_left");
            textOutput = "a was pressed";
        }
        else if (Input.GetAxis("LeftJoystickHorizontal") < 0) 
        {
            APICall(robot_motion + "walk_right");
            textOutput = "d was pressed";
        }
        // Walk forward
        else if (Input.GetAxis("LeftJoystickVertical") < 0)
        {
            APICall(robot_motion+ "walk_forward_short");
            textOutput = "w was pressed";
        }
        // Rotate left/right
        else if (Input.GetAxis("RightJoystickHorizontal") < 0)
        {
            APICall(robot_motion + "turn_left");
            textOutput = "q was pressed";
        }
        else if (Input.GetAxis("RightJoystickHorizontal") > 0)
        {
            APICall(robot_motion + "turn_right");
            textOutput = "e was pressed";
        }
    }
}
