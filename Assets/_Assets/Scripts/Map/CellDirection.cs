using UnityEngine;

public class CellDirection : MonoBehaviour
{
    public Vector3 direction = Vector3.forward;   // Hướng mũi tên của ô

    public Collider triggerCollider;

    // Lấy vị trí trung tâm của ô
    public Vector3 GetCenter()
    {
        return triggerCollider.bounds.center;
    }

    // Lấy hướng di chuyển dựa trên hướng mũi tên của ô
    public Vector3 GetMoveDirection()
    {
        return transform.forward;
    }
}
