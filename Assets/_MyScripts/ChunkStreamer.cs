using System.Collections.Generic;
using UnityEngine;

public class ChunkStreamer : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Material material;

    [Header("Chunk Settings")]
    // Number of cubes along each axis in one chunk (not points).
    public Vector3Int chunkSize = new Vector3Int(16, 16, 16);
    // How many chunks out from the player to keep (Manhattan/chebyshev radius)
    public int viewRadius = 2;

    [Header("Noise")]
    public float zoom = 1.0f;
    [Range(0f, 1f)] public float noiseLimit = 0.5f;

    // Active chunks: key = chunk coord (in chunk units), value = chunk component
    private readonly Dictionary<Vector3Int, ProceduralChunk> live = new Dictionary<Vector3Int, ProceduralChunk>();
    // Pool of disabled chunk GOs we can reuse
    private readonly Stack<ProceduralChunk> pool = new Stack<ProceduralChunk>();

    // Integer chunk coord from world pos
    private Vector3Int WorldToChunk(Vector3 world) {
        return new Vector3Int(
            Mathf.FloorToInt(world.x / (float)chunkSize.x),
            Mathf.FloorToInt(world.y / (float)chunkSize.y),
            Mathf.FloorToInt(world.z / (float)chunkSize.z)
        );
    }

    private void Update()
    {
        if (player == null) return;

        var playerChunk = WorldToChunk(player.position);

        // Determine desired set
        var desired = new HashSet<Vector3Int>();
        for (int dz = -viewRadius; dz <= viewRadius; dz++)
        for (int dy = -viewRadius; dy <= viewRadius; dy++)
        for (int dx = -viewRadius; dx <= viewRadius; dx++)
        {
            var c = new Vector3Int(playerChunk.x + dx, playerChunk.y + dy, playerChunk.z + dz);
            desired.Add(c);
            if (!live.ContainsKey(c))
                SpawnOrReuseChunk(c);
        }

        // Deactivate chunks that are no longer desired (move to pool)
        var toDisable = new List<Vector3Int>();
        foreach (var kv in live)
            if (!desired.Contains(kv.Key))
                toDisable.Add(kv.Key);

        foreach (var key in toDisable)
        {
            var ch = live[key];
            live.Remove(key);
            ch.gameObject.SetActive(false);
            pool.Push(ch);
        }
    }

    private void SpawnOrReuseChunk(Vector3Int chunkCoord)
    {
        ProceduralChunk chunk;
        if (pool.Count > 0)
        {
            chunk = pool.Pop();
            chunk.gameObject.SetActive(true);
        }
        else
        {
            var go = new GameObject($"Chunk {chunkCoord}");
            go.transform.parent = transform;
            chunk = go.AddComponent<ProceduralChunk>();
        }

        // Position the GameObject at the chunk's world origin
        Vector3 origin = new Vector3(
            chunkCoord.x * chunkSize.x,
            chunkCoord.y * chunkSize.y,
            chunkCoord.z * chunkSize.z
        );
        chunk.transform.position = origin;

        // Build/refresh its mesh (local-space vertices, world-space sampling)
        chunk.Build(
            chunkCoord: chunkCoord,
            chunkSize: chunkSize,
            material: material,
            zoom: zoom,
            noiseLimit: noiseLimit
        );

        live[chunkCoord] = chunk;
        chunk.name = $"Chunk {chunkCoord}";
    }
}
