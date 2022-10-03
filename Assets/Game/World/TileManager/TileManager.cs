using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileManager : MonoBehaviour
{
    public static TileManager instance { get; private set; }

    [Header("World")]
    [SerializeField] private Vector2Int mapSize;
    [SerializeField] private Vector2 tileSize;
    [SerializeField] private int dropAmountIncrease;
    [SerializeField] private int initialDropAmount;
    [SerializeField] private int maximumDropAmount;
    [Header("Import")]
    [SerializeField] private GameObject tilePrefab;


    private static System.Random rnd = new System.Random();
    private Grid<Tile> tiles;
    private List<Tile> lastDroppedTiles = new List<Tile>();
    private int currentDropAmount;

    public Grid<Tile> GetTiles() {
        return tiles;
    }

    public bool TestTile(Vector2 position)
    {
        Tile tile = GetTiles().GetValueAtWorld(position);
        return tile != null && tile.isWalkable;
    }

    public void GenerateTilesToDrop()
    {
        RecalculateCurrentDropAmount();
        GenerateRandomTiles();
        foreach (Tile tile in lastDroppedTiles)
        {
            tile.Warn();
        }
    }

    public void DropTiles()
    {
        foreach (Tile tile in lastDroppedTiles)
        {
            tile.Drop();
        }
    }

    public void RegenerateTiles()
    {
        foreach (Tile tile in lastDroppedTiles)
        {
            tile.Regenerate();
        }
    }

    private void Start()
    {
        currentDropAmount = initialDropAmount - dropAmountIncrease;
        tiles = new Grid<Tile>(mapSize.x, mapSize.y, tileSize);
        Vector2 mapPosition = new Vector2(mapSize.x * tileSize.x / 2 - 0.5f, mapSize.y * tileSize.y / 2 - 0.5f);
        tiles.SetOrigin(-mapPosition + new Vector2(0.5f, 0.5f));

        Vector3 offset = new Vector3(mapPosition.x, mapPosition.y, 0);
        for (int x = 0; x < mapSize.x; x++) {
            for (int y = 0; y < mapSize.y; y++) {
                GameObject tileObject = Instantiate(tilePrefab, new Vector3(x * tileSize.x, y * tileSize.y, 0) - offset, Quaternion.identity);
                tileObject.transform.SetParent(transform);
                tiles.SetValueAt(x,y, tileObject.GetComponent<Tile>());
            }
        }
    }

    private void RecalculateCurrentDropAmount() {
        currentDropAmount = Mathf.Clamp(currentDropAmount + dropAmountIncrease, initialDropAmount, maximumDropAmount);
    }

    private void GenerateRandomTiles() {
        lastDroppedTiles.Clear();
        for (int i = 0; i < currentDropAmount; i++)
        {
            Tile tile = tiles.GetValueAt(rnd.Next(tiles.GetSize().x), rnd.Next(tiles.GetSize().y));
            lastDroppedTiles.Add(tile);
        }
    }

    private void Awake()
    {

        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
}
