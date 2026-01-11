using UnityEngine;

public class TestingGrid : MonoBehaviour
{

    [SerializeField] private int x;
    [SerializeField] private int y;
    [SerializeField] private float cellSize;
    void Start()
    {
        GridMap grid = new GridMap(x,y,cellSize);        
    }


}
