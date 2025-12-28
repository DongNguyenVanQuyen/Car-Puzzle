using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private ParkingManager parkingManager;
    [SerializeField] private ParkingSlot parkingSlot;


    private bool isParkingIn = false;

    public float moveSpeed = 3f;
    public float rotateSpeed = 5f;

    private List<Node> path;
    private int index = 0;
    private float baseY;

    private bool isMoving = false;
    private bool isRotating = false;

    private void Awake()
    {
        if (parkingManager == null)
            parkingManager = FindObjectOfType<ParkingManager>();
    }

    void Start()
    {
        baseY = transform.position.y;
    }

    private void OnMouseDown()
    {
        if (isMoving || isParkingIn)
        {
            Debug.Log("Xe đang chạy.");
            return;
        }

        // Check 4 hướng – chỉ cần 1 hướng thoáng
        if (!HasAnyFreeDirection())
        {
            Debug.Log("Xe bị kẹt – không thể di chuyển.");
            return;
        }

        parkingSlot = parkingManager.GetAvailableSlot();

        if (parkingSlot == null)
        {
            Debug.Log("Không còn chỗ đậu.");
            return;
        }

        parkingSlot.isReserved = true;

        GridManager.Instance.UpdateObstacles();

        Node start = GridManager.Instance.WorldToNode(transform.position);
        Node end = GridManager.Instance.WorldToNode(parkingSlot.transform.position);

        path = AStarPathfinding.FindPath(start, end);

        if (path == null || path.Count == 0)
        {
            Debug.Log("Không tìm được đường đi!");
            parkingSlot.isReserved = false;
            return;
        }

        index = 0;
        isMoving = true;
        isRotating = true;

        Debug.Log("Xe bắt đầu di chuyển...");
    }

    void Update()
    {
        // Đang tự chạy vào slot
        if (isParkingIn)
        {
            MoveIntoParkingSlot();
            return;
        }

        // Đang chạy path A*
        if (!isMoving || path == null || path.Count == 0) return;

        Node node = path[index];
        Vector3 targetPos = new Vector3(node.worldPosition.x, baseY, node.worldPosition.z);

        Vector3 direction = (targetPos - transform.position).normalized;

        if (isRotating)
            RotateTowards(direction);
        else
            MoveForward(targetPos, direction);
    }

    // -------------------- ROTATION --------------------

    void RotateTowards(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.0001f)
            return;

        Quaternion targetRot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);

        if (Quaternion.Angle(transform.rotation, targetRot) < 3f)
            isRotating = false;
    }

    // -------------------- MOVE --------------------

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
                ArriveDestination();
            else
                isRotating = true;  // Tới node → xoay node tiếp theo
        }
    }

    // -------------------- ARRIVE --------------------

    void ArriveDestination()
    {
        isMoving = false;

        parkingSlot.isReserved = false;
        parkingSlot.isOccupied = true;

        // Xoay về hướng đậu
        Vector3 dir = parkingSlot.GetParkingOrientation();
        if (dir != Vector3.zero)
        {
            Quaternion correctRot = Quaternion.LookRotation(dir);
            transform.rotation = correctRot;
        }

        isParkingIn = true;

        Debug.Log("Xe tới cửa bãi → đang tiến vào slot...");
    }

    // -------------------- ENTER SLOT --------------------

    void MoveIntoParkingSlot()
    {
        Vector3 finalPos = parkingSlot.transform.position;
        finalPos.y = baseY; // Giữ nguyên Y

        transform.position = Vector3.MoveTowards(
            transform.position,
            finalPos,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, finalPos) < 0.05f)
        {
            isParkingIn = false;
            Debug.Log("Xe đậu thành công!");
        }
    }

    // -------------------- BLOCK CHECKING --------------------

    bool IsDirectionClear(Vector3 dir)
    {
        float checkDist = 1.1f;
        LayerMask mask = LayerMask.GetMask("Car");

        return !Physics.Raycast(
            transform.position + Vector3.up * 0.5f,
            dir,
            checkDist,
            mask
        );
    }

    bool HasAnyFreeDirection()
    {
        bool up = IsDirectionClear(Vector3.forward);
        bool down = IsDirectionClear(Vector3.back);
        bool left = IsDirectionClear(Vector3.left);
        bool right = IsDirectionClear(Vector3.right);

        return up || down || left || right;
    }

    // -------------------- DEBUG --------------------

    private void OnDrawGizmos()
    {
        if (path == null) return;

        Gizmos.color = Color.green;
        foreach (Node n in path)
            Gizmos.DrawSphere(n.worldPosition, 0.15f);
    }
}
