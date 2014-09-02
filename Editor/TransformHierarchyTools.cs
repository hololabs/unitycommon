using UnityEngine;
using System;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

using UnityEditor;

using System.Linq;
using System.Text;

using System.Reflection;

public static class TransformHierarchyTools
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

        copiedHierarchy = selection[0];
        Debug.Log("Copied " + copiedHierarchy.name + ".");
    }

    [MenuItem("Utility/Paste Hierarchy and Components Additively", false, 121)]
    static void PasteHierarchyAdditively()
    {
        var newSelection = Selection.GetTransforms(
            SelectionMode.TopLevel |
            SelectionMode.ExcludePrefab |
            SelectionMode.Editable
        );

        var report = new StringBuilder();
        report.AppendLine("Paste Hierarchy Additively Report:");

        var componentsToVerify = new List<Component>();

        RecursiveAdditivePaste(copiedHierarchy, newSelection[0], componentsToVerify, report);

        VerifyAndRewireComponents(copiedHierarchy, newSelection[0], componentsToVerify, report);

        Debug.Log(report.ToString());
    }

    [MenuItem("Utility/Paste Components Additively", false, 122)]
    static void PasteComponentsAdditively()
    {
        var newSelection = Selection.GetTransforms(
            SelectionMode.TopLevel |
            SelectionMode.ExcludePrefab |
            SelectionMode.Editable
        );

        var report = new StringBuilder();
        report.AppendLine("Paste Components Additively Report:");

        var componentsToVerify = new List<Component>();

        TransferComponents(copiedHierarchy, newSelection[0], componentsToVerify, report);

        VerifyAndRewireComponents(copiedHierarchy, newSelection[0], componentsToVerify, report);

        Debug.Log(report.ToString());
    }

    static void RecursiveAdditivePaste(Transform source, Transform destination, List<Component> componentsToVerify, StringBuilder report)
    {
        TransferComponents(source, destination, componentsToVerify, report);

        var sourceChildren = source.Cast<Transform>().OrderBy(c => c.name);
        var destinationChildren = destination.Cast<Transform>().OrderBy(c => c.name);

        foreach(var child in sourceChildren) {
            var destMatch = destinationChildren.FirstOrDefault(c => c.name == child.name);
            if(destMatch != null) {
                RecursiveAdditivePaste(child, destMatch, componentsToVerify, report);
            }
            else {
                var newChild = (Transform)Object.Instantiate(child, child.position, child.rotation);
                newChild.name = child.name;
                newChild.parent = destination.transform;
                newChild.localPosition = child.localPosition;
                newChild.localRotation = child.localRotation;
                newChild.localScale = child.localScale;

                Undo.RegisterCreatedObjectUndo(newChild.gameObject, "Paste Hierarchy Additively");
                report.AppendLine("Added " + newChild.name + " to " + destination.name + ";");

                var newComponentsToVerify = newChild.GetComponentsInChildren<Component>(true).Where(c => !(c is Transform)).ToArray();
                if(newComponentsToVerify.Any()) {
                    componentsToVerify.AddRange(newComponentsToVerify);
                    report.AppendLine();
                    report.Append("Queued components to verify: ");
                    foreach(var c in newComponentsToVerify) {
                        report.Append(c.GetType().Name + " on " + c.name + "; ");
                    }
                }
            }
        }
    }

    // Limitations:
    // - Only transfers one of each component type (ie. won't paste two instances of the same script onto the destination.)
    // - Will only transfer over public fields (ie. will miss non-public but serializable fields on scripts.)
    static void TransferComponents(Transform source, Transform destination, List<Component> componentsToVerify, StringBuilder report)
    {
        report.AppendLine("\nTransfering components from " + source.name + " to " + destination.name + ".");
        var sourceComponents = source.GetComponents<Component>().Where(c => !(c is Transform));
        var destComponents = destination.GetComponents<Component>().Where(c => !(c is Transform)).ToArray();

        foreach(var sc in sourceComponents) {
            var componentType = sc.GetType();
            var destMatch = destComponents.FirstOrDefault(dc => dc.GetType() == componentType);
            if(destMatch == null) {
                if(!UnityEditorInternal.ComponentUtility.CopyComponent(sc)) {
                    Debug.Log("Error copying component " + componentType.Name + " from " + sc.name);
                    continue;
                }
                if(!UnityEditorInternal.ComponentUtility.PasteComponentAsNew(destination.gameObject)) {
                    Debug.Log("Error pasting component " + componentType.Name + " to " + destination.name);
                    continue;
                }

                var newComponent = destination.GetComponent(componentType);

                //Pretty sure PasteComponentAsNew handles this itself.
                //Undo.RegisterCreatedObjectUndo(newComponent, "Paste Component Additively");

                report.AppendLine("\tCreated " + componentType.Name + " on " + destination.name + ";");

                componentsToVerify.Add(newComponent);
            }
        }
    }

    //We perform this as a separate step at the end of any paste because we only want to
    //verify components after all possible references have also been pasted to the destination.
    static void VerifyAndRewireComponents(Transform sourceRoot, Transform destinationRoot, IEnumerable<Component> components, StringBuilder report)
    {
        foreach(var c in components) {
            report.AppendLine("\nVerifying and rewiring fields on " + destinationRoot.name + "'s " + c.GetType().Name);

            bool componentDidChange;

            if(c is MonoBehaviour) {
                componentDidChange = RewireMonoBehaviours(sourceRoot, destinationRoot, report, c);
            }
            else {
                componentDidChange = RewireBuiltInComponents(sourceRoot, destinationRoot, report, c);
            }

            if(componentDidChange) {
                EditorUtility.SetDirty(c);
            }
        }
    }



    static bool RewireBuiltInComponents(Transform sourceRoot, Transform destinationRoot, StringBuilder report, Component component)
    {
        bool componentDidChange = false;

        var props = component.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(pi => pi.CanWrite && pi.CanRead)
            .Where(p => typeof(Component).IsAssignableFrom(p.PropertyType));
        

        foreach(var prop in props) {
            var propertyType = prop.PropertyType;
            if(typeof(Component).IsAssignableFrom(propertyType)) {
                var fieldValue = (Component)prop.GetGetMethod().Invoke(component, new object[] {});

                if(fieldValue.transform.root == sourceRoot) {
                    string fieldPath = GetTransformPath(fieldValue);

                    //We need to cut the first segment from the path, since it's the root
                    string truncatedFieldPath = fieldPath.Substring(fieldPath.IndexOf('/') + 1);

                    Transform newTargetTransform;
                    if(truncatedFieldPath == sourceRoot.name) {
                        //The component is on the root of the object, so let's use that.
                        newTargetTransform = destinationRoot;
                    }
                    else {
                        newTargetTransform = destinationRoot.Find(truncatedFieldPath);
                    }

                    if(newTargetTransform == null) {
                        report.AppendLine("\tTried to rewire component " + component.GetType().Name + "'s field '" + prop.Name + "' on " +
                                          component.name + " from " + fieldPath + ", but there was no corresponding transform at the destination.");
                        continue;
                    }
                    var newTargetComponent = newTargetTransform.GetComponent(propertyType);
                    if(newTargetComponent == null) {
                        report.AppendLine("\tTried to rewire component " + component.GetType().Name + "'s field '" + prop.Name + "' on " +
                                          component.name + " from " + fieldPath + ", but there was no corresponding component at the destination.");
                        continue;
                    }

                    //This one isn't necessary since we're already recording the component creation!
                    //Undo.RecordObject(component, "Rewire component values");

                    prop.GetSetMethod().Invoke(component, new object[] { newTargetComponent });
                    componentDidChange = true;

                    string newPath = fieldPath.Replace(sourceRoot.name, destinationRoot.name);
                    report.AppendLine("\tRewired component " + component.GetType().Name + "'s field '" + prop.Name + "' on " + component.name + " from " + fieldPath + " to " + newPath);
                }
                else {
                    report.AppendLine("\tIgnoring '" + prop.Name + "', since its root is " + fieldValue.transform.root + " instead of " + sourceRoot);
                }
            }
        }
        return componentDidChange;
    }

    static bool RewireMonoBehaviours(Transform sourceRoot, Transform destinationRoot, StringBuilder report, Component component)
    {
        bool componentDidChange = false;
        var fields = component.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach(var field in fields) {
            var fieldType = field.FieldType;
            if(typeof(Component).IsAssignableFrom(fieldType)) {
                var fieldValue = (Component)field.GetValue(component);

                if(fieldValue.transform.root == sourceRoot) {
                    string fieldPath = GetTransformPath(fieldValue);

                    //We need to cut the first segment from the path, since it's the root
                    string truncatedFieldPath = fieldPath.Substring(fieldPath.IndexOf('/') + 1);

                    Transform newTargetTransform;
                    if(truncatedFieldPath == sourceRoot.name) {
                        //The component is on the root of the object, so let's use that.
                        newTargetTransform = destinationRoot;
                    }
                    else {
                        newTargetTransform = destinationRoot.Find(truncatedFieldPath);
                    }

                    if(newTargetTransform == null) {
                        report.AppendLine("\tTried to rewire component " + component.GetType().Name + "'s field '" + field.Name + "' on " +
                                          component.name + " from " + fieldPath + ", but there was no corresponding transform at the destination.");
                        continue;
                    }
                    var newTargetComponent = newTargetTransform.GetComponent(fieldType);
                    if(newTargetComponent == null) {
                        report.AppendLine("\tTried to rewire component " + component.GetType().Name + "'s field '" + field.Name + "' on " +
                                          component.name + " from " + fieldPath + ", but there was no corresponding component at the destination.");
                        continue;
                    }

                    Undo.RecordObject(component, "Rewire component values");
                    field.SetValue(component, newTargetComponent);
                    componentDidChange = true;

                    string newPath = fieldPath.Replace(sourceRoot.name, destinationRoot.name);
                    report.AppendLine("\tRewired component " + component.GetType().Name + "'s field '" + field.Name + "' on " + component.name + " from " + fieldPath + " to " + newPath);
                }
                else {
                    report.AppendLine("\tIgnoring '" + field.Name + "', since its root is " + fieldValue.transform.root + " instead of " + sourceRoot);
                }
            }
        }
        return componentDidChange;
    }

    static string GetTransformPath(Component c)
    {
        var sb = new StringBuilder();

        var t = c.transform;
        sb.Append(t.name);

        while(t.parent != null) {
            t = t.parent;
            sb.Insert(0, t.name + "/");
        }
        return sb.ToString();
    }


    #region Menu verification methods
    [MenuItem("Utility/Copy Hierarchy", true, 120)]
    static bool CanCopyHierarchy()
    {
        var selection = Selection.GetTransforms(
            SelectionMode.TopLevel |
            SelectionMode.ExcludePrefab |
            SelectionMode.Editable
        );

        return selection.Length == 1;
    }

    [MenuItem("Utility/Paste Hierarchy and Components Additively", true, 121)]
    static bool CanPasteHierarchyAdditively()
    {
        return CanPaste();
    }

    [MenuItem("Utility/Paste Components Additively", true, 122)]
    static bool CanPasteComponentsAdditively()
    {
        return CanPaste();
    }

    static bool CanPaste()
    {
        if(copiedHierarchy == null) {
            return false;
        }

        var newSelection = Selection.GetTransforms(
            SelectionMode.TopLevel |
            SelectionMode.ExcludePrefab |
            SelectionMode.Editable
        );

        if(newSelection.Length != 1) {
            return false;
        }

        return true;
    }
    #endregion

}
