#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MissingScriptFinder : EditorWindow
{
    private List<string> missingScriptObjects = new List<string>();

    [MenuItem("Luzart/LuzartTool/Find Missing Scripts in Scene")]
    public static void ShowWindow()
    {
        GetWindow<MissingScriptFinder>("Missing Script Finder");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Find Missing Scripts"))
        {
            FindMissingScripts();
        }

        GUILayout.Label("GameObjects with Missing Scripts:", EditorStyles.boldLabel);
        foreach (var obj in missingScriptObjects)
        {
            GUILayout.Label(obj);
        }
    }

    private void FindMissingScripts()
    {
        missingScriptObjects.Clear();
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (var obj in allObjects)
        {
            if (obj.hideFlags == HideFlags.NotEditable || obj.hideFlags == HideFlags.HideAndDontSave)
                continue;

            if (PrefabUtility.GetPrefabInstanceStatus(obj) == PrefabInstanceStatus.Connected)
                continue;

            if (HasMissingScripts(obj))
            {
                missingScriptObjects.Add(GetFullPath(obj));
            }
        }

        Debug.Log("Missing script search complete.");
    }

    private bool HasMissingScripts(GameObject obj)
    {
        Component[] components = obj.GetComponents<Component>();
        foreach (var component in components)
        {
            if (component == null)
            {
                return true;
            }
        }
        return false;
    }

    private string GetFullPath(GameObject obj)
    {
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path;
    }
}
#endif