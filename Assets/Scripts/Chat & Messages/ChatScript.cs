using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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
        IEnumerator apparentlyThisHasToRunAFrameLaterToWork()
        {
            yield return null;
			input.Select();
            input.ActivateInputField();
		}
        StartCoroutine(apparentlyThisHasToRunAFrameLaterToWork());
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

        if(command.Equals("/c") || command.Equals("/creative"))
        {
			unknownCommand = false;
			if (playerControllerScript.isInCreativeMode()) canvasScript.sendChatMessage("You're already in creative mode");
			else toggleCreativeMode();
        }
		else if (command.Equals("/s") || command.Equals("/survival"))
		{
			unknownCommand = false;
			if (!playerControllerScript.isInCreativeMode()) canvasScript.sendChatMessage("You're already in survival mode");
			else toggleCreativeMode(false);
		}
        else if (command.Equals("/tl") || command.Equals("/togglelighting"))
        {
            unknownCommand = false;
            bool enabled = SpawningChunkData.lightingEnabled;

			if(enabled) canvasScript.sendChatMessage("Lighting disabled");
            else canvasScript.sendChatMessage("Lighting enabled");
			toggleLights(!enabled);
            SpawningChunkData.lightingEnabled = !enabled;
		}

		canvasScript.closeChat();
        if (unknownCommand) canvasScript.sendChatMessage("Unknown command");
    }

    private void toggleCreativeMode(bool creative = true)
    {
		playerControllerScript.toggleCreativeMode(creative);
        armorCanvasGroup.alpha = creative ? 0 : 1;
	}

    private void toggleLights(bool enabled = true)
    {
		Light2D[] lights = FindObjectsOfType<Light2D>();

		// Disable/enable each Light2D component
		foreach (Light2D light in lights)
		{
			light.enabled = enabled;
		}
	}
}
