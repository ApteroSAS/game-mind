using UnityEngine;

public class Wand : MonoBehaviour
{
    [SerializeField] private Material fireMaterial;
    [SerializeField] private Material iceMaterial;
    public void SetWandColor(PlayerAttribute playerAttribute)
    {
        if(playerAttribute == PlayerAttribute.Fire)
        {
            GetComponent<MeshRenderer>().material = fireMaterial;
        }
        else
        {
            GetComponent<MeshRenderer>().material = iceMaterial;
        }
    }
}
