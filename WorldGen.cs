

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
    public static bool hasinit = false;
    public static int seed;
    private static Vector2 perlinOffset;
    private static readonly float perlinOffsetMax = 10000f;
    private static float perlinAddition;

    public static void init() {
        SetSeed(); 
        perlinOffset = new Vector2(
            UnityEngine.Random.Range(0f, perlinOffsetMax),
            UnityEngine.Random.Range(0f, perlinOffsetMax));
        
        hasinit = true;
    }

    public static void SetSeed(int newSeed = -1)
    {
        seed = newSeed == -1 ? (int)System.DateTime.Now.Ticks : newSeed;
        UnityEngine.Random.InitState(seed);
    }

    public static (float, float) getCurrentPerlin(Vector2Int pos) {

        float noiseX, noiseY;       
        noiseX = (pos.x + perlinOffset.x) * 0.01f;
        noiseY = (pos.y + perlinOffset.y) * 0.01f;

        return (Mathf.Clamp(Mathf.PerlinNoise(noiseX, noiseY), 0f, 1f), 
            Mathf.Clamp(Mathf.PerlinNoise(noiseX + Constants.CHUNK_SIZE + Constants.CHUNK_SIZE/2, noiseY), 0f, 1f));
    }


    public static void ChunkGen(Vector2Int chunkpos) {
        element_s[] wall = new element_s[(int)Mathf.Pow(Constants.CHUNK_SIZE, 2)];
        element_s[] ground  = new element_s[(int)Mathf.Pow(Constants.CHUNK_SIZE, 2)];
        element_s[] roof  = new element_s[(int)Mathf.Pow(Constants.CHUNK_SIZE, 2)];
        if (!hasinit) {
            init();
        }
        // List<Vector2Int> vlist = Chunks.GetLinearList(chunkpos+ new Vector2Int(8,8), chunkpos + new Vector2Int(14,15));
        float aa, bb;
        (aa, bb) = getCurrentPerlin(chunkpos);
        if (bb > 0.7) {
            BuildingGen(chunkpos);
        } else {
            GroundGen(chunkpos);
        }
        
    }

    public static void BuildingGen(Vector2Int chunkpos) {
        element_s[] wall = new element_s[(int)Mathf.Pow(Constants.CHUNK_SIZE, 2)];
        element_s[] ground = new element_s[(int)Mathf.Pow(Constants.CHUNK_SIZE, 2)];
        for(int ii =0;ii < Mathf.Pow(Constants.CHUNK_SIZE, 2); ii++) {
            wall[ii] = e_gen.Brick(chunkpos + new Vector2Int((int)ii % Constants.CHUNK_SIZE, (int)ii/Constants.CHUNK_SIZE));
            ground[ii] = e_gen.Sand(chunkpos + new Vector2Int((int)ii % Constants.CHUNK_SIZE, (int)ii/Constants.CHUNK_SIZE));
        }
        World.world_dict.Add(chunkpos , wall);
        World.world_dict.Add(chunkpos+ Vector2Int.right, ground);
    }

    public static void SandGen(Vector2Int chunkpos) {
        element_s[] wall = new element_s[(int)Mathf.Pow(Constants.CHUNK_SIZE, 2)];
        element_s[] ground = new element_s[(int)Mathf.Pow(Constants.CHUNK_SIZE, 2)];
        for(int ii =0;ii < Mathf.Pow(Constants.CHUNK_SIZE, 2); ii++) {
            wall[ii] = new element_s(chunkpos + new Vector2Int((int)ii % Constants.CHUNK_SIZE, (int)ii/Constants.CHUNK_SIZE));
            ground[ii] = e_gen.Sand(chunkpos + new Vector2Int((int)ii % Constants.CHUNK_SIZE, (int)ii/Constants.CHUNK_SIZE));
        }
        World.world_dict.Add(chunkpos , wall);
        World.world_dict.Add(chunkpos+ Vector2Int.right, ground);
    }

    public static void GroundGen(Vector2Int chunkpos) {
        element_s[] wall = new element_s[(int)Mathf.Pow(Constants.CHUNK_SIZE, 2)];
        element_s[] ground = new element_s[(int)Mathf.Pow(Constants.CHUNK_SIZE, 2)];
        float aa, bb;
        Vector2Int curpos; 
        for(int ii =0;ii < Mathf.Pow(Constants.CHUNK_SIZE, 2); ii++) {
            curpos = chunkpos + new Vector2Int((int)ii % Constants.CHUNK_SIZE, (int)ii/Constants.CHUNK_SIZE);
            (aa, bb) = getCurrentPerlin(curpos);
            wall[ii] = new element_s(curpos);            
            if (aa > 0.5) {
                ground[ii] = e_gen.Dirt(curpos);
            } else {
                ground[ii] = e_gen.Sand(curpos);
            }
        }
        World.world_dict.Add(chunkpos , wall);
        World.world_dict.Add(chunkpos+ Vector2Int.right, ground);
    }




}

