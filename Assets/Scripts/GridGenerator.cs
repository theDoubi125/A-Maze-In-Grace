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
    public int id;
    public WallDir dir;
    public static Wall CreateWall(int id, WallDir dir)
    {
        Wall result = new Wall();
        result.id = id;
        result.dir = dir;
        return result;
    }

    public override bool Equals(object obj)
    {
        Wall other = (Wall)obj;
        return id == other.id && dir == other.dir;
    }

    public override int GetHashCode() {
        const uint hash = 0x9e3779b9;
        var seed = id + hash;
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
                wall.id = nextCell.x + nextCell.y * (w + 1);
                wall.dir = WallDir.Vertical;
                break;
            case 1: 
                wall.id = nextCell.x + nextCell.y * w;
                wall.dir = WallDir.Horizontal;
                break;
            case 2:
                wall.id = cell.x + cell.y * (w + 1);
                wall.dir = WallDir.Vertical;
                break;
            case 3: 
                wall.id = cell.x + cell.y * w;
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
        Cell currentCell = new Cell(Random.Range(0, w), Random.Range(0, h));
        for(int i=0; i <= w; i++)
        {
            for(int j=0; j <= h; j++)
            {
                if(i < w)
                    result.walls.Add(Wall.CreateWall(i + j * w, WallDir.Horizontal));
                if(j < h)
                    result.walls.Add(Wall.CreateWall(i + j * (w + 1), WallDir.Vertical));
            }
        }
        List<Cell> visitedCells = new List<Cell>();
        for(int i=4; i<6; i++)
        {
            for(int j=2; j<4; j++)
            {
                visitedCells.Add(new Cell(i, j));
            }
        }

        int genSteps = 0;
        while(visitedCells.Count < w * h && genSteps < 100000)
        {
            genSteps++;
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
                    case 0: result.walls.Remove(Wall.CreateWall(nextCell.x + nextCell.y * (w + 1), WallDir.Vertical)); break;
                    case 1: result.walls.Remove(Wall.CreateWall(nextCell.x + nextCell.y * w, WallDir.Horizontal)); break;
                    case 2: result.walls.Remove(Wall.CreateWall(currentCell.x + currentCell.y * (w + 1), WallDir.Vertical)); break;
                    case 3: result.walls.Remove(Wall.CreateWall(currentCell.x + currentCell.y * w, WallDir.Horizontal)); break;
                }
            }
            currentCell = nextCell;
            noteCursor.localPosition = new Vector3(currentCell.x, currentCell.y, 0);
        }
        Debug.Log(genSteps);
        return result;
    }

    private void DisplayMaze(Maze maze)
    {
        foreach(Wall wall in maze.walls)
        {
            int x = 0, y = 0;
            Vector3 position = Vector3.zero;
            Quaternion rotation = Quaternion.identity;
            switch(wall.dir)
            {
                case WallDir.Vertical:
                    x = wall.id % (w + 1);
                    y = wall.id / (w + 1);
                    position = new Vector3((x - 0.5f) * cellSize, y * cellSize);
                    break;
                case WallDir.Horizontal:
                    x = wall.id % w;
                    y = wall.id / w;
                    rotation = Quaternion.AngleAxis(90, Vector3.forward);
                    position = new Vector3(x * cellSize, (y - 0.5f) * cellSize);
                    break;
            }
            Transform instance = Instantiate(wallPrefab, Vector3.zero, rotation, transform);
            instance.localPosition = position;
            instance.name = " Wall " + x + " " + y;
        }
        
    }

    void Update()
    {
        
    }
}
