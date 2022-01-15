

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;
// using System.Linq; //for the foreach loop thing
using UnityEngine.Jobs;

public static class WorldGen {
    public static element_s[] ChunkGen(Vector2Int chunkpos) {
        element_s[] fish = new element_s[(int)Mathf.Pow(Constants.CHUNK_SIZE, 2)];

        // List<Vector2Int> vlist = Chunks.GetLinearList(chunkpos+ new Vector2Int(8,8), chunkpos + new Vector2Int(14,15));
        float val = 0.5f; 
        if (chunkpos.y < 0|| chunkpos.x != 0) { //  ){ // 
            val = 1f; 
        }
        for(int ii =0;ii < Mathf.Pow(Constants.CHUNK_SIZE, 2); ii++) {
            // // // cur index in chunk is [ii + ii * Constants.CHUNK_SIZE]
            if (UnityEngine.Random.Range(0f, 1f) > val ){//&& Chunks.mod(ii,Constants.CHUNK_SIZE) == 0) {
                fish[ii] = e_gen.Sand(chunkpos + new Vector2Int((int)ii % Constants.CHUNK_SIZE, (int)ii/Constants.CHUNK_SIZE));
            } else {
                fish[ii] = new element_s(chunkpos + new Vector2Int((int)ii % Constants.CHUNK_SIZE, (int)ii/Constants.CHUNK_SIZE));
            }
        }
        return fish;

    }


}