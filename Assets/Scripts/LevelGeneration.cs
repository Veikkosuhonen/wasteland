using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour {

    [SerializeField]
    private int mapWidthInTiles, mapDepthInTiles;

    [SerializeField]
    private GameObject tilePrefab;

    [SerializeField]
    private float mapScale;

    [SerializeField]
    private float heightMultiplier;

    void Start() {
        GenerateMap();
    }

    void GenerateMap() {
        // get the tile dimensions from the tile Prefab
        Vector3 tileSize = tilePrefab.GetComponent<MeshRenderer>().bounds.size;

        int tileWidth = (int)tileSize.x;
        int tileDepth = (int)tileSize.z;

        // for each Tile, instantiate a Tile in the correct position
        for (int xTileIndex = 0; xTileIndex < mapWidthInTiles; xTileIndex++) {
            for (int zTileIndex = 0; zTileIndex < mapDepthInTiles; zTileIndex++) {
                // calculate the tile position based on the X and Z indices
                Vector3 tilePosition = new(
                    gameObject.transform.position.x + xTileIndex * tileWidth,
                    gameObject.transform.position.y,
                    gameObject.transform.position.z + zTileIndex * tileDepth
                );

                // instantiate a new Tile
                GameObject tileObject = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
                var tile = tileObject.GetComponent<TileGeneration>();
                tile.MapScale = mapScale;
                tile.HeightMultiplier = heightMultiplier;

            }
        }
    }
}
