using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class ChunkTileController : MonoBehaviour
{
    public Tilemap tilemap; 
    // Start is called before the first frame update
    public Vector2Int mychunkpos;
    void Start()
    {
        Vector3Int[] positions = new Vector3Int[Constants.CHUNK_SIZE * Constants.CHUNK_SIZE];
        TileBase[] tileArray = new TileBase[Constants.CHUNK_SIZE* Constants.CHUNK_SIZE];
        for(int ii =0;ii < Mathf.Pow(Constants.CHUNK_SIZE, 2); ii++) {
            tileArray[ii] = null; 
            positions[ii] = new Vector3Int((int) ii%Constants.CHUNK_SIZE, (int)ii/Constants.CHUNK_SIZE, 0);
        }
        tilemap.SetTiles(positions, tileArray);
        this.gameObject.GetComponent<BoxCollider2D>().size = new Vector2(Constants.CHUNK_SIZE,Constants.CHUNK_SIZE);
        this.gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(Constants.CHUNK_SIZE/2,Constants.CHUNK_SIZE/2);

    }

    public void fillSolidChunk(Vector2Int chunkpos, Tile tile) {
        this.gameObject.transform.position =new Vector3(chunkpos.x, chunkpos.y); 
        Vector3Int[] positions = new Vector3Int[Constants.CHUNK_SIZE * Constants.CHUNK_SIZE];
        TileBase[] tileArray = new TileBase[Constants.CHUNK_SIZE* Constants.CHUNK_SIZE];
        for(int ii =0;ii < Mathf.Pow(Constants.CHUNK_SIZE, 2); ii++) {
            if(World.world_dict[Chunks.GetChunkPos(chunkpos)][ii].matter == Matter.Solid) {
                tileArray[ii] = tile; 
            } else {
                tileArray[ii] = null; 
            }
            positions[ii] =new Vector3Int((int) ii%Constants.CHUNK_SIZE, (int)ii/Constants.CHUNK_SIZE, 0);
        }
        tilemap.SetTiles(positions, tileArray);
        mychunkpos = chunkpos;
    }

    public void drawChunkTiles(Vector2Int chunkpos) {
        element_s[] curchunk = World.world_dict[chunkpos];
        for(int ii =0;ii < Mathf.Pow(Constants.CHUNK_SIZE, 2); ii++) {
                SetTileColour(curchunk[ii].color, (Vector3Int)curchunk[ii].position, tilemap);
        }
    }

    public static void SetTileColour(Color32 colour, Vector3Int position, Tilemap tilemap)
    {
        // Flag the tile, inidicating that it can change colour.
        // By default it's set to "Lock Colour".
        tilemap.SetTileFlags(position, TileFlags.None);

        // Set the colour.
        tilemap.SetColor(position, colour);
    }

    // public void handleClearTileEvent(Vector2Int pos) {
    //     if (pos == mychunkpos) {
    //         this.gameObject.SetActive(false);
    //     }

    // }
    


}
