using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GridSpawner : MonoBehaviour
{
    [Header("Cấu hình Grid")]
    public int cellsX = 5;
    public int cellsZ = 5;

    [Header("Prefabs")]
    public GameObject[] carPrefabs;
    public Transform carParent;

    [Header("Grid Cell Prefab")]
    private Vector3[,] cellDir;
    public GameObject cellFramePrefab;   
    public Transform frameParent;       
    private float cellFrameY = 0.5f;
    private float carFrameY = 0.9f;

    private BoxCollider area;
    private Vector3[,] cellPos;
    private GameObject[,] spawnedCars;

    void Start()
    {
        area = GetComponent<BoxCollider>();
        GenerateLevel();
    }

    // GENERATE LEVEL
    public void GenerateLevel()
    {
        ClearOldLevel();

        cellPos = new Vector3[cellsX, cellsZ];
        spawnedCars = new GameObject[cellsX, cellsZ];
        cellDir = new Vector3[cellsX, cellsZ];

        BuildGrid();                      // 1. có vị trí
        SpawnGrid_NoFacingConflict();     // 2. spawn xe + hướng
        BuildFrames();                    // 3. build khung theo hướng xe
    }

    // Build vị trí cell
    void BuildGrid()
    {
        Vector3 size = area.size;
        Vector3 worldCenter = transform.TransformPoint(area.center);
        Vector3 worldSize = Vector3.Scale(size, transform.lossyScale);

        float cellSizeX = worldSize.x / cellsX;
        float cellSizeZ = worldSize.z / cellsZ;

        Vector3 origin =
            worldCenter
            - new Vector3(worldSize.x / 2, 0, worldSize.z / 2)
            + new Vector3(cellSizeX / 2, 0, cellSizeZ / 2);

        for (int x = 0; x < cellsX; x++)
        {
            for (int z = 0; z < cellsZ; z++)
            {
                cellPos[x, z] = origin + new Vector3(x * cellSizeX, carFrameY, z * cellSizeZ);
            }
        }
    }

    void BuildFrames()
    {
        Vector3 size = area.size;
        Vector3 worldSize = Vector3.Scale(size, transform.lossyScale);

        float cellSizeX = worldSize.x / cellsX;
        float cellSizeZ = worldSize.z / cellsZ;

        for (int x = 0; x < cellsX; x++)
        {
            for (int z = 0; z < cellsZ; z++)
            {
                SpawnCellFrame(
                    new Vector3(cellPos[x, z].x, cellFrameY, cellPos[x, z].z),
                    cellSizeX,
                    cellSizeZ,
                    cellDir[x, z]
                );
            }
        }
    }

    // Spawn full grid nhưng không bao giờ có xe đối đầu nhau
    void SpawnGrid_NoFacingConflict()
    {
        for (int x = 0; x < cellsX; x++)
        {
            for (int z = 0; z < cellsZ; z++)
            {
                Vector3 dir = PickValidDirection(x, z);
                cellDir[x, z] = dir; // LƯU HƯỚNG TƯƠNG ỨNG CHO Ô
                spawnedCars[x, z] = CreateCar(x, z, dir, false);
            }
        }
    }

    // Lấy hướng hợp lệ cho 1 cell
    Vector3 PickValidDirection(int cx, int cz)
    {
        List<Vector3> dirs = new List<Vector3>()
        {
            Vector3.left,
            Vector3.right,
            Vector3.forward,
            Vector3.back
        };

        Shuffle(dirs);

        foreach (var dir in dirs)
        {
            if (Conflict(cx, cz, dir)) continue;
            return dir;
        }

        // Trường hợp rất hiếm → ép quay ngang
        return Vector3.right;
    }

    // Kiểm tra xung đột (Facing Conflict)
    bool Conflict(int cx, int cz, Vector3 newDir)
    {
        // Hàng (Z cố định)
        for (int x = 0; x < cellsX; x++)
        {
            GameObject car = spawnedCars[x, cz];
            if (car == null) continue;

            Vector3 dir = car.transform.forward;

            // Cùng hàng → cấm Left đối đầu Right
            if ((dir == Vector3.left && newDir == Vector3.right) ||
                (dir == Vector3.right && newDir == Vector3.left))
                return true;
        }

        // Cột (X cố định)
        for (int z = 0; z < cellsZ; z++)
        {
            GameObject car = spawnedCars[cx, z];
            if (car == null) continue;

            Vector3 dir = car.transform.forward;

            // Cùng cột → cấm Forward đối đầu Back
            if ((dir == Vector3.forward && newDir == Vector3.back) ||
                (dir == Vector3.back && newDir == Vector3.forward))
                return true;
        }

        return false;
    }

    // Spawn car
    GameObject CreateCar(int x, int z, Vector3 lookDir, bool isTarget)
    {
        GameObject prefab = carPrefabs[Random.Range(0, carPrefabs.Length)];
        GameObject car = Instantiate(prefab, cellPos[x, z], Quaternion.LookRotation(lookDir), carParent);

        if (isTarget)
        {
            var renderer = car.GetComponentInChildren<Renderer>();
            if (renderer != null) renderer.material.color = Color.red;
        }

        return car;
    }

    // Clear Level cũ
    void ClearOldLevel()
    {
        if (carParent == null) carParent = this.transform;

        for (int i = carParent.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying) Destroy(carParent.GetChild(i).gameObject);
            else DestroyImmediate(carParent.GetChild(i).gameObject);
        }
    }


    // Dùng để xáo trộn danh sách
    void Shuffle(List<Vector3> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }


    // Dùng để spawn khung ô
    void SpawnCellFrame(Vector3 position, float sizeX, float sizeZ, Vector3 dir)
    {
        if (cellFramePrefab == null) return;

        Quaternion rot = Quaternion.LookRotation(dir == Vector3.zero ? Vector3.forward : dir);

        GameObject frame = Instantiate(
            cellFramePrefab,
            position,
            rot,
            frameParent == null ? this.transform : frameParent
        );

        frame.transform.localScale = new Vector3(sizeX, 1, sizeZ);
    }

    // Vẽ Grid
    void OnDrawGizmos()
    {
        area = GetComponent<BoxCollider>();
        if (area == null) return;

        Gizmos.color = Color.cyan;

        Vector3 worldCenter = transform.TransformPoint(area.center);
        Vector3 worldSize = Vector3.Scale(area.size, transform.lossyScale);

        Gizmos.DrawWireCube(worldCenter, worldSize);

        if (cellsX <= 0 || cellsZ <= 0) return;

        float cX = worldSize.x / cellsX;
        float cZ = worldSize.z / cellsZ;

        for (int x = 0; x < cellsX; x++)
        {
            for (int z = 0; z < cellsZ; z++)
            {
                Vector3 localPos = new Vector3(
                    -area.size.x / 2 + (area.size.x / cellsX) * (x + 0.5f),
                    0,
                    -area.size.z / 2 + (area.size.z / cellsZ) * (z + 0.5f)
                );

                Gizmos.DrawWireCube(transform.TransformPoint(localPos), new Vector3(cX, 0.1f, cZ));
            }
        }
    }
}
