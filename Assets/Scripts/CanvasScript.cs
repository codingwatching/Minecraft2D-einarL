using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;

public class CanvasScript : MonoBehaviour
{
    GameObject deathScreen;
    PlayerControllerScript playerController;
    GameObject gameMenuScreen;
    GameObject chat;
    GameObject chatMessage;
    TMP_Text chatMessageText;
    bool isPaused = false;
    bool isInChat = false;

    // Start is called before the first frame update
    void Start()
    {
        deathScreen = transform.Find("DeathScreen").gameObject;
		playerController = GameObject.Find("SteveContainer").GetComponent<PlayerControllerScript>();
        gameMenuScreen = transform.Find("GameMenu").gameObject;
        chat = transform.Find("Chat").gameObject;
        chatMessage = transform.Find("ChatMessage").gameObject;
		chatMessageText = chatMessage.transform.Find("Text").GetComponent<TMP_Text>();
	}

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(isInChat) closeChat();
            else if (isPaused)
            {
				closeMenuAndResumeGame();
			}
			else if (playerController.isSleeping()) // stop sleeping
			{
				playerController.stopSleeping();
			}
			else if (!InventoryScript.getIsInUI())
            {
                openMenuAndPauseGame();
            }

        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            if(!isInChat && !InventoryScript.getIsInUI())
            {
                openChat();
            }
        }

	}

    private void openMenuAndPauseGame()
    {
		Time.timeScale = 0;
        isPaused = true;
		gameMenuScreen.SetActive(true);
		InventoryScript.setIsInUI(true);
	}

    public void closeMenuAndResumeGame()
    {
		Time.timeScale = 1;
        isPaused = false;
		gameMenuScreen.SetActive(false);
		InventoryScript.setIsInUI(false);
	}

    public void showDeathScreen()
    {
        deathScreen.SetActive(true);
    }

    public void closeDeathScreen()
    {
        deathScreen.SetActive(false);
    }

    private void openChat()
    {
        isInChat = true;
        InventoryScript.setIsInUI(true);
		chat.SetActive(true);
    }

    public void closeChat()
    {
        isInChat = false;
		InventoryScript.setIsInUI(false);
		chat.SetActive(false);
	}

    public void sendChatMessage(string message)
    {
        if(chatMessage.activeSelf) chatMessage.SetActive(false);
        chatMessage.SetActive(true);
        chatMessageText.text = message;
    }
}
