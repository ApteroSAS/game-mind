using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Q4_Results : NetworkBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] Material textureGuest;

    [ClientRpc]
    public void OnSpawnClientRpc(float value, bool isHost, Vector3 pos)
    {
        Debug.Log("Cauldron gets moved");
        slider.value = value;
        transform.position = pos;
        if(!isHost)
        {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            Material[] materials = meshRenderer.materials;
            materials[1] = textureGuest;
            meshRenderer.materials = materials;
        }
    }
}
