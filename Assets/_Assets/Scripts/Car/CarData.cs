using UnityEngine;

[CreateAssetMenu(menuName = "Parking/Car Database")]
public class CarData : ScriptableObject
{
    public CarType[] cars;
}