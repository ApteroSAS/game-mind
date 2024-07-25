using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Q3_Results : NetworkBehaviour
{
    [SerializeField] GameObject[] childSymbols;
    [SerializeField] Material[] materials;
    [SerializeField] string[] questionText;
    [SerializeField] MeshRenderer[] meshRendererToChange;

    [ClientRpc]
    public void OnSpawnClientRpc(Symbol symbol, Vector3 pos)
    {
        transform.position = pos;
        int index = (int)symbol;

        ShowText(index);
        childSymbols[index].SetActive(true);
        ApplyMaterialAllMeshRenderers(materials[index]);
    }

    private void ShowText(int index)
    {
        GetComponentInChildren<TextMeshProUGUI>().text = questionText[index];
    }

    private void ApplyMaterialAllMeshRenderers(Material newMaterial)
    {
        foreach (var meshRenderer in meshRendererToChange)
        {
            meshRenderer.material = newMaterial;
        }
    }
}
