using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DungeonGenerator
{
    private int dungeonWidth, dungeonLength;
    RoomNode rootNode;
    List<RoomNode> allSpaceNodes = new List<RoomNode>();

    public DungeonGenerator(int width, int length)
    {
        this.dungeonWidth = width;
        this.dungeonLength = length;
    }

    public List<Node> CalculateRooms(int maxIterations, int roomWidthMin, int roomLengthMin)
    {
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(dungeonWidth, dungeonLength);

        allSpaceNodes = bsp.PrepareNodesCollection(maxIterations, roomWidthMin, roomLengthMin);

        return new List<Node>(allSpaceNodes);
    }
}
