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
    [SerializeField] string[] questionText;
    private bool isHolding = false;
    private float _posX;
    private readonly float margin = 2;


    public void OnInstantiate(int index, bool showText)
    {
        _posX = transform.position.x;
        RandomizeSpawn();

        ShowText(index, showText);
        
        //increase objsize later based on text length

        childSymbols[index].SetActive(true);
        Material materialOfSymbol = childSymbols[index].GetComponent<MeshRenderer>().material;
        ApplyMaterialAllMeshRenderers(materialOfSymbol);
    }

    private void RandomizeSpawn()
    {
        Vector3 pos = transform.position;
        float randomFloat = Random.Range(-margin, margin);
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

    private void Update()
    {
        if (isHolding)
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

            mouseWorldPosition.z = transform.position.z;
            
            mouseWorldPosition.x = Mathf.Clamp(mouseWorldPosition.x, _posX - margin, _posX + margin);

            transform.position = mouseWorldPosition;
        }
    }
}
