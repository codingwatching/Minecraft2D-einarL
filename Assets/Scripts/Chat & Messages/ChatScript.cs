using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class ChatScript : MonoBehaviour
{

    private TMP_InputField input;
    private CanvasScript canvasScript;
    private PlayerControllerScript playerControllerScript;
    private CanvasGroup armorCanvasGroup;

    void Awake()
    {
        input = transform.Find("InputField").GetComponent<TMP_InputField>();
        canvasScript = transform.parent.GetComponent<CanvasScript>();
        playerControllerScript = GameObject.Find("SteveContainer").GetComponent<PlayerControllerScript>();
        armorCanvasGroup = transform.parent.Find("Armorbar").GetComponent<CanvasGroup>();
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
    /**
     * runs the command that is in the input. there are only two commands:
     *      /c or /creative for going into creative mode, and
     *      /s or /survival for going into surival mode
     */
    private void runCommand()
    {
        bool unknownCommand = true;
        string command = input.text.Trim().ToLower();
        Debug.Log("run command: " + command);

        if(command.Equals("/c") || command.Equals("/creative"))
        {
            Debug.Log("creative mode");
            unknownCommand = false;
            toggleCreativeMode();
        }
		else if (command.Equals("/s") || command.Equals("/survival"))
		{
			Debug.Log("survival mode");
			unknownCommand = false;
			toggleCreativeMode(false);
		}

		canvasScript.closeChat();
        if (unknownCommand) canvasScript.sendChatMessage("Unknown command");
    }

    private void toggleCreativeMode(bool creative = true)
    {
		playerControllerScript.toggleCreativeMode(creative);
        armorCanvasGroup.alpha = creative ? 0 : 1;
	}
}
