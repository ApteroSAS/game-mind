using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SexMeter : Interactable
{
    [SerializeField] private Slider slider;

    public NetworkVariable<float> sliderValue = new NetworkVariable<float>();
    public NetworkVariable<uint> myOwner = new NetworkVariable<uint>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            sliderValue.Value = slider.value;
        }

        if (!IsOwner)
        {
            slider.gameObject.SetActive(false);
        }

        sliderValue.OnValueChanged += OnSliderValueChanged;
    }

    override public void OnDestroy()
    {
        sliderValue.OnValueChanged -= OnSliderValueChanged;
    }

    private void OnSliderValueChanged(float oldValue, float newValue)
    {
        slider.value = newValue;
    }

    override public void Interact(GameObject playerObject)
    {
        PlayerAttribute playerAttribute = playerObject.GetComponent<PlayerNetwork>().NetworkAttribute.Value;

        if (playerAttribute == PlayerAttribute.Ice)
        {
            Debug.Log("Reduce amount");
            ChangeSliderValueServerRpc(-0.1f);
        }
        else if (playerAttribute == PlayerAttribute.Fire)
        {
            Debug.Log("Increase amount");
            ChangeSliderValueServerRpc(0.1f);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeSliderValueServerRpc(float changeAmount)
    {
        sliderValue.Value += changeAmount;
        if(sliderValue.Value > 1) sliderValue.Value = 1;
        else if(sliderValue.Value < 0) sliderValue.Value = 0;
    }
}
