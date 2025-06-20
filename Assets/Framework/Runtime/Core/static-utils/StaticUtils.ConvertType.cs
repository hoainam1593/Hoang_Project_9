using System.Globalization;
using System;
using UnityEngine;

public static partial class StaticUtils
{
	#region string to types

	public static bool StringToBool(string s)
	{
		try
		{
			return bool.Parse(s);
		}
		catch (Exception e)
		{
			Debug.LogError($"parse bool failed, value={s}");
			RethrowException(e);

			return false;
		}
	}

	public static int StringToInt(string s)
	{
		try
		{
			return int.Parse(s, CultureInfo.InvariantCulture);
		}
		catch (Exception e)
		{
			Debug.LogError($"parse int failed, value={s}");
			RethrowException(e);

			return int.MinValue;
		}
	}

	public static long StringToLong(string s)
	{
		try
		{
			return long.Parse(s, CultureInfo.InvariantCulture);
		}
		catch (Exception e)
		{
			Debug.LogError($"parse long failed, value={s}");
			RethrowException(e);

			return long.MinValue;
		}
	}

	public static ulong StringToULong(string s)
	{
		try
		{
			return ulong.Parse(s, CultureInfo.InvariantCulture);
		}
		catch (Exception e)
		{
			Debug.LogError($"parse ulong failed, value={s}");
			RethrowException(e);

			return ulong.MinValue;
		}
	}

	public static float StringToFloat(string s)
	{
		try
		{
			return float.Parse(s, CultureInfo.InvariantCulture);
		}
		catch (Exception e)
		{
			Debug.LogError($"parse float failed, value={s}");
			RethrowException(e);

			return float.MinValue;
		}
	}

	public static double StringToDouble(string s)
	{
		try
		{
			return double.Parse(s, CultureInfo.InvariantCulture);
		}
		catch (Exception e)
		{
			Debug.LogError($"parse double failed, value={s}");
			RethrowException(e);

			return double.MinValue;
		}
	}

	public static decimal StringToDecimal(string s)
	{
		try
		{
			return decimal.Parse(s, CultureInfo.InvariantCulture);
		}
		catch (Exception e)
		{
			Debug.LogError($"parse decimal failed, value={s}");
			RethrowException(e);

			return decimal.MinValue;
		}
	}

	public static DateTime StringToDateTime(string s)
	{
		try
		{
			return DateTime.Parse(s, CultureInfo.InvariantCulture);
		}
		catch (Exception e)
		{
			Debug.LogError($"parse date time failed, value={s}");
			RethrowException(e);

			return new DateTime();
		}
	}

	public static T StringToEnum<T>(string s) where T : struct
	{
		if (Enum.TryParse(s, out T val))
		{
			return val;
		}
		else
		{
			return IntToEnum<T>(StringToInt(s));
		}
	}

	public static ulong StringToBytes(string str)
	{
		var i = 0;
		while (str[i] == '.' || IsDigitCharacter(str[i]))
		{
			i++;
		}
		var val = StringToFloat(str.Substring(0, i));

		i = str.Length - 1;
		while (IsAlphabetCharacter(str[i]))
		{
			i--;
		}
		var unit = str.Substring(i + 1);

		return unit.ToLower() switch
		{
			"b" => (ulong)val,
			"kb" => (ulong)(val * 1024),
			"mb" => (ulong)(val * 1024 * 1024),
			"gb" => (ulong)(val * 1024 * 1024 * 1024),
			_ => throw new Exception($"unit {unit} is invalid for bytes")
		};
	}

    #endregion

    #region types to string

    public static string DoubleToString(double val)
    {
        return val.ToString(CultureInfo.InvariantCulture);
    }

    public static string FloatToString(float val)
    {
        return val.ToString(CultureInfo.InvariantCulture);
    }

	public static string DateTimeToString(DateTime val)
	{
        return val.ToString(CultureInfo.InvariantCulture);
    }

    public static string BytesToString(ulong bytes)
	{
		if (bytes < 1024)
		{
			return $"{bytes} B";
		}

		if (bytes < 1024 * 1024)
		{
			var kb = (float)bytes / 1024;
			return $"{kb:0.##} KB";
		}

		if (bytes < 1024 * 1024 * 1024)
		{
			var mb = (float)bytes / 1024 / 1024;
			return $"{mb:0.##} MB";
		}

		var gb = (float)bytes / 1024 / 1024 / 1024;
		return $"{gb:0.##} GB";
	}

	public static string TimespanToString(TimeSpan timeSpan)
	{
		if (timeSpan.Days > 0)
		{
			return $"{timeSpan.Days}d {timeSpan.Hours}h";
		}

		if (timeSpan.Hours > 0)
		{
			return $"{timeSpan.Hours}h {timeSpan.Minutes}m";
		}

		if (timeSpan.Minutes > 0)
		{
			return $"{timeSpan.Minutes}m {timeSpan.Seconds}s";
		}

		return $"{timeSpan.Seconds}s";
	}

	#endregion

	#region others

	public static T IntToEnum<T>(int i) where T : struct
	{
		return (T)Enum.ToObject(typeof(T), i);
	}

	#endregion
}