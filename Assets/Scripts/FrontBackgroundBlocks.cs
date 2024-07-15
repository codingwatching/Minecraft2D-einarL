using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FrontBackgroundBlocks
{
	// only blocks that you can place which need to go on the FrontBackground layer have to be here
	private static HashSet<string> frontLayerBlocks = new HashSet<string>()
	{
		{"SaplingOak"},
		{"Rose"},
		{"Dandelion"},
		{"Torch"},
		{"TorchWall"},
		{"TorchLeft"},
		{"TorchRight"},
		{"Tombstone"},
		{"Cactus"},
		{"DeadBush"},
		{"Grass"},
		{"MushroomBrown"},
		{"MushroomRed"},
		{"Ladder"},
		{"TulipOrange"},
		{"TulipPink"},
		{"TulipRed"},
		{"TulipWhite"}
	};

	// wallblocks are blocks that have to have a wall behind them, like ladders
	private static HashSet<string> wallBlocks = new HashSet<string>()
	{
		{"Ladder"}
	};

	// you can place blocks next to these blocks
	private static HashSet<string> frontLayerBlocksThatCanBePlacedNextTo = new HashSet<string>()
	{
		{"Tombstone"},
		{"Cactus"},
	};

	private static HashSet<string> fallTypes = new HashSet<string>()
	{
		{"Sand"},
		{"Gravel"}
	};

	private static HashSet<string> burnableBlocks = new HashSet<string>()
	{
		{"PlankOak"},
		{"LogOak"},
		{"LeavesOak"},
		{"PlankSpruce"},
		{"LogSpruce"},
		{"LeavesSpruce"},
		{"Wool"},
		{"PlankCherry"},
		{"LogCherry"},
		{"LeavesCherry"},
		{"TNT"},
	};

	private static HashSet<string> movable = new HashSet<string>()
	{
		{"Boat"}
	};

	public static bool isFallType(string blockName)
	{
		return fallTypes.Contains(blockName);
	}

	public static bool isWallBlock(string blockName)
	{
		return wallBlocks.Contains(blockName);
	}

	public static bool isFrontBackgroundBlock(string blockName)
	{
		return frontLayerBlocks.Contains(blockName);
	}

	public static bool isFrontBackgroundBlockPlaceableNextTo(string blockName)
	{
		return frontLayerBlocksThatCanBePlacedNextTo.Contains(blockName);
	}

	public static bool isBurnable(string blockName)
	{
		return burnableBlocks.Contains(blockName);
	}

	public static bool isMovable(string blockName)
	{
		return movable.Contains(blockName);
	}
}
