using UnityEngine;
using System.Collections;

public class CarController : MonoBehaviour
{
    public float moveSpeed = 4f;

    private bool isMoving = false;
    private bool isRotating = false;

    private LaneBlockChecker checker;
    private CameraShake cameraShake;

    private Vector3 currentDir;

    [SerializeField] private CellDirection lastCell = null;
    public LayerMask cellLayer;

    public bool isActivated = false;   // mặc định: xe đứng im, không trigger

    private void Start()
    {
        checker = GetComponent<LaneBlockChecker>();
        cameraShake = Camera.main.GetComponent<CameraShake>();
        currentDir = transform.forward;
    }

    private void Update()
    {
        if (!isActivated) return;   // CHẶN XE KHI CHƯA ĐƯỢC BẬT

        if (isMoving && !isRotating)
        {
            MoveForward();
        }
    }

    public void TryMoveByPlayer()
    {
        if (!checker.IsLaneClear())
        {
            StartCoroutine(cameraShake.Shake(0.15f, 0.15f));
            return;
        }

        checker.ignoreCheck = true;
        isMoving = true;
    }

    private void StopMoving()
    {
        isMoving = false;
        checker.ignoreCheck = false;
    }

    private void MoveForward()
    {
        if (!checker.IsLaneClearDuringMove())
        {
            StopMoving();
            return;
        }

        Vector3 dir = currentDir;
        dir.y = 0;

        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActivated) return;
        if (other.gameObject.layer != LayerMask.NameToLayer("CellTrigger"))
            return;

        var cell = other.GetComponent<CellDirection>() ??
                   other.GetComponentInParent<CellDirection>();

        if (cell == null) return;

        lastCell = cell;

        // KHÔNG xoay ngay, KHÔNG chạy tiếp
        isMoving = false;

        // Đưa xe vào tâm cell → rồi mới xoay + chạy tiếp
        StartCoroutine(MoveToCellCenter(cell));
    }

    // Di chuyển xe vào tâm cell
    private IEnumerator MoveToCellCenter(CellDirection cell)
    {
        // Lấy tâm collider của cell
        Vector3 target = cell.GetCenter();
        target.y = transform.position.y;

        float speed = moveSpeed * 1.2f;  // vào giữa cell hơi nhanh cho mượt

        // Chạy vào tâm cell
        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                speed * Time.deltaTime
            );

            yield return null;
        }

        // Snap lại chính xác
        transform.position = target;

        // Lấy hướng di chuyển của cell
        Vector3 newDir = cell.GetMoveDirection();
        newDir.y = 0;

        // Xoay nếu hướng khác
        if (newDir != Vector3.zero && newDir != currentDir)
            StartCoroutine(RotateTo(newDir));
        else
            isMoving = true;   // không cần xoay → chạy tiếp
    }



    // Xoay hướng khi sang cell khác
    private IEnumerator RotateTo(Vector3 newDir)
    {
        isRotating = true;
        isMoving = false;

        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.LookRotation(newDir);

        float duration = 0.25f;
        float t = 0;

        while (t < duration)
        {
            transform.rotation = Quaternion.Slerp(startRot, endRot, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRot;
        currentDir = newDir;

        isRotating = false;
        isMoving = true;
    }
}
