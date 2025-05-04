using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public int width = 256;
    public int depth = 256;
    public int height = 20;
    public float scale1 = 20f;
    public float scale2 = 5f;
    public float influence = 0.5f;

    private float offsetX;
    private float offsetZ;

    private void Start()
    {
        offsetX = Random.Range(0f, 10000f);
        offsetZ = Random.Range(0f, 10000f);
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrainData(terrain.terrainData);
    }

    TerrainData GenerateTerrainData(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, height, depth);
        terrainData.SetHeights(0, 0, GenerateHeights());
        return terrainData;
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[width, depth];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                float xCoord1 = (float)x / width * scale1 + offsetX;
                float zCoord1 = (float)z / depth * scale1 + offsetZ;
                float perlinValue1 = Mathf.PerlinNoise(xCoord1, zCoord1);

                float xCoord2 = (float)x / width * scale2 + offsetX;
                float zCoord2 = (float)z / depth * scale2 + offsetZ;
                float perlinValue2 = Mathf.PerlinNoise(xCoord2, zCoord2);

                float combinedHeight = Mathf.Lerp(perlinValue1, perlinValue2, influence);

                if (combinedHeight < 0.3f)
                {
                    combinedHeight *= 0.2f;
                }

                if (combinedHeight > 0.7f)
                {
                    float t = (combinedHeight - 0.7f) / (1.0f - 0.7f);
                    float modified = Mathf.Pow(combinedHeight, 1.5f);
                    combinedHeight = Mathf.Lerp(combinedHeight, modified, t);
                }

                heights[x, z] = combinedHeight;
            }
        }
        return heights;
    }
}
