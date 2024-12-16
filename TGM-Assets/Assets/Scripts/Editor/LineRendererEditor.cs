using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using SupplyRaid;

[CustomEditor(typeof(LineRenderer))]
public class LineRendererEditor : Editor
{
    public static Transform[] list = new Transform[1];
    public static int listSize = 1;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (LineRenderer)target;

        int num;
        GUILayout.BeginHorizontal();
        GUILayout.Label("Transform Count: ", GUILayout.Height(32));
        if (int.TryParse(GUILayout.TextField(listSize.ToString("0"), GUILayout.MaxWidth(96)), out num))
        {
            Undo.RecordObject(script, "List Size");
            listSize = num;
        }
        GUILayout.EndHorizontal();

        if (list != null)
        {
            if (list.Length != listSize)
            {
                Transform[] newList = new Transform[listSize];

                for (int i = 0; i < Mathf.Min(list.Length, newList.Length); i++)
                {
                    newList[i] = list[i];
                }
                // Assign the new array to the list
                list = newList;
            }

            for (int i = 0; i < list.Length; i++)
            {
                list[i] = (Transform)EditorGUILayout.ObjectField(list[i], typeof(Transform), true);
            }
        }


        if (GUILayout.Button("Assign All Positions", GUILayout.Height(20)))
        {
            Undo.RecordObject(script, "Assign All Positions");
            script.positionCount = list.Length;
            for (int i = 0; i < list.Length; i++)
            {
                if(list[i] != null)
                    script.SetPosition(i, list[i].position);
            }
        }

    }
}
