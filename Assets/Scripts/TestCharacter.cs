using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharacter : MonoBehaviour
{
    public float movementSpeed = 2;
    private float time = 0;
    private List<int> path;
    private float pathTime = 0;
    private bool moving = false;

    void Start()
    {
        
    }

    void Update()
    {
        time -= Time.deltaTime;
        if(!moving && Input.GetMouseButtonDown(0))
        {
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int targetX;
            int targetY;
            int targetCell = Grid.WorldPosToCell(GridGenerator.instance, worldMousePos, out targetX, out targetY);
            int currentCell = Grid.WorldPosToCell(GridGenerator.instance, transform.position);
            if(targetX >= 0 && targetX < GridGenerator.instance.w && targetY >= 0 && targetY < GridGenerator.instance.h)
            {
                path = Path.PathFind(GridGenerator.instance, currentCell, targetCell, 10);
                path.Insert(0, currentCell);
                moving = true;
                pathTime = 0;
            }
        }
        if(moving)
        {
            pathTime += Time.deltaTime * movementSpeed;
            if(pathTime >= path.Count - 1)
            {
                moving = false;
            }
            else
            {
                int step = (int)pathTime;
                Vector3 A = Grid.CellToWorldPos(GridGenerator.instance, path[step]);
                Vector3 B = Grid.CellToWorldPos(GridGenerator.instance, path[step + 1]);
                transform.position = A + (B - A) * (pathTime - step);
            }
        }
    }
}
