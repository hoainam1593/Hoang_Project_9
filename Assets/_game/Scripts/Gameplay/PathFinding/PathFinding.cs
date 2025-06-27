
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;

public class PathFinding : SingletonMonoBehaviour<PathFinding>
{
    public static Vector3 UnDefinePos {
        get
        {
            Debug.LogWarning("UnDefinePos");
            return Vector3.one * (-1000);
        }
    }
    
    private MatrixPath<MapCoordinate> matrixPath;
    private Dictionary<MapCoordinate, Vector3> convertPos;

    [SerializeField] private MapCtrl mapCtrl;

    private void Start()
    {
        var aPath = mapCtrl.GetMatrixPath();
        Debug.Log("Path: " + aPath);
        SetEnemyPath(aPath);
    }

    public void SetEnemyPath(MatrixPath<MapCoordinate> aPath)
    {
        matrixPath = new MatrixPath<MapCoordinate>();
        convertPos = new  Dictionary<MapCoordinate, Vector3>();
        foreach (var point in aPath.Points)
        {
            matrixPath.Add(point);
            var convertPos = mapCtrl.ConvertMatrixCoordinateToCenterTileWorldPos(point);
            this.convertPos.Add(point, convertPos);
        }
        
    }

    #region Get StartPoint
    public MapCoordinate GetStartPoint()
    {
        return matrixPath.GetRoot();
    }

    public Vector3 GetStartWorldPos()
    {
        var startPoint = GetStartPoint();
        return GetWorldPos(startPoint);
    }
    
    #endregion Get StartPoint!!
    
    #region Get Next Point

    public MapCoordinate GetNextPoint(MapCoordinate crrPoint)
    {
        return matrixPath.Next(crrPoint);
    }

    public Vector3 GetNextWorldPos(MapCoordinate crrPoint)
    {
        var nextPoint = GetNextPoint(crrPoint);
        if (nextPoint != null)
        {
            return GetWorldPos(nextPoint);
        }

        return UnDefinePos;
    }
    
    #endregion Get Next!!
    
    #region IsEndOfPath

    public bool IsEnd(MapCoordinate crrPoint)
    {
        return matrixPath.IsEnd(crrPoint);
    }
    
    #endregion IsEndOfPath!!
    
    

    public Vector3 GetWorldPos(MapCoordinate point)
    {
        if (convertPos.ContainsKey(point))
        {
            return convertPos[point];
        }

        return UnDefinePos;
    }
}