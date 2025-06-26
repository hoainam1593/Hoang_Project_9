using System;
using System.Collections.Generic;

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

    public bool IsEndItem()
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