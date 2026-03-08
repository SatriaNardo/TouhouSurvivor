using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UltimateController))]
public class UltimateControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UltimateController controller = (UltimateController)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Apply Debug Upgrade"))
        {
            controller.ApplyDebugUpgrade();
        }
    }
}