

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
            Mathf.Clamp(Mathf.PerlinNoise(noiseX *6f + Constants.CHUNK_SIZE + Constants.CHUNK_SIZE/2, noiseY*6f), 0f, 1f));
    }


    public static void ChunkGen(Vector2Int chunkpos) {
        element_s[] wall = new element_s[(int)Mathf.Pow(Constants.CHUNK_SIZE, 2)];
        element_s[] ground  = new element_s[(int)Mathf.Pow(Constants.CHUNK_SIZE, 2)];
        element_s[] roof  = new element_s[(int)Mathf.Pow(Constants.CHUNK_SIZE, 2)];
        if (!hasinit) {
            init();
        }
        // List<Vector2Int> vlist = Chunks.GetLinearList(chunkpos+ new Vector2Int(8,8), chunkpos + new Vector2Int(14,15));
        if (IsBuldingChunk(chunkpos)) {
            BuildingGen(chunkpos);
        } else {
            GroundGen(chunkpos);
        }  
    }

    public static bool IsBuldingChunk(Vector2Int chunkpos) {
        float aa, bb;
        (aa, bb) = getCurrentPerlin(chunkpos);
        return (bb > 0.5);
    }

    public static void BuildingGen(Vector2Int chunkpos) {
        element_s[] wall = new element_s[(int)Mathf.Pow(Constants.CHUNK_SIZE, 2)];
        element_s[] ground = new element_s[(int)Mathf.Pow(Constants.CHUNK_SIZE, 2)];
        List<int> bricklist = Chunks.layerIndices1();

        float aa, bb;
        (aa, bb) = getCurrentPerlin(chunkpos);
        int test = 0b0000; 
        float doorodds = 0.4f; 
        Chunks.Edge dooredge = Chunks.Edge.none;
        if (IsBuldingChunk(chunkpos + Vector2Int.up*Constants.CHUNK_SIZE)) {
            test |= 0b1000; 
        } else {
            if (aa > doorodds) {
                dooredge = Chunks.Edge.up;
            }
        }

        if (IsBuldingChunk(chunkpos + Vector2Int.down*Constants.CHUNK_SIZE)) {
            test |= 0b0100; 
        } else {
            if (aa > doorodds) {
                dooredge = Chunks.Edge.down;
            }
        }
        

        if (IsBuldingChunk(chunkpos + Vector2Int.left*Constants.CHUNK_SIZE)) {
            test |= 0b0010; 
        } else {
            if (aa > doorodds) {
                dooredge = Chunks.Edge.left;
            }
        }

        if (IsBuldingChunk(chunkpos + Vector2Int.right*Constants.CHUNK_SIZE)) {
            test |= 0b0001; 
        } else {
            if (aa > doorodds) {
                dooredge = Chunks.Edge.right;
            }
        }

        for(int ii =0;ii < Mathf.Pow(Constants.CHUNK_SIZE, 2); ii++) {
            if (bricklist.Contains(ii)) {
                if ((test & 0b1000) == 0b1000 &&     Chunks.isNumFromEdge(ii, Chunks.Edge.up, 1)      ||
                        (test & 0b0100) == 0b0100 && Chunks.isNumFromEdge(ii, Chunks.Edge.down,  1)   || 
                        (test & 0b0010) == 0b0010 && Chunks.isNumFromEdge(ii, Chunks.Edge.left,  1)   ||
                        (test & 0b0001) == 0b0001 && Chunks.isNumFromEdge(ii, Chunks.Edge.right, 1)    ) {
                    wall[ii] = new element_s(chunkpos + new Vector2Int((int)ii % Constants.CHUNK_SIZE, (int)ii/Constants.CHUNK_SIZE));
                } else {
                    if (Chunks.isNumFromEdge(ii, Chunks.Edge.up, 1) && dooredge == Chunks.Edge.up || 
                        Chunks.isNumFromEdge(ii, Chunks.Edge.down, 1) && dooredge == Chunks.Edge.down || 
                        Chunks.isNumFromEdge(ii, Chunks.Edge.left, 1) && dooredge == Chunks.Edge.left || 
                        Chunks.isNumFromEdge(ii, Chunks.Edge.right, 1) && dooredge == Chunks.Edge.right) 
                    {
                        wall[ii] = new element_s(chunkpos + new Vector2Int((int)ii % Constants.CHUNK_SIZE, (int)ii/Constants.CHUNK_SIZE));
                    } else {
                        wall[ii] = e_gen.Brick(chunkpos + new Vector2Int((int)ii % Constants.CHUNK_SIZE, (int)ii/Constants.CHUNK_SIZE));
                    }
                }
            } else {
                wall[ii] = new element_s(chunkpos + new Vector2Int((int)ii % Constants.CHUNK_SIZE, (int)ii/Constants.CHUNK_SIZE));
            }
            ground[ii] = e_gen.Stone(chunkpos + new Vector2Int((int)ii % Constants.CHUNK_SIZE, (int)ii/Constants.CHUNK_SIZE));
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

