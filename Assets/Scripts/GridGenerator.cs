using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze
{
    public int w;
    public int h;
    public List<int> hWalls = new List<int>();
    public List<int> vWalls = new List<int>();
    public bool IsMovementAllowed(int cell, int direction)
    {
        int cellX = cell % w;
        int cellY = cell / w;
        int dx = 0;
        int dy = 0;
        if(cellX < 0 || cellX >= w || cellY < 0 || cellY >= h)
            return false;
        switch(direction)
        {
            case 0: dx = 1; if(cellX >= w - 1) return false; break;
            case 1: dy = 1; if(cellY >= h - 1) return false; break;
            case 2: dx = -1; if(cellX <= 0) return false; break;
            case 3: dy = -1; if(cellY <= 0) return false; break;
        }

        int nextX = cellX + dx;
        int nextY = cellY + dy;
        int nextCell = nextX + nextY * w;
        switch(direction)
        {
            case 0: return !vWalls.Contains(nextX + nextY * (w + 1));
            case 1: return !hWalls.Contains(nextX + nextY * w);
            case 2: return !vWalls.Contains(cellX + cellY * (w + 1));
            case 3: return !hWalls.Contains(cellX + cellY * w);
        }
        return false;
    }
}

public class GridGenerator : MonoBehaviour
{
    public int w, h;
    public Transform emptyCellPrefab;
    public Transform wallPrefab;
    public float cellSize;
    public Transform noteCursor;
    public static Maze instance;

    private void Start()
    {
        instance = GenerateMaze();
        DisplayMaze(instance);
    }

    public Maze GenerateMaze()
    {
        Maze result = new Maze();
        result.w = w;
        result.h = h;
        for(int i=0; i<w; i++)
        {
            for(int j=0; j<h; j++)
            {
                Instantiate(emptyCellPrefab, transform).localPosition = new Vector3(i * cellSize, j * cellSize);
            }
        }
        List<int> visitedCells = new List<int>();
        int currentCell = Random.Range(0, w * h);
        for(int i=0; i <= w; i++)
        {
            for(int j=0; j <= h; j++)
            {
                if(i < w)
                    result.hWalls.Add(i + j * w);
                if(j < h)
                    result.vWalls.Add(i + j * (w + 1));
            }
        }

        int genSteps = 0;
        while(visitedCells.Count < w * h - 1 && genSteps < 100000)
        {
            genSteps++;
            int dx = 0;
            int dy = 0;
            List<int> availableDirections = new List<int>();
            int currentX = currentCell % w;
            int currentY = currentCell / w;
            if(currentX < w - 1) availableDirections.Add(0);
            if(currentY < h - 1) availableDirections.Add(1);
            if(currentX > 0) availableDirections.Add(2);
            if(currentY > 0) availableDirections.Add(3);
            
            int direction = availableDirections[Random.Range(0, availableDirections.Count)];
            switch(direction)
            {
                case 0: dx = 1; break;
                case 1: dy = 1; break;
                case 2: dx = -1; break;
                case 3: dy = -1; break;
            }

            int nextX = currentX + dx;
            int nextY = currentY + dy;
            int nextCell = nextX + nextY * w;
            if(!visitedCells.Contains(nextCell))
            {
                visitedCells.Add(nextCell);
                switch(direction)
                {
                    case 0: result.vWalls.Remove(nextX + nextY * (w + 1)); break;
                    case 1: result.hWalls.Remove(nextX + nextY * w); break;
                    case 2: result.vWalls.Remove(currentX + currentY * (w + 1)); break;
                    case 3: result.hWalls.Remove(currentX + currentY * w); break;
                }
            }
            currentCell = nextCell;
            noteCursor.localPosition = new Vector3(currentX, currentY, 0);
        }
        Debug.Log(genSteps);
        return result;
    }

    private void DisplayMaze(Maze maze)
    {
        foreach(int wallIndex in maze.hWalls)
        {
            int x = wallIndex % w;
            int y = wallIndex / w;
            Transform instance = Instantiate(wallPrefab, Vector3.zero, Quaternion.AngleAxis(90, Vector3.forward), transform);
            instance.localPosition = new Vector3(x * cellSize, (y - 0.5f) * cellSize);
            instance.name = " V " + x + " " + y;
        }
        
        foreach(int wallIndex in maze.vWalls)
        {
            int x = wallIndex % (w + 1);
            int y = wallIndex / (w + 1);
            Transform instance = Instantiate(wallPrefab, Vector3.zero, Quaternion.identity, transform);
            instance.localPosition = new Vector3((x - 0.5f) * cellSize, y * cellSize);
            instance.name = " H " + x + " " + y;
        }
    }

    void Update()
    {
        
    }
}
