using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BubblebarScript : MonoBehaviour
{

	private Sprite bubbleImage;
    private Sprite bubblePopImage;

	private Image[] bubbleImages = new Image[10];

	private HealthbarScript healthbarScript;


	private void Awake()
	{
		for (int i = 0; i < bubbleImages.Length; i++)
		{
			bubbleImages[i] = transform.Find("Bubble" + i).GetComponent<Image>();
		}

		Sprite[] iconImages = Resources.LoadAll<Sprite>("Textures/UI/icons");
		bubbleImage = getSpriteWithName(iconImages, "icons_7");
		bubblePopImage = getSpriteWithName(iconImages, "icons_8");

		healthbarScript = GameObject.Find("Canvas").transform.Find("Healthbar").GetComponent<HealthbarScript>();
	}

	void OnEnable()
    {
		foreach(Image image in bubbleImages)
		{
			image.sprite = bubbleImage;
			image.color = new Color(1, 1, 1, 1);
		}

        StartCoroutine(removeBubbles());
    }

    private IEnumerator removeBubbles()
    {
		int index = 9;
        while (true)
        {
            yield return new WaitForSeconds(1.5f);
			bubbleImages[index].sprite = bubblePopImage;
			StartCoroutine(removePopImage(index));
			index--;
			if(index < 0)
			{
				StartCoroutine(suffocate());
				break;
			}
		}
    }

	private IEnumerator removePopImage(int index)
	{
		yield return new WaitForSeconds(0.3f);
		bubbleImages[index].color = new Color(1, 1, 1, 0);
	}


    private IEnumerator suffocate()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.5f);
			if (healthbarScript.getHealth() <= 0) break;
			healthbarScript.takeDamage(2, 0, false, false);
		}
    }

	private Sprite getSpriteWithName(Sprite[] list, string name)
	{
		for (int i = 0; i < list.Length; i++)
		{
			if (list[i].name == name)
			{
				return list[i];
			}
		}
		Debug.LogError("ERROR: sprite with name " + name + " was not located in the icons sprite");
		return null;
	}
}
