using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WallDir { Horizontal, Vertical }
public struct Cell
{
    public int x;
    public int y;

    public Cell(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static bool operator== (Cell A, Cell B)
    {
        return A.x == B.x && A.y == B.y;
    }

    public static bool operator!= (Cell A, Cell B)
    {
        return A.x != B.x && A.y != B.y;
    }

    public override bool Equals(object obj)
    {
        Cell other = (Cell)obj;
        return x == other.x && y == other.y;
    }

    public override int GetHashCode() {
        const uint hash = 0x9e3179b9;
        var seed = x + hash;
        seed ^= y + hash + (seed << 6) + (seed >> 2);
        return (int)seed;
    }
}

public struct Wall
{
    public Cell cell;
    public WallDir dir;
    
    public Wall(Cell cell, WallDir dir)
    {
        this.cell = cell;
        this.dir = dir;
    } 

    public override bool Equals(object obj)
    {
        Wall other = (Wall)obj;
        return cell == other.cell && dir == other.dir;
    }

    public override int GetHashCode() {
        const uint hash = 0x9e3779b9;
        var seed = cell.GetHashCode() + hash;
        seed ^= (int)dir + hash + (seed << 6) + (seed >> 2);
        return (int)seed;
    }

}
public class Maze
{
    public int w;
    public int h;
    public List<Wall> walls = new List<Wall>();
    public bool IsMovementAllowed(Cell cell, int direction)
    {
        int dx = 0;
        int dy = 0;
        if(cell.x < 0 || cell.x >= w || cell.y < 0 || cell.y >= h)
            return false;
        switch(direction)
        {
            case 0: dx = 1; if(cell.x >= w - 1) return false; break;
            case 1: dy = 1; if(cell.y >= h - 1) return false; break;
            case 2: dx = -1; if(cell.x <= 0) return false; break;
            case 3: dy = -1; if(cell.y <= 0) return false; break;
        }

        Cell nextCell = new Cell(cell.x + dx, cell.y + dy);
        Wall wall = new Wall();
        switch(direction)
        {
            case 0:
                wall.cell = nextCell;
                wall.dir = WallDir.Vertical;
                break;
            case 1: 
                wall.cell = nextCell;
                wall.dir = WallDir.Horizontal;
                break;
            case 2:
                wall.cell = cell;
                wall.dir = WallDir.Vertical;
                break;
            case 3: 
                wall.cell = cell;
                wall.dir = WallDir.Horizontal;
                break;
        }
        return !walls.Contains(wall);
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

    public void MakeRoom(Cell topLeftCorner, Cell bottomRightCorner, List<Cell> visitedCells, List<Wall> walls)
    {
        for(int i=topLeftCorner.x; i < bottomRightCorner.x; i++)
        {
            for(int j=topLeftCorner.y; j < bottomRightCorner.y; j++)
            {
                visitedCells.Add(new Cell(i, j));
               
            }
        }
    }
    
    public void PopulateMaze(Maze maze, List<Cell> visitedCells, ref Cell currentCell)
    {
        int dx = 0;
        int dy = 0;
        List<int> availableDirections = new List<int>();
        if(currentCell.x < w - 1) availableDirections.Add(0);
        if(currentCell.y < h - 1) availableDirections.Add(1);
        if(currentCell.x > 0) availableDirections.Add(2);
        if(currentCell.y > 0) availableDirections.Add(3);
        
        int direction = availableDirections[Random.Range(0, availableDirections.Count)];
        switch(direction)
        {
            case 0: dx = 1; break;
            case 1: dy = 1; break;
            case 2: dx = -1; break;
            case 3: dy = -1; break;
        }

        Cell nextCell = new Cell(currentCell.x + dx, currentCell.y + dy);
        if(!visitedCells.Contains(nextCell))
        {
            visitedCells.Add(nextCell);
            switch(direction)
            {
                case 0: maze.walls.Remove(new Wall(nextCell, WallDir.Vertical)); break;
                case 1: maze.walls.Remove(new Wall(nextCell, WallDir.Horizontal)); break;
                case 2: maze.walls.Remove(new Wall(currentCell, WallDir.Vertical)); break;
                case 3: maze.walls.Remove(new Wall(currentCell, WallDir.Horizontal)); break;
            }
        }
        currentCell = nextCell;
        noteCursor.localPosition = new Vector3(currentCell.x, currentCell.y, 0);
    }

    void RemoveWalls(Maze maze, List<Cell> path, List<Cell> visitedCells, List<Cell> remainingCells)
    {
        for(int i=0; i<path.Count - 1; i++)
        {
            Cell A = path[i];
            Cell B = path[i + 1];
            if(!visitedCells.Contains(B) || i == path.Count - 2)
            {
                int dx = B.x - A.x;
                int dy = B.y - A.y;
                WallDir dir = WallDir.Horizontal;
                if(dx != 0)
                    dir = WallDir.Vertical;
                if(dx < 0 || dy < 0)
                    maze.walls.Remove(new Wall(A, dir));
                else
                    maze.walls.Remove(new Wall(B, dir));
                remainingCells.Remove(B);
                visitedCells.Add(B);
            }
        }
    }

    void PopulateMaze2(Maze maze, List<Cell> visitedCells, List<Cell> remainingCells)
    {
        Cell cursor = remainingCells[Random.Range(0, remainingCells.Count)];
        remainingCells.Remove(cursor);
        List<Cell> path = new List<Cell>();
        path.Add(cursor);
        while(!visitedCells.Contains(cursor))
        {
            List<int> availableDirections = new List<int>();
            if(cursor.x < w - 1) availableDirections.Add(0);
            if(cursor.y < h - 1) availableDirections.Add(1);
            if(cursor.x > 0) availableDirections.Add(2);
            if(cursor.y > 0) availableDirections.Add(3);
            
            int direction = availableDirections[Random.Range(0, availableDirections.Count)];
            int dx = 0, dy = 0;
            switch(direction)
            {
                case 0: dx = 1; break;
                case 1: dy = 1; break;
                case 2: dx = -1; break;
                case 3: dy = -1; break;
            }
            cursor.x += dx;
            cursor.y += dy;
            path.Add(cursor);
        }
        RemoveWalls(maze, path, visitedCells, remainingCells);
    }

    public Maze GenerateMaze()
    {
        Maze result = new Maze();
        result.w = w;
        result.h = h;
        Cell currentCell = new Cell(Random.Range(0, w), Random.Range(0, h));
        for(int i=0; i <= w; i++)
        {
            for(int j=0; j <= h; j++)
            {
                if(i < w)
                    result.walls.Add(new Wall(new Cell(i, j), WallDir.Horizontal));
                if(j < h)
                    result.walls.Add(new Wall(new Cell(i, j), WallDir.Vertical));
            }
        }
        List<Cell> visitedCells = new List<Cell>();
        visitedCells.Add(currentCell);
        List<Cell> remainingCells = new List<Cell>();
        for(int i=0; i<w * h; i++)
            remainingCells.Add(new Cell(i % w, i%h));

        int genSteps = 0;
        while(remainingCells.Count > 0 && genSteps < 5000)
        {
            genSteps++;
            PopulateMaze(result, visitedCells, ref currentCell);
            remainingCells.Remove(currentCell);
        }
        while(remainingCells.Count > 0 && genSteps < 20000)
        {
            genSteps++;
            PopulateMaze2(result, visitedCells, remainingCells);
            //remainingCells.Remove(currentCell);
        }
        Debug.Log(genSteps);
        return result;
    }

    private void DisplayMaze(Maze maze)
    {
        for(int i=0; i<w; i++)
        {
            for(int j=0; j<h; j++)
            {
                Instantiate(emptyCellPrefab, transform).localPosition = new Vector3(i * cellSize, j * cellSize);
            }
        }
        foreach(Wall wall in maze.walls)
        {
            Vector3 position = Vector3.zero;
            Quaternion rotation = Quaternion.identity;
            switch(wall.dir)
            {
                case WallDir.Vertical:
                    position = new Vector3((wall.cell.x - 0.5f) * cellSize, wall.cell.y * cellSize);
                    break;
                case WallDir.Horizontal:
                    rotation = Quaternion.AngleAxis(90, Vector3.forward);
                    position = new Vector3(wall.cell.x * cellSize, (wall.cell.y - 0.5f) * cellSize);
                    break;
            }
            Transform instance = Instantiate(wallPrefab, Vector3.zero, rotation, transform);
            instance.localPosition = position;
            instance.name = "Wall " + wall.cell.x + " " + wall.cell.y;
        }
        
    }

    void Update()
    {
        
    }
}
