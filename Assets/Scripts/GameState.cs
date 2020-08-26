using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public GridGenerator generator;
    public Maze maze;

    void Start()
    {
        maze = generator.GenerateMaze(5, 5);
        generator.DisplayMaze(maze);
    }

    void Update()
    {
        
    }
}
