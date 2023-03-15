using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextToConsole : MonoBehaviour
{
    public int maxMessages = 10;
    [SerializeField]
    List<Message> messageList = new List<Message>();

    public GameObject textObject, consolePanel;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    public void SendToConsole(string text)
    {
        if (messageList.Count >= maxMessages) 
        {
            Destroy(messageList[0].textElement.gameObject);
            messageList.Remove(messageList[0]);
        }

        Message newMessage = new Message();
        newMessage.text = text;

        GameObject newText = Instantiate(textObject, consolePanel.transform);
        newMessage.textElement = newText.GetComponent<Text>();
        newMessage.textElement.text = newMessage.text;

        messageList.Add(newMessage);
    }
}

[System.Serializable]
public class Message 
{
    public string text;
    public Text textElement;
}

