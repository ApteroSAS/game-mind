using UnityEngine;
using UnityEngine.EventSystems;

public class HoldButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Cauldron cauldron; // Reference to the SexMeter script
    private bool isHolding = false;
    private float holdTime = 0f;
    private bool isMySexMeter;

    public void Initialize(Cauldron meter, bool mySexMeter)
    {
        cauldron = meter; // Assign the SexMeter reference
        isMySexMeter = mySexMeter;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
        holdTime = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;

        // Check if sexMeter is not null before using it
        if (cauldron != null)
        {
            float funnyNumber = Mathf.Pow(holdTime, 2) / 4;
            cauldron.AddAttribute(funnyNumber, isMySexMeter);
        }
        else
        {
            Debug.LogError("SexMeter reference is null in HoldButtonHandler.OnPointerUp!");
        }
    }

    private void Update()
    {
        if (isHolding)
        {
            holdTime += Time.deltaTime;
        }
    }
}
