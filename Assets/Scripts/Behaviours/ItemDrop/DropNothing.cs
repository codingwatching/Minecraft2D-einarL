using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropNothing : ItemDropBehaviour
{

	public DropNothing()
	{

	}

	public override GameObject dropItem(string gameObjectaName, ToolInstance usingTool)
	{
		return null;
	}
}
