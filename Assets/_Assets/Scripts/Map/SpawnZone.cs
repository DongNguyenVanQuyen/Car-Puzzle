using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    public static SpawnZone Instance;

    private void Awake()
    {
        Instance = this;
    }

    public bool IsInside(Vector3 pos)
    {
        return GetComponent<Collider>().bounds.Contains(pos);
    }
}

