using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipScript : MonoBehaviour
{

    private IDataService dataService = JsonDataService.Instance;
    private GameObject zoomTipInstance = null;
    private Coroutine showZoomTipCoroutine = null;

    private GameObject placeTipInstance = null;

    // Start is called before the first frame update
    void Start()
    {
        if (!dataService.exists("inventory.json"))
        {
			showZoomTipCoroutine = StartCoroutine(showZoomTip());
        }
        else Destroy(this);
    }


    private IEnumerator showZoomTip()
    {
        yield return new WaitForSeconds(5);
        zoomTipInstance = Instantiate(Resources.Load<GameObject>("Prefabs\\UI\\Zoom Tip"), transform);
        StartCoroutine(moveToPosition(zoomTipInstance.GetComponent<RectTransform>(), new Vector2(-289, -45), 0.7f));
        yield return new WaitForSeconds(15);
        removeZoomTip();
    }

    public void removeZoomTip()
    {
        StopCoroutine(showZoomTipCoroutine);
        if (zoomTipInstance == null) return;
		StartCoroutine(moveToPosition(zoomTipInstance.GetComponent<RectTransform>(), new Vector2(-289, 42.7f), 0.7f, true));

	}

    public void showBackgroundPlaceTip()
    {
        IEnumerator showTip()
        {
            while(zoomTipInstance != null) yield return new WaitForSeconds(3);

			placeTipInstance = Instantiate(Resources.Load<GameObject>("Prefabs\\UI\\Background Place Tip"), transform);
			StartCoroutine(moveToPosition(placeTipInstance.GetComponent<RectTransform>(), new Vector2(-352, -45), 0.7f));
			IEnumerator removeTip()
			{
				yield return new WaitForSeconds(15);
				StartCoroutine(moveToPosition(placeTipInstance.GetComponent<RectTransform>(), new Vector2(-352, 45), 0.7f, false, true));
			}
			StartCoroutine(removeTip());
		}
        StartCoroutine(showTip());
    }

    // Coroutine to move the UI component
    private IEnumerator moveToPosition(RectTransform component, Vector2 target, float duration, bool destroyComponent = false, bool destroyScript = false)
	{
		Vector2 startPosition = component.anchoredPosition;
		float elapsedTime = 0;

		while (elapsedTime < duration)
		{
			component.anchoredPosition = Vector2.Lerp(startPosition, target, (elapsedTime / duration));
			elapsedTime += Time.deltaTime;
			yield return null; // Wait for the next frame
		}

        // Ensure the target position is set at the end
		component.anchoredPosition = target;
        if (destroyComponent) Destroy(component?.gameObject);
        if (destroyScript) Destroy(this);
	}
}
