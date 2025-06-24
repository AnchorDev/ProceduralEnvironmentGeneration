using UnityEngine;

public class CityPlacer : MonoBehaviour
{
    [Header("Terrain & Prefab")]
    public Terrain terrain;
    public GameObject cityPrefab;
    public float citySize = 40f;
    public float cityYOffset = 1f;
    public float waterHeight = 5.5f;

    [HideInInspector] public Vector3 cityWorldPos;
    [HideInInspector] public float cityRadius;

    private TerrainData tData;
    private float[,] heights;
    private int res;
    private float maxHeight;

    private bool cityPlaced = false;

    void Start()
    {
        if (!terrain || !cityPrefab || cityPlaced) return;

        Random.InitState(System.DateTime.Now.GetHashCode());

        tData = terrain.terrainData;
        heights = tData.GetHeights(0, 0, tData.heightmapResolution, tData.heightmapResolution);
        res = tData.heightmapResolution;
        maxHeight = tData.size.y;

        TryPlaceCity();
        cityPlaced = true;
    }


void TryPlaceCity()
{
    int attempts = 1000;
    int sizeInSamples = Mathf.RoundToInt((citySize / tData.size.x) * res);

    float bestAvgHeight = 0f;
    int bestX = -1, bestY = -1;

    for (int i = 0; i < attempts; i++)
    {
        int x = Random.Range(0, res - sizeInSamples);
        int y = Random.Range(0, res - sizeInSamples);

        float avgHeight = GetAverageHeight(x, y, sizeInSamples);

        if (avgHeight > bestAvgHeight)
        {
            bestAvgHeight = avgHeight;
            bestX = x;
            bestY = y;
        }

        if (IsAreaDry(x, y, sizeInSamples))
        {
            PlaceCity(x, y, sizeInSamples, avgHeight);
            return;
        }
    }

    if (bestX != -1 && bestY != -1)
    {
        PlaceCity(bestX, bestY, sizeInSamples, bestAvgHeight);
        Debug.LogWarning("City placed at best available (non-ideal) location.");
    }
    else
    {
        Debug.LogError("City placement completely failed. This should never happen.");
    }
}


    void PlaceCity(int x, int y, int sizeInSamples, float avgHeight)
    {
        FlattenArea(x, y, sizeInSamples, avgHeight);

        Vector3 pos = HeightToWorldPos(x + sizeInSamples / 2, y + sizeInSamples / 2);
        pos.y = avgHeight * maxHeight + cityYOffset;

        Instantiate(cityPrefab, pos, Quaternion.identity, transform);
        cityWorldPos = pos;
        cityRadius = citySize * 0.6f;

        cityPlaced = true;
        Debug.Log("City placed at: " + pos);
    }



    bool IsAreaDry(int xStart, int yStart, int size)
    {
        float normWater = waterHeight / maxHeight;

        for (int y = yStart; y < yStart + size; y++)
        {
            for (int x = xStart; x < xStart + size; x++)
            {
                if (heights[y, x] < normWater + 0.01f)
                    return false;
            }
        }

        return true;
    }

    float GetAverageHeight(int xStart, int yStart, int size)
    {
        float sum = 0f;
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                sum += heights[yStart + y, xStart + x];

        return sum / (size * size);
    }

    void FlattenArea(int xStart, int yStart, int size, float flatHeight)
    {
        float[,] newHeights = new float[size, size];

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                newHeights[y, x] = flatHeight;

        tData.SetHeights(xStart, yStart, newHeights);
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
