using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private int rows = 5;
    [SerializeField] private int cols = 5;
    public GridCell[,] grid;
}
