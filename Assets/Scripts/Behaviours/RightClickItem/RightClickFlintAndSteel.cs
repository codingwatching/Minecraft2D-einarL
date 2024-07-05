using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RightClickFlintAndSteel : RightClickItemBehaviour
{
	private Animator anim;
	private Tilemap tilemap;
	private spawnChunkScript scScript;
	private GameObject firePrefab;

	public RightClickFlintAndSteel()
	{
		anim = GameObject.Find("SteveContainer").transform.Find("Steve").GetComponent<Animator>();
		tilemap = GameObject.Find("Grid").transform.Find("Tilemap").GetComponent<Tilemap>();
		scScript = GameObject.Find("Main Camera").GetComponent<spawnChunkScript>();
		firePrefab = Resources.Load<GameObject>("Prefabs\\Blocks\\Fire");
	}
	// CHANGE IT SO IT WILL BE LIKE WATERBUCKET IS IMPLEMENTED.
	public override void rightClickItem()
	{
		doPlaceAnimation();

		if (isBlockBelow())
		{
			if (isFireAtPosition()) return;
			Vector2 mousePos = getRoundedMousePosition();
			GameObject.Instantiate(firePrefab, mousePos, Quaternion.identity);
			SpawningChunkData.updateChunkData(mousePos.x, mousePos.y, 66);
		}
		else
		{
			GameObject blockAtPosition = getHoveredBlock(); // the block at the mouse position
		}



		ToolInstance flintAndSteelTool = InventoryScript.getHeldTool();
		if (flintAndSteelTool != null)
		{
			flintAndSteelTool.reduceDurability();
		}
		else Debug.LogError("Flint and steel tool instance is null");
	}

	private bool isBlockBelow()
	{
		Vector2 mousePos = getRoundedMousePosition();
		return Physics2D.OverlapCircle(new Vector2(mousePos.x, mousePos.y - 1f), 0.0001f, LayerMask.GetMask("Default") | LayerMask.GetMask("FrontBackground") | LayerMask.GetMask("BackBackground") | LayerMask.GetMask("Tilemap"));
	}

	private bool isFireAtPosition()
	{
		return Physics2D.OverlapCircle(getRoundedMousePosition(), 0.0001f, LayerMask.GetMask("Fire"));
	}

	private GameObject getHoveredBlock(bool firstTry = true)
	{
		Vector2 mousePos = getRoundedMousePosition();

		ContactFilter2D filter = new ContactFilter2D();
		filter.SetLayerMask(LayerMask.GetMask("Default") | LayerMask.GetMask("FrontBackground") | LayerMask.GetMask("BackBackground"));

		// Create a list to store the results
		List<Collider2D> results = new List<Collider2D>();

		// Check for overlaps
		Physics2D.OverlapCircle(mousePos, 0.0001f, filter, results);

		GameObject objToReturn = null;
		foreach (Collider2D collider in results) // return the frontmost block
		{
			if (collider.gameObject.layer == LayerMask.NameToLayer("FrontBackground")) return collider.gameObject;
			if (collider.gameObject.layer == LayerMask.NameToLayer("Default")) objToReturn = collider.gameObject;
			else if (objToReturn == null && collider.gameObject.layer == LayerMask.NameToLayer("BackBackground")) objToReturn = collider.gameObject;
		}
		if (objToReturn != null) return objToReturn;
		if (!firstTry) return null;
		// if we reach here, then there is no gameobject at this position (maybe a tile though)
		Vector3Int tilePos = tilemap.WorldToCell(mousePos);
		TileBase tile = tilemap.GetTile(tilePos);
		if(tile != null)
		{
			scScript.spawnGameObjectInsteadOfTile(tile, tilePos); // place gameObject at tiles position
			tilemap.SetTile(tilePos, null); // remove tile
			return getHoveredBlock(false);
		}
		return null;
	}



	private void doPlaceAnimation()
	{
		bool facingRight = anim.GetBool("isFacingRight");

		if (facingRight) anim.Play("fightFrontArm");
		else anim.Play("fightBackArm");
	}



	// this function should be empty:
	public override void stopHoldingRightClick(bool executeDefaultBehaviour = true)
	{

	}
}
