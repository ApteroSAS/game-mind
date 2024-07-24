using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Q4_Cauldron : NetworkBehaviour, IInteractable
{
    [SerializeField] private Material textureGuest;
    [SerializeField] private Slider slider;

    private bool isHolding = false;
    private float holdTime = 0;

    private void Update()
    {
        if (isHolding)
        {
            holdTime += Time.deltaTime;
            float valueChange = holdTime * 100;
            NetworkManager.LocalClient.PlayerObject.GetComponentInChildren<Q4_Wand>().AnimateWand(valueChange);
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        AdjustTexture();
        UpdateSliderToClient();
    }

    public void Interact()
    {
        isHolding = true;
    }

    public void StopInteract()
    {
        isHolding = false;
        NetworkManager.LocalClient.PlayerObject.GetComponentInChildren<Q4_Wand>().ResetWand();
        ApplyAttributeServerRpc(holdTime);
        holdTime = 0;
    }

    [ServerRpc(RequireOwnership = false)]
    protected virtual void ApplyAttributeServerRpc(float valueChange, ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        float newValue = valueChange.FunnyNumber();

        PlayerAttribute attribute = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerAnswers>().NetworkAttribute.Value;
        if (attribute == PlayerAttribute.Ice) newValue *= -1;

        float currentValue = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerAnswers>().NetworkSexMeter.Value;
        newValue = Mathf.Clamp(currentValue + newValue, 0, 1);
        NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerAnswers>().NetworkSexMeter.Value = newValue;
        
        UpdateSlider(newValue, clientId);
    }

    private void AdjustTexture()
    {
        if (NetworkManager.Singleton.LocalClientId != OwnerClientId)
        {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            Material[] materials = meshRenderer.materials;
            materials[1] = textureGuest;
            meshRenderer.materials = materials;
        }
    }

    private void UpdateSliderToClient()
    {
        slider.value = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerAnswers>().NetworkSexMeter.Value;
    }

    protected void UpdateSlider(float newValue,ulong clientId)
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
