using UnityEngine;

public class ParkingSlot : MonoBehaviour
{
    [SerializeField] private Transform parkingDirection;

    public bool isReserved = false;
    public bool isOccupied = false;
    

    public bool IsAvailable()
    {
        return !isReserved && !isOccupied;
    }

    public Vector3 GetParkingOrientation()
    {
        return parkingDirection.forward;
    }
}
