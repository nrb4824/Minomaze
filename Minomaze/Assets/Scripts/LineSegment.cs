using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LineSegment
{
    public Vector2 Start;
    public Vector2 End;

    public LineSegment(Vector2 start, Vector2 end)
    {
        Start = start;
        End = end;
    }

    /// <summary>
    /// Check if this line segment intersects another line segment
    /// </summary>
    /// <param name="other"></param> The other line segment
    /// <returns></returns> True if the line segments intersect, false otherwise
    public bool Intersects(LineSegment other)
    {
        // Determine intersection of two line segments
        float o1 = Orientation(Start, End, other.Start);
        float o2 = Orientation(Start, End, other.End);
        float o3 = Orientation(other.Start, other.End, Start);
        float o4 = Orientation(other.Start, other.End, End);

        // General case: if the orientations differ, the line segments intersect
        if (o1 != o2 && o3 != o4) return true;

        // No intersection detected
        return false;
    }

    /// <summary>
    /// Determine the orientation of three points
    /// </summary>
    /// <param name="p"></param> First point
    /// <param name="q"></param> Second point
    /// <param name="r"></param> Third point
    /// <returns></returns> 0 if colinear, 1 if clockwise, 2 if counterclockwise
    private static float Orientation(Vector2 p, Vector2 q, Vector2 r)
    {
        float val = (q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y);
        if (val == 0)
        {
            return 0; // colinear
        }
        return (val > 0) ? 1 : 2; // clockwise or counterclock wise
    }

    /// <summary>
    /// Check if this line segment shares an endpoint with another line segment
    /// </summary>
    /// <param name="other"></param> The other line segment
    /// <returns></returns> True if the line segments share an endpoint, false otherwise
    public bool SharesEndpoint(LineSegment other)
    {
        return Start == other.Start || Start == other.End || End == other.Start || End == other.End;
    }
}
