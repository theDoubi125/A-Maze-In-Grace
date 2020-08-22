using UnityEngine;
public class Grid
{
    public static int OffsetCell(Maze maze, int cell, int offsetX, int offsetY)
    {
        int cellX = cell % maze.w;
        int cellY = cell / maze.w;
        return cellX + offsetX + (cellY + offsetY) * maze.w;
    }

    public static int GetNeighbourCell(Maze maze, int cell, int dir)
    {
        switch(dir)
        {
            case 0:
                return OffsetCell(maze, cell, 1, 0);
            case 1:
                return OffsetCell(maze, cell, 0, 1);
            case 2:
                return OffsetCell(maze, cell, -1, 0);
            case 3:
                return OffsetCell(maze, cell, 0, -1);
        }
        return cell;
    }

    public static int CellSqrdDistance(Maze maze, int A, int B)
    {
        int xA = A % maze.w;
        int yA = A / maze.w;
        int xB = B % maze.w;
        int yB = B / maze.w;
        int dx = xB - xA;
        int dy = yB - yA;
        return dx * dx + dy * dy;
    }
    public static int WorldPosToCell(Maze maze, Vector3 pos)
    {
        int x = Mathf.RoundToInt(pos.x);
        int y = Mathf.RoundToInt(pos.y);
        return x + y * maze.w;
    }

    public static int WorldPosToCell(Maze maze, Vector3 pos, out int x, out int y)
    {
        x = Mathf.RoundToInt(pos.x);
        y = Mathf.RoundToInt(pos.y);
        return x + y * maze.w;
    }

    public static Vector3 CellToWorldPos(Maze maze, int cell)
    {
        int x = cell % maze.w;
        int y = cell / maze.w;
        return new Vector3(x, y, 0);
    }
}