using Unity.VisualScripting;
using UnityEngine;

public static class FloatExtentions
{
    public static float FunnyNumber(this float number)
    {
        float newNumber = Mathf.Pow(number, 3) / 5;
        return newNumber;
    }
}
