using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreativeInventoryScript : MonoBehaviour
{
    private InventorySlot[,] allItems; // these are only the items that the player can obtain

	private readonly string[] allItemNames = new string[] { 
		"Dirt", "Stone", "Cobblestone", "Sand", "Gravel", "LogOak", "LogSpruce", "LogCherry",
		"PlankOak", "PlankSpruce", "PlankCherry", "Wool", "Glass", "SnowBlock", "TNT", "CraftingTable",
		"Furnace", "Chest", "Bed", "Ladder", "DoorOak", "DoorSpruce", "DoorCherry", "IronOre", "Cactus", "Boat",
		"Torch", "Dandelion", "Rose", "TulipOrange", "TulipPink", "TulipRed", "TulipWhite", "MushroomBrown", "MushroomRed",
		"SaplingOak", "SaplingSpruce", "SaplingCherry", "Bucket", "WaterBucket", "Coal", "Charcoal", "IronIngot", "Diamond", "String",
		"Stick", "Leather", "Flint", "Feather", "Gunpowder", "Apple", "ChickenCooked", "ChickenRaw", "MuttonCooked", "MuttonRaw",
		"PorkchopCooked", "PorkchopRaw", "SteakCooked", "SteakRaw", "RottenFlesh", "Snowball", "Arrow"
	};
	private readonly string[] allTools = new string[] { 
		"Bow", "FlintAndSteel", "WoodSword", "WoodPickaxe", "WoodAxe", "WoodShovel", "StoneSword", "StonePickaxe",
		"StoneAxe", "StoneShovel", "IronSword", "IronPickaxe", "IronAxe", "IronShovel", "DiamondSword", "DiamondPickaxe",
		"DiamondAxe", "DiamondShovel"
	};
	private readonly string[] allArmors = new string[] {
		"LeatherHelmet", "LeatherChestplate", "LeatherLeggings", "LeatherBoots", "IronHelmet", "IronChestplate",
		"IronLeggings", "IronBoots", "DiamondHelmet", "DiamondChestplate", "DiamondLeggings", "DiamondBoots"
	};

	private Slider slider;

	private CreativeSlotScript[] creativeSlotScripts = new CreativeSlotScript[45];

    void Awake()
    {
		int columns = 9;
		int rows = Mathf.CeilToInt((float)(allItemNames.Length + allTools.Length + allArmors.Length) / columns);
		int hiddenRows = rows - 5; // amount of rows that you need to scroll down to see

		List<string[]> itemLists = new List<string[]>() { allItemNames, allTools, allArmors };

		allItems = new InventorySlot[rows, columns];

		int listIndex = 0;
		int index = 0;
		for(int i = 0; i < rows; i++)
		{
			for(int j = 0; j < columns; j++)
			{
				if (index >= itemLists[listIndex].Length)
				{
					if (listIndex >= itemLists.Count - 1) break;
					listIndex++;
					index = 0;
				}
				if (listIndex == 0) allItems[i, j] = new InventorySlot(allItemNames[index], 1);
				else if (listIndex == 1) allItems[i, j] = new InventorySlot(new ToolInstance(getToolScriptable(allTools[index])), allTools[index]);
				else if (listIndex == 2) allItems[i, j] = new InventorySlot(new ArmorInstance(getArmorScriptable(allArmors[index])), allArmors[index]);
				index++;
			}
		}

		slider = transform.parent.Find("Slider").GetComponent<Slider>();
		slider.maxValue = hiddenRows;
		Transform creativeSlotsParent = transform.Find("CreativeInventorySlots");
		for(int i = 0; i < creativeSlotScripts.Length; i++)
		{
			creativeSlotScripts[i] = creativeSlotsParent.Find($"CreativeInventorySlot{i}").GetComponent<CreativeSlotScript>();
		}
	}

	private void OnEnable()
	{
		slider.value = 0;
		showItems(0);
	}

	private void showItems(int startRow)
	{
		if (startRow > allItems.GetLength(0) - 5)
		{
			Debug.LogError($"slider value can only be between 0 and {allItems.GetLength(0) - 5} (inclusive), but it is: {startRow}");
			return;
		}
		int currentRow = 36;
		int columnIndex = 0;
		int rowIndex = startRow;
		for(int i = 36; i >= 0; i++)
		{
			creativeSlotScripts[i].itemInSlot = allItems[rowIndex, columnIndex];
			creativeSlotScripts[i].updateSlot();
			if(i >= currentRow + 8)
			{
				currentRow -= 9;
				i = currentRow - 1;
			}
			columnIndex++;
			if(columnIndex >= 9)
			{
				columnIndex = 0;
				rowIndex++;
			}
		}
	}
	// runs when the value of the slider gets changed
	public void scroll()
	{
		showItems((int)slider.value);
	}

	private static Tool getToolScriptable(string toolName)
	{
		return Resources.Load<Tool>("ToolScriptables\\" + toolName);
	}

	private static Armor getArmorScriptable(string armorName)
	{
		return Resources.Load<Armor>("ToolScriptables\\Armor\\" + armorName);
	}
}
