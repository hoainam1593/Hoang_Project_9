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
        this.points = points ?? new List<T>();
    }

    public void Reverse()
    {
        points.Reverse();
    }

    public void Add(T point)
    {
        if (point != null && !points.Contains(point))
        {
            points.Add(point);
        }
    }

    public T GetRoot()
    {
        if (points.Count > 0)
        {
            crrIdx = 0;
            return points[crrIdx];
        }
        return null;
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
            return points[index + 1];
        }

        return null;
    }

    public T Previous(T point)
    {
        var index = IndexOf(point);
        if (index > 0)
        {
            return points[index - 1];
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
        return index == points.Count - 1;
    }

    public bool IsEnd()
    {
        return crrIdx == points.Count - 1;
    }

    public bool IsStart(T point)
    {
        var index = IndexOf(point);
        return index == 0;
    }

    public int Count => points.Count;

    public T this[int index]
    {
        get
        {
            if (index >= 0 && index < points.Count)
                return points[index];
            return null;
        }
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