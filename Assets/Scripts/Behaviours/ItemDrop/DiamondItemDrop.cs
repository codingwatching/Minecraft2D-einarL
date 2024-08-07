using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondItemDrop : ItemDropBehaviour
{
	public override List<GameObject> dropItem(string gameObjectName, ToolInstance usingTool, Vector2 blockPosition = default)
	{
		// if pickaxe && not wood && not stone
		if (usingTool != null && usingTool.getToolType().Equals(ToolType.Pickaxe) && !usingTool.getToolMaterial().Equals(ToolMaterial.Wood) && !usingTool.getToolMaterial().Equals(ToolMaterial.Stone))
		{
			return base.dropItem("Diamond", usingTool);
		}
		return null;
	}
}
