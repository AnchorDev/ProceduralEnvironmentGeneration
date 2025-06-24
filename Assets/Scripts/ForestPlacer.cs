using UnityEngine;

public class ForestPlacer : MonoBehaviour
{
    [Header("Terrain and Tree Prefabs")]
    public Terrain terrain;
    public GameObject[] treePrefabs;

    [Header("Forest Settings")]
    public int maxTrees = 500;
    public float minFlatness = 0.995f;
    public int sampleRadius = 2;
    public float spacing = 4f;

    [Header("Avoid")]
    public float riverThreshold = 0.02f;
    public float waterHeight = 5.5f;

    [Header("City Avoidance")]
    public Vector3 cityPos;
    public float cityAvoidRadius = 25f;

    private TerrainData tData;
    private float[,] heights;
    private int res;
    private float maxHeight;

    void Start()
    {
        if (!terrain || treePrefabs == null || treePrefabs.Length == 0) return;

        tData = terrain.terrainData;
        heights = tData.GetHeights(0, 0, tData.heightmapResolution, tData.heightmapResolution);
        res = tData.heightmapResolution;
        maxHeight = tData.size.y;

        GenerateForests();
    }

    void GenerateForests()
    {
        int treesPlaced = 0;

        for (int i = 0; i < 50000 && treesPlaced < maxTrees; i++)
        {
            int x = Random.Range(sampleRadius, res - sampleRadius);
            int y = Random.Range(sampleRadius, res - sampleRadius);

            if (!IsFlat(x, y) || IsNearRiverOrWater(x, y)) continue;

            Vector3 pos = HeightToWorldPos(x, y);
            if (Vector3.Distance(pos, cityPos) < cityAvoidRadius) continue;

            GameObject chosenTree = treePrefabs[Random.Range(0, treePrefabs.Length)];
            Vector3 jitter = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * spacing * 0.3f;
            Instantiate(chosenTree, pos + jitter, Quaternion.Euler(0, Random.Range(0f, 360f), 0), transform);

            treesPlaced++;
        }

        Debug.Log($"Placed {treesPlaced} trees.");
    }

    bool IsFlat(int x, int y)
    {
        float center = heights[y, x];
        for (int oy = -sampleRadius; oy <= sampleRadius; oy++)
            for (int ox = -sampleRadius; ox <= sampleRadius; ox++)
                if (ox != 0 || oy != 0)
                    if (Mathf.Abs(center - heights[y + oy, x + ox]) > (1f - minFlatness))
                        return false;
        return true;
    }

    bool IsNearRiverOrWater(int x, int y)
    {
        float height = heights[y, x] * maxHeight;
        return height < waterHeight + 0.2f;
    }

    Vector3 HeightToWorldPos(int x, int y)
    {
        float xf = (float)x / (res - 1);
        float yf = (float)y / (res - 1);
        float height = heights[y, x] * maxHeight;

        return terrain.transform.position + new Vector3(
            xf * tData.size.x,
            height,
            yf * tData.size.z
        );
    }
}
