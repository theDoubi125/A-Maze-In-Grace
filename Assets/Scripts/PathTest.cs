using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTest : MonoBehaviour
{
    public GridGenerator mazeGenerator;
    public Transform start;
    public Transform target;

    public Transform stepPrefab;
    private List<Transform> instances = new List<Transform>();

    public int maxLength = 100;

    void Start()
    {
        
    }

    void Update()
    {
        int startCell = Mathf.RoundToInt(start.position.x) + Mathf.RoundToInt(start.position.y) * GridGenerator.instance.w;
        int targetCell = Mathf.RoundToInt(target.position.x) + Mathf.RoundToInt(target.position.y) * GridGenerator.instance.w;
        List<int> path = Path.PathFind(GridGenerator.instance, startCell, targetCell, maxLength);
        while(instances.Count < path.Count)
            instances.Add(Instantiate(stepPrefab, transform));
        for(int i=0; i<path.Count; i++)
        {
            instances[i].gameObject.SetActive(true);
            instances[i].position = new Vector2(path[i] % GridGenerator.instance.w, path[i] / GridGenerator.instance.w);
            
        }
        for(int i=path.Count; i<instances.Count; i++)
        {
            instances[i].gameObject.SetActive(false);
        }
    }
}
