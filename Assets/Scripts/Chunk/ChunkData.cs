using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChunkData
{
	private int[,] chunkData;
	private List<float[]> frontBackgroundBlocks = new List<float[]>(); // list of type {[x,y, blockID]}
	private List<float[]> backBackgroundBlocks = new List<float[]>(); // list of type {[x,y, blockID]}
	private List<float[]> backgroundVisualBlocks = new List<float[]>(); // list of type {[x,y, blockID]}
	private List<float[]> fireBlocks = new List<float[]>(); // list of type [[x,y,a], [x,y,a], ...], a is 0 if its attached to the block at the same position, 1 if its attatched to the block below it
	private int chunkPosition;
	private float startHeight = -2.5f; // height of the chunk (height of grass block that is in the last vertical line in the chunk)
									   // we use this variable to find what the height of the first vertical line in the next chunk should be
	// need to also have here the variables dontGoUpRight, dontGoUpLeft, etc.
	private List<object[]> entities; // list of entities in this chunk // of type {[x, y, entityName], [x, y, entityName], ...}
	private string biome;

	// heights of the vertical lines (i.e. the height of the grass blocks)
	// if this is a right chunk, then the leftmost vertical line height would be at index 0
	// but with a left chunk the leftmost vertical line is at index 9
	private float[] verticalLineHeights; 


	// keeps track of where ores spawned, this is needed for a smooth transition for ores in the next chunk
	private Hashtable prevOreSpawns = new Hashtable(); // (y, blockID) 

	public ChunkData(int chunkPosition, int[,] chunkData, float startHeight, Hashtable prevOreSpawns, List<float[]> frontBackgroundBlocks, List<object[]> entities, float[] vLineHeights, List<float[]> backgroundVisualBlocks, string biome)
	{
		this.chunkData = chunkData;
		this.chunkPosition = chunkPosition;
		this.startHeight = startHeight;
		this.prevOreSpawns = prevOreSpawns;
		this.frontBackgroundBlocks = frontBackgroundBlocks;
		this.entities = entities;
		verticalLineHeights = vLineHeights;
		this.backgroundVisualBlocks = backgroundVisualBlocks;
		this.biome = biome;
	}
	
	public void changeBlock(float x, float y, int newBlockID, string layer = "Default")
	{
		if (layer.Equals("Default") || layer.Equals("Water"))
		{
			chunkData[(int)x, (int)y] = newBlockID;
		}
		else if (layer.Equals("FrontBackground"))
		{
			if (newBlockID == 0) // if we're removing a block
			{
				removeFrontBackgroundBlockByPosition(x, y);
			}
			else frontBackgroundBlocks.Add(new float[] { x, y, newBlockID });
		}
		else if (layer.Equals("BackBackground"))
		{
			if (newBlockID == 0) // if we're removing a block
			{
				removeBackBackgroundBlockByPosition(x, y);
			}
			else backBackgroundBlocks.Add(new float[] { x, y, newBlockID });
		}
		else Debug.LogError("The layer you're trying to access doesnt exist. layer: " + layer);
		
	}
	// fireAttachement is 1 if its attatched to the block below it, but 0 if its attached to the block at the same position
	public void addFireBlock(float x, float y, int fireAttachement)
	{
		fireBlocks.Add(new float[] { x, y, fireAttachement });
	}
	public void removeFireBlock(float x, float y, int fireAttachment)
	{
		fireBlocks.RemoveAll(block => block[0] == x && block[1] == y);
	}


	private void removeBackBackgroundBlockByPosition(float x, float y)
	{
		for (int i = 0; i < backBackgroundBlocks.Count; i++)
		{
			if (backBackgroundBlocks[i][0] == x && backBackgroundBlocks[i][1] == y)
			{
				backBackgroundBlocks.RemoveAt(i);
				return;
			}
		}
		Debug.LogError("Did not find a block in BackBackground layer to remove at: " + x + ", " + y);
	}

	private void removeFrontBackgroundBlockByPosition(float x, float y)
	{
		for (int i = 0; i < frontBackgroundBlocks.Count; i++)
		{
			if (frontBackgroundBlocks[i][0] == x && frontBackgroundBlocks[i][1] == y)
			{
				frontBackgroundBlocks.RemoveAt(i);
				return;
			}
		}
		Debug.LogError("Did not find a block in FrontBackground layer to remove at: " + x + ", " + y);
	}
	// returns true if it added the block
	public bool addBackgroundVisualBlock(float x, float y, int blockID)
	{
		foreach (float[] block in backgroundVisualBlocks)
		{
			if (block[0] == x && block[1] == y) return false; // if there already is a background visual block at this position, then dont add a new one
		}
		backgroundVisualBlocks.Add(new float[] {x,y,blockID});
		return true;
	}

	public float getVerticalLineHeight(int index)
	{
		return verticalLineHeights[index];
	}

	public float[] getVerticalLineHeights()
	{
		return verticalLineHeights;
	}

	public List<object[]> getEntities()
	{
		return entities;
	}

	public void setEntities(List<object[]> entities)
	{
		this.entities = entities;
	}

	public List<float[]> getFrontBackgroundBlocks() 
	{
		return frontBackgroundBlocks;
	}

	public List<float[]> getBackBackgroundBlocks()
	{
		return backBackgroundBlocks;
	}

	public List<float[]> getBackgroundVisualBlocks()
	{
		return backgroundVisualBlocks;
	}

	public List<float[]> getFireBlocks()
	{
		return fireBlocks;
	}

	public int[,] getChunkData()
	{
		return chunkData;
	}

	public int getChunkPosition()
	{
		return chunkPosition;
	}

	public float getStartHeight()
	{
		return startHeight;
	}

	public Hashtable getPrevOreSpawns()
	{
		return prevOreSpawns;
	}

	public string getBiome()
	{
		return biome;
	}
}
