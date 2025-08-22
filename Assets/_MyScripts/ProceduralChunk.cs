using UnityEngine;

public class ProceduralChunk : MonoBehaviour
{
    private Mesh mesh;

    // Build (or rebuild) this chunk's mesh
    public void Build(Vector3Int chunkCoord, Vector3Int chunkSize, Material material, float zoom, float noiseLimit)
    {
        // Points/vertices grid has +1 along each axis to form 'chunkSize' cubes.
        int px = chunkSize.x + 1;
        int py = chunkSize.y + 1;
        int pz = chunkSize.z + 1;

        // Allocate marching grid
        MarchingCube.grd = new GridPoint[px, py, pz];

        // This chunk's world origin
        Vector3 worldOrigin = new Vector3(
            chunkCoord.x * chunkSize.x,
            chunkCoord.y * chunkSize.y,
            chunkCoord.z * chunkSize.z
        );

        // Fill positions (LOCAL space) and set On via noise sampled in WORLD space
        for (int z = 0; z < pz; z++)
        {
            for (int y = 0; y < py; y++)
            {
                for (int x = 0; x < px; x++)
                {
                    var gp = new GridPoint();

                    // Vertex position in LOCAL space (so the mesh stays fixed under this GameObject)
                    gp.Position = new Vector3(x, y, z);

                    // Sample noise in WORLD space for seamless tiling between chunks
                    float wx = (worldOrigin.x + x) / (float)chunkSize.x * zoom;
                    float wy = (worldOrigin.y + y) / (float)chunkSize.y * zoom;
                    float wz = (worldOrigin.z + z) / (float)chunkSize.z * zoom;
                    float n = MarchingCube.PerlinNoise3D(wx, wy, wz);

                    gp.On = (n > noiseLimit);
                    MarchingCube.grd[x, y, z] = gp;
                }
            }
        }

        // Get or create mesh on this GO
        var go = this.gameObject;
        mesh = MarchingCube.GetMesh(ref go, ref material);

        // Run marching on this local grid and set the mesh
        MarchingCube.Clear();
        MarchingCube.MarchCubes();
        MarchingCube.SetMesh(ref mesh);
    }
}
