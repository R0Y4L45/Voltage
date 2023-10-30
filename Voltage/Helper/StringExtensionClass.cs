namespace Voltage.Helper;

public static class StringExtensionClass
{
    public static bool IsContainSpecialChar(this string str, char[] chars)
    {
        foreach (char c in chars)
            if(str.Contains(c))
                return true;

        return false;
    }
}
