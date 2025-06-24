using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public Terrain terrain;
    public GameObject cityPrefab;
    public GameObject[] treePrefabs;

    private CityPlacer cityPlacer;
    private ForestPlacer forestPlacer;

    void Start()
    {
        cityPlacer = gameObject.AddComponent<CityPlacer>();
        cityPlacer.terrain = terrain;
        cityPlacer.cityPrefab = cityPrefab;

        forestPlacer = gameObject.AddComponent<ForestPlacer>();
        forestPlacer.terrain = terrain;
        forestPlacer.treePrefabs = treePrefabs;

        Invoke(nameof(DelayedForestPlacement), 0.5f);
    }

    void DelayedForestPlacement()
    {
        forestPlacer.cityPos = cityPlacer.cityWorldPos;
        forestPlacer.cityAvoidRadius = 25f;
    }
}
