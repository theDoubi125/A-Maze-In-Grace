using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    List<int> cells = new List<int>();

    private struct PathCell
    {
        public int cell;
        public int distance;
    }

    private static List<int> BuildPathFromPrecedents(int startCell, int targetCell, Dictionary<int, int> precedents)
    {
        List<int> result = new List<int>();
        int cursor = targetCell;
        while(cursor != startCell)
        {
            result.Add(cursor);
            cursor = precedents[cursor];
        }
        result.Reverse();
        return result;
    }

    public static List<int> PathFind(Maze maze, int startCell, int targetCell, int maxLength)
    {
        Dictionary<int, int> cellDistances = new Dictionary<int, int>();
        Dictionary<int, int> previousCells = new Dictionary<int, int>();
        cellDistances.Add(startCell, 0);
        List<PathCell> toCheck = new List<PathCell>();
        PathCell startPathCell = new PathCell();
        startPathCell.cell = startCell;
        startPathCell.distance = 0;
        toCheck.Add(startPathCell);
        int closestCell = startCell;
        int iterationCount = 0;

        while(toCheck.Count > 0 && toCheck[0].distance <= maxLength)
        {
            iterationCount++;
            PathCell pathCell = toCheck[0];
            toCheck.RemoveAt(0);
            for(int dir=0; dir<4; dir++)
            {
                int nextCell = Grid.GetNeighbourCell(maze, pathCell.cell, dir);
                Cell cell = new Cell(pathCell.cell % maze.w, pathCell.cell / maze.w);
                if(maze.IsMovementAllowed(cell, dir) && (!cellDistances.ContainsKey(nextCell) || cellDistances[nextCell] > pathCell.distance + 1))
                {
                    previousCells[nextCell] = pathCell.cell;
                    cellDistances[nextCell] = pathCell.distance + 1;
                    PathCell nextPathCell = new PathCell();
                    nextPathCell.cell = nextCell;
                    nextPathCell.distance = pathCell.distance + 1;
                    if(nextPathCell.cell == targetCell)
                    {
                        return BuildPathFromPrecedents(startCell, targetCell, previousCells);
                    }
                    toCheck.Add(nextPathCell);
                    if(Grid.CellSqrdDistance(maze, closestCell, targetCell) > Grid.CellSqrdDistance(maze, nextCell, targetCell))
                        closestCell = nextCell;
                    toCheck.Sort(delegate(PathCell A, PathCell B){
                        int ADist = Grid.CellSqrdDistance(maze, A.cell, targetCell);
                        int BDist = Grid.CellSqrdDistance(maze, B.cell, targetCell);
                        return ADist.CompareTo(BDist);
                    });
                }
            }
        }

        return BuildPathFromPrecedents(startCell, closestCell, previousCells);
    }
}



