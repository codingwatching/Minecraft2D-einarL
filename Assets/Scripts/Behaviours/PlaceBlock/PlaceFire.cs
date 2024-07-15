using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class PlaceFire : PlaceBlockBehaviour
{
	private spawnChunkScript scScript = GameObject.Find("Main Camera").GetComponent<spawnChunkScript>();
	private Tilemap tilemap = GameObject.Find("Grid").transform.Find("Tilemap").GetComponent<Tilemap>();
	private AudioClip igniteSound = Resources.Load<AudioClip>("Sounds\\Random\\ignite");

	public List<GameObject> placeBlock(GameObject blockToPlace, PlaceBlockScript pbScript, BreakBlockScript bbScript)
	{
		if (isFireAtPosition(blockToPlace.transform.position)) return new List<GameObject>() { };

		GameObject blockBelow = getBlock(new Vector2(blockToPlace.transform.position.x, blockToPlace.transform.position.y - 1f));
		GameObject hoveredBlock = getBlock(blockToPlace.transform.position);
		if(hoveredBlock != null && hoveredBlock.name.Equals("TNT"))
		{
			hoveredBlock.GetComponent<TNTScript>().ignite();
			return new List<GameObject>() { };
		}
		GameObject block = hoveredBlock == null ? blockBelow : hoveredBlock;
		if (block != null)
		{
			
			AudioSource.PlayClipAtPoint(igniteSound, blockToPlace.transform.position);

			GameObject fireInstance = GameObject.Instantiate(BlockHashtable.getBlockByID(66), blockToPlace.transform.position, Quaternion.identity);
			fireInstance.transform.parent = block.transform;
			if (!SpawningChunkData.lightingEnabled) fireInstance.transform.Find("Light 2D").GetComponent<Light2D>().enabled = false;

			ToolInstance flintAndSteelTool = InventoryScript.getHeldTool();
			if (flintAndSteelTool != null)
			{
				flintAndSteelTool.reduceDurability();
			}
			else Debug.LogError("Flint and steel tool instance is null");

			SpawningChunkData.addOrRemoveFireBlock(blockToPlace.transform.position.x, blockToPlace.transform.position.y, hoveredBlock == null ? 1 : 0);
		}
		
		return new List<GameObject>() { };
	}


	private GameObject getBlock(Vector2 blockPos, bool firstTry = true)
	{
		ContactFilter2D filter = new ContactFilter2D();
		filter.SetLayerMask(LayerMask.GetMask("Default") | LayerMask.GetMask("FrontBackground") | LayerMask.GetMask("BackBackground"));

		// Create a list to store the results
		List<Collider2D> results = new List<Collider2D>();

		// Check for overlaps
		Physics2D.OverlapCircle(blockPos, 0.45f, filter, results);

		GameObject objToReturn = null;
		foreach (Collider2D collider in results) // return the frontmost block
		{
			if (collider.gameObject.name == "TNT") return collider.gameObject;
			if (collider.gameObject.layer == LayerMask.NameToLayer("FrontBackground")) objToReturn = collider.gameObject;
			if (collider.gameObject.layer == LayerMask.NameToLayer("Default") && (objToReturn == null || objToReturn.gameObject.layer == LayerMask.NameToLayer("BackBackground"))) objToReturn = collider.gameObject;
			else if (objToReturn == null && collider.gameObject.layer == LayerMask.NameToLayer("BackBackground")) objToReturn = collider.gameObject;
		}
		if (objToReturn != null) return objToReturn;
		if (!firstTry) return null;
		// if we reach here, then there is no gameobject at this position (maybe a tile though)
		Vector3Int tilePos = tilemap.WorldToCell(blockPos);
		TileBase tile = tilemap.GetTile(tilePos);
		if (tile != null)
		{
			scScript.spawnGameObjectInsteadOfTile(tile, tilePos); // place gameObject at tiles position
			tilemap.SetTile(tilePos, null); // remove tile
			return getBlock(blockPos, false);
		}
		return null;
	}

	private bool isFireAtPosition(Vector2 blockPos)
	{
		return Physics2D.OverlapCircle(blockPos, 0.0001f, LayerMask.GetMask("Fire"));
	}
}
