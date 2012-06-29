using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

//Based on Pixelplacement's original script "Group"
public class GroupObjects
{	
	[MenuItem ("Utility/Group Selected %g", false, 1)]
	static void MakeGroup()
	{
		Undo.CreateSnapshot();
		
		//cache selected objects in scene:
		Transform[] selectedObjects = Selection.transforms;
		
		//early exit if nothing is selected:
		if(selectedObjects.Length == 0)
		{
			return;
		}
		
		Vector3 averagePosition = Vector3.zero;

		//parent construction and hierarchy structure decision:
		bool nestParent = true;
		Undo.RegisterSceneUndo("Create Group");
		Transform newGroupTransform = new GameObject("Group").transform; //naming convention mirrors Photoshop's
		Transform coreParent = selectedObjects[0].parent;
		foreach(Transform item in selectedObjects)
		{
			if(item.parent != coreParent)
			{
				nestParent = false;
			}
			averagePosition += item.position;
		}

		if(nestParent)
		{
			newGroupTransform.parent = coreParent;
			newGroupTransform.position = averagePosition / selectedObjects.Length;
		}
		else
		{
			//place group's pivot on the active transform in the scene:
			newGroupTransform.position = Selection.activeTransform.position;
		}
		
		//set selected objects as children of the group:
		foreach(Transform item in selectedObjects)
		{
			item.parent = newGroupTransform;
		}
	}	
}

