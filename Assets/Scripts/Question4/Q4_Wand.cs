using UnityEngine;

public class Q4_Wand : MonoBehaviour
{
    [SerializeField] private Material iceMaterial;
    private bool isCasting;


    private void Update()
    {
        if (isCasting) return;
        ResetWand();
    }

    public void SetWandColor(PlayerAttribute playerAttribute)
    {
        if(playerAttribute == PlayerAttribute.Ice)
        {
            GetComponent<MeshRenderer>().material = iceMaterial;
        }
    }

    public void SetDestroy()
    {
        Destroy(gameObject);
    }

    public void AnimateWand(float currentValue)
    {
        isCasting = true;
        if (currentValue > 90) currentValue = 90;
        transform.eulerAngles = new Vector3(currentValue, 0, 0);
    }

    public void ResetWand()
    {
        isCasting = false;
        float currentXAxis = transform.eulerAngles.x;
        transform.eulerAngles = Vector3.zero;
    }
}
