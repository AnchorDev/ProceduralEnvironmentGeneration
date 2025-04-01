using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public int width = 256;
    public int depth = 256;
    public int height = 20;
    public float scale = 20f;
    
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
                float xCoord = (float)x / width * scale + offsetX;
                float zCoord = (float)z / depth * scale + offsetZ;
                float perlinValue = Mathf.PerlinNoise(xCoord, zCoord);

                if (perlinValue < 0.3f)
                {
                    heights[x, z] = perlinValue * 0.2f;
                }
                else
                {
                    heights[x, z] = perlinValue;
                }
            }
        }
        return heights;
    }
}
