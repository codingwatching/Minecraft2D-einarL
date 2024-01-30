using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class EatScript : MonoBehaviour
{

    private bool isEating = false;

	// the slot with the food when the player started eating
    // we need to have this to check if the player switches slots when eating, then the player should stop eating
	private int selectedSlotWhenStartedEating = -1;

    private HungerbarScript hungerbarScript;

    // Start is called before the first frame update
    void Start()
    {
        hungerbarScript = GameObject.Find("Canvas").transform.Find("Hungerbar").GetComponent<HungerbarScript>();
    }

    // Update is called once per frame
    void Update()
    {
        // if clicking right click and the player is holding food
        if (Input.GetMouseButtonDown(1) && InventoryScript.isHoldingFood())
        {
            Debug.Log("Start eating");
            StartCoroutine(finishEating());
            selectedSlotWhenStartedEating = InventoryScript.getSelectedSlot();
        }
		// if the player is eating && (he is not holding down right click || switched selected slots in hotbar)
		else if (isEating && (!Input.GetMouseButton(1) || selectedSlotWhenStartedEating != InventoryScript.getSelectedSlot())) 
        {
            Debug.Log("Stopped eating");
            StopAllCoroutines(); // stop eating
            isEating = false;
        }
    }

    private IEnumerator finishEating()
    {
        isEating = true;
        yield return new WaitForSeconds(2);
		int foodAddition = FoodHashtable.getFoodAddition(InventoryScript.getHeldItemName());
        Assert.IsTrue(foodAddition >= 0);
		InventoryScript.decrementSlot(InventoryScript.getSelectedSlot()); // remove food
        Debug.Log("FINISHED EATING");
        hungerbarScript.eatFood(foodAddition); // restore hunger

        isEating = false;
    }
}
