using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorUtils
{

    //绘制开启栏
    public static bool DrawHeader(string caption,string key,bool defaultOpen = true)
    {
        GUILayout.Space(5.0f);
        key = key + caption;
        bool open = EditorPrefs.GetBool(key, defaultOpen);
        if (open) caption = "\u25BC" + (char)0x200a + caption;
        else caption = "\u25BA" + (char)0x200a + caption;

        if (!open) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);

        EditorGUILayout.BeginHorizontal();
        GUI.changed = false;

        GUILayout.BeginHorizontal();
        if (!GUILayout.Toggle(true, caption,"dragtab", GUILayout.MinWidth(20f))) open = !open;
        if (GUI.changed) EditorPrefs.SetBool(key, open);
        GUILayout.EndHorizontal();
        EditorGUILayout.EndHorizontal();

        return open;
    }

    private static int beginCount = 0;

    public static void BeginContents()
    {
        beginCount++;
        GUILayout.BeginHorizontal();
        EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(10f));
        GUILayout.BeginVertical();
        GUILayout.Space(3.0f);
    }

    public static void EndContents()
    {
        if(beginCount>0)
        {
            GUILayout.Space(5.0f);
            GUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(3.0f);
            GUILayout.EndHorizontal();
            GUILayout.Space(3.0f);
            beginCount--;
        }
    }
}
