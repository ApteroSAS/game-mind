using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SexMeter : Interactable, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject duringInteraction;
    [SerializeField] private GameObject outsideInteraction;
    [SerializeField] private Button addAttributeButton;
    [SerializeField] private Button addAttributeOtherPlayerButton;
    [SerializeField] private Button leaveInteractionButton;
    private GameObject player;
    private bool isHolding = false;
    private float holdTime = 0f;

    public NetworkVariable<float> sliderValue = new NetworkVariable<float>();
    public NetworkVariable<uint> myOwner = new NetworkVariable<uint>();

    private void Start()
    {
        //addAttributeButton.onClick.AddListener(AddAttribute);

        EventTrigger trigger = addAttributeButton.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerDown
        };
        pointerDownEntry.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
        trigger.triggers.Add(pointerDownEntry);

        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerUp
        };
        pointerUpEntry.callback.AddListener((data) => { OnPointerUp((PointerEventData)data); });
        trigger.triggers.Add(pointerUpEntry);

        leaveInteractionButton.onClick.AddListener(LeaveInteraction);
    }

    private void FixedUpdate()
    {
        if (isHolding)
        {
            holdTime += Time.fixedDeltaTime;
        }
    }

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
        player = playerObject;
        player.GetComponent<PlayerNetwork>().SetMovementEnabled(false);
        Vector3 newPosition = transform.position;
        newPosition.z -= 4;
        newPosition.y = player.transform.position.y;
        player.transform.position = newPosition;
        player.transform.rotation = Quaternion.Euler(0, 0, 0);

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

    private void AddAttribute(float value)
    {
        PlayerAttribute playerAttribute = player.GetComponent<PlayerAnswers>().NetworkAttribute.Value;

        if (playerAttribute == PlayerAttribute.Ice)
        {
            Debug.Log("Reduce amount");
            ChangeSliderValueServerRpc(-value);
        }
        else if (playerAttribute == PlayerAttribute.Fire)
        {
            Debug.Log("Increase amount");
            ChangeSliderValueServerRpc(value);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeSliderValueServerRpc(float changeAmount)
    {
        sliderValue.Value += changeAmount;
        if(sliderValue.Value > 1) sliderValue.Value = 1;
        else if(sliderValue.Value < 0) sliderValue.Value = 0;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Start tracking the hold time
        isHolding = true;
        holdTime = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Stop tracking the hold time
        isHolding = false;
        Debug.Log("Button held for: " + holdTime + " seconds");
        //Give it an odd and hard to understand number x)
        float funnyNumber = Mathf.Pow(holdTime, 2) / 4;
        AddAttribute(funnyNumber);
    }
}
