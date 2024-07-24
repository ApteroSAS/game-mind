using TMPro;
using Unity.Netcode;
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

public class Q3_Block : Interactable
{
    [SerializeField] GameObject[] childSymbols;
    [SerializeField] Material[] materials;
    [SerializeField] string[] questionText;
    [SerializeField] MeshRenderer[] meshRendererToChange;
    private Symbol currentSymbol;
    private bool isHolding = false;
    private Vector3 _pos;
    private readonly float margin = 2;

    bool isInteractable = true;


    public void OnInstantiate(int index, bool showText)
    {
        _pos = GetComponent<TeleportOnSpawn>().GetSpawn().position;
        RandomizeSpawn();

        ShowText(index, showText);
        
        
        //increase objsize later based on text length

        childSymbols[index].SetActive(true);
        currentSymbol = (Symbol)index;
        ApplyMaterialAllMeshRenderers(materials[index]);
    }

    public void OnInstantiateForResult(Symbol symbol, Vector3 pos)
    {
        isInteractable = false;
        transform.position = pos;
        int index = (int)symbol;

        ShowText(index, true);
        childSymbols[index].SetActive(true);
        ApplyMaterialAllMeshRenderers(materials[index]);
    }

    private void RandomizeSpawn()
    {
        Vector3 pos = transform.position;
        float randomFloat = Random.Range(margin / 2, margin);
        if (Utils.GetRandomBool()) randomFloat *= -1;
        pos.x += randomFloat;
        transform.position = pos;
    }

    private void ShowText(int index, bool showText)
    {
        if (OwnerClientId != NetworkManager.Singleton.LocalClientId)
        {
            showText = !showText;
        }

        if (showText)
        {
            GetComponentInChildren<TextMeshProUGUI>().text = questionText[index];
        }
        else
        {
            GetComponentInChildren<TextMeshProUGUI>().enabled = false;
        }


    }

    private void ApplyMaterialAllMeshRenderers(Material newMaterial)
    {
        foreach (var meshRenderer in meshRendererToChange)
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
        isHolding = true;
        SetPhysics(false);
    }

    public override void StopInteract()
    {
        isHolding = false;
        SetPhysics(true);
    }

    private void SetPhysics(bool interactWithWorld)
    {
        GetComponentInChildren<BoxCollider>().enabled = interactWithWorld;
        GetComponent<Rigidbody>().useGravity = interactWithWorld;
    }

    private Vector3 ContainInsideBorders(Vector3 currentPos)
    {
        Vector3 newPos;
        newPos.z = transform.position.z;
        newPos.y = Mathf.Clamp(currentPos.y, _pos.y, 10);
        newPos.x = Mathf.Clamp(currentPos.x, _pos.x - margin, _pos.x + margin);
        return newPos;
    }

    private void Update()
    {
        if (!isInteractable) return;
        if (isHolding)
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

            transform.position = ContainInsideBorders(mouseWorldPosition);
        }
    }

    public Symbol GetSymbol()
    {
        return currentSymbol;
    }
}
