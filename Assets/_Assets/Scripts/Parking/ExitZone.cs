using UnityEngine;

public class ExitZone : MonoBehaviour
{
    public bool isOccupied = false;

    void OnTriggerEnter(Collider other)
    {
        CarController car = other.GetComponent<CarController>();
        if (car != null)
        {
          //  isOccupied = false;   // trả lại trạng thái trống
           // Destroy(car.gameObject);
        }
    }
}
