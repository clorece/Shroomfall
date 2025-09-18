using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

/*
    Will later on be changed into a A* search algorithm combined with Delaunay Triangulation/Tetrahedralization

    BSP is a way to recursively divide a space into convex subsets which is then organized into a tree data structure called a BSP tree
    - or so whatever chatgpt says lol
*/

/* unity notes
    Random.Range(inclusive value, exclusive value) "example: (0, 2) the range will actually be (0, 1)"
*/
public class BinarySpacePartitioner
{
    RoomNode rootNode;

    public BinarySpacePartitioner(int width, int length)
    {
        this.rootNode = new RoomNode(new Vector2Int(0, 0), new Vector2Int(width, length), null, 0);
    }

    public RoomNode RootNode { get => rootNode; }

    public List<RoomNode> PrepareNodesCollection(int maxIterations, int roomWidthMin, int roomLengthMin)
    {
        Queue<RoomNode> graph = new Queue<RoomNode>();
        List<RoomNode> listToReturn = new List<RoomNode>();

        graph.Enqueue(this.rootNode);
        listToReturn.Add(this.rootNode);
        int iterations = 0;

        while (iterations < maxIterations && graph.Count > 0) // graph is if graph has any more to split, hence, graphCount > 0
        {
            iterations++;

            RoomNode currentNode = graph.Dequeue(); // take first node from the graph

            if (currentNode.Width >= roomWidthMin * 2 || currentNode.Length >= roomLengthMin * 2)
            {
                SplitTheSpace(currentNode, listToReturn, roomWidthMin, roomLengthMin, graph);
            }
        }

        return listToReturn;
    }

    // create a line to split our space
    private void SplitTheSpace(RoomNode currentNode, List<RoomNode> listToReturn, int roomWidthMin, int roomLengthMin, Queue<RoomNode> graph)
    {
        Line line = GetLineDividingSpace(currentNode.BottomLeftAreaCorner,
                                            currentNode.TopRightAreaCorner,
                                            roomWidthMin,
                                            roomLengthMin
                                        );

        RoomNode node1, node2;

        // is this even right???? lol
        // this whole if else block is magic to me
        if (line.Orientation == Orientation.Horizontal)
        {
            node1 = new RoomNode(currentNode.BottomLeftAreaCorner, new Vector2Int(currentNode.TopRightAreaCorner.x, line.Coordinates.y), currentNode, currentNode.TreeLayerIndex + 1);
            node2 = new RoomNode(new Vector2Int(currentNode.BottomLeftAreaCorner.x, line.Coordinates.y), currentNode.TopRightAreaCorner, currentNode, currentNode.TreeLayerIndex + 1);

        }
        else
        {
            node1 = new RoomNode(currentNode.BottomLeftAreaCorner, new Vector2Int(line.Coordinates.x, currentNode.TopRightAreaCorner.y), currentNode, currentNode.TreeLayerIndex + 1);
            node2 = new RoomNode(new Vector2Int(line.Coordinates.x, currentNode.BottomLeftAreaCorner.y), currentNode.TopRightAreaCorner, currentNode, currentNode.TreeLayerIndex + 1);
        }

        AddNewNodesToCollections(listToReturn, graph, node1);
        AddNewNodesToCollections(listToReturn, graph, node2);
    }

    private void AddNewNodesToCollections(List<RoomNode> listToReturn, Queue<RoomNode> graph, RoomNode node)
    {
        listToReturn.Add(node);
        graph.Enqueue(node);
    }
    private Line GetLineDividingSpace(Vector2Int bottomLeftAreaCorner, Vector2Int topRightAreaCorner, int roomWidthMin, int roomLengthMin)
    {
        Orientation orientation;

        /*
            check length and width statuses to check in which direction we can divide a space

            if the width is enough, we will create a line dividing through in a vertical direction
            if both are available, we will randomly pick an orientation
            else if the length status is valid only, then we will create a horizontal line
        */

        bool widthStatus = (topRightAreaCorner.x - bottomLeftAreaCorner.x) >= (2 * roomWidthMin);
        bool lengthStatus = (topRightAreaCorner.y - bottomLeftAreaCorner.y) >= (2 * roomLengthMin);

        if (lengthStatus && widthStatus)
        {
            orientation = (Orientation)(Random.Range(0, 2));
        }
        else if (widthStatus)
        {
            orientation = Orientation.Vertical;
        }
        else
        {
            orientation = Orientation.Horizontal;
        }

        return new Line(orientation, GetCoordinatesForOrientation(orientation, bottomLeftAreaCorner, topRightAreaCorner, roomWidthMin, roomLengthMin));
    }

    private Vector2Int GetCoordinatesForOrientation(Orientation orientation, Vector2Int bottomLeftAreaCorner, Vector2Int topRightAreaCorner, int roomWidthMin, int roomLengthMin)
    {
        Vector2Int coordinates = Vector2Int.zero;

        if (orientation == Orientation.Horizontal)
        {
            // create a point between the values of the minimum and maximums
            coordinates = new Vector2Int(0, Random.Range((bottomLeftAreaCorner.y + roomLengthMin), // <= minimum : distance from the bottom left area corner + minimum room length
                                                            (topRightAreaCorner.y - roomLengthMin))); // <= maximum : distance from the top right area corner - minimum room length

        }
        else
        {
            // create a point between the values of the minimum and maximums
            coordinates = new Vector2Int(Random.Range((bottomLeftAreaCorner.x + roomWidthMin), // <= minimum : distance from the bottom left area corner + minimum room width
                                                        (topRightAreaCorner.x - roomWidthMin)), 0); // <= maximum : distance from the top right area corner - minimum room width
        }

        return coordinates;
    }
}
