using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Transform targetPoint;
    public float moveSpeed = 3f;
    public float rotateSpeed = 5f;

    private List<Node> path;
    private int index = 0;
    private float baseY;

    private bool isMoving = false;
    private bool isRotating = false;

    void Start()
    {
        baseY = transform.position.y;
    }

    private void OnMouseDown()
    {
        GridManager.Instance.UpdateObstacles();

        Node start = GridManager.Instance.WorldToNode(transform.position);
        Node end = GridManager.Instance.WorldToNode(targetPoint.position);

        path = AStarPathfinding.FindPath(start, end);

        if (path == null)
        {
            Debug.Log("Không tìm được đường đi.");
            return;
        }

        index = 0;
        isMoving = true;
        isRotating = true;   // Bắt đầu bằng xoay đầu về node đầu tiên
    }

    void Update()
    {
        if (!isMoving || path == null || path.Count == 0) return;

        Node node = path[index];
        Vector3 targetPos = new Vector3(node.worldPosition.x, baseY, node.worldPosition.z);

        // Tính hướng từ xe → node
        Vector3 direction = (targetPos - transform.position).normalized;

        // Nếu còn phải xoay
        if (isRotating)
        {
            RotateTowards(direction, targetPos);
        }
        else
        {
            MoveForward(targetPos, direction);
        }
    }

    void RotateTowards(Vector3 direction, Vector3 targetPos)
    {
        if (direction.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotateSpeed * Time.deltaTime
            );

            // Khi gần giống hướng → bắt đầu chạy
            if (Quaternion.Angle(transform.rotation, targetRot) < 3f)
            {
                isRotating = false;
            }
        }
    }

    void MoveForward(Vector3 targetPos, Vector3 direction)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            index++;

            if (index >= path.Count)
            {
                isMoving = false;
                Debug.Log("Xe đến đích.");
            }
            else
            {
                isRotating = true;  // Tới node → xoay hướng cho node tiếp theo
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (path == null) return;

        Gizmos.color = Color.green;
        foreach (Node n in path)
            Gizmos.DrawSphere(n.worldPosition, 0.15f);
    }
}
