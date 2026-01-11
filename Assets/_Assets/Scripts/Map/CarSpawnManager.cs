using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CarSpawnManager : MonoBehaviour
{
    public List<GameObject> carPrefabs;
    public int carAmount = 20;

    // Kích thước VÙNG theo local của cha
    public Vector2 areaSize = new Vector2(10f, 10f);

    public LayerMask carLayer;
    public float safetyMargin = 0.05f;
    public int maxTryPerCar = 40;

    [SerializeField] private BoxCollider box;

    void Start()
    {
        SpawnCars();
    }

    void SpawnCars()
    {
        int count = 0;

        for (int i = 0; i < carAmount; i++)
            if (SpawnOneCar()) count++;

        Debug.Log("Spawned: " + count);
    }

    bool SpawnOneCar()
    {
        GameObject prefab = carPrefabs[Random.Range(0, carPrefabs.Count)];
        BoxCollider carCol = prefab.GetComponentInChildren<BoxCollider>();

        if (carCol == null)
        {
            Debug.LogWarning(prefab.name + " missing BoxCollider");
            return false;
        }

        // real collider size theo scale prefab
        Vector3 carSize = Vector3.Scale(carCol.size, prefab.transform.localScale) + Vector3.one * safetyMargin;
        Vector3 halfExt = carSize * 0.5f;

        for (int attempt = 0; attempt < maxTryPerCar; attempt++)
        {
            // RANDOM LOCAL POS TRONG BOXCOLLIDER CHA
            Vector3 localPos = new Vector3(
                Random.Range(-box.size.x * 0.5f, box.size.x * 0.5f),
                0f,
                Random.Range(-box.size.z * 0.5f, box.size.z * 0.5f)
            );

            localPos += box.center; // bù center

            // Chuyển sang WORLD POS
            Vector3 worldPos = transform.TransformPoint(localPos);

            // 4 hướng parking jam
            Quaternion rot = Quaternion.Euler(0, 90 * Random.Range(0, 4), 0);

            // CHECK COLLISION
            if (!Physics.CheckBox(worldPos, halfExt, rot, carLayer))
            {
                Instantiate(prefab, worldPos, rot, transform);
                return true;
            }
        }

        return false;
    }
}
