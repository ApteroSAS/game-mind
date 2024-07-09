using UnityEngine;

public enum Symbol
{
    Circle,
    Cross,
    Moon,
    Square,
    Star,
    Triangle,
}

public class Cube : MonoBehaviour
{
    [SerializeField] private Symbol symbol;
    [SerializeField] private string questionText;
    [SerializeField] GameObject[] childSymbols;

    private void Awake()
    {
        for (int i = 0; i < childSymbols.Length; i++)
        {
            if (i == (int)symbol)
            {
                childSymbols[i].SetActive(true);
                Material materialOfSymbol = childSymbols[i].GetComponent<MeshRenderer>().material;
                ApplyMaterialAllMeshRenderers(materialOfSymbol);
            }
        }
    }

    private void ApplyMaterialAllMeshRenderers(Material newMaterial)
    {
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var item in meshRenderers)
        {
            item.material = newMaterial;
        }
    }
}
