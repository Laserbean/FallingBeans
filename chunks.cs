using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;


public static class Chunks
{
    // public Vector3Int GetWorldCoordinates(Vector2Int chunkpos, )
    public static void fillChunkWithTiles(Vector2Int chunkpos, Tilemap tilemap, Tile tile) {
        Vector3Int[] positions = new Vector3Int[Constants.CHUNK_SIZE * Constants.CHUNK_SIZE];
        TileBase[] tileArray = new TileBase[Constants.CHUNK_SIZE* Constants.CHUNK_SIZE];
        for(int ii =0;ii < Mathf.Pow(Constants.CHUNK_SIZE, 2); ii++) {
            tileArray[ii] = tile; 
            positions[ii] =(Vector3Int)chunkpos + new Vector3Int((int) ii%Constants.CHUNK_SIZE, (int)ii/Constants.CHUNK_SIZE, 0);
        }
        tilemap.SetTiles(positions, tileArray);
    }

    public static void fillChunkWithNull(Vector2Int chunkpos, Tilemap tilemap) {
        Vector3Int[] positions = new Vector3Int[Constants.CHUNK_SIZE * Constants.CHUNK_SIZE];
        TileBase[] tileArray = new TileBase[Constants.CHUNK_SIZE* Constants.CHUNK_SIZE];
        for(int ii =0;ii < Mathf.Pow(Constants.CHUNK_SIZE, 2); ii++) {
            tileArray[ii] = null; 
            positions[ii] =(Vector3Int)chunkpos + new Vector3Int((int) ii%Constants.CHUNK_SIZE, (int)ii/Constants.CHUNK_SIZE, 0);
        }
        tilemap.SetTiles(positions, tileArray);
    }

    public static void fillSolidChunk(Vector2Int chunkpos, Tilemap tilemap, Tile tile) {
        Vector3Int[] positions = new Vector3Int[Constants.CHUNK_SIZE * Constants.CHUNK_SIZE];
        TileBase[] tileArray = new TileBase[Constants.CHUNK_SIZE* Constants.CHUNK_SIZE];
        for(int ii =0;ii < Mathf.Pow(Constants.CHUNK_SIZE, 2); ii++) {
            if(World.world_dict[GetChunkPos(chunkpos)][ii].matter == Matter.Solid) {
                tileArray[ii] = tile; 
            } else {
                tileArray[ii] = null; 
            }
            positions[ii] =(Vector3Int)chunkpos + new Vector3Int((int) ii%Constants.CHUNK_SIZE, (int)ii/Constants.CHUNK_SIZE, 0);
        }
        tilemap.SetTiles(positions, tileArray);
    }


    public static void drawChunkTiles(Vector2Int chunkpos, Tilemap tilemap) {
        element_s[] curchunk = World.world_dict[chunkpos];
        for(int ii =0;ii < Mathf.Pow(Constants.CHUNK_SIZE, 2); ii++) {
            // if (curchunk[ii].matter == Matter.Solid) {
                SetTileColour(curchunk[ii].color, (Vector3Int)curchunk[ii].position, tilemap);
            // }
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

    public static List<Vector2> GetSquare(Vector2Int pos) {
        List<Vector2> fish = new List<Vector2>();
        fish.Add((Vector2) pos);
        fish.Add((Vector2) pos + Vector2.up);
        fish.Add((Vector2) pos + Vector2.up + Vector2.right);
        fish.Add((Vector2) pos + Vector2.right);
        return fish;
    }

    public static List<List<Vector2>> GetChunkMesh(Vector2Int chunkpos) {
        List<List<Vector2>> fish = new List<List<Vector2>>();
        // List<Vector2> fish = new List<Vector2>();
        element_s curcell; 
        Vector2Int curpos = chunkpos;

        curcell = GetCell(curpos);
        for (int curind =0; curind < Constants.CHUNK_SIZE*Constants.CHUNK_SIZE; curind++) {
            curpos = chunkpos + GetVectorIndex(curind);
            curcell = GetCell(curpos);

            if ((curcell.matter == Matter.Powder || curcell.matter == Matter.Solid) && curcell.IsFreeFalling == 0) {
                fish.Add(GetSquare(curpos)); 

                // Debug.LogError( curcell.matter + "" + curcell.IsFreeFalling);
            } else { 
            }
        }
        TilePolygon chicken = new TilePolygon(); 
        return chicken.UniteCollisionPolygons(fish);
        // return fish;
    }

    // public static Vector2[]  GetChunkMesh(Vector2Int chunkpos) {
    //     // Vector2[] fish = new Vector2[99];
    //     // MeshFilter meshFilter;
    //     // PolygonCollider2D polygonCollider;
    //     List<Vector2> fish = new List<Vector2>();


    //     int curind = 0;
    //     Vector2Int curpos = chunkpos;

    //     Matter curmatter = GetCell(curpos).matter;
    //     while (curind < Constants.CHUNK_SIZE*Constants.CHUNK_SIZE) {
    //         curpos = chunkpos + GetVectorIndex(curind);
    //         if (GetCell(curpos).matter != Matter.Solid && GetCell(curpos).IsFreeFalling > 0) {
    //         } else {
    //             fish.Add((Vector2) curpos); 
    //         }
    //         curind++;
    //     }

    //     Vector2[] myArrayOfPoints =  fish.ToArray();
    //      // We need to convert those Vector2 into Vector3
    //     Vector3[] vertices3D = System.Array.ConvertAll<Vector2, Vector3>(myArrayOfPoints, v => v);
 
    //      // Then, we need to calculate the indices of each vertex.
    //      // For that, you can use the Triangulator class available in:
    //      //https://gist.github.com/N-Carter/12242476dc4e4036db34
    //     Triangulator triangulator = new Triangulator(myArrayOfPoints);
    //     int[] indices = triangulator.Triangulate();


    //     Mesh mesh = new Mesh();
    //     mesh.vertices = vertices3D;
    //     mesh.triangles = indices;
    //     // mesh.colors = colors;

    //     // Recalculate the shape of the mesh
    //     mesh.RecalculateNormals();
    //     mesh.RecalculateBounds();

    //     // meshFilter = new MeshFilter(); 
    //     // meshFilter.mesh = mesh;
    //     // polygonCollider = new PolygonCollider2D();
 
    //     //  // For the collisions, basically you need to add the vertices to your PolygonCollider2D            
    //     // polygonCollider.points = myArrayOfPoints;


    //     //Adapted from 3rd party edge collider optimizer
    //     int tolerance = 1;

    //     myArrayOfPoints = System.Array.ConvertAll<Vector3, Vector2>(mesh.vertices, v => v);
    //     List<Vector2> path = new List<Vector2>(myArrayOfPoints);
    //     path = Collider2DOptimization.ShapeOptimizationHelper.DouglasPeuckerReduction(path, tolerance);
        

    //     return path.ToArray(); 
    // }
    //     // int totalcount = 0;
        // int curind = 0;
        // int curfish = 0;
        // Vector2Int curpos = chunkpos;
        // Vector2Int startpos;
        // // Edge curedge = Edge.up; 
        // int curdir = 4; // 0 is down, 1 is downleft, 2 is left etc. 



        // Matter curmatter = GetCell(curpos).matter;
        // while (curind < Constants.CHUNK_SIZE*Constants.CHUNK_SIZE) {
        //     if (GetCell(curpos).matter != Matter.Solid && GetCell(curpos).IsFreeFalling > 0) {
        //         curpos = chunkpos + GetVectorIndex(curind);
        //         curind++;
        //         totalcount++;
        //     } else {
        //         fish.Add((Vector2) curpos); 
        //         startpos = curpos;
        //         totalcount++;
        //         break;
        //     }
        // }

        
    /*
        bool found = false; 
        int curcurdir = curdir;
        // Edge curedge1,  curedge2;
        while(totalcount < Constants.CHUNK_SIZE*Constants.CHUNK_SIZE) {
            found = false; 
            while (!found) {
                if (curcurdir == curdir || curcurdir == -1) {
                    if (curcurdir == -1) {
                        break; 
                    }
                    curcurdir = -1;
                }
                switch(curdir) {
                    case 0:
                        if (EdgeType(curpos+ Vector2Int.down).Item1 != Edge.down) {
                            curpos += Vector2Int.down;
                            found = true;
                            curcurdir = curdir;
                        }
                    break;
                    case 1:
                    // (curedge1, curedge2) = EdgeType(curpos+ Vector2Int.down + Vector2Int.left);
                    //     if (curedge1 != Edge.down && curedge2 != Edge.left ) {
                        if (EdgeType(curpos+ Vector2Int.down + Vector2Int.left) != (Edge.down, Edge.left)) {
                            curpos += Vector2Int.down + Vector2Int.left;
                            found = true;
                            curcurdir = curdir;

                        }
                    break;
                    case 2:
                        if (EdgeType(curpos+ Vector2Int.left).Item2 != Edge.left) {
                            curpos += Vector2Int.left;
                            found = true;
                            curcurdir = curdir;

                        }
                    break;
                    case 3:
                        if (EdgeType(curpos+ Vector2Int.up + Vector2Int.left) != (Edge.up, Edge.left)) {
                            curpos +=  Vector2Int.up + Vector2Int.left;
                            found = true;
                            curcurdir = curdir;

                        }
                    break;
                    case 4:
                        if (EdgeType(curpos+ Vector2Int.up).Item1 != Edge.up) {
                            curpos +=  Vector2Int.up;
                            found = true; 
                            curcurdir = curdir;
                        }
                    break;
                    case 5:
                        if (EdgeType(curpos+ Vector2Int.up + Vector2Int.right) != (Edge.up, Edge.right)) {
                            curpos +=  Vector2Int.up + Vector2Int.right;
                            found = true;
                            curcurdir = curdir;

                        }
                    break;
                    case 6:
                        if (EdgeType(curpos+ Vector2Int.right).Item2 != Edge.right) {
                            curpos += Vector2Int.right;
                            found = true;
                            curcurdir = curdir;
                        }
                    break;
                    case 7:
                        if (EdgeType(curpos+ Vector2Int.down + Vector2Int.right) != (Edge.down, Edge.right)) {
                            curpos += Vector2Int.down + Vector2Int.right;
                            found = true;
                        }
                        curdir = 0;
                    break;
                }

            }
            if (found) {
                if (fish.Contains(curpos)) {
                    break;
                }
                fish.Add(curpos); 
            }
                totalcount++;

            
        }
    */
        // // Vector2 curpos = new Vector2(chunkpos.x, chunkpos.y);         
        // // // fish[0] = curpos;
        // // // fish[1] = curpos + new Vector2(0, Constants.CHUNK_SIZE);
        // // // fish[2] = curpos + new Vector2(Constants.CHUNK_SIZE, Constants.CHUNK_SIZE);
        // // // fish[3] = curpos + new Vector2(Constants.CHUNK_SIZE, 0);

        // // fish.Add(curpos);
        // // fish.Add(curpos + new Vector2(0, Constants.CHUNK_SIZE));
        // // fish.Add(curpos + new Vector2(Constants.CHUNK_SIZE, Constants.CHUNK_SIZE));
        // // fish.Add(curpos + new Vector2(Constants.CHUNK_SIZE, 0));




    //     return fish;
    // }

    
    public static void Swap(Vector2Int pos1, Vector2Int pos2, Tilemap tilemap) 
    {
        element_s e1, e2;
        e1 = GetCell(pos1);
        e2 = GetCell(pos2);
        // if (e1.matter == e2.matter) {
            
        // } else if (e1.matter == Matter.Solid) {
        //     TileManager.AddTile(pos2);
        //     TileManager.RemoveTile(pos1);
        // } else if (e2.matter == Matter.Solid) {
        //     TileManager.AddTile(pos1);
        //     TileManager.RemoveTile(pos2);
        // }


        e1.position = pos2;
        e2.position = pos1; 
        SetCell(e1); 
        SetCell(e2);
        SetTileColour(e1.color, (Vector3Int)e1.position, tilemap);
        SetTileColour(e2.color, (Vector3Int)e2.position, tilemap);
        
    }

    public static void AddCell(element_s cell, Tilemap tilemap) 
    {
        if (GetCell(cell.position).matter == Matter.None) {
            SetCell(cell); 
             SetTileColour(cell.color, (Vector3Int)cell.position, tilemap);
        }
    }

    public static int mod(int x, int m) {
        // return (x%m + m)%m;
        int r = x%m;
        return r<0 ? r+m : r;
    }

    public static Vector2Int GetChunkPos(Vector2Int pos) 
    {
        /*Returns the origin of the chunk the thing is in*/
        // Debug.Log(pos.y + "  mod "  + pos.y % Constants.CHUNK_SIZE);
        // return new Vector2Int(pos.x - pos.x % Constants.CHUNK_SIZE,  pos.y - pos.y % Constants.CHUNK_SIZE);
        return new Vector2Int(pos.x -mod(pos.x, Constants.CHUNK_SIZE),  pos.y - mod(pos.y, Constants.CHUNK_SIZE));
    }

    public static Vector2Int GetChunkNumber(Vector2Int pos) 
    {
        Vector2Int fish = GetChunkPos(pos);
        /*Returns the origin of the chunk the thing is in*/
        // Debug.Log(pos.y + "  mod "  + pos.y % Constants.CHUNK_SIZE);
        // return new Vector2Int(pos.x - pos.x % Constants.CHUNK_SIZE,  pos.y - pos.y % Constants.CHUNK_SIZE);
        return new Vector2Int((int)(fish.x/Constants.CHUNK_SIZE), (int)(fish.y/Constants.CHUNK_SIZE));
    }

    public static element_s GetCell(Vector2Int pos) 
    {
        if (World.world_dict.ContainsKey(GetChunkPos(pos))) {
            try {
                // Debug.Log("chunk pos" + GetChunkPos(pos) + "GetIndex(pos)" + GetIndex(pos));
                return World.world_dict[GetChunkPos(pos)][GetIndex(pos)];
            } catch {
                // Debug.LogError("GetIndex(pos)" + GetIndex(pos));
                return World.world_dict[GetChunkPos(pos)][GetIndex(pos)];
            }
        } else {
            // Debug.Log("ret   urn bedrock" + GetChunkPos(pos) + " " + pos + " " + GetIndex(pos));
            return e_gen.Bedrock(pos);
        }
    }

    public static void SetCell(element_s cell) 
    {
        Vector2Int pos = cell.position; 
        if (World.world_dict.ContainsKey(GetChunkPos(pos))) {
            World.world_dict[GetChunkPos(pos)][GetIndex(pos)] = cell;
        } else {
            Debug.LogError("ehhhh")  ;
        }
    }

    public static void TryWakeCell(Vector2Int pos) {
        if (World.world_dict.ContainsKey(GetChunkPos(pos))) {
            element_s curcell = GetCell(pos);
            curcell.IsFreeFalling = e_step.TryWakeCell(World.world_dict[GetChunkPos(pos)][GetIndex(pos)] );
            SetCell(curcell);
        } else {
            Debug.LogError("ehhhh Can't wake cell cause world dones't whatever")  ;
        }
    }
    public static int GetIndex(Vector2Int pos) {
        pos.y = mod(pos.y, Constants.CHUNK_SIZE);
        pos.x = mod(pos.x, Constants.CHUNK_SIZE);
        // if (pos.y >= 0) {
        //     pos.y = pos.y % Constants.CHUNK_SIZE;
        // } else {
        //     // pos.y = Constants.CHUNK_SIZE - (pos.y % Constants.CHUNK_SIZE);
        //     pos.y = (-pos.y % Constants.CHUNK_SIZE);

        //     // Debug.Log("posy " + pos.y + "  (-pos.y % Constants.CHUNK_SIZE) " +  (-pos.y % Constants.CHUNK_SIZE));
        // }
        // if (pos.x >= 0) {
        //     pos.x = pos.x % Constants.CHUNK_SIZE;
        // } else {
        //     pos.x = (-pos.x % Constants.CHUNK_SIZE);
        // }
        // return pos;
        return pos.x + pos.y * Constants.CHUNK_SIZE;
    }

    public static Vector2Int GetVectorIndex(int ind) {
        return new Vector2Int(mod(ind, Constants.CHUNK_SIZE), (int)Mathf.Round(ind/Constants.CHUNK_SIZE));
    }

    // public static Vector2Int GetChunkIndex(Vector2Int pos) {
    //     pos.x =
    //     return
    // }

    public static (Edge, Edge) EdgeType(Vector2Int cell) {
        int curindex = GetIndex(cell);
        Edge edge1= Edge.none, edge2 = Edge.none; 

        if(curindex + Constants.CHUNK_SIZE > Constants.CHUNK_SIZE * Constants.CHUNK_SIZE) {
            edge1= Edge.up;
        }else
        if (curindex < Constants.CHUNK_SIZE) {
            edge1= Edge.down;
        } 

        if (mod(curindex, Constants.CHUNK_SIZE)== 0) {
            edge2= Edge.left; 
        } else
        if (mod(curindex, Constants.CHUNK_SIZE)+1-Constants.CHUNK_SIZE== 0) {
            edge2= Edge.right;
        }
        
        return (edge1, edge2);
    }

    public enum Edge {
        up, down, left, right, none, upleft, downleft, upright, downright
        // ,
        // upleft, upright, downleft, downright
    }
    public static float map(float OldValue, float OldMin, float OldMax, float NewMin, float NewMax){
        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;
        return(NewValue);
    }

    public static List<Vector2Int> GetLinearList(Vector2Int pos1, Vector2Int pos2) {
        List<Vector2Int> returnlist = new List<Vector2Int>();
        // float gradient;
        int numberOfPoints;
        Vector2Int curpoint = Vector2Int.zero;

        // gradient = (pos2.y - pos1.y)/(pos2.x - pos1.x); 
        if (Mathf.Abs(pos2.y-pos1.y) < Mathf.Abs(pos2.x - pos1.x)) {
            numberOfPoints = Mathf.Abs(pos2.x - pos1.x);
        }
        else { 
            numberOfPoints = Mathf.Abs(pos2.y - pos1.y);
        }
        for(int i = 0; i < numberOfPoints; i++){
            // map the counter to a normalized (0.0 to 1.0) value for lerp
            // 0.0 = 0 % along the line, 0.5 = 50% along the line, 1.0 = 100% along the line
            float t = map(i, 0, numberOfPoints, 0.0f, 1.0f);
            // linearly interpolate between the start / end points (and snap to whole pixels (casting to integer type))
            curpoint.x = (int)Mathf.Lerp(pos1.x, pos2.x, t);
            curpoint.y = (int)Mathf.Lerp(pos1.y, pos2.y, t);

            returnlist.Add(curpoint);            
        }
        returnlist.Add(pos2);            

        // if (pos2.x != pos1.x) {
            
                
        // } else {
        //     for(int i = pos1.y; i <pos2.y; i++) {
        //         returnlist.Add(new Vector2Int(pos1.x, i));
        //     }
        // }




        return returnlist; 

        // if (gradient < 0.5) {
        //     returnarr = new Vector2Int[(int)(pos2.x - pos1.x)];
        //     for (int i = pos1.x; i < pos2.x; i++) {
        //         int y = (int)(i * gradient + pos1.y);
        //         // Debug.Log("y" + y);
        //         returnarr[i- pos1.x] = new Vector2Int(i, y); 
        //     }
        // }
        // else { 
        //     gradient = (pos2.x - pos1.x)/(pos2.y - pos1.y); 
        //     returnarr = new Vector2Int[(int)(pos2.y - pos1.y)];
        //     for (int i = pos1.y; i < pos2.y; i++) {
        //         returnarr[i-pos1.y] = new Vector2Int((int)(i * gradient + pos1.x), i); 
        //     }
        // }
        // return new List<Vector2Int>(returnarr);
    }
}

public struct ChunkState {
    public int state {get; set;}
    public int tilestate {get; set;}
    public bool needGenCollider {get; set;}
    public bool needDrawCollider {get; set;}
    public int index {get; set;}
    public Biome biome {get; set;} 
    public ChunkState(int _state) {
        state =_state;
        index = -1; 
        tilestate = 0;
        biome = Biome.Default;
        needGenCollider = true; 
        needDrawCollider = true;


    }
}

public enum Biome {
    City,
    Beach,
    Ocean,
    Default
}