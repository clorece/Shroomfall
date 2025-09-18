using UnityEngine;

public class DungeonCreator : MonoBehaviour
{
    public int dungeonWidth, dungeonLength, dungeonFloors;
    public int roomWidthMin, roomLengthMin;
    public int corridorWidth;
    public int maxIterations;
    void Start()
    {
        CreateDungeon();
    }

    void Update()
    {

    }

    void CreateDungeon()
    {
        DungeonGenerator generator = new DungeonGenerator(dungeonWidth, dungeonLength);

        var listOfRooms = generator.CalculateRooms(maxIterations, roomWidthMin, roomLengthMin);
    }
}