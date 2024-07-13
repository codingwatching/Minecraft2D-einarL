using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatScript : MonoBehaviour
{

    private TMP_InputField input;
    private CanvasScript canvasScript;

    void Awake()
    {
        input = transform.Find("InputField").GetComponent<TMP_InputField>();
        canvasScript = transform.parent.GetComponent<CanvasScript>();
    }

	private void OnEnable()
	{
        input.text = "";
        input.ActivateInputField();
	}

	// Update is called once per frame
	void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return)) runCommand();
        else if(Input.GetMouseButtonDown(0)) canvasScript.closeChat();
    }

    private void runCommand()
    {
        bool unknownCommand = true;
        string command = input.text.Trim().ToLower();
        Debug.Log("run command: " + command);

        if(command.Equals("/c") || command.Equals("/creative"))
        {
            Debug.Log("creative mode");
            unknownCommand = false;
        }
		else if (command.Equals("/s") || command.Equals("/survival"))
		{
			Debug.Log("survival mode");
			unknownCommand = false;
		}


		canvasScript.closeChat();
        if (unknownCommand) canvasScript.sendChatMessage("Unknown command");
    }
}
