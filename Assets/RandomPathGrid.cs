using System.Collections.Generic;
using UnityEngine;

public class RandomPathGrid : MonoBehaviour
{
    [Header("Grid")]
    public int width = 10;
    public int height = 10;
    public float tileSize = 2f;
    public float gap = 0.1f;

    [Header("Prefabs")]
    public GameObject safeTilePrefab;
    public GameObject deathTilePrefab;

    [Header("Player")]
    public Transform player;

    [Header("Path")]
    public bool randomizeOnStart = true;

    private Vector3 playerStartPosition;
    private Quaternion playerStartRotation;

    private HashSet<Vector2Int> safePath =
        new HashSet<Vector2Int>();

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player er ikke sat!");
            return;
        }

        playerStartPosition = player.position;
        playerStartRotation = player.rotation;

        GenerateGrid();
    }

    void GenerateGrid()
    {
        safePath.Clear();

        // Tilfældig start på nederste række
        int x = Random.Range(0, width);
        int y = 0;

        safePath.Add(new Vector2Int(x, y));

        // Arbejd os op mod sidste række
        while (y < height - 1)
        {
            int move = Random.Range(0, 3);

            // 0 = venstre
            // 1 = højre
            // 2 = frem

            if (move == 0 && x > 0)
            {
                x--;
            }
            else if (move == 1 && x < width - 1)
            {
                x++;
            }
            else
            {
                y++;
            }

            safePath.Add(new Vector2Int(x, y));
        }

        // Lav alle tiles
        for (int gridX = 0; gridX < width; gridX++)
        {
            for (int gridY = 0; gridY < height; gridY++)
            {
                Vector2Int gridPosition =
                    new Vector2Int(gridX, gridY);

                Vector3 worldPosition =
                    transform.position +
                    new Vector3(
                        gridX * tileSize,
                        0f,
                        gridY * tileSize
                    );

                GameObject prefab;

                if (safePath.Contains(gridPosition))
                {
                    prefab = safeTilePrefab;
                }
                else
                {
                    prefab = deathTilePrefab;
                }

                GameObject tile = Instantiate(
                    prefab,
                    worldPosition,
                    Quaternion.identity,
                    transform
                );

                tile.name =
                    safePath.Contains(gridPosition)
                    ? $"Safe_{gridX}_{gridY}"
                    : $"Death_{gridX}_{gridY}";

                // Lille mellemrum mellem felterne
                tile.transform.localScale =
                    new Vector3(
                        tileSize - gap,
                        0.2f,
                        tileSize - gap
                    );

                // Fortæl death tiles hvor Player skal sendes hen
                DeathTile deathTile =
                    tile.GetComponent<DeathTile>();

                if (deathTile != null)
                {
                    deathTile.player = player;
                    deathTile.respawnPosition =
                        playerStartPosition;
                    deathTile.respawnRotation =
                        playerStartRotation;
                }
            }
        }
    }
}