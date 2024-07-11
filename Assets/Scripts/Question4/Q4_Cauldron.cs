using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Q4_Cauldron : Interactable
{
    [SerializeField] Material cauldronTextureGuest;

    [SerializeField] Button interact;
    [SerializeField] Slider slider;



    //make particlesystem go crazy depending on heat!

    private void Awake()
    {
        AdjustCauldronTexture();

        interact.onClick.AddListener(OnButtonClick);
    }

    public override void Interact(GameObject gameObject)
    {
        //animation on wand and track duration ig?
    }

    private void OnButtonClick()
    {
        TestMethodInteractServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void TestMethodInteractServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        Debug.Log(clientId + " triggered this method");
        PlayerAttribute attribute = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerAnswers>().NetworkAttribute.Value;
        float value = 0.1f;
        if (attribute == PlayerAttribute.Ice) value *= -1;

        float currentValue = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerAnswers>().NetworkSexMeter.Value;
        float newValue = Mathf.Clamp(currentValue + value, 0, 1);
        NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerAnswers>().NetworkSexMeter.Value = newValue;
        UpdateSlider(newValue, clientId);
    }

    private void AdjustCauldronTexture()
    {
        if (NetworkManager.Singleton.LocalClientId != OwnerClientId)
        {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            Material[] materials = meshRenderer.materials;
            materials[1] = cauldronTextureGuest;
            meshRenderer.materials = materials;
            Debug.Log("changed texture");
        }
    }

    private void UpdateSlider(float newValue,ulong clientId)
    {
        //if (!IsServer) return;
        // NOTE! In case you know a list of ClientId's ahead of time, that does not need change,
        // Then please consider caching this (as a member variable), to avoid Allocating Memory every time you run this function
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        };
        DoSomethingClientRpc(newValue, clientRpcParams);
    }

    [ClientRpc]
    private void DoSomethingClientRpc(float newValue, ClientRpcParams clientRpcParams = default)
    {
        //if (IsOwner) return;

        // Run your client-side logic here!!
        slider.value = newValue;
    }
}
