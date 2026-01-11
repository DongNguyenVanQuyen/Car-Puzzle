using UnityEngine;

public class ParkingManager : MonoBehaviour
{
    public ExitZone[] exitZones;
    public LayerMask carLayer;

    public static ParkingManager Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    // Trả về target còn trống và đường thẳng không bị chặn
    public ExitZone GetAvailableTarget(Vector3 carPos)
    {
        foreach (var exit in exitZones)
        {
            if (exit.isOccupied) continue;

            Vector3 dir = (exit.transform.position - carPos).normalized;
            float distance = Vector3.Distance(carPos, exit.transform.position);

            // Raycast kiểm tra có xe chắn đường hay không
            if (!Physics.Raycast(carPos, dir, distance, carLayer))
            {
                return exit; // target hợp lệ
            }
        }

        return null; // không có target phù hợp
    }
}
