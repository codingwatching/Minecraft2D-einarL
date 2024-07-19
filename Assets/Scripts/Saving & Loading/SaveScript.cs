using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/**
 * this script is responsible for saving inventory and furnaces, etc.
 * this script is not used for saving chunks when unrendering/rendering them.
 * 
 */
public class SaveScript : MonoBehaviour
{

	private OpenFurnaceScript openFurnaceScript;
	private OpenChestScript openChestScript;
	private HealthbarScript healthbarScript;
	private HungerbarScript hungerbarScript;
	private DayProcessScript dayProcessScript;
	private ArmorScript armorScript;
	private static IDataService dataService = JsonDataService.Instance;
	private Transform steve;
	private spawnChunkScript scScript;

	// Start is called before the first frame update
	void Start()
    {
		openFurnaceScript = GameObject.Find("Canvas").transform.Find("InventoryParent").GetComponent<OpenFurnaceScript>();
		openChestScript = GameObject.Find("Canvas").transform.Find("InventoryParent").GetComponent<OpenChestScript>();
		healthbarScript = GameObject.Find("Canvas").transform.Find("Healthbar").GetComponent<HealthbarScript>();
		hungerbarScript = GameObject.Find("Canvas").transform.Find("Hungerbar").GetComponent<HungerbarScript>();
		dayProcessScript = GameObject.Find("CM vcam").transform.Find("SunAndMoonTexture").GetComponent<DayProcessScript>();
		armorScript = GameObject.Find("Canvas").transform.Find("Armorbar").GetComponent<ArmorScript>();
		steve = GameObject.Find("SteveContainer").transform;
		scScript = GameObject.Find("Main Camera").GetComponent<spawnChunkScript>();
		StartCoroutine(saveCoroutine());
	}

    // Update is called once per frame
	/*
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.I))
		{
			save();
		}
	}
	*/

	private IEnumerator saveCoroutine()
	{
		while (true) {
			yield return new WaitForSeconds(5 * 60); // save every 5 min
			save();
		}
	}


	// saves everything except the chunks
	public void save()
	{
		InventoryScript.saveInventory(); // save inventory
		openChestScript.saveChests();
		openFurnaceScript.saveFurnaces(); // save furnaces
		armorScript.saveArmor(); // save which armor you have on

		int health = healthbarScript.getHealth();
		float hunger = hungerbarScript.getHunger();
		if (!dataService.saveData("health-and-hunger-bar.json", new float[] { health, hunger })) // save health bar and food bar
		{
			Debug.LogError("Could not save health and hunger bar file :(");
		}
		// save day time
		if (!dataService.saveData("day-time.json", dayProcessScript.getDataToSave()))
		{
			Debug.LogError("Could not save day time file :(");
		}

		// save player position
		if (!dataService.saveData("player-position.json", new float[] { steve.position.x, steve.position.y }))
		{
			Debug.LogError("Could not save player position :(");
		}
		// we'll save the data needed for world generation, which is the following:
		// [chunk, nextChunk, prevChunk, chunkLength, treeProgressRight, treeProgressLeft, treeHeightRight, treeHeightLeft, bottomPosRight, bottomPosLeft, spawningTreeTypeLeft, spawningTreeTypeRight]
		string chunk = scScript.spawnChunkStrategy?.biomeType;
		string nextChunk = scScript.nextSpawnChunkStrategy?.biomeType;
		string prevChunk = scScript.previousSpawnChunkStrategy?.biomeType;
		object[] biomeData = new object[]{ chunk, nextChunk, prevChunk, scScript.biomeLength };
		if(!dataService.saveData("world-data.json", biomeData.Concat(SpawnTreeScript.getTreeData()).ToArray()))
		{
			Debug.LogError("Could not save world data");
		}
	}
}
