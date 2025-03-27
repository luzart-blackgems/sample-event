using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Sirenix.Serialization;

#if UNITY_EDITOR
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
#endif

public class PrefPrefix
{
    public const string Prefix = "PP.";
    public const string Bool = Prefix + "Bool.";
    public const string Long = Prefix + "Long.";
    public const string LongLowBit = Long + "LowBits.";
    public const string LongHighBit = Long + "HighBits.";
    public const string Vector2 = Prefix + "Vector2.";
}

public class PlayerPrefs2
{
    public static Type[] SupportedTypes = new Type[] {
        typeof(float),
        typeof(int),
        typeof(string),
        typeof(bool),
        typeof(long),
        typeof(Vector2)
    };

    public static T Get<T>(string key)
    {
        if (typeof(T) == typeof(float)) return (T)(object)GetFloat(key);
        if (typeof(T) == typeof(int)) return (T)(object)GetInt(key);
        if (typeof(T) == typeof(string)) return (T)(object)GetString(key);
        if (typeof(T) == typeof(bool)) return (T)(object)GetBool(key);
        if (typeof(T) == typeof(long)) return (T)(object)GetLong(key);
        if (typeof(T) == typeof(Vector2)) return (T)(object)GetVector2(key);
        return default(T);
    }

    public static void Set<T>(string key, T value)
    {
        if (typeof(T) == typeof(float)) SetFloat(key, (float)(object)value);
        if (typeof(T) == typeof(int)) SetInt(key, (int)(object)value);
        if (typeof(T) == typeof(string)) SetString(key, (string)(object)value);
        if (typeof(T) == typeof(bool)) SetBool(key, (bool)(object)value);
        if (typeof(T) == typeof(long)) SetLong(key, (long)(object)value);
        if (typeof(T) == typeof(Vector2)) SetVector2(key, (Vector2)(object)value);
    }

    public static void Delete<T>(string key)
    {
        if (typeof(T) == typeof(float) || typeof(T) == typeof(int) || typeof(T) == typeof(string))
        {
            PlayerPrefs.DeleteKey(key);
        }
        if (typeof(T) == typeof(bool)) DeleteBool(key);
        if (typeof(T) == typeof(long)) DeleteLong(key);
        if (typeof(T) == typeof(Vector2)) DeleteVector2(key);
    }

    // String
    public static string GetString(string key)
    {
        return PlayerPrefs.GetString(key);
    }
    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    // Int
    public static int GetInt(string key)
    {
        return PlayerPrefs.GetInt(key);
    }
    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    // Float
    public static float GetFloat(string key)
    {
        return PlayerPrefs.GetFloat(key);
    }
    public static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }

    // Bool
    public static bool GetBool(string key)
    {
        return (PlayerPrefs.GetInt(PrefPrefix.Bool + key, 0) == 1);
    }
    public static void SetBool(string key, bool value)
    {
        PlayerPrefs.SetInt(PrefPrefix.Bool + key, value ? 1 : 0);
    }
    public static void DeleteBool(string key)
    {
        PlayerPrefs.DeleteKey(PrefPrefix.Bool + key);
    }

    // Long
    public static long GetLong(string key)
    {
        int lowBits, highBits;
        SplitLong(default(long), out lowBits, out highBits);
        lowBits = PlayerPrefs.GetInt(PrefPrefix.LongLowBit + key, lowBits);
        highBits = PlayerPrefs.GetInt(PrefPrefix.LongHighBit + key, highBits);
        // unsigned, to prevent loss of sign bit.
        ulong ret = (uint)highBits;
        ret = (ret << 32);
        return (long)(ret | (ulong)(uint)lowBits);
    }
    public static void SetLong(string key, long value)
    {
        int lowBits, highBits;
        SplitLong(value, out lowBits, out highBits);
        PlayerPrefs.SetInt(PrefPrefix.LongLowBit + key, lowBits);
        PlayerPrefs.SetInt(PrefPrefix.LongHighBit + key, highBits);
    }
    public static void DeleteLong(string key)
    {
        PlayerPrefs.DeleteKey(PrefPrefix.LongLowBit + key);
        PlayerPrefs.DeleteKey(PrefPrefix.LongHighBit + key);
    }
    public static void SplitLong(long input, out int lowBits, out int highBits)
    {
        // unsigned everything, to prevent loss of sign bit.
        lowBits = (int)(uint)(ulong)input;
        highBits = (int)(uint)(input >> 32);
    }

    // Vector2
    static Vector2 GetVector2(String key)
    {
        byte[] data = Encoding.UTF8.GetBytes(GetString(PrefPrefix.Vector2 + key));
        return SerializationUtility.DeserializeValue<Vector2>(data, DataFormat.JSON);
    }
    public static void SetVector2(String key, Vector2 value)
    {

        string data = Encoding.UTF8.GetString(SerializationUtility.SerializeValue<Vector2>(value, DataFormat.JSON));
        SetString(PrefPrefix.Vector2 + key, data);
    }
    public static void DeleteVector2(string key)
    {
        PlayerPrefs.DeleteKey(PrefPrefix.Vector2 + key);
    }


    // Add more types here and expose them in the generic Get/Set methods
}

#if UNITY_EDITOR

public interface IPrefsEntry
{
    string Key { get; }
}

public class PrefsEntry<T> : IPrefsEntry, IComparable
{
    [TableColumnWidth(80)]
    [ShowInInspector, ReadOnly, DisplayAsString, PropertyOrder(-1)]
    private string ValueType
    {
        get { return typeof(T).Name; }
    }

    private string m_Key;
    [ShowInInspector]
    public string Key
    {
        get { return m_Key; }
        set
        {
            PlayerPrefs2.Delete<T>(m_Key);
            m_Key = value;
            PlayerPrefs2.Set<T>(m_Key, m_Value);
        }
    }

    private T m_Value;
    [ShowInInspector]
    public T Value
    {
        get { return m_Value; }
        set
        {
            m_Value = value;
            PlayerPrefs2.Set<T>(m_Key, m_Value);
        }
    }

    [TableColumnWidth(60)]
    [Button]
    public void Delete()
    {
        PlayerPrefs2.Delete<T>(m_Key);
        PlayerPrefsEditorWindow.Refresh();
    }

    public PrefsEntry(string key)
    {
        m_Key = key;
        m_Value = PlayerPrefs2.Get<T>(key);
    }

    public int CompareTo(object obj)
    {
        if (obj == null) return 1;

        IPrefsEntry other = obj as IPrefsEntry;
        if (other != null)
            return m_Key.CompareTo(other.Key);
        else
            throw new ArgumentException("Object is not a IPrefsEntry!");
    }
}

public class PlayerPrefsEditorWindow : OdinEditorWindow
{
    [TableListAttribute(IsReadOnly = true), HideReferenceObjectPicker]
    public List<IPrefsEntry> Prefs;

    private static PlayerPrefsEditorWindow m_Window;

    [UnityEditor.MenuItem("Window/PlayerPrefsEditor")]
    static void Init()
    {
        m_Window = GetWindow<PlayerPrefsEditorWindow>();
        m_Window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
        m_Window.titleContent = new GUIContent("Player Prefs");
        m_Window.LoadCurrentPrefs();
        m_Window.Repaint();
    }

    public static void Refresh()
    {
        m_Window.LoadCurrentPrefs();
    }

    private void LoadCurrentPrefs()
    {
        string key;
        Prefs = new List<IPrefsEntry>();
        List<string> CurrentKeys = new List<string>();
        m_Window.GetAllPlayerPrefKeys(ref CurrentKeys);
        for (int i = 0; i < CurrentKeys.Count; i++)
        {
            CreatePrefEntry(CurrentKeys[i]);
        }
        Prefs.Sort();
    }

    private void CreatePrefEntry(string key)
    {
        if (key.StartsWith(PrefPrefix.Prefix))
        {
            if (key.StartsWith(PrefPrefix.Bool))
            {
                Prefs.Add(new PrefsEntry<bool>(key.Replace(PrefPrefix.Bool, "")));
            }
            if (key.StartsWith(PrefPrefix.Long))
            {
                if (key.StartsWith(PrefPrefix.LongLowBit)) return;
                Prefs.Add(new PrefsEntry<long>(key.Replace(PrefPrefix.LongHighBit, "")));
            }
            if (key.StartsWith(PrefPrefix.Vector2))
            {
                Prefs.Add(new PrefsEntry<Vector2>(key.Replace(PrefPrefix.Vector2, "")));
            }
        }
        else
        {
            if (PlayerPrefs.GetString(key, "PP.XXX_DEFAULT_XXX.PP") != "PP.XXX_DEFAULT_XXX.PP")
            {
                Prefs.Add(new PrefsEntry<string>(key));
            }
            // Not sure how better to do this?
            else if (PlayerPrefs.GetFloat(key, float.MaxValue) != float.MaxValue)
            {
                Prefs.Add(new PrefsEntry<float>(key));
            }
            else if (PlayerPrefs.GetInt(key, int.MaxValue) != int.MaxValue)
            {
                Prefs.Add(new PrefsEntry<int>(key));
            }
        }
    }

    private void AddPref(Type type)
    {
        MethodInfo method = typeof(PlayerPrefs2).GetMethod("Set");
        MethodInfo genericMethod = method.MakeGenericMethod(type);
        genericMethod.Invoke(null, new object[] { "NewPrefEntry", Activator.CreateInstance(type) });
        LoadCurrentPrefs();
    }

    private void GetAllPlayerPrefKeys(ref List<string> keys)
    {
        if (keys != null)
        {
            keys.Clear();
        }
        else
        {
            keys = new List<string>();
        }

        string regKeyPathPattern = @"Software\Unity\UnityEditor\{0}\{1}";


        string regKeyPath = string.Format(regKeyPathPattern, UnityEditor.PlayerSettings.companyName, UnityEditor.PlayerSettings.productName);
        Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(regKeyPath);
        if (regKey == null)
        {
            return;
        }

        string[] playerPrefKeys = regKey.GetValueNames();
        for (int i = 0; i < playerPrefKeys.Length; i++)
        {
            string playerPrefKey = playerPrefKeys[i];

            // Remove the _hXXXXX suffix
            playerPrefKey = playerPrefKey.Substring(0, playerPrefKey.LastIndexOf("_h"));

            keys.Add(playerPrefKey);
        }
    }

    protected override void OnBeginDrawEditors()
    {
        SirenixEditorGUI.BeginHorizontalToolbar(25);
        {
            if (SirenixEditorGUI.ToolbarButton(new GUIContent("Refresh")))
            {
                LoadCurrentPrefs();
            }
            if (SirenixEditorGUI.ToolbarButton(new GUIContent("Add New Pref")))
            {
                TypeSelector selector = new TypeSelector(PlayerPrefs2.SupportedTypes, false);
                selector.SelectionTree.EnumerateTree((x) => { x.Toggled = true; });
                selector.SelectionConfirmed += (selection) => { AddPref(selection.ToArray()[0]); };
                selector.ShowInPopup();
            }
        }
        SirenixEditorGUI.EndHorizontalToolbar();
    }
}
#endif