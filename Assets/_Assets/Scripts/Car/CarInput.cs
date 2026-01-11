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
        carController.TryMoveByPlayer();
    }

}
