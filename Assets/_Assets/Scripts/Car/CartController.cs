using UnityEngine;

public class CarController : MonoBehaviour
{
    public float moveSpeed = 4f;
    public bool isMoving = false;

    private LaneBlockChecker checker;
    private CameraShake cameraShake;

    void Start()
    {
        checker = GetComponent<LaneBlockChecker>();
        cameraShake = Camera.main.GetComponent<CameraShake>();
    }

    void Update()
    {
        if (isMoving)
        {
            MoveForward();
        }
    }

    public void TryMoveByPlayer()
    {
        Debug.Log("Player requested move");
        // Kiểm tra trước khi bắt đầu chạy
        if (!checker.IsLaneClear())
        {
            Debug.Log("Lane is blocked, cannot move");
            StartCoroutine(cameraShake.Shake(0.15f, 0.15f));
            return;
        }
        Debug.Log("Lane is clear, starting to move");
        // Bắt đầu chạy thẳng
        isMoving = true;
    }

    void MoveForward()
    {
        if (!checker.IsLaneClearDuringMove())
        {
            isMoving = false;
            return;
        }

        Vector3 dir = checker.boundPoint.forward;
        dir.y = 0;

        transform.position += dir * moveSpeed * Time.deltaTime;
    }

}
