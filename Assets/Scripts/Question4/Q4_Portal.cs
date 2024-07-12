using UnityEngine;
using Unity.Netcode;

public class Q4_Portal : Q4_Cauldron
{
    [ServerRpc (RequireOwnership = false)]
    protected override void ApplyAttributeServerRpc(float valueChange, ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        float newValue = valueChange.FunnyNumber();

        PlayerAttribute attribute = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerAnswers>().NetworkAttribute.Value;
        if (attribute == PlayerAttribute.Ice) newValue *= -1;

        foreach (var otherClientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (otherClientId != clientId)
            {
                float currentValue = NetworkManager.Singleton.ConnectedClients[otherClientId].PlayerObject.GetComponent<PlayerAnswers>().NetworkSexMeter.Value;
                newValue = Mathf.Clamp(currentValue + newValue, 0, 1);
                NetworkManager.Singleton.ConnectedClients[otherClientId].PlayerObject.GetComponent<PlayerAnswers>().NetworkSexMeter.Value = newValue;
                UpdateSlider(newValue, otherClientId);
            }
        }
    }
}
