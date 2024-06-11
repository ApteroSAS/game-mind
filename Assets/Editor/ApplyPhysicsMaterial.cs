using UnityEditor;
using UnityEngine;

public class ApplyPhysicsMaterial : EditorWindow
{
    private PhysicsMaterial material;

    [MenuItem("Tools/Apply Physics Material to All")]
    public static void ShowWindow()
    {
        GetWindow<ApplyPhysicsMaterial>("Apply Physics Material");
    }

    void OnGUI()
    {
        GUILayout.Label("Select Physics Material", EditorStyles.boldLabel);
        material = (PhysicsMaterial)EditorGUILayout.ObjectField("Physics Material", material, typeof(PhysicsMaterial), false);

        if (GUILayout.Button("Apply to All"))
        {
            ApplyMaterialToAll();
        }
    }

    void ApplyMaterialToAll()
    {
        if (material == null)
        {
            Debug.LogError("Please assign a physics material.");
            return;
        }

        Collider[] colliders = FindObjectsByType<Collider>(FindObjectsSortMode.None);

        foreach (Collider collider in colliders)
        {
            collider.sharedMaterial = material;
        }

        Debug.Log("Applied physics material to " + colliders.Length + " colliders.");
    }
}
