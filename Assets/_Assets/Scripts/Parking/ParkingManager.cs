using UnityEngine;

public class ParkingManager : MonoBehaviour
{
    public ParkingSlot[] slots;

    public ParkingSlot GetAvailableSlot()
    {
        foreach (var slot in slots)
        {
            if (slot.IsAvailable())
                return slot;
        }
        return null;
    }
}
