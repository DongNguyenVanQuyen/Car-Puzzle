using UnityEngine;

public class SpawnRegion : MonoBehaviour
{
    public Vector2 size = new Vector2(20, 20);   // Size X-Z
    public float fixedY = 0f;                    // Y cố định để đặt xe

    public Vector3 GetRandomPoint()
    {
        float halfX = size.x * 0.5f;
        float halfZ = size.y * 0.5f;

        float x = Random.Range(transform.position.x - halfX, transform.position.x + halfX);
        float z = Random.Range(transform.position.z - halfZ, transform.position.z + halfZ);

        return new Vector3(x, fixedY, z);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(size.x, 0.1f, size.y));
    }
}
