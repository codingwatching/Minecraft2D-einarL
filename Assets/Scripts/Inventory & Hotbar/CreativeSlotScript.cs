using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;


public class CreativeSlotScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, Slot
{

	private GameObject itemImage;
	private Image hoverTexture;
	private OpenInventoryScript openInventoryScript;
	public InventorySlot itemInSlot = new InventorySlot();

	// Start is called before the first frame update
	void Start()
	{
		itemImage = transform.Find("ItemImage").gameObject;
		hoverTexture = transform.Find("HoverTexture").GetComponent<Image>();

		openInventoryScript = transform.parent.parent.parent.parent.GetComponent<OpenInventoryScript>();
	}

	/**
     * updates what is in the slot 
     * 
     * DurabilityItem tool: the tool/armor that is in this slot. this is null if there is no tool nor armor in the slot.
     */
	public void updateSlot()
	{
		if (itemImage == null) Start();
		Sprite image;
		if (itemInSlot == null) image = null;
		else image = Resources.Load<Sprite>("Textures\\ItemTextures\\" + itemInSlot.itemName);

		if (image != null) // found item to display
		{
			itemImage.GetComponent<Image>().sprite = image;

			itemImage.SetActive(true);
		}
		else
		{
			itemImage.SetActive(false);
		}
	}

	/**
    * runs when player left clicks the item slot.
    * 
    * if the player has an item: then remove it,
    * otherwise pick up the item in this slot
    * 
    */
	public void leftClickSlot()
	{
		if (InventoryScript.getHasItemsPickedUp()) // if holding an item
		{
			InventoryScript.setItemsPickedUp(new InventorySlot());
			openInventoryScript.setIsItemBeingHeld(false);
			return;
		}

		if (itemImage.activeSelf) // if there is an item in this slot
		{
			// pickup the item in this slot

			InventorySlot slotItems = itemInSlot.copy();

			InventoryScript.setItemsPickedUp(slotItems); // set the picked up items to be the items that are in this slot
			openInventoryScript.setIsItemBeingHeld(true);

		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		hoverTexture.color = new Color(0.7f, 0.7f, 0.7f, 0.7f); // remove transparency
		openInventoryScript.setHoveringOverSlotScript(this);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		hoverTexture.color = new Color(0.7f, 0.7f, 0.7f, 0f); // make hoverTexture invisible
	}

	public void OnPointerClick(PointerEventData eventData)
	{

		if (eventData.button == PointerEventData.InputButton.Left)
		{
			leftClickSlot();
		}

	}
}
