using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Node {
    public bool walkable;
    public Vector2Int position;

    public int gCost;
    public int hCost;
    public Node parent;

    public Node(bool walkable, Vector2Int position) {
        this.walkable = walkable;
        this.position = position;
    }

    public int fCost() {
        return gCost + hCost;
    }
}

public class Pathfinder
{
    private Grid<Node> grid;

    public void SetGrid(Grid<bool> grid) {
        this.grid = new Grid<Node>(grid.GetSize().x, grid.GetSize().y, grid.GetCellSize());
        for (int x = 0; x < grid.GetSize().x; x++) {
            for (int y = 0; y < grid.GetSize().y; y++)
            {
                this.grid.SetValueAt(x, y, new Node(grid.GetValueAt(x,y),new Vector2Int(x,y)));
            }
        }
    }

    public List<Vector2Int> FindPath(Vector2Int startingPosition, Vector2Int targetPosition) {
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        Node startingNode = new Node(true, startingPosition);
        startingNode.gCost = 0;
        Node targetNode = new Node(true, targetPosition);
        targetNode.gCost = GetDistance(startingNode, targetNode);

        openSet.Add(startingNode);

        while (openSet.Count > 0) {
            Node currentNode = openSet[0];
            for (int i = 0; i < openSet.Count; i++) {
                if (openSet[i].fCost() < currentNode.fCost() || openSet[i].fCost() == currentNode.fCost() && openSet[i].hCost < currentNode.hCost) {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode.position == targetNode.position) {
                return RetracePath(startingNode, targetNode);
            }

            foreach(Node neighbour in grid.Get8Neighbours(currentNode.position)) {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;
                    if (neighbour.position == targetNode.position)
                    {
                        targetNode = neighbour;
                    }
                    if (!openSet.Contains(neighbour)) 
                        openSet.Add(neighbour);
                }
            }
        }
        return new List<Vector2Int>();
    }

    private List<Vector2Int> RetracePath(Node startNode, Node endNode) {
        List<Vector2Int> path = new List<Vector2Int>();
        Node currentNode = endNode;
        while (currentNode.position != startNode.position) {
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }

    private int GetDistance(Node nodeA, Node nodeB) {
        int distanceX = Mathf.Abs(nodeA.position.x - nodeB.position.x);
        int distanceY = Mathf.Abs(nodeA.position.y - nodeB.position.y);
        if (distanceX > distanceY)
            return 14 * distanceY + 10 * (distanceX - distanceY);
        return 14 * distanceX + 10 * (distanceY - distanceX);
    }
}
