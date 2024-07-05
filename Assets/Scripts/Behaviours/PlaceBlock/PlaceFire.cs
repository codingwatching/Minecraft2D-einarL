using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceFire : PlaceBlockBehaviour
{
	public List<GameObject> placeBlock(GameObject blockToPlace, PlaceBlockScript pbScript, BreakBlockScript bbScript)
	{
		ContactFilter2D filter = new ContactFilter2D();
		filter.SetLayerMask(LayerMask.GetMask("BackBackground") | LayerMask.GetMask("FrontBackground") | LayerMask.GetMask("BackgroundVisual"));
		Debug.Log("place fire");
		// there is a block in the backgroundVisualLayer or background layer, then return null to place torch normally
		if (bbScript.isBlockBelowBlock(blockToPlace.transform.position)) // place fire normally
		{
			return null;
		}
		// place fire on wall
		else if (pbScript.checkIfBlockInPosition(filter) || pbScript.backgroundVisualTiles.HasTile(pbScript.backgroundVisualTiles.WorldToCell(blockToPlace.transform.position)))
		{
			return new List<GameObject>() { GameObject.Instantiate(BlockHashtable.getBlockByID(25), blockToPlace.transform.position, Quaternion.identity) };
		}
		
		return new List<GameObject>() { };
	}
}
