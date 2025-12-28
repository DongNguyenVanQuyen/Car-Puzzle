using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public SpawnRegion region;
    public GameObject carPrefab;
    public Transform parentFolder;

    public int carCount = 20;
    public float checkRadius = 1f;

    public LayerMask obstacleLayer;

    void Start()
    {
        SpawnCars();
    }

    void SpawnCars()
    {
        if (region == null || carPrefab == null)
        {
            Debug.LogError("Chưa gán region hoặc carPrefab!");
            return;
        }

        for (int i = 0; i < carCount; i++)
        {
            Vector3 spawnPos;
            int tries = 0;

            do
            {
                spawnPos = region.GetRandomPoint();
                tries++;
            }
            while (!IsAreaFree(spawnPos) && tries < 15);

            if (tries >= 15)
            {
                Debug.LogWarning("Không tìm được vị trí trống để spawn xe: " + i);
                continue;
            }

            Quaternion rot = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            Instantiate(carPrefab, spawnPos, rot, parentFolder);
        }
    }

    bool IsAreaFree(Vector3 position)
    {
        return !Physics.CheckSphere(position, checkRadius, obstacleLayer);
    }
}
