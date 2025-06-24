using UnityEngine;

public class CityPlacer : MonoBehaviour
{
    [Header("Terrain & Prefab")]
    public Terrain terrain;
    public GameObject cityPrefab;
    public float citySize = 40f;
    public float minFlatness = 0.995f;
    public int sampleRadius = 4;

    [Header("Vertical Offset")]
    public float verticalOffset = 0f; // dodatkowe podniesienie, np. jeśli pivot jest pod ziemią

    [HideInInspector]
    public Vector3 cityWorldPos;

    private TerrainData tData;
    private float[,] heights;
    private int res;
    private float maxHeight;

    private float flattestDiff = float.MaxValue;
    private Vector2Int flattestPos = Vector2Int.zero;

    void Start()
    {
        if (!terrain || !cityPrefab) return;

        tData = terrain.terrainData;
        heights = tData.GetHeights(0, 0, tData.heightmapResolution, tData.heightmapResolution);
        res = tData.heightmapResolution;
        maxHeight = tData.size.y;

        TryPlaceCity();
    }

    void TryPlaceCity()
    {
        int attempts = 1000;
        int sizeInSamples = Mathf.RoundToInt((citySize / tData.size.x) * res);

        for (int i = 0; i < attempts; i++)
        {
            int x = Random.Range(sampleRadius, res - sampleRadius - sizeInSamples);
            int y = Random.Range(sampleRadius, res - sampleRadius - sizeInSamples);

            if (!IsFlat(x, y, sizeInSamples)) continue;

            // Środek miasta
            int midX = x + sizeInSamples / 2;
            int midY = y + sizeInSamples / 2;

            Vector3 pos = HeightToWorldPos(midX, midY);
            pos.y += verticalOffset;

            GameObject city = Instantiate(cityPrefab, pos, Quaternion.identity, transform);
            cityWorldPos = pos;

            Debug.Log("City placed at: " + pos);
            return;
        }

        Debug.LogWarning("Could not find flat area for city.");
        Debug.Log($"Flattest area found at ({flattestPos.x}, {flattestPos.y}) with max height difference: {flattestDiff}");
    }

    bool IsFlat(int cx, int cy, int size)
    {
        float refH = heights[cy, cx];
        float maxDiff = 0f;

        for (int y = cy; y < cy + size; y++)
        {
            for (int x = cx; x < cx + size; x++)
            {
                float h = heights[y, x];
                float diff = Mathf.Abs(h - refH);
                if (diff > maxDiff) maxDiff = diff;

                if (diff > (1f - minFlatness)) return false;
            }
        }

        if (maxDiff < flattestDiff)
        {
            flattestDiff = maxDiff;
            flattestPos = new Vector2Int(cx, cy);
        }

        return true;
    }

    Vector3 HeightToWorldPos(int x, int y)
    {
        float xf = (float)x / (res - 1);
        float yf = (float)y / (res - 1);
        float height = heights[y, x] * maxHeight;

        Vector3 pos = new Vector3(
            xf * tData.size.x,
            height,
            yf * tData.size.z
        );

        return terrain.transform.position + pos;
    }
}
