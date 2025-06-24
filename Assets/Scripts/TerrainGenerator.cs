using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class TerrainGenerator : MonoBehaviour
{
    [Header("Terrain Size")]
    public int terrainSize = 256;
    public int heightmapResolution = 257;
    public float maxHeight = 20f;

    [Header("Noise Settings")]
    public float noiseScale1 = 0.07f;
    public float noiseScale2 = 0.1f;
    [Range(0f, 1f)] public float noiseMix = 0.5f;

    [Header("Mountains")]
    public int mountainCount = 6;
    public float mountainMinRadius = 30f;
    public float mountainMaxRadius = 50f;
    public float mountainMinHeight = 0.6f;
    public float mountainMaxHeight = 0.85f;

    [Header("Rivers")]
    public float riverStartThreshold = 0.7f;
    public float riverWidth = 2f;

    [Header("Water")]
    public float waterLevel = 5.5f;
    public Transform waterPlane;

    private TerrainData tData;
    private bool[,] isRiver;

    void Start()
    {
        var terrain = GetComponent<Terrain>();
        tData = terrain.terrainData;

        tData.heightmapResolution = heightmapResolution;
        tData.size = new Vector3(terrainSize, maxHeight, terrainSize);

        float[,] h = new float[heightmapResolution, heightmapResolution];
        isRiver = new bool[heightmapResolution, heightmapResolution];

        GenerateBase(h);
        AddMountains(h);
        CarveRiverSinusoidal(h);
        ApplyHeights(h);

        if (waterPlane)
        {
            var p = waterPlane.position;
            waterPlane.position = new Vector3(p.x, waterLevel, p.z);
        }
    }

    void GenerateBase(float[,] h)
    {
        int res = heightmapResolution;
        for (int y = 0; y < res; y++)
        {
            for (int x = 0; x < res; x++)
            {
                float nx = (float)x / res;
                float ny = (float)y / res;

                float n1 = Mathf.PerlinNoise(nx / noiseScale1, ny / noiseScale1);
                float n2 = Mathf.PerlinNoise((nx + 1000) / noiseScale2, (ny + 1000) / noiseScale2);

                h[y, x] = Mathf.Lerp(n1, n2, noiseMix);
            }
        }
    }

    void AddMountains(float[,] h)
    {
        int res = heightmapResolution;
        for (int i = 0; i < mountainCount; i++)
        {
            int cx = Random.Range(0, res);
            int cy = Random.Range(0, res);
            float radius = Random.Range(mountainMinRadius, mountainMaxRadius);
            float peak = Random.Range(mountainMinHeight, mountainMaxHeight);

            for (int y = 0; y < res; y++)
            {
                for (int x = 0; x < res; x++)
                {
                    if (isRiver[y, x]) continue;

                    float dx = (x - cx) / radius;
                    float dy = (y - cy) / radius;
                    float d2 = dx * dx + dy * dy;

                    if (d2 < 1f)
                    {
                        float height = peak * (1f - d2);
                        h[y, x] = Mathf.Max(h[y, x], height);
                    }
                }
            }
        }
    }

    void CarveRiverSinusoidal(float[,] h)
    {
        int res = heightmapResolution;
        float waterNorm = waterLevel / maxHeight;
        float riverRadius = 5f;

        for (int i = 0; i < res; i++)
        {
            float y = res * 0.5f + Mathf.Sin(i * 0.03f) * res * 0.2f;
            int iy = Mathf.RoundToInt(y);

            for (int dy = -4; dy <= 4; dy++)
            {
                for (int dx = -4; dx <= 4; dx++)
                {
                    int xi = i + dx;
                    int yi = iy + dy;
                    if (xi < 0 || xi >= res || yi < 0 || yi >= res) continue;

                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    if (dist > riverRadius + 1f) continue;

                    float t = Mathf.InverseLerp(riverRadius + 1f, 0f, dist);
                    float targetHeight = Mathf.Lerp(h[yi, xi], waterNorm, t);

                    h[yi, xi] = Mathf.Min(h[yi, xi], targetHeight);
                    isRiver[yi, xi] = true;
                }
            }
        }
    }

    void ApplyHeights(float[,] h)
    {
        int res = heightmapResolution;
        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
                h[y, x] = Mathf.Clamp01(h[y, x]);

        tData.SetHeights(0, 0, h);
    }
}
