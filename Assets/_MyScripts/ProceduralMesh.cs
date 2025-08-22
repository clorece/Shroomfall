using UnityEngine;
using System.Collections.Generic;

public class ProceduralMesh : MonoBehaviour
{
    public Material material = null;
    public GameObject player;

    // Half-extent in each axis, in cells (the chunk will be 2*GridSize + 1 cells per axis)
    public Vector3Int GridSize = new Vector3Int(64, 64, 64);

    public float zoom = 4f;
    [Range(0f, 1f)] public float noiselimit = 0.5f;

    private Mesh mesh = null;

    // Optional cache of generated cell owners per world cell
    private readonly Dictionary<Vector3Int, GameObject> container = new Dictionary<Vector3Int, GameObject>();

    // The integer world position at which the current chunk is centered
    private Vector3Int startPosition; // (center)

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("[ProceduralMesh] Player reference is missing.");
            enabled = false;
            return;
        }

        // Center the initial chunk on the player's integer position
        startPosition = WorldFloorToInt(player.transform.position);

        BuildChunk();
    }

    private void Update()
    {
        Vector3Int playerCell = WorldFloorToInt(player.transform.position);
        Vector3Int move = playerCell - startPosition;

        // Rebuild if the player moved at least 1 whole world unit in any axis
        if (Mathf.Abs(move.x) >= 1 || Mathf.Abs(move.y) >= 1 || Mathf.Abs(move.z) >= 1)
        {
            startPosition = playerCell;
            BuildChunk();
        }
    }

    private void BuildChunk()
    {
        // Compute array extents (we’ll use 2*GridSize + 1 in each axis so the chunk is centered)
        int sx = GridSize.x * 2 + 1;
        int sy = GridSize.y * 2 + 1;
        int sz = GridSize.z * 2 + 1;

        // Allocate marching-cubes grid
        MarchingCube.grd = new GridPoint[sx, sy, sz];

        // Fill positions & Off flags
        // Local index space [0..sx-1], map to world coords centered at startPosition
        // world = startPosition + (ix - GridSize.x, iy - GridSize.y, iz - GridSize.z)
        for (int iz = 0; iz < sz; iz++)
        {
            int wz = startPosition.z + (iz - GridSize.z);
            for (int iy = 0; iy < sy; iy++)
            {
                int wy = startPosition.y + (iy - GridSize.y);
                for (int ix = 0; ix < sx; ix++)
                {
                    int wx = startPosition.x + (ix - GridSize.x);

                    var gp = new GridPoint();
                    gp.Position = new Vector3(wx, wy, wz);
                    gp.On = false;

                    MarchingCube.grd[ix, iy, iz] = gp;

                    // Optional: track which GameObject “owns” this world cell
                    var cell = new Vector3Int(wx, wy, wz);
                    if (!container.ContainsKey(cell))
                        container.Add(cell, this.gameObject);
                }
            }
        }

        // Populate density using 3D noise
        Noise3D();

        // March & build mesh
        GameObject go = this.gameObject;
        mesh = MarchingCube.GetMesh(ref go, ref material);
        MarchingCube.Clear();
        MarchingCube.MarchCubes();
        MarchingCube.SetMesh(ref mesh);
    }

    private void Noise3D()
    {
        int sx = MarchingCube.grd.GetLength(0);
        int sy = MarchingCube.grd.GetLength(1);
        int sz = MarchingCube.grd.GetLength(2);

        // Use the same centered mapping used in BuildChunk for normalized coordinates
        for (int iz = 0; iz < sz; iz++)
        {
            int wz = startPosition.z + (iz - GridSize.z);
            float nz = ((float)wz / (float)(GridSize.z * 2 + 1)) * zoom;

            for (int iy = 0; iy < sy; iy++)
            {
                int wy = startPosition.y + (iy - GridSize.y);
                float ny = ((float)wy / (float)(GridSize.y * 2 + 1)) * zoom;

                for (int ix = 0; ix < sx; ix++)
                {
                    int wx = startPosition.x + (ix - GridSize.x);
                    float nx = ((float)wx / (float)(GridSize.x * 2 + 1)) * zoom;

                    float noise = MarchingCube.PerlinNoise3D(nx, ny, nz); // 0..1
                    MarchingCube.grd[ix, iy, iz].On = (noise > noiselimit);
                }
            }
        }
    }

    // Helpers
    private static Vector3Int WorldFloorToInt(Vector3 v)
    {
        return new Vector3Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z));
    }

    // (Optional) Expose these if you still want to read them elsewhere
    public int xPlayerLocation => Mathf.FloorToInt(player.transform.position.x);
    public int yPlayerLocation => Mathf.FloorToInt(player.transform.position.y);
    public int zPlayerLocation => Mathf.FloorToInt(player.transform.position.z);
    public int xPlayerMove => xPlayerLocation - startPosition.x;
    public int yPlayerMove => yPlayerLocation - startPosition.y;
    public int zPlayerMove => zPlayerLocation - startPosition.z;
}
