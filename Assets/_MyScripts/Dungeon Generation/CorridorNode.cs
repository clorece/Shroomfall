using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorNode : Node
{
    private Node structure1;
    private Node structure2;
    private int corridorWidth;

    public CorridorNode(Node node1, Node node2, int corridorWidth) : base(null)
    {
        this.structure1 = node1;
        this.structure2 = node2;
        this.corridorWidth = corridorWidth;

        GenerateCorridor();
    }

    private void GenerateCorridor()
    {
        var relativePosition = CheckPosition();

        switch (relativePosition)
        {
            case RelativePosition.Up:
                ProcessUpDown(this.structure1, this.structure2);
                break;
            case RelativePosition.Down:
                ProcessUpDown(this.structure2, this.structure1);
                break;
            case RelativePosition.Right:
                ProcessRightLeft(this.structure1, this.structure2);
                break;
            case RelativePosition.Left:
                ProcessRightLeft(this.structure2, this.structure1);
                break;
            default:
                break;
        }
    }

    private void ProcessRightLeft(Node structure1, Node structure2)
    {
        Node left = null;
        Node right = null;
        List<Node> leftChildren = StructureHelper.ExtractLowestLeaves(structure1);
        List<Node> rightChildren = StructureHelper.ExtractLowestLeaves(structure2);

        var sortedLeft = leftChildren.OrderByDescending(child => child.TopRightAreaCorner.x).ToList();

        // check if left structure has no children
        if (sortedLeft.Count == 1)
        {
            left = sortedLeft[0];
        }
        else
        {
            int maxX = sortedLeft[0].TopRightAreaCorner.x;

            sortedLeft = sortedLeft.Where(children => Math.Abs(maxX - children.TopRightAreaCorner.x) < 10).ToList();

            int index = UnityEngine.Random.Range(0, sortedLeft.Count); // randomly choose which index we take

            left = sortedLeft[index];
        }

        // do the same with the right
        var possibleNeighborsRight = rightChildren.Where(child => GetValidY(left.TopRightAreaCorner, left.BottomRightAreaCorner, child.TopLeftAreaCorner, child.BottomLeftAreaCorner) != -1).ToList();
    }

    private int GetValidY(Vector2Int leftUp, Vector2Int leftDown, Vector2Int rightUp, Vector2Int rightDown)
    {
        // case if left node is bigger on y's than the right node
        if (rightUp.y >= leftUp.y && leftDown.y >= rightDown.y)
        {

        }

        // reverse case
        if (rightUp.y <= leftUp.y && leftDown.y <= rightDown.y)
        {

        }

        // check if the bottom point of left node is inside the right node
        if (leftUp.y >= rightDown.y && leftUp.y <= rightUp.y)
        {

        }

        // reverse case
        if (leftDown.y >= rightDown.y && leftDown.y <= rightUp.y)
        {

        }

        return -1;
    }

    private void ProcessUpDown(Node structure1, Node structure2)
    {
        throw new NotImplementedException();
    }

    private RelativePosition CheckPosition()
    {
        // temporary middle points of structure 1 and structure 2
        Vector2 m1Temp = ((Vector2)(structure1.TopRightAreaCorner + structure1.BottomLeftAreaCorner)) / 2;
        Vector2 m2Temp = ((Vector2)(structure2.TopRightAreaCorner + structure2.BottomLeftAreaCorner)) / 2;

        float angle = CalculateAngle(m1Temp, m1Temp);

        if ((angle <= 45 && angle >= 0) || (angle > -45 && angle < 0))
        {
            return RelativePosition.Right;
        }
        else if (angle > 45 && angle < 135)
        {
            return RelativePosition.Up;
        }
        else if (angle > -135 && angle < -45)
        {
            return RelativePosition.Down;
        }
        else
        {
            return RelativePosition.Left;
        }
    }

    private float CalculateAngle(Vector2 m1, Vector2 m2)
    {
        return Mathf.Atan2(m2.y - m1.y, m2.x - m1.x) * Mathf.Rad2Deg;
    }
}
