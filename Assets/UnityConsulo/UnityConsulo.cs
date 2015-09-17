using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class UnityConsulo : MonoBehaviour
{
    [MenuItem("Edit/Use Consulo", true)]
    static bool ValidateUncheckConsulo()
    {
        bool state = UseConsulo();
        Menu.SetChecked("Edit/Use Consulo", state);
        return IsOnAMac();
    }

    [MenuItem("Edit/Use Consulo")]
    static void UncheckConsulo()
    {
        bool state = UseConsulo();
        Menu.SetChecked("Edit/Use Consulo", !state);
        EditorPrefs.SetBool("UseConsulo", !state);
    }

    [MenuItem("File/Open Project in Consulo")]
    static void OpenProjectInConsulo()
    {
        UnityEngine.Debug.Log(ProjectPath());
		CallConsulo("-n -b \"org.mustbe.consulo\" --args --line 0 \"" + ProjectPath() + "\"");
    }

    [UnityEditor.Callbacks.OnOpenAssetAttribute()]
    static bool OnOpenedAssetCallback(int instanceID, int line)
    {
        // bail out if we are not on a Mac or if we don't want to use Consulo
        if (!IsOnAMac() || !UseConsulo())
        {
            return false;
        }

        // current path without the asset folder
        string appPath = ProjectPath();

        // determine asset that has been double clicked in the project view
        UnityEngine.Object selected = EditorUtility.InstanceIDToObject(instanceID);

        // only recognize c# files
        if (selected.GetType().ToString() == "UnityEditor.MonoScript")
        {
            // determine the complete absolute path to the asset file
            string completeFilepath = appPath + "/" + AssetDatabase.GetAssetPath(selected);

            string args = null;
            if (line == -1)
            {
                args = "-n -b \"org.mustbe.consulo\" --args --line 0 \"" + completeFilepath + "\"";
            }
            else
            {
                args = "-n -b \"org.mustbe.consulo\" --args --line " + line.ToString() + " \"" + completeFilepath + "\"";
            }

            // call 'open'
            CallConsulo(args);

            return true;
        }

        // let unity open other assets with other apps.
        return false;
    }

    static string ProjectPath()
    {
        return System.IO.Path.GetDirectoryName(Application.dataPath);
    }

    static void CallConsulo(string args)
    {
        Process proc = new Process();
        proc.StartInfo.FileName = "open";
        proc.StartInfo.Arguments = args;
        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.RedirectStandardOutput = true;
        proc.Start();

		BringConsuloToFront();
    }

	static void BringConsuloToFront()
	{
		Process proc = new Process();
		proc.StartInfo.FileName = "open";
		proc.StartInfo.Arguments = "-b \"org.mustbe.consulo\"";
		proc.StartInfo.UseShellExecute = false;
		proc.StartInfo.RedirectStandardOutput = true;
		proc.Start();
	}

    static bool IsOnAMac()
    {
        return (Application.platform == RuntimePlatform.OSXEditor);
    }

    static bool UseConsulo()
    {
        // if this is the first start we will enable Consulo by default
        if (!EditorPrefs.HasKey("UseConsulo"))
        {
            EditorPrefs.SetBool("UseConsulo", true);
            return true;
        }

        return EditorPrefs.GetBool("UseConsulo");
    }
}