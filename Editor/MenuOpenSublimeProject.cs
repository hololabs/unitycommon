using UnityEngine;
using System;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

using UnityEditor;
using System.IO;
using System.Diagnostics;

public class MenuOpenSublimeProject
{
	[MenuItem ("File/Open Sublime Project", false, 0)]
	static void OpenSublimeProject()
	{
		if(File.Exists("proj.sublime-project"))
			Process.Start("proj.sublime-project");
		else
			Debug.Log("No Sublime Project found!");
	}
}
