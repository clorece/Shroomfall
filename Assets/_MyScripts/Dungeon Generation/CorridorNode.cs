using UnityEngine;

public class CorridorNode
{
    private Node node1;
    private Node node2;
    private int corridorWidth;

    public CorridorNode(Node node1, Node node2, int corridorWidth)
    {
        this.node1 = node1;
        this.node2 = node2;
        this.corridorWidth = corridorWidth;
    }
}
