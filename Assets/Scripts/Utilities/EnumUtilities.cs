using System;
using System.Collections.Generic;
using System.Linq;

public static class EnumUtilities
{
	public static T RandomEnumValue<T>()
	{
		var v = Enum.GetValues(typeof(T));
		return (T) v.GetValue(new Random().Next(v.Length));
	}

	public static List<T> GetValues<T>()
	{
		return new List<T>(Enum.GetValues(typeof(T)).Cast<T>());
	}
}