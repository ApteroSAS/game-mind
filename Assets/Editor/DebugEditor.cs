#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DebugManager))]
public class DebugEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DebugManager debugManager = (DebugManager)target;
        if (GUILayout.Button("Start singleplayer"))
        {
            debugManager.StartSinglePlayer();
        }

        if (GUILayout.Button("Jump to Gamestate"))
        {
            debugManager.JumpToGameState();
        }
    }
}
#endif
