public static class CacheStringInt
{
    private const int MaxCache = 128;
    private static bool isInited = false;
    private static readonly string[] _mapCount = new string[MaxCache];

    public static string ToTxt(this int value)
    {
        if (!isInited)
        {
            for (int i = 0; i < MaxCache; i++)
                _mapCount[i] = i.ToString();
            isInited = true;
        }

        if (value >= 0 && value < MaxCache)
            return _mapCount[value];

        return value.ToString();
    }
}