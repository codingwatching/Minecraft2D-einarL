using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class ChatMessageScript : MonoBehaviour
{
    private bool isCoroutineRunning = false;
	private bool makeMessageFadeOut = false;

	private CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

	void Update()
	{
		if (makeMessageFadeOut)
		{
			canvasGroup.alpha -= 0.01f;
			if (canvasGroup.alpha <= 0f)
			{
				makeMessageFadeOut = false;
				gameObject.SetActive(false);
			}
		}
	}

	private void OnEnable()
	{
		canvasGroup.alpha = 1f;
		makeMessageFadeOut = false;
		if (isCoroutineRunning) StopAllCoroutines();
        StartCoroutine(closeMessage());
	}

    private IEnumerator closeMessage()
    {
        isCoroutineRunning = true;
        yield return new WaitForSeconds(5);
		makeMessageFadeOut = true;
		isCoroutineRunning = false;
    }
}
