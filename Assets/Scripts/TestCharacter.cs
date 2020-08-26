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

    private Animator animator;

    public System.Action<Cell> stopAtCellDelegate; 

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        time -= Time.deltaTime;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(!moving && Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit))
        {
            Vector3 worldMousePos = hit.point;
            int targetX;
            int targetY;
            int targetCell = Grid.WorldPosToCell(GridGenerator.instance, worldMousePos, out targetX, out targetY);
            int currentCell = Grid.WorldPosToCell(GridGenerator.instance, transform.position);
            if(targetX >= 0 && targetX < GridGenerator.instance.w && targetY >= 0 && targetY < GridGenerator.instance.h)
            {
                path = Path.PathFind(GridGenerator.instance, currentCell, targetCell, 10);
                path.Insert(0, currentCell);
                moving = true;
                animator.SetBool("Moving", true);
                pathTime = 0;
            }
        }
        if(moving)
        {
            pathTime += Time.deltaTime * movementSpeed;
            if(pathTime >= path.Count - 1)
            {
                animator.SetBool("Moving", false);
                moving = false;
                int currentCell = Grid.WorldPosToCell(GridGenerator.instance, transform.position);
            }
            else
            {
                int step = (int)pathTime;
                Vector3 A = Grid.CellToWorldPos(GridGenerator.instance, path[step]);
                Vector3 B = Grid.CellToWorldPos(GridGenerator.instance, path[step + 1]);
                transform.position = A + (B - A) * (pathTime - step);
                if(B.x - A.x > 0)
                    animator.SetInteger("Direction", 0);
                if(B.x - A.x < 0)
                    animator.SetInteger("Direction", 2);
                if(B.y - A.y > 0)
                    animator.SetInteger("Direction", 1);
                if(B.y - A.y < 0)
                    animator.SetInteger("Direction", 3);
            }
        }
    }
}
