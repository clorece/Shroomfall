using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class GenerateChunk : MonoBehaviour
{
    public GameObject blockGameObject;

    public int worldSizeX = 16;
    public int worldSizeY = 16;
    public int worldSizeZ = 16;
    public float detailSize = 5f;  // noise scale
    public float threshold = 0.5f; // how dense the caves are
    public float gridOffset = 1.0f;

    private Vector3 randomOffset; // Random offset for Perlin noise

    public static int seed = 0;

    private System.Random rng = new System.Random(seed);

    private float GetRandomFloat(float minValue, float maxValue) {
        return minValue + (float)rng.NextDouble() * (maxValue - minValue);
    }

    void Start()
    {
        /* 
            - create a vec3 with a random offset to apply randomized offsets for the noise
            - use a range from 0 to 1000 to determine a random number offset for each coordinate: x, y, z
        */
        randomOffset = new Vector3(
            GetRandomFloat(0f, 1000f),
            GetRandomFloat(0f, 1000f),
            GetRandomFloat(0f, 1000f)
        );

        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                for (int z = 0; z < worldSizeZ; z++)
                {
                    // apply offset to perlin noise
                    float noiseValue = Perlin3D(
                        x + randomOffset.x,
                        y + randomOffset.y,
                        z + randomOffset.z,
                        detailSize
                    );

                    // if noise value is greater than the threshold we set initially, we can render the block at that x,y,z position, else there is nothing
                    if (noiseValue > threshold)
                    {
                        Vector3 pos = new Vector3(x * gridOffset, y * gridOffset, z * gridOffset);
                        GameObject block = Instantiate(blockGameObject, pos, Quaternion.identity);
                        block.transform.SetParent(this.transform);
                    }
                }
            }
        }


    }

    /*
        Mathf.PerlinNoise is a 2D texture given by unity, so we approximate 3D noise by sampling 2D slices of the 3D space.
        In this case we use different coordinate combinations and apply the noise texture to each coordinate/axis
    */
    private float Perlin3D(float x, float y, float z, float scale)
    {
        float xy = Mathf.PerlinNoise(x / scale, y / scale);
        float yz = Mathf.PerlinNoise(y / scale, z / scale);
        float xz = Mathf.PerlinNoise(x / scale, z / scale);
        float yx = Mathf.PerlinNoise(y / scale, x / scale);
        float zy = Mathf.PerlinNoise(z / scale, y / scale);
        float zx = Mathf.PerlinNoise(z / scale, x / scale);

        return (xy + yz + xz + yx + zy + zx) / 6f;
    }
}