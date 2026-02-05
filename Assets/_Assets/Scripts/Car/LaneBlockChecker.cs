using UnityEngine;

public class LaneBlockChecker : MonoBehaviour
{
    public float checkDistance = 0.8f;
    public LayerMask carLayer;
    public Transform boundPoint;

    bool isBlocked = false;
    public bool ignoreCheck = false;


    public bool IsLaneClear()
    {
        if (ignoreCheck) return true;   // Không check nữa

        Vector3 dir = GetForwardDirection();

        isBlocked = Physics.Raycast(boundPoint.position, dir, checkDistance, carLayer);

        return !isBlocked;
    }

    public bool IsLaneClearDuringMove()
    {
        if (ignoreCheck) return true;   // Không check nữa

        Vector3 dir = GetForwardDirection();

        isBlocked = Physics.Raycast(boundPoint.position, dir, 1f, carLayer);

        return !isBlocked;
    }

    Vector3 GetForwardDirection()
    {
        Vector3 dir = boundPoint.forward;
        dir.y = 0;
        return dir.normalized;
    }

    private void OnDrawGizmos()
    {
        if (boundPoint == null)
        {
            Debug.LogWarning("Bound Point is not assigned in LaneBlockChecker.");
            return;
        }

        Vector3 dir;

        if (Application.isPlaying)
        {
            dir = GetForwardDirection();
        }
        else
        {
            dir = boundPoint.forward;
            dir.y = 0;
        }

        Gizmos.color = isBlocked ? Color.red : Color.green;

        Gizmos.DrawLine(boundPoint.position, boundPoint.position + dir * checkDistance);
        Gizmos.DrawSphere(boundPoint.position + dir * checkDistance, 0.15f);
    }
}
