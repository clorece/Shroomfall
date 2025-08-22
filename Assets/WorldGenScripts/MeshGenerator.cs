/*using UnityEngine;

public class MeshGenerator : MonoBehaviour {
    public int worldSizeX = 10;
    public int worldSizeY = 10;
    public int worldSizeZ = 0;

    private Mesh mesh;

    private int[] triangles;
    private Vector3[] vertices;

    void Start() {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        GenerateMesh();
        UpdateMesh();
    }

    // generate our mesh
    void GenerateMesh() {
        // uncomment all the commented lines to start testing noise generation

        triangles = new int[worldSizeX * worldSizeZ * 6];
        vertices = new Vector3[(worldSizeX + 1) * (worldSizeZ + 1)];

        //triangles = new int[worldSizeX * worldSizeY * worldSizeZ * 9];
        //vertices = new int[(WorldSizeX + 1) * (WorldSizeY + 1) * (WorldSizeZ + 1)];

        // when adding 3d noise for cave generation, we might have to switch positions of z and x for loops
        for (int i = 0, z = 0; z < worldSizeZ; z++) {
            //for (int y = 0; y < worldSizeY; y++) {
            for (int x = 0; x < worldSizeX; x++) {
                vertices[i] = new Vector3(x, 0, z);
                //vertices[] = new Vector3[x, y, z];

                i++;
            }
            //}
        }

        int tris = 0;
        int verts = 0;

        // when adding 3d noise for cave generation, we might have to switch positions of z and x for loops
        // IF MESH GENERATOR WORKS PLEASE LOOK OVER THIS AGAIN
        for (int z = 0; z < worldSizeZ; z++) {
            //for (int y = 0; y < worldSizeY; y++) {
            for (int x = 0; x < worldSizeX; x++) {
                triangles[tris + 0] = verts + 0; 
                triangles[tris + 1] = verts + worldSizeX + 1;
                triangles[tris + 2] = verts + 1;

                triangles[tris + 3] = verts + 1;
                triangles[tris + 4] = verts + worldSizeX + 1;
                triangles[tris + 5] = verts + worldSizeX + 2;
                
                verts++;
                tris += 6;
            }

            verts++;
            //}
        }
    }

    // assign and load our mesh
    void UpdateMesh() {
        mesh.Clear(); // clear any meshes that might already exist
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }
}*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    --- Optional Enhancers ---
    ToDo: Implement noise smoothing (Theres a tutorial for this on my channel ;)
    --------------------------
*/
public class MeshGenerator : MonoBehaviour
{
    public int Worldx;
    public int Worldy;
    public int Worldz;
        
    private Vector3[] vertices;
    private int[] triangles;

    private MeshCollider GetMeshCollider
    {
        get
        {
            return GetComponent<MeshCollider>();
        }
    }

    private MeshFilter GetMeshFilter
    {
        get
        {
            return GetComponent<MeshFilter>();
        }
    }

    void Start()
    {
        generateMesh();
    }

    // Method that does our mesh stuff :)
    private void generateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "ProceduralMesh";

        mesh.Clear(); // clear any meshes that might already exist

        triangles = new int[Worldx * Worldz * 6];
        vertices = new Vector3[(Worldx + 1) * (Worldz + 1)];

        for (int i = 0, z = 0; z <= Worldz; z++)
        {
            for (int y = 0; y < Worldy; y++) 
            {
            for (int x = 0; x <= Worldx; x++)
            {
                //vertices[i] = new Vector3(x, Mathf.PerlinNoise(x * 2, z * .3f) / .3f, z);
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
            }
        }

        int tris = 0;
        int verts = 0;

        for (int z = 0; z < Worldz; z++)
        {
            for (int x = 0; x < Worldx; x++)
            {
                triangles[tris + 0] = verts + 0;
                triangles[tris + 1] = verts + Worldz + 1;
                triangles[tris + 2] = verts + 1;

                triangles[tris + 3] = verts + 1;
                triangles[tris + 4] = verts + Worldz + 1;
                triangles[tris + 5] = verts + Worldz + 2;

                verts++;
                tris += 6;
            }
            verts++;
        }

        // update mesh
        mesh.vertices = vertices; // this must be here before triangles
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        GetMeshFilter.mesh = mesh;
        GetMeshCollider.sharedMesh = mesh;

    }

    // 3D Perlin Noise for meshes to wrap around/position at
    /*
    private float Perlin3D(float x, float y, float z, float scale) {
        float xy = Mathf.PerlinNoise(x / scale, y / scale);
        float yz = Mathf.PerlinNoise(y / scale, z / scale);
        float xz = Mathf.PerlinNoise(x / scale, z / scale);
        float yx = Mathf.PerlinNoise(y / scale, x / scale);
        float zy = Mathf.PerlinNoise(z / scale, y / scale);
        float zx = Mathf.PerlinNoise(z / scale, x / scale);

        return (xy + yz + xz + yx + zy + zx) / 6f;
    }
    */
}
