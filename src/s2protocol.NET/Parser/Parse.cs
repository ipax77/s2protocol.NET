using IronPython.Runtime;
using System.Numerics;
using System.Text;

namespace s2protocol.NET.Parser;
internal partial class Parse
{
    internal static string GetString(PythonDictionary pydic, string property)
    {
        if (pydic.TryGetValue(property, out object? value))
        {
            if (value != null && value.GetType() == typeof(Bytes))
            {
                Bytes? b = value as Bytes;
                if (b != null)
                    return Encoding.UTF8.GetString(b.ToArray());
                else
                    return "";
            }
            else
            {
                return value?.ToString() ?? "";
            }
        }
        else return "";
    }

    internal static string? GetNullableString(PythonDictionary pydic, string property)
    {
        if (pydic.TryGetValue(property, out object? value))
        {
            if (value != null && value.GetType() == typeof(Bytes))
            {
                Bytes? b = value as Bytes;
                if (b != null)
                    return Encoding.UTF8.GetString(b.ToArray());
                else
                    return null;
            }
            else
            {
                return value?.ToString();
            }
        }
        else return null;
    }

    internal static int GetInt(PythonDictionary pydic, string property)
    {
        if (pydic.TryGetValue(property, out object? value))
        {
            if (value != null && value.GetType() == typeof(Int32))
            {
                int? i = value as int?;
                if (i != null)
                {
                    return i.Value;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
        else
        {
            return 0;
        }
    }

    internal static int? GetNullableInt(PythonDictionary pydic, string property)
    {
        if (pydic.TryGetValue(property, out object? value))
        {
            if (value != null && value.GetType() == typeof(Int32))
            {
                int? i = value as int?;
                if (i != null)
                {
                    return i.Value;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    internal static long GetBigInt(PythonDictionary pydic, string property)
    {
        if (pydic.TryGetValue(property, out object? value))
        {
            if (value != null && value.GetType() == typeof(BigInteger))
            {
                BigInteger? i = value as BigInteger?;
                if (i != null)
                {
                    return (long)i;
                }
                else
                {
                    return 0;
                }

            }
            else
            {
                return 0;
            }
        }
        else
        {
            return 0;
        }
    }

    internal static bool GetBool(PythonDictionary pydic, string property)
    {
        if (pydic.TryGetValue(property, out object? value))
        {
            if (value != null && value.GetType() == typeof(Boolean))
            {
                bool? b = value as bool?;
                if (b != null)
                {
                    return (bool)b;
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
        else
        {
            return false;
        }
    }

    internal static string GetAsciiString(PythonDictionary pydic, string property)
    {
        if (pydic.TryGetValue(property, out object? value))
        {
            if (value != null && value.GetType() == typeof(String))
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

    internal static double GetDouble(PythonDictionary pydic, string property)
    {
        if (pydic.TryGetValue(property, out object? value))
        {
            if (value != null && value.GetType() == typeof(double))
            {
                return (double)value;
            }
            else
            {
                return 0;
            }
        }
        else return 0;
    }

    internal static List<int> GetIntList(PythonDictionary pydic, string property)
    {
        List<int> intList = new List<int>();
        if (pydic.TryGetValue(property, out object? value))
        {
            if (value != null && value.GetType() == typeof(List))
            {
                List list = (List)value;
                foreach (var item in list)
                {
                    int? i = item as int?;
                    if (i != null)
                    {
                        intList.Add(i.Value);
                    }
                }
            }
        }
        return intList;
    }

    internal static List<long> GetLongList(PythonDictionary pydic, string property)
    {
        List<long> longList = new List<long>();
        if (pydic.TryGetValue(property, out object? value))
        {
            if (value != null && value.GetType() == typeof(List))
            {
                List list = (List)value;
                foreach (var item in list)
                {
                    long? i = item as long?;
                    if (i != null)
                    {
                        longList.Add(i.Value);
                    }
                }
            }
        }
        return longList;
    }

    internal static KeyValuePair<int, BigInteger> GetIntBigTuple(PythonDictionary pydic, string property)
    {
        int intEnt = 0;
        BigInteger bigEnt = 0;
        if (pydic.TryGetValue(property, out object? value))
        {
            if (value != null && value.GetType() == typeof(PythonTuple))
            {
                PythonTuple? tuple = value as PythonTuple;
                if (tuple != null)
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
                    if (tuple[1].GetType() == typeof(BigInteger))
                    {
                        var bitInt = tuple[1] as BigInteger?;
                        if (bitInt != null)
                        {
                            bigEnt = bitInt.Value;
                        }
                    }
                }
            }
        }
        return new KeyValuePair<int, BigInteger>(intEnt, bigEnt);
    }

    internal static List<string> GetStringList(PythonDictionary pydic, string property)
    {
        List<string> stringList = new List<string>();
        if (pydic.TryGetValue(property, out object? value))
        {
            if (value != null && value.GetType() == typeof(List))
            {
                List list = (List)value;
                foreach (var item in list)
                {
                    string? i = item as string;
                    if (i != null)
                    {
                        stringList.Add(i);
                    }
                }
            }
        }
        return stringList;
    }
}
