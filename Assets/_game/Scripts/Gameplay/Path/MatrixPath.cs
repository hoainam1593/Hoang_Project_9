using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class MatrixPath<T> where T : class
{
    private int crrIdx = 0;
    private List<T> points;
    public List<T> Points => points;

    public MatrixPath()
    {
        points = new List<T>();
    }

    public MatrixPath(List<T> points)
    {
        this.points = points;
    }

    public void Reverse()
    {
        points.Reverse();
    }

    public void Add(T point)
    {
        if (!points.Contains(point))
        {
            points.Add(point);
        }
    }

    public T GetRoot()
    {
        crrIdx = 0;
        return points[crrIdx];
    }

    public T Next()
    {
        crrIdx++;
        if (crrIdx < points.Count)
        {
            return points[crrIdx];
        }

        return null;
    }

    public T Next(T point)
    {
        var index = IndexOf(point);
        if (index != -1 && index < points.Count - 1)
        {
            return points[index+1];
        }

        return null;
    }

    public int IndexOf(T point)
    {
        return points.IndexOf(point);
    }

    public bool IsEnd(T point)
    {
        var index = IndexOf(point);
        if (index == points.Count - 1)
        {
            return true;
        }

        return false;
    }

    public bool IsEnd()
    {
        return crrIdx == points.Count - 1;
    }

    public override string ToString()
    {
        string str = "";
        foreach (T point in points)
        {
            str += point + " > ";
        }

        return str;
    }
}