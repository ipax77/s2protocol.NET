using System.Numerics;
using System.Text;

namespace s2protocol.NET.Parser;

internal static partial class Parse
{
    internal static string GetString(Dictionary<string, object> pydic, string property)
    {
        return pydic.TryGetValue(property, out object? value)
            ? value != null && value.GetType() == typeof(byte[])
                ? value is byte[] b ? Encoding.UTF8.GetString(b) : ""
                : value?.ToString() ?? ""
            : "";
    }

    internal static string? GetNullableString(Dictionary<string, object> pydic, string property)
    {
        return pydic.TryGetValue(property, out object? value)
            ? value != null && value.GetType() == typeof(byte[]) ? value is byte[] b ? Encoding.UTF8.GetString(b) : null : (value?.ToString())
            : null;
    }

    internal static int GetInt(Dictionary<string, object> pydic, string property)
    {
        if (pydic.TryGetValue(property, out object? value))
        {
            if (value != null && value.GetType() == typeof(Int32))
            {
                int? i = value as int?;
                return i != null ? i.Value : 0;
            }
            else
            {
                return value != null && value.GetType() == typeof(Int64) ? (int)(long)value : 0;
            }
        }
        else
        {
            return 0;
        }
    }

    internal static int? GetNullableInt(Dictionary<string, object> pydic, string property)
    {
        if (pydic.TryGetValue(property, out object? value))
        {
            if (value != null && value.GetType() == typeof(Int32))
            {
                int? i = value as int?;
                return i;
            }
            else if (value != null && value.GetType() == typeof(Int64))
            {
                return (int)(long)value;
            }
            else if (value != null && value.GetType() == typeof(BigInteger))
            {
                return (int)(BigInteger)value;
            }
            else
            {
                if (value != null)
                {
                }
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    internal static long GetBigInt(Dictionary<string, object> pydic, string property)
    {
        return pydic.TryGetValue(property, out object? value)
            ? value != null
                ? value.GetType() == typeof(BigInteger)
                    ? (long)(BigInteger)value
                    : value.GetType() == typeof(Int32)
                        ? (int)value
                        : value.GetType() == typeof(Int64) ? (long)value : value.GetType() == typeof(int) ? (int)value : 0
                : 0
            : 0;
    }

    internal static long? GetNullableBigInt(Dictionary<string, object> pydic, string property)
    {
        return pydic.TryGetValue(property, out object? value)
            ? value != null
                ? value.GetType() == typeof(BigInteger)
                    ? (long)(BigInteger)value
                    : value.GetType() == typeof(int)
                        ? (int)value
                        : value.GetType() == typeof(Int32) ? (int)value : value.GetType() == typeof(Int64) ? (long)value : null
                : null
            : null;
    }

    internal static bool GetBool(Dictionary<string, object> pydic, string property)
    {
        if (pydic.TryGetValue(property, out object? value))
        {
            if (value != null && value.GetType() == typeof(bool))
            {
                bool? b = value as bool?;
                return b != null && (bool)b;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    internal static string GetAsciiString(Dictionary<string, object> pydic, string property)
    {
        if (pydic.TryGetValue(property, out object? value))
        {
            if (value != null && value.GetType() == typeof(string))
            {
                var bytes = Encoding.UTF8.GetBytes(value.ToString() ?? "");
                return Encoding.UTF8.GetString(bytes);
            }
            else
            {
                return value?.ToString() ?? "";
            }
        }
        else return "";
    }

    internal static double GetDouble(Dictionary<string, object> pydic, string property)
    {
        return pydic.TryGetValue(property, out object? value)
            ? value != null && value.GetType() == typeof(double)
                ? (double)value
                : value != null && value.GetType() == typeof(float)
                    ? (double)(float)value
                    : value != null && value.GetType() == typeof(int)
                                    ? (int)value
                                    : value != null && value.GetType() == typeof(long) ? (long)value : 0
            : 0;
    }

    internal static List<int> GetIntList(Dictionary<string, object> pydic, string property)
    {
        List<int> intList = [];
        if (pydic.TryGetValue(property, out object? value))
        {
            if (value != null && value.GetType() == typeof(List<object>))
            {
                List<object> list = (List<object>)value;
                foreach (var item in list)
                {
                    if (item is int i)
                    {
                        intList.Add(i);
                    }
                    else if (item is long l)
                    {
                        intList.Add((int)l);
                    }
                    else if (item is BigInteger bi)
                    {
                        intList.Add((int)bi);
                    }
                    else if (item is double d)
                    {
                        intList.Add((int)d);
                    }
                }
            }
            else
            {
            }
        }
        return intList;
    }

    internal static List<long> GetLongList(Dictionary<string, object> pydic, string property)
    {
        List<long> longList = [];
        if (pydic.TryGetValue(property, out object? value))
        {
            if (value != null && value.GetType() == typeof(List<object>))
            {
                List<object> list = (List<object>)value;
                foreach (var item in list)
                {
                    if (item is int i)
                    {
                        longList.Add(i);
                    }
                    else if (item is long l)
                    {
                        longList.Add(l);
                    }
                    else if (item is BigInteger bi)
                    {
                        longList.Add((long)bi);
                    }
                    else if (item is double d)
                    {
                        longList.Add((long)d);
                    }
                }
            }
            else
            {
            }
        }
        return longList;
    }

    internal static KeyValuePair<int, BigInteger> GetIntBigTuple(Dictionary<string, object> pydic, string property)
    {
        int intEnt = 0;
        BigInteger bigEnt = 0;
        if (pydic.TryGetValue(property, out object? value))
        {
            if (value != null && value.GetType() == typeof(object[]))
            {
                if (value is object[] tuple)
                {
                    int? tKey = tuple[0] as int?;
                    if (tKey != null)
                    {
                        intEnt = tKey.Value;
                    }

                    if (tuple[1].GetType() == typeof(int))
                    {
                        bigEnt = (tuple[1] as int?) ?? 0;
                    }
                    else if (tuple[1].GetType() == typeof(BigInteger))
                    {
                        var bitInt = tuple[1] as BigInteger?;
                        if (bitInt != null)
                        {
                            bigEnt = bitInt.Value;
                        }
                    }
                    else
                    {
                    }
                }
            }
            else
            {
            }
        }
        return new KeyValuePair<int, BigInteger>(intEnt, bigEnt);
    }

    internal static List<string> GetStringList(Dictionary<string, object> pydic, string property)
    {
        List<string> stringList = [];
        if (pydic.TryGetValue(property, out object? value))
        {
            if (value != null && value.GetType() == typeof(List<object>))
            {
                List<object> list = (List<object>)value;
                foreach (var item in list)
                {
                    if (item is string i)
                    {
                        stringList.Add(i);
                    }
                }
            }
            else
            {
            }
        }
        return stringList;
    }
}
