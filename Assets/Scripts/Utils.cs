using System;

public static class Utils
{
    public static bool GetRandomBool()
    {
        Random random = new Random();
        return random.Next(2) == 0;
    }
}
