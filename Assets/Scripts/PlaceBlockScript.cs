using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Collections.AllocatorManager;


/**
 * 2D placing/breaking block mechanics:
 * Breaking blocks:
 * hold down left click to break blocks, doesn�t matter which layer theyre on
 * you will only be able to break background blocks that are not a part of the world generation (except maybe villages if we create those)
 * Placing blocks:
 * right click in order to place blocks on the foreground layer
 * hold right click in order to place blocks on the background layer
 */
public class PlaceBlockScript : MonoBehaviour
{
    private bool holdingItemIsPlaceable = false;
    private GameObject holdingItem; // the item that the player is holding
    private BreakBlockScript breakBlockScript;

    private GameObject hoverGrid = null; // this is the grid that gets placed where mouse is
    private Vector2 hoveringOverPosition; // the position that the mouse is hovering over, rounded to a potential block position
    private GameObject hoverTexture; // this is a texture for the hoverGrid
    private bool didPress = false; // true if the player pressed the right click button on the previous frame
    private bool canPlaceAgain = true; // this needed so that the player cant continue holding right click to continue placing more blocks
    private float holdRightClickTimer = 0f;
    private float holdDownThreshold = 0.2f; // hold right click for this many seconds to place on background layer
    
	// Start is called before the first frame update
	void Start()
    {
		breakBlockScript = GetComponent<BreakBlockScript>();
        hoverTexture = breakBlockScript.hoverTexture;
	}

    // Update is called once per frame
    void Update()
    {

        if (holdingItemIsPlaceable)
        {
            checkIfWeCanDisplayHoverTexture();
		}

        if(hoverGrid != null) // if we can place a block
        {
            if (Input.GetMouseButton(1) && canPlaceAgain)
            {
                didPress = true;
                holdRightClickTimer += Time.deltaTime;
                if(holdRightClickTimer > holdDownThreshold)
                {
                    didPress = false;
                    canPlaceAgain = false;
					holdRightClickTimer = 0;
                    // if the block to be placed is a NoFloatType && we can only place the float type block in the background layer
                    if (canPlaceInBackBackground())
                    {
						if (holdingItem.tag.Equals("NoFloatType"))
						{
							if (canPlaceNoFloatTypeInBackground()) placeBlockInBackground(); // place block in background layer
						}
						else
						{
							placeBlockInBackground(); // place block in background layer
						}
					}

				}
            }
            else if (didPress && Input.GetMouseButtonUp(1))
            {
                didPress = false;
                holdRightClickTimer = 0;
				// if the block to be placed is a NoFloatType && we can only place the float type block in the foreground
				if (holdingItem.tag.Equals("NoFloatType"))
                {
                    // returns true if the block below is in the default layer
					if (breakBlockScript.isBlockBelowBlock(holdingItem)) placeBlockInForeground(); // place block in foreground layer
                }
                else
                {
					placeBlockInForeground(); // place block in foreground layer
				}
                
            }
            else if(!Input.GetMouseButton(1))
            {
                canPlaceAgain = true;
            }
        }

    }

    /**
     * places the block that the player is holding on the foreground/Default layer
     */
    private void placeBlockInForeground()
    {
		GameObject placedBlock = Instantiate(holdingItem, hoveringOverPosition, Quaternion.identity); // place block
        InventoryScript.decrementSlot(InventoryScript.getSelectedSlot()); // remove the block from the inventory
		// here we need to check if placedBlock is a special type of block which goes on the FrontBackground Layer
		placedBlock.name = placedBlock.name.Replace("(Clone)", "").Trim(); // remove (Clone) from object name

		if (FrontBackgroundBlocks.isFrontBackgroundBlock(placedBlock.name)) // if its a "front background" block
		{
			placedBlock.layer = LayerMask.NameToLayer("FrontBackground");
		}

		// update the chunkData
		SpawningChunkData.updateChunkData(hoveringOverPosition.x, hoveringOverPosition.y, BlockHashtable.getIDByBlockName(placedBlock.name), LayerMask.LayerToName(placedBlock.layer)); 
		

		if (placedBlock.tag.Equals("FallType"))
        {
            // we need FallScript to execute its Start() function before we call fall()
            IEnumerator executeAfterStart(FallScript fallScript)
            {
                yield return null;
                fallScript.fall();
            }

            StartCoroutine(executeAfterStart(placedBlock.GetComponent<FallScript>()));
        }
        
        
	}

	/**
     * places the block that the player is holding on the BackBackground layer
     */
	private void placeBlockInBackground()
	{
		GameObject placedBlock = Instantiate(holdingItem, hoveringOverPosition, Quaternion.identity); // place block
		InventoryScript.decrementSlot(InventoryScript.getSelectedSlot()); // remove the block from the inventory
																		  
		placedBlock.name = placedBlock.name.Replace("(Clone)", "").Trim(); // remove (Clone) from object name
	    // here we need to check if placedBlock is a special type of block which goes on the FrontBackground Layer
		if (FrontBackgroundBlocks.isFrontBackgroundBlock(placedBlock.name)) // if its a "front background" block
		{
			placedBlock.layer = LayerMask.NameToLayer("FrontBackground");
		}
        else // else place on BackBackground layer
        {
			placedBlock.layer = LayerMask.NameToLayer("BackBackground");

            SpriteRenderer blockRenderer = placedBlock.GetComponent<SpriteRenderer>();
			blockRenderer.color = new Color(170f/255f, 170f / 255f, 170f / 255f); // dark tint
            blockRenderer.sortingOrder = -10;

		}

		// update the chunkData
		SpawningChunkData.updateChunkData(hoveringOverPosition.x, hoveringOverPosition.y, BlockHashtable.getIDByBlockName(placedBlock.name), LayerMask.LayerToName(placedBlock.layer));


		if (placedBlock.tag.Equals("FallType"))
		{
			placedBlock.GetComponent<FallScript>().fall();
		}

	}

	private void createHighlight(Vector2 position)
    {
        if (hoverGrid != null) return;
        Vector2 highlightPos = new Vector3(position.x + 0.205f, position.y + 0.33f);
        hoverGrid = Instantiate(hoverTexture, highlightPos, Quaternion.identity);
        hoveringOverPosition = position;

    }

    private void removeHighlight()
    {
        Destroy(hoverGrid);
        hoverGrid = null;
    }
	/**
     * the player can place in the BackBackground if the player can place the block in Default/foreground layer, and
     * there is not a BackBackground block already in this spot.
     * the function checkIfWeCanDisplayHoverTexture() checks if we can place a block on the Default/foreground layer,
     * so we should use that function and this function to check if the block can be placed on the FrontBackground layer
     * 
     * this function returns true if there is not a block in the BackBackground in the spot where the hoverGrid is, otherwise false
     */
	private bool canPlaceInBackBackground()
    {
        return !checkIfBlockInPosition(true, false);

	}

    /**
     * this gets called when the player is holding a different item in his hand
     * 
     * checks if the player is holding a placeable item by checking if the item exists in the folder "Blocks"
     * if its placeable, then we create a hoverTexture where the mouse is if we can place the block in that place.
     */
    public void checkIfHoldingPlaceableItem(string itemName)
    {
		holdingItem = Resources.Load<GameObject>("Prefabs\\Blocks\\" +  itemName);
        if (holdingItem == null) // didn't find the block, so the block isn't placeable
		{
            holdingItemIsPlaceable = false;
            removeHighlight();
			return; 
        }
        holdingItemIsPlaceable = true;
	}

    // checks if we can place the block on the Default/foreground layer, if so it calls createHighlight()
    private void checkIfWeCanDisplayHoverTexture()
    {
        if (InventoryScript.getIsInUI()) // if user is in UI, then we cant place a block
		{
            removeHighlight();
            return; 
        }
        Vector2 mousePos = breakBlockScript.getMousePosition();
        Vector2 RoundedMousePos = new Vector2((float)Math.Round(mousePos.x + 0.5f) - 0.5f, (float)Math.Round(mousePos.y + 0.5f) - 0.5f); // round it to the closes possible "block position"

        //if (RoundedMousePos == hoveringOverPosition && transform.position == prevPosition) return; // if have not moved mouse nor player, we know we dont need to create a new hoverTexture
		removeHighlight();
		holdingItem.transform.position = RoundedMousePos;
        // if mousePosition isnt behind another block and mousePos is within placing range
        if(breakBlockScript.isBlockReachable(holdingItem) && breakBlockScript.isBlockWithinRange(holdingItem.transform))
        {
            // lets check if there is already a block in this position; then we cant place it
            if (checkIfBlockInPosition()) return;
			

			if (checkIfPlaceable(holdingItem)) // if its placeable, then display the hoverTexture
            {
                // if its a no float type then there must be a block below in order to place it
                if (holdingItem.tag.Equals("NoFloatType"))
                {
                    if (isBlockBelow(holdingItem)) createHighlight(RoundedMousePos);
				}
				else createHighlight(RoundedMousePos);
            }
		}

    }

	/*
     * bool includeBackBackground: true if the block should not be placeable if there is a BackBackground block in this position
     * bool includeEntity: true if the block should not be placeable if there is an entity in this position
     * returns true if there is a block in this position with the corresponding layer, otherwise false
     */
	private bool checkIfBlockInPosition(bool includeBackBackground = false, bool includeEntity = true)
    {
		// Create a collision filter to only include colliders in the default layer
		ContactFilter2D filter = new ContactFilter2D();
		filter.SetLayerMask(LayerMask.GetMask("Default") | LayerMask.GetMask("Player"));

        if (includeBackBackground) filter.SetLayerMask(filter.layerMask | LayerMask.GetMask("BackBackground"));
		if (includeEntity) filter.SetLayerMask(filter.layerMask | LayerMask.GetMask("Entity"));

		// if the item is a "Front Background" type, then we cant place it if there already is a block in the FrontBackground in this spot
		if (FrontBackgroundBlocks.isFrontBackgroundBlock(holdingItem.name)) filter.SetLayerMask(filter.layerMask | LayerMask.GetMask("FrontBackground"));
		// Create a list to store the results
		List<Collider2D> results = new List<Collider2D>();

		// Check for overlaps
		Physics2D.OverlapCircle(holdingItem.transform.position, 0.45f, filter, results);

		foreach (Collider2D coll in results)
		{
			// if the collider isn't the inactiveBlock, then there is already a block here and we cant display the hover texture
			if (!ReferenceEquals(holdingItem, coll.gameObject)) return true;
		}
        return false;
	}


    // checks if the block is placeable in the cursor position
    private bool checkIfPlaceable(GameObject futureBlockPos)
    {
		SpriteRenderer blockRenderer = futureBlockPos.GetComponent<SpriteRenderer>();

		// if player is below the mousePosition
		if (breakBlockScript.isPlayerBelowBlock(blockRenderer))
		{
            if(breakBlockScript.isBlockAboveBlock(futureBlockPos, true)) return true;

			if (breakBlockScript.isPlayerOnRightSideOfBlock(blockRenderer)) // right side
			{
				if (breakBlockScript.isBlockOnLeftSideOfBlock(futureBlockPos, true)) return true;

			}
			else if (breakBlockScript.isPlayerOnLeftSideOfBlock(blockRenderer)) // left side
			{
				if (breakBlockScript.isBlockOnRightSideOfBlock(futureBlockPos, true)) return true;
            }
            else
            {
                if (breakBlockScript.isBlockOnRightSideOfBlock(futureBlockPos, true) || breakBlockScript.isBlockOnLeftSideOfBlock(futureBlockPos, true)) return true;
            }
		}

        // if player is above the mousePosition
        else if (breakBlockScript.isPlayerAboveBlock(blockRenderer))
        {
			if (breakBlockScript.isBlockBelowBlock(futureBlockPos, true)) return true;

			if (breakBlockScript.isPlayerOnRightSideOfBlock(blockRenderer)) // right side
            {
				if (breakBlockScript.isBlockOnLeftSideOfBlock(futureBlockPos, true)) return true;
			}
			else if (breakBlockScript.isPlayerOnLeftSideOfBlock(blockRenderer)) // left side
			{
				if (breakBlockScript.isBlockOnRightSideOfBlock(futureBlockPos, true)) return true;
			}
            else
            {
				if (breakBlockScript.isBlockOnRightSideOfBlock(futureBlockPos, true) || breakBlockScript.isBlockOnLeftSideOfBlock(futureBlockPos, true)) return true;
			}
		}
		else // if player head is on the same level as the mousePosition
		{
			if (breakBlockScript.isBlockAboveBlock(futureBlockPos, true) || breakBlockScript.isBlockBelowBlock(futureBlockPos, true)) return true;

			if (breakBlockScript.isPlayerOnRightSideOfBlock(blockRenderer))
			{
				if (breakBlockScript.isBlockOnLeftSideOfBlock(futureBlockPos, true)) return true;
			}
			else if (breakBlockScript.isPlayerOnLeftSideOfBlock(blockRenderer))
			{
				if (breakBlockScript.isBlockOnRightSideOfBlock(futureBlockPos, true)) return true;
			}
		}
        return false;
	}

    private bool isBlockBelow(GameObject futureBlockPos)
    {
        if (breakBlockScript.isBlockBelowBlock(futureBlockPos, true))
        {
            return true;
        }
        return false;
	}

    /**
     * returns true if one of these conditions is true:
     *   a) there is a block below which is in the BackBackground layer
     *   b) there is a block below which is in the Default layer && the block below is not a NoFloatType
     */
    private bool canPlaceNoFloatTypeInBackground()
    {
		Vector2 belowBlockPosition = new Vector2(holdingItem.transform.position.x, holdingItem.transform.position.y - holdingItem.GetComponent<SpriteRenderer>().bounds.size.y);
		
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.layerMask = LayerMask.GetMask("Default") | LayerMask.GetMask("BackBackground");
		contactFilter.useLayerMask = true;
		
        Collider2D[] result = new Collider2D[1];

		Physics2D.OverlapCircle(belowBlockPosition, 0.1f, contactFilter, result);

        if (LayerMask.LayerToName(result[0].gameObject.layer).Equals("BackBackground")) return true;
        if (LayerMask.LayerToName(result[0].gameObject.layer).Equals("Default") && !result[0].gameObject.tag.Equals("NoFloatType")) return true;
        return false;
	}

}
