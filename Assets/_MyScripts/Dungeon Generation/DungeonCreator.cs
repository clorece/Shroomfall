using UnityEngine;

public class DungeonCreator : MonoBehaviour
{
    public int dungeonWidth, dungeonLength, dungeonFloors;
    public int roomWidthMin, roomLengthMin;
    public int roomOffset;
    public int corridorWidth;
    public int maxIterations;
    [Range(0.0f, 0.3f)]
    public float bottomCornerModifier;
    [Range(0.7f, 1.0f)]
    public float topCornerModifier;
    public Material material;

    /*
    public GameObject floorPrefab; // <- assign in Inspector
    private float cellSize = 1f;     // world units per grid cell (your generator’s unit)
    private float yOffset = 0f;      // height to place rooms (e.g., 0 for floor)
    private Transform roomsParent;   // optional parent for hierarchy
    */
    void Start()
    {
        CreateDungeon();
    }

    /*
    void Update()
    {

    }
    */

    private void CreateDungeon()
    {
        DungeonGenerator generator = new DungeonGenerator(dungeonWidth, dungeonLength);

        var listOfRooms = generator.CalculateDungeon(maxIterations, roomWidthMin, roomLengthMin, bottomCornerModifier, topCornerModifier, roomOffset, corridorWidth);

        for (int i = 0; i < listOfRooms.Count; i++)
        {
            CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner);
        }
        
        /*
        for (int i = 0; i < listOfRooms.Count; i++)
        {
            Create(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner);
        }
        */
        
    }

    
    private void CreateMesh(Vector2 bottomLeft, Vector2 topRight)
    {
        Vector3 bottomLeftCorner = new Vector3(bottomLeft.x, 0, bottomLeft.y);
        Vector3 bottomRightCorner = new Vector3(topRight.x, 0, bottomLeft.y);
        Vector3 topLeftCorner = new Vector3(bottomLeft.x, 0, topRight.y);
        Vector3 topRightCorner = new Vector3(topRight.x, 0, topRight.y);

        // create an area of vertices
        Vector3[] vertices = new Vector3[] {
            // order here matters as it will have to do with the triangles of our mesh
            topLeftCorner,
            topRightCorner,
            bottomLeftCorner,
            bottomRightCorner
        };

        Vector2[] uv = new Vector2[vertices.Length];

        for (int i = 0; i < uv.Length; i++)
        {
            uv[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        int[] triangles = new int[] {
            // vertices indices
            0,
            1,
            2,
            // end of vertices indices
            2,
            1,
            3
        };

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        GameObject dungeonFloor = new GameObject("Mesh" + bottomLeft, typeof(MeshFilter), typeof(MeshRenderer));

        // place accordingly
        dungeonFloor.transform.position = Vector3.zero;
        dungeonFloor.transform.localScale = Vector3.one;
        dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
        dungeonFloor.GetComponent<MeshRenderer>().material = material;
    }


    //instead of creating a mesh, spawn prefabs
    /*
    private void Create(Vector2 bottomLeft, Vector2 topRight)
    {
        // floor
        if (floorPrefab == null) { Debug.LogWarning("Room Prefab not assigned."); return; }

        int widthCells = Mathf.CeilToInt(topRight.x - bottomLeft.x);
        int lengthCells = Mathf.CeilToInt(topRight.y - bottomLeft.y);

        for (int x = 0; x < widthCells; x++)
        {
            for (int z = 0; z < lengthCells; z++)
            {
                Vector3 pos = new Vector3(
                    (bottomLeft.x + x + 0.5f) * cellSize, // +0.5 to center each tile in its cell
                    yOffset,
                    (bottomLeft.y + z + 0.5f) * cellSize
                );

                Instantiate(floorPrefab, pos, Quaternion.identity, roomsParent);

                // in case we make a new floor asset and it requires for material to be seperate from the prefab
                //GameObject floor = Instantiate(floorPrefab, pos, Quaternion.identity, roomsParent);

                //if (material != null)
                //{
                //    Renderer rend = floor.GetComponent<Renderer>();
                //    if (rend != null)
                //    {
                //        rend.material = material;
                //    }
                //}
            }
        }
        // end of floor

        // walls (to be implemented)
        // end of walls
    }
    */
    
}