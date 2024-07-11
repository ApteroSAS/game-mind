using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Cauldron : Interactable
{
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject duringInteraction;
    [SerializeField] private GameObject outsideInteraction;
    [SerializeField] private Button addAttributeButton;
    [SerializeField] private Button leaveInteractionButton;
    [SerializeField] private TextMeshProUGUI yourAttributeText;

    private GameObject player;
    private HoldButtonHandler addAttributeButtonHandler;
    private HoldButtonHandler addAttributeOtherPlayerButtonHandler;

    private void Start()
    {
        addAttributeButtonHandler = SetupButton(addAttributeButton, true);

        leaveInteractionButton.onClick.AddListener(LeaveInteraction);
    }

    public override void Interact(GameObject playerObject)
    {
        player = playerObject;
        player.GetComponent<PlayerNetwork>().SetMovementEnabled(false);
        Vector3 newPosition = transform.position;
        newPosition.z -= 4;
        newPosition.y = player.transform.position.y;
        player.transform.position = newPosition;
        player.transform.rotation = Quaternion.Euler(0, 0, 0);

        SetAttribute();
        ToggleUI(true);
    }

    private void ToggleUI(bool isInteracting)
    {
        duringInteraction.GetComponent<CanvasGroup>().ToggleCanvasGroup(isInteracting);
        outsideInteraction.GetComponent<CanvasGroup>().ToggleCanvasGroup(!isInteracting);
    }

    private void LeaveInteraction()
    {
        player.GetComponent<PlayerNetwork>().SetMovementEnabled(true);
        ToggleUI(false);
    }

    private HoldButtonHandler SetupButton(Button button, bool myMeter)
    {
        HoldButtonHandler handler = button.gameObject.AddComponent<HoldButtonHandler>();
        handler.Initialize(this, myMeter);
        return handler;
    }

    public void AddAttribute(float value, bool mySexMeter)
    {
        PlayerAttribute playerAttribute = player.GetComponent<PlayerAnswers>().NetworkAttribute.Value;

        if (playerAttribute == PlayerAttribute.Ice)
            value *= -1;

        RequestPlayerIdServerRpc(mySexMeter, value);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestPlayerIdServerRpc(bool isOwner, float value)
    {
        ulong clientId = GetPlayerIdOnServer(isOwner);

        PlayerAnswers targetPlayer = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerAnswers>();
        float currentSexMeter = targetPlayer.NetworkSexMeter.Value;
        currentSexMeter = Mathf.Clamp(currentSexMeter + value, 0, 1);
        targetPlayer.NetworkSexMeter.Value = currentSexMeter;

        if (isOwner)
        {
            Debug.Log("Updating local Cauldron");
            slider.value = currentSexMeter;
        }
    }

    private ulong GetPlayerIdOnServer(bool isOwner)
    {
        if (player == null)
        {
            Debug.LogError("Player is not assigned.");
            return 0;
        }

        ulong ownerId = player.GetComponent<NetworkObject>().OwnerClientId;
        Debug.Log($"Owner Client ID: {ownerId}");

        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            Debug.Log($"Connected Client Key: {client.Key}");
            if ((isOwner && client.Key == ownerId) || (!isOwner && client.Key != ownerId))
            {
                return client.Key;
            }
        }

        Debug.LogError("No matching client found.");
        return 0;
    }

    private void SetAttribute()
    {
        if (player == null)
        {
            Debug.LogError("Player is not assigned.");
            return;
        }
        yourAttributeText.text = "Your attribute: " + player.GetComponent<PlayerAnswers>().NetworkAttribute.Value.ToString();
    }
}
