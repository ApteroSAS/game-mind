using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum Symbol
{
    Circle,
    Cross,
    Moon,
    Square,
    Star,
    Triangle,
}

public class Q3_Cube : Interactable
{
    [SerializeField] GameObject[] childSymbols;

    public void OnInstantiate(Symbol symbol, string questionText)
    {
        GetComponentInChildren<TextMeshProUGUI>().text = questionText;

        //increase objsize later based on text length

        int symbolNumber = (int)symbol;
        childSymbols[symbolNumber].SetActive(true);
        Material materialOfSymbol = childSymbols[symbolNumber].GetComponent<MeshRenderer>().material;
        ApplyMaterialAllMeshRenderers(materialOfSymbol);
    }

    private void ApplyMaterialAllMeshRenderers(Material newMaterial)
    {
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (var meshRenderer in meshRenderers)
        {
            meshRenderer.material = newMaterial;
        }
    }

    public void SetDestroy()
    {
        Destroy(gameObject);
    }

    public override void Interact()
    {
        //follow the mouse, locked on 1 axis
    }

    public override void StopInteract()
    {
        //stop following the mouse duh
    }
}
