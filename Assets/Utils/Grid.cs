using System.Collections.Generic;
using UnityEngine;
/// Basic grid system
/// Layout:
///   0 1 2 3
/// 0 * - - -
/// 1 | \
/// 2 |   \
/// 3 |     \
public class Grid<T>
{
    protected int width,height;
    protected Vector2 cellSize;
    protected T[,] dataArray;
    protected Vector2 origin;

    public Grid(int width, int height, Vector2 cellSize){
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        dataArray = new T[width,height];
    }
    #region Setting
    public void SetCellSize(Vector2 cellSize) { 
        this.cellSize = cellSize; 
    }

    public void SetOrigin(float x, float y) {
        origin = new Vector2(x, y);
    }
    public void SetOrigin(Vector2 position)
    {
        origin = position;
    }

    public void SetValueAt(int x, int y, T value) {
        if (!isPositionValid(x, y)) return;
        dataArray[x, y] = value;
    }
    public void SetValueAt(Vector2Int position, T value)
    {
        if (!isPositionValid(position)) return;
        dataArray[position.x, position.y] = value;
    }

    #endregion
    #region Getting

    public Vector2Int GetSize()
    {
        return new Vector2Int(width, height);
    }
    public Vector2 GetCellSize() {
        return cellSize;
    }

    public T GetValueAt(int x, int y) {
        if (!isPositionValid(x, y)) return default;
        return dataArray[x, y];
    }
    public T GetValueAt(Vector2Int position)
    {
        if (!isPositionValid(position)) return default;
        return dataArray[position.x, position.y];
    }

    public Vector2Int GetCellAtWorld(float x, float y) {
        int xCoord = Mathf.FloorToInt((origin.x + x) / cellSize.x);
        int yCoord = Mathf.FloorToInt((origin.y + y) / cellSize.y);
        return new Vector2Int(xCoord, yCoord);
    }
    public Vector2Int GetCellAtWorld(Vector2 position)
    {
        int xCoord = Mathf.FloorToInt((position.x - origin.x + cellSize.x) / cellSize.x);
        int yCoord = Mathf.FloorToInt((position.y - origin.y + cellSize.y) / cellSize.y);
        return new Vector2Int(xCoord, yCoord);
    }

    public T GetValueAtWorld(float x, float y) {
        return GetValueAt(GetCellAtWorld(x, y));
    }
    public T GetValueAtWorld(Vector2 position)
    {
        return GetValueAt(GetCellAtWorld(position));
    }

    public Vector2 GetWorldPosition(int x, int y) {
        return origin + new Vector2(x * cellSize.x, y * cellSize.y);
    }
    public Vector2 GetWorldPosition(Vector2Int position)
    {
        return origin + position * cellSize;
    }

    public List<T> Get8Neighbours(Vector2Int position) {
        List<T> neighbours = new List<T>();
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++){
                if (x == 0 && y == 0) continue;
                int cx = position.x + x;
                int cy = position.y + y;
                if (cx >= 0 && cx < width && cy >= 0 && cy < height) {
                    neighbours.Add(dataArray[cx, cy]);
                }
            }
        }

        return neighbours;
    }

    public List<T> Get4Neighbours(Vector2Int position)
    {
        List<T> neighbours = new List<T>();
        Vector2Int[] positions = new Vector2Int[] { new Vector2Int(-1,0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1) };
        for (int i = 0; i < 4; i++) {
            int cx = position.x + positions[i].x;
            int cy = position.y + positions[i].y;
            if (cx >= 0 && cx < width && cy >= 0 && cy < height)
            {
                neighbours.Add(dataArray[cx, cy]);
            }
        }
        return neighbours;
    }

    public List<Vector2Int> Get4NeighboursPositions(Vector2Int position)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        Vector2Int[] positionsToCheck = new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1) };
        for (int i = 0; i < 4; i++)
        {
            if (isPositionValid(positionsToCheck[i]))
            {
                positions.Add(position + positionsToCheck[i]);
            }
        }
        return positions;
    }


    #endregion
    #region Essential
    public bool isPositionValid(int x, int y) {
        if (x < 0 || x > width - 1) return false;
        if (y < 0 || y > height - 1) return false;
        return true;
    }
    public bool isPositionValid(Vector2Int position)
    {
        if (position.x < 0 || position.x > width - 1) return false;
        if (position.y < 0 || position.y > height - 1) return false;
        return true;
    }
    #endregion
}
