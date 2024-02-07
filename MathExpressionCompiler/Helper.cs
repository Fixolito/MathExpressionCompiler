using System.Linq.Expressions;
using System.Numerics;

namespace MEC;

internal static class Helper
{
    internal static void AddToListANoDuplication<T>(List<T> a, params List<T>[] b)
    {
        foreach (var list in b)
        {
            AddToListANoDuplication(a, list);
        }
    }

    internal static void AddToListANoDuplication<T>(List<T> a, List<T> b)
    {
        foreach (T item in b)
        {
            if (!a.Contains(item))
            {
                a.Add(item);
            }
        }
    }

    internal static int IndexOfFirstOccurrence<T>(IEnumerable<T> list, T item)
    {
        int index = 0;
        foreach (var listItem in list)
        {
            if (EqualityComparer<T>.Default.Equals(listItem, item))
            {
                return index;
            }
            index++;
        }
        return -1;
    }

    internal static int IndexOfNextOccurrence<T>(IEnumerable<T> list, T item, int startIndex)
    {
        int index = 0;
        bool startChecking = false;
        
        foreach (var listItem in list)
        {
            if (index >= startIndex) startChecking = true;
            
            if (startChecking && EqualityComparer<T>.Default.Equals(listItem, item))
            {
                return index;
            }
            index++;
        }
        return -1;
    }

    internal static bool RemoveLastOccurrence<T>(IList<T> list, T item)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (EqualityComparer<T>.Default.Equals(list[i], item))
            {
                list.RemoveAt(i);
                return true; // Item found and removed
            }
        }
        return false; // Item not found
    }

    internal static NumberSpace GetNumberSpace(Type type)
    {
        if (type == typeof(double)) return NumberSpace.REAL;
        else if (type == typeof(Complex)) return NumberSpace.COMPLEX;
        else throw new Exception("Only <double> for real numbers or <Complex> for complex numbers are allowed.");
    }

    internal static string[] VariableNames(ParameterExpression[] input)
    {
        string[] result = new string[input.Length];
        for (int i = 0; i < input.Length; i++)
        {
            result[i] = input[i].Name!;
        }
        return result;
    }

    internal static Object Zero(NumberSpace space)
    {
        if (NumberSpace.REAL == space) return 0d;
        if (NumberSpace.COMPLEX == space) return Complex.Zero;
        throw new NotImplementedException();
    }

    internal static Object One(NumberSpace space)
    {
        if (NumberSpace.REAL == space) return 1d;
        if (NumberSpace.COMPLEX == space) return Complex.One;
        throw new NotImplementedException();
    }
}