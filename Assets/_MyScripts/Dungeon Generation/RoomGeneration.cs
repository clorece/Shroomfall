using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomGeneration
{
    private int maxIterations;
    private int roomWidthMin, roomLengthMin;
    public RoomGeneration(int maxI, int width, int length)
    {
        this.maxIterations = maxI;
        this.roomWidthMin = width;
        this.roomLengthMin = length;
    }

    public List<RoomNode> GenerateRooms(List<Node> roomSpaces, float bottomCornerModifier, float topCornerModifier, int roomOffset)
    {
        List<RoomNode> listToReturn = new List<RoomNode>();

        foreach (var space in roomSpaces)
        {
            // create new corner
            Vector2Int newBottomLeftPoint = StructureHelper.GenerateBottomLeftCornerBetween(space.BottomLeftAreaCorner, space.TopRightAreaCorner, bottomCornerModifier, roomOffset);
            Vector2Int newTopRightPoint = StructureHelper.GenerateTopRightCornerBetween(space.BottomLeftAreaCorner, space.TopRightAreaCorner, topCornerModifier, roomOffset);

            space.BottomLeftAreaCorner = newBottomLeftPoint;
            space.TopRightAreaCorner = newTopRightPoint;
            space.BottomRightAreaCorner = new Vector2Int(newTopRightPoint.x, newBottomLeftPoint.y);
            space.TopLeftAreaCorner = new Vector2Int(newBottomLeftPoint.x, newTopRightPoint.y);

            listToReturn.Add((RoomNode)(space));
        }

        return listToReturn;
    }
}
