using UnityEngine;

public class CarInput : MonoBehaviour
{
    [SerializeField] private CarController carController;

    void Start()
    {
        carController = GetComponent<CarController>();
    }

    void OnMouseDown()
    {
        carController.isActivated = true;   // CHỈ XE NÀY HOẠT ĐỘNG
        carController.TryMoveByPlayer();    // CHỈ XE NÀY CHẠY
    }


}
