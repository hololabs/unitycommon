using UnityEngine;
using UnityEditor;
 
public class DeselectAll
{
    [MenuItem ("Edit/Deselect All _a")]
    static void DoDeselect()
    {
        Selection.objects = new UnityEngine.Object[0];
    }
}