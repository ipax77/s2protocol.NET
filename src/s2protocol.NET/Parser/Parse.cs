using IronPython.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    internal static long GetBigInt(PythonDictionary pydic, string property)
    {
        if (pydic.TryGetValue(property, out object? value))
        {
            if (value != null && value.GetType() == typeof(System.Numerics.BigInteger))
            {
                System.Numerics.BigInteger? i = value as System.Numerics.BigInteger?;
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
}
