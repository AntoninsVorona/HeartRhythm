using System;

public static class EnumUtilities
{
    public static T RandomEnumValue<T>()
    {
        var v = Enum.GetValues(typeof(T));
        return (T) v.GetValue(new Random().Next(v.Length));
    }
}