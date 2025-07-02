using System;
using UnityEngine;
using System.Collections.Generic;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;

public class PathFinding : SingletonMonoBehaviour<PathFinding>
{
    public static Vector3 UnDefinePos {
        get
        {
            Debug.LogWarning("UnDefinePos");
            return Vector3.one * (-1000);
        }
    }
    
    private MatrixPath<PathEntry> matrixPath;
    private Dictionary<MapCoordinate, Vector3> convertPos;

    [SerializeField] private MapCtrl mapCtrl;

    private void Start()
    {
        var aPath = mapCtrl.GetMatrixPath();
        Debug.Log("Original Path: " + aPath);
        SetEnemyPath(aPath);
        
        // Debug print the converted path with corner information
        DebugPrintPath();
    }

    public void SetEnemyPath(MatrixPath<MapCoordinate> coordinatePath)
    {
        // Convert MatrixPath<MapCoordinate> to MatrixPath<PathEntry>
        matrixPath = ConvertToPathEntryMatrix(coordinatePath);
        
        // Build world position dictionary
        convertPos = new Dictionary<MapCoordinate, Vector3>();
        foreach (var pathEntry in matrixPath.Points)
        {
            var worldPos = mapCtrl.ConvertMatrixCoordinateToCenterTileWorldPos(pathEntry.mapCoordinate);
            convertPos.Add(pathEntry.mapCoordinate, worldPos);
        }
    }

    /// <summary>
    /// Convert MatrixPath<MapCoordinate> to MatrixPath<PathEntry> with corner detection
    /// </summary>
    private MatrixPath<PathEntry> ConvertToPathEntryMatrix(MatrixPath<MapCoordinate> coordinatePath)
    {
        var pathEntries = new List<PathEntry>();
        var coordinates = coordinatePath.Points;

        for (int i = 0; i < coordinates.Count; i++)
        {
            var currentCoord = coordinates[i];
            bool isCorner = false;

            // Get previous and next coordinates
            MapCoordinate preCoordinate = (i > 0) ? coordinates[i - 1] : null;
            MapCoordinate nextCoordinate = (i < coordinates.Count - 1) ? coordinates[i + 1] : null;

            // Check if this is a corner point
            if (preCoordinate != null && nextCoordinate != null)
            {
                // Calculate direction from previous to current
                Vector2 dirToCurrent = new Vector2(
                    currentCoord.x - preCoordinate.x, 
                    currentCoord.y - preCoordinate.y
                );

                // Calculate direction from current to next
                Vector2 dirToNext = new Vector2(
                    nextCoordinate.x - currentCoord.x, 
                    nextCoordinate.y - currentCoord.y
                );

                // If directions are different, this is a corner
                isCorner = (dirToCurrent != dirToNext);

                Debug.Log($"[PathFinding] Coord {currentCoord}: " +
                         $"Pre({preCoordinate}) -> Curr({currentCoord}) -> Next({nextCoordinate}) " +
                         $"DirIn({dirToCurrent}) DirOut({dirToNext}) IsCorner: {isCorner}");
            }
            else
            {
                // First or last point cannot be corners
                isCorner = false;
                Debug.Log($"[PathFinding] Coord {currentCoord}: Start/End point, IsCorner: {isCorner}");
            }

            pathEntries.Add(new PathEntry(currentCoord, isCorner));
        }

        return new MatrixPath<PathEntry>(pathEntries);
    }

    #region Get StartPoint
    public MapCoordinate GetStartPoint()
    {
        var startEntry = matrixPath.GetRoot();
        return startEntry?.mapCoordinate;
    }

    public PathEntry GetStartEntry()
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
        var currentEntry = FindPathEntry(crrPoint);
        if (currentEntry != null)
        {
            var nextEntry = matrixPath.Next(currentEntry);
            return nextEntry?.mapCoordinate;
        }
        return null;
    }

    public PathEntry GetNextEntry(PathEntry currentEntry)
    {
        return matrixPath.Next(currentEntry);
    }

    public PathEntry GetNextEntry(MapCoordinate coordinate)
    {
        var currentEntry = FindPathEntry(coordinate);
        if (currentEntry != null)
        {
            return matrixPath.Next(currentEntry);
        }
        return null;
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

    /// <summary>
    /// Get the point after the next point (for lookahead corner detection)
    /// </summary>
    public MapCoordinate GetNextNextPoint(MapCoordinate crrPoint)
    {
        var nextPoint = GetNextPoint(crrPoint);
        if (nextPoint != null)
        {
            return GetNextPoint(nextPoint);
        }
        return null;
    }

    /// <summary>
    /// Get the PathEntry after the next PathEntry
    /// </summary>
    public PathEntry GetNextNextEntry(MapCoordinate crrPoint)
    {
        var nextEntry = GetNextEntry(crrPoint);
        if (nextEntry != null)
        {
            return GetNextEntry(nextEntry.mapCoordinate);
        }
        return null;
    }

    /// <summary>
    /// Get the world position of the point after next point
    /// </summary>
    public Vector3 GetNextNextWorldPos(MapCoordinate crrPoint)
    {
        var nextNextPoint = GetNextNextPoint(crrPoint);
        if (nextNextPoint != null)
        {
            return GetWorldPos(nextNextPoint);
        }
        return UnDefinePos;
    }

    /// <summary>
    /// Check if the given coordinate is marked as a corner in the path
    /// </summary>
    public bool IsCornerPoint(MapCoordinate coordinate)
    {
        var pathEntry = FindPathEntry(coordinate);
        return pathEntry?.isCorner ?? false;
    }

    /// <summary>
    /// Check if there's a corner between current, next, and next-next points
    /// </summary>
    public bool IsCornerAhead(MapCoordinate current, MapCoordinate next, MapCoordinate nextNext)
    {
        if (current == null || next == null || nextNext == null)
            return false;

        // Check if the next point is marked as a corner
        return IsCornerPoint(next);
    }

    /// <summary>
    /// Find PathEntry by MapCoordinate
    /// </summary>
    private PathEntry FindPathEntry(MapCoordinate coordinate)
    {
        if (coordinate == null) return null;

        foreach (var entry in matrixPath.Points)
        {
            if (entry.mapCoordinate.Equals(coordinate))
            {
                return entry;
            }
        }
        return null;
    }

    /// <summary>
    /// Get the corner entry and exit positions for smooth curved movement
    /// </summary>
    /// <param name="current">Current tile coordinate</param>
    /// <param name="next">Next tile coordinate (corner tile)</param>
    /// <param name="nextNext">Next-next tile coordinate</param>
    /// <param name="cornerStart">Position where enemy should start curving</param>
    /// <param name="cornerExit">Position where enemy should end curving</param>
    public void GetCornerPositions(PathEntry current, out Vector3 cornerStart, out Vector3 cornerExit)
    {
        cornerStart = UnDefinePos;
        cornerExit = UnDefinePos;

        if (!current.isCorner)
        {
            return;
        }

        var prePos = matrixPath.Previous(current);
        var nextPos = matrixPath.Next(current);
        if (prePos == null || nextPos == null)
        {
            return;
        }

        Vector3 preWorldPos = GetWorldPos(prePos.mapCoordinate); // Current tile center position
        Vector3 crrWorldPos = GetWorldPos(current.mapCoordinate); // Corner tile center position
        Vector3 nextWorldPos = GetWorldPos(nextPos.mapCoordinate); // Next-next tile center position

        cornerStart = (preWorldPos + crrWorldPos) * 0.5f; // Midpoint between current and next
        cornerExit = (crrWorldPos + nextWorldPos) * 0.5f; // Midpoint between next and next-next

        Debug.Log($"[PathFinding] Corner positions for {current.mapCoordinate}: Start({cornerStart}) Exit({cornerExit})");
    }

    /// <summary>
    /// Calculate a curved path between two positions using a simple arc
    /// </summary>
    /// <param name="startPos">Starting position</param>
    /// <param name="endPos">Ending position</param>
    /// <param name="currentPos">Current enemy position</param>
    /// <param name="curveIntensity">How curved the path should be (0-1)</param>
    /// <returns>Next position along the curved path</returns>
    public Vector3 GetCurvedPosition(Vector3 startPos, Vector3 endPos, Vector3 currentPos, float curveIntensity = 0.5f)
    {
        // Calculate the midpoint
        Vector3 midPoint = (startPos + endPos) * 0.5f;
        
        // Calculate perpendicular direction for curve offset
        Vector3 direction = (endPos - startPos).normalized;
        Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0);
        
        // Add curve offset to midpoint
        Vector3 curveCenter = midPoint + perpendicular * curveIntensity;
        
        // Calculate progress along the curve (0 to 1)
        float totalDistance = Vector3.Distance(startPos, endPos);
        float currentDistance = Vector3.Distance(startPos, currentPos);
        float t = Mathf.Clamp01(currentDistance / totalDistance);
        
        // Use quadratic Bezier curve: P(t) = (1-t)²P₀ + 2(1-t)tP₁ + t²P₂
        Vector3 curvedPos = Mathf.Pow(1 - t, 2) * startPos + 
                           2 * (1 - t) * t * curveCenter + 
                           Mathf.Pow(t, 2) * endPos;
        
        return curvedPos;
    }
    
    #endregion Get Next!!
    
    #region IsEndOfPath

    public bool IsEnd(MapCoordinate crrPoint)
    {
        var pathEntry = FindPathEntry(crrPoint);
        if (pathEntry != null)
        {
            return matrixPath.IsEnd(pathEntry);
        }
        return false;
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

    /// <summary>
    /// Get all path entries for debugging
    /// </summary>
    public List<PathEntry> GetAllPathEntries()
    {
        return matrixPath.Points;
    }

    /// <summary>
    /// Debug method to print all path entries with corner information
    /// </summary>
    public void DebugPrintPath()
    {
        Debug.Log("=== PATH ENTRIES WITH CORNER INFORMATION ===");
        for (int i = 0; i < matrixPath.Points.Count; i++)
        {
            var entry = matrixPath.Points[i];
            Debug.Log($"[{i}] {entry}");
        }
        Debug.Log("=== END PATH ENTRIES ===");
    }
}