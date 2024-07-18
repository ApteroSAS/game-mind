using UnityEngine;

public class Q4_Wand : MonoBehaviour
{
    [SerializeField] private Material iceMaterial;

    public void SetWandColor(PlayerAttribute playerAttribute)
    {
        if(playerAttribute == PlayerAttribute.Ice)
        {
            GetComponent<MeshRenderer>().material = iceMaterial;
        }
    }

    public void AnimateWand(float currentValue)
    {
        currentValue = Mathf.Clamp(currentValue, 0, 30);
        transform.localEulerAngles = new Vector3(currentValue, 0, 0);
    }

    public void ResetWand()
    {
        transform.localEulerAngles = Vector3.zero;
    }
}
