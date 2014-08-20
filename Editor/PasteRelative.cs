using UnityEngine;
using System;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

using UnityEditor;
using System.Linq;

public static class PasteRelative
{
    static Transform copiedHierarchy;

    [MenuItem("Utility/Copy Hierarchy", false, 120)]
    static void CopyHierarchy()
    {
        var selection = Selection.GetTransforms(
            SelectionMode.TopLevel |
            SelectionMode.ExcludePrefab |
            SelectionMode.Editable
        );

        if(selection.Length == 1) {
            copiedHierarchy = selection[0];
            Debug.Log("Copied: " + copiedHierarchy.name);
        }
        else {
            Debug.LogWarning("Didn't copy! Invalid selection.");
        }

    }

    [MenuItem("Utility/Paste Hierarchy Additively", false, 121)]
    static void PasteHierarchyAdditively()
    {
        Debug.Log("Pasting: " + copiedHierarchy.name);

        if(copiedHierarchy == null) {
            Debug.Log("Nothing to paste!");
            return;
        }

        var newSelection = Selection.GetTransforms(
            SelectionMode.TopLevel |
            SelectionMode.ExcludePrefab |
            SelectionMode.Editable
        );

        if(newSelection.Length != 1 || newSelection[0].name != copiedHierarchy.name) {
            Debug.LogWarning("Not pasting! Invalid target.");
            return;
        }

        RecursiveAdditivePaste(copiedHierarchy, newSelection[0]);

        //This happens automatically, no need to collapse it
        //Undo.CollapseUndoOperations(undoGroup - 1);
    }

    static void RecursiveAdditivePaste(Transform source, Transform destination)
    {
        var sourceChildren = source.Cast<Transform>().OrderBy(c => c.name);
        var destinationChildren = destination.Cast<Transform>().OrderBy(c => c.name);

        foreach(var child in sourceChildren) {
            var destMatch = destinationChildren.FirstOrDefault(c => c.name == child.name);
            if(destMatch != null) {
                //We don't need to care about components now
                //var sourceComponents = child.GetComponents<Component>().Where(c => !(c is Transform));
                //var destComponents = destMatch.GetComponents<Component>().Where(c => !(c is Transform));

                RecursiveAdditivePaste(child, destMatch);
            }
            else {
                var newChild = (Transform)Object.Instantiate(child, child.position, child.rotation);
                newChild.name = child.name;
                newChild.parent = source.transform;
                var localPos = newChild.localPosition;
                var localRot = newChild.localRotation;
                newChild.parent = destination.transform;
                newChild.localPosition = localPos;
                newChild.localRotation = localRot;

                //FIXME: there's just a lot of float drift here, but probably isn't actually what we want...
                newChild.localScale = Vector3.one;

                Undo.RegisterCreatedObjectUndo(newChild.gameObject, "Paste Additively");
                Debug.Log("Added " + newChild.name + " to " + destination.name + "!");
            }
        }
    }
}
