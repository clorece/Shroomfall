using System.Numerics;
using UnityEngine;

/* 
    this class stores the information of the line that we have generated
    later, refer this as an object to pass to other methods
*/
public enum Orientation
{
    Horizontal = 0,
    Vertical = 1
}

public class Line
{
    Orientation orientation;
    Vector2Int coordinates;

    public Line(Orientation orient, Vector2Int coords)
    {
        this.orientation = orient;
        this.coordinates = coords;
    }

    public Orientation Orientation { get => orientation; set => orientation = value; }
    public Vector2Int Coordinates { get => coordinates; set => coordinates = value; }
}
