using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
	void rightClick();

	void mineBlock(ToolInstance heldTool);
}
