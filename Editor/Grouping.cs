using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

//Based on Pixelplacement's original script "Group"
public class Grouping
{	
	[MenuItem ("Utility/Group Selected %g", false, 1)]
	static void GroupSelected()
	{
		Undo.CreateSnapshot();
		
		//cache selected objects in scene:
		Transform[] selectedObjects = Selection.transforms;
		
		//early exit if nothing is selected:
		if(selectedObjects.Length == 0)
		{
			return;
		}
		

		Undo.RegisterSceneUndo("Group selected");
		
		Vector3 averagePosition = Vector3.zero;
		//parent construction and hierarchy structure decision:
		bool nestParent = true;
		Transform newGroupTransform = new GameObject("Group").transform;
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

	[MenuItem ("Utility/Ungroup Selected %#g", false, 2)]
	static void UngroupSelected()
	{
		Undo.CreateSnapshot();
		
		Transform activeSelection = Selection.activeTransform;

		//early out if there's nothing to do.
		if(activeSelection.childCount == 0)
		{
			Debug.Log("Ungroup " + activeSelection.name + ": selected object has nothing to ungroup!");
			return;
		}
		
		Undo.RegisterSceneUndo("Ungroup " + activeSelection.name);

		Transform selectedParent = activeSelection.parent;

		//We store each child in a list, since otherwise we'd mess with the
		//enumerable state while enumerating it!

		List<Transform> toUnparent = new List<Transform>();

		foreach(Transform child in activeSelection)
		{
			toUnparent.Add(child);
		}

		foreach(Transform child in toUnparent)
		{
			child.parent = selectedParent;
		}

		if(activeSelection.GetComponents<Component>().Length > 1)
		{
			Debug.Log("Ungroup " + activeSelection.name + ": not deleting the group parent because it has other components on it!\nDelete it manually if you need to.");
		}
		else
		{
			Object.DestroyImmediate(activeSelection.gameObject);
		}
	}

}

