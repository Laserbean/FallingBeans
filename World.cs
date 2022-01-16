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


public class World : MonoBehaviour
{
    public GameObject player;
    public Camera camera;

    public Vector3 playerposition; 

    // Global constants
    // // // const int CHUNK_AREA = CHUNK_SIZE* CHUNK_SIZE;

    public Tilemap colorTileMap;
    // public Tilemap solidTileMap;

    public Tile basictile;
    // TileBase[] tileArray = new TileBase[Constants.CHUNK_SIZE* Constants.CHUNK_SIZE];

    // public static Dictionary<Vector2Int, bool[,]> doneChunks; 
    // public static Dictionary<Vector2Int, TileBase[]> world_dict;
    public static Dictionary<Vector2Int, element_s[]> world_dict;
    public static Dictionary<Vector2Int, ChunkState> chunkstate_dict;
    public static List<List<Vector2>> list_o_collider_points;
    public static List<Vector2Int> LoadedChunksList;
    public static List<Vector2Int> SimulationChunkList;

    public static List<List<List<Vector2>>> colliderList; 

    public static Dictionary<Vector2Int, List<List<Vector2>>> chunkHitbox_dict;


    public static Dictionary<Vector2Int, Vector2Int> execution_dict;



    public int minx = -5, maxx  =5;
    public int miny = -5, maxy  =5;
    
    // public int minx = 0, maxx  =1;
    // public int miny = 0, maxy  =1;

    // public const int minx = -1, maxx  =3;
    // public const int miny = -1, maxy  =3;
    // public PolygonCollider2D thispolygon; 
    TilePolygon Tilepoly; 
    void Start()
    {
        // TileManager.init(basictile, solidTileMap);
        // colorTileMap = GameObject.Find("solid").GetComponent<Tilemap>();
        World.world_dict = new Dictionary<Vector2Int, element_s[]>(); 
        World.chunkstate_dict = new Dictionary<Vector2Int, ChunkState>(); 
        World.execution_dict = new Dictionary<Vector2Int, Vector2Int>();
        World.chunkHitbox_dict = new Dictionary<Vector2Int, List<List<Vector2>>>();     
        LoadedChunksList = new List<Vector2Int>();
        SimulationChunkList = new List<Vector2Int>();


        list_o_collider_points = new List<List<Vector2>>();

        Tilepoly = new TilePolygon();

        // World.print("done");
        StartCoroutine(LoadChunks());
        StartCoroutine(UnloadChunks());
        InvokeRepeating("MyUpdate", Constants.PERIOD, Constants.PERIOD);
    }
    private bool isUpdatingChunks = false;
    
    /// <summary>
    /// Starts and maintains the sequence for loading chunks.
    /// </summary>
    /// <param name="done"></param>
    /// <param name="loadAll"></param>
    private IEnumerator LoadChunks()
    {
        while (true)
        {
            isUpdatingChunks = true;
            yield return StartCoroutine(PerformLoadChunks());                
            isUpdatingChunks = false;
            yield return null;
        }
    }

    /// <summary>
    /// Starts and maintains the sequence for unloading chunks.
    /// </summary>
    /// <returns></returns>
    private IEnumerator UnloadChunks()
    {
        while (true)
        {
            if (!isUpdatingChunks)
                yield return StartCoroutine(PerformUnloadChunks());
            yield return null;
        }
    }



    // void Update() {
        // MyUpdate();
    // }
    
    void chunkInit(Vector2Int chunkpos) {
        Chunks.fillChunkWithTiles(chunkpos, colorTileMap, basictile); //fills with basic tile
        Chunks.drawChunkTiles(chunkpos, colorTileMap);
    }

    void chunkgen(Vector2Int chunkpos)
    {
        if (World.world_dict.ContainsKey(chunkpos)) {
            return;
        }
        
        World.world_dict.Add(chunkpos, WorldGen.ChunkGen(chunkpos));
        World.chunkHitbox_dict.Add(chunkpos, Chunks.GetChunkMesh(chunkpos));
        World.chunkstate_dict.Add(chunkpos, new ChunkState(1));
        SimulationChunkList.Add(chunkpos);
        // // // Debug.LogError(chunkpos);
    }
    public GameEvent deleteevent; 
    Vector3 cammin, cammax; 

    [SerializeField] private bool isdebug = false;
    void MyUpdate() {
        // playerposition = player.transform.position; 
        // cammin = camera.ScreenToWorldPoint(Vector3.zero);
        // cammax = camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth, camera.pixelHeight));
        // cammin = new Vector3(cammin.x/ Constants.PIXEL_SCALE, cammin.y/ Constants.PIXEL_SCALE);
        // cammax = new Vector3(cammax.x/ Constants.PIXEL_SCALE, cammax.y/ Constants.PIXEL_SCALE);


        float starttime = Time.realtimeSinceStartup;
        float framestart = starttime; 
        // UpdateChunksWithJobs();
        UpdateChunks();
        // while (i < 98) {
        //     thispolygon.SetPath(i, new Vector2[0]);
        // }
        if (isdebug) {
            Debug.Log("First "+((Time.realtimeSinceStartup - starttime)*1000f) + "ms"); 
            // Debug.Log("\tTotal "+((Time.realtimeSinceStartup - framestart)*1000f) + "ms"); 
        }

        starttime = Time.realtimeSinceStartup;

        ExecuteSwaps();
        if (isdebug) {
            Debug.Log("Execute swaps"+((Time.realtimeSinceStartup - starttime)*1000f) + "ms"); 
            // Debug.Log("\tTotal "+((Time.realtimeSinceStartup - framestart)*1000f) + "ms"); 
        }
        starttime = Time.realtimeSinceStartup;

        // int ii = 0;
        list_o_collider_points.Clear();

        // Vector2Int  midpos;
        // midpos = Chunks.GetChunkPos(new Vector2Int((int) (playerposition.x / Constants.PIXEL_SCALE), (int) (playerposition.y/  Constants.PIXEL_SCALE)));
        // midpos = (new Vector2Int((int)(midpos.x *Constants.PIXEL_SCALE), (int)(midpos.y *Constants.PIXEL_SCALE)));
        // Debug.DrawLine(playerposition,  new Vector3(midpos.x* Constants.PIXEL_SCALE, midpos.y* Constants.PIXEL_SCALE, 0));
        

        if (isdebug) {
        Debug.Log("\tTotal Frame"+((Time.realtimeSinceStartup - framestart)*1000f) + "ms"); 
        }
    }
    [SerializeField] bool UseJobs = false; 

    void UpdateCollidersAndTiles(Vector2Int midpos) {
        midpos = Chunks.GetChunkNumber(midpos);
        
        minx = midpos.x - Constants.RENDER_DISTANCE;
        maxx = midpos.x + Constants.RENDER_DISTANCE;
        miny = midpos.y - Constants.RENDER_DISTANCE;
        maxy = midpos.y + Constants.RENDER_DISTANCE;

        // curpos = Chunks.GetChunkPos(new Vector2Int(i * Constants.CHUNK_SIZE, j * Constants.CHUNK_SIZE));
        Vector2Int curpos; 
        // drawChunkBox(midpos, Color.green);
         for (int i = minx; i < maxx+1; i++) {
            for (int j = miny; j < maxy+1; j++) {
                curpos = Chunks.GetChunkPos(new Vector2Int(i * Constants.CHUNK_SIZE, j * Constants.CHUNK_SIZE));
                if ((j > miny && j < maxy) &&( i > minx && i < maxx)) {
                    drawChunkBox(curpos, Color.green);
                } else {
                    drawChunkBox(curpos, Color.magenta);
                }
            }
         }

    }
    const int extra = 3; 
    bool tick = false;

    void UpdateCollidersAndTiles2(Vector2Int midpos) {
        midpos = Chunks.GetChunkNumber(midpos);

        Vector2Int curpos; 
        minx = midpos.x - Constants.RENDER_DISTANCE;
        maxx = midpos.x + Constants.RENDER_DISTANCE;
        miny = midpos.y - Constants.RENDER_DISTANCE;
        maxy = midpos.y + Constants.RENDER_DISTANCE;

       ChunkState curchunkstate;
        for (int i = minx - extra-1; i < maxx+extra; i++) {
            for (int j = miny- extra-1; j < maxy+extra; j++) {
                curpos = Chunks.GetChunkPos(new Vector2Int(i * Constants.CHUNK_SIZE, j * Constants.CHUNK_SIZE));
                if (tick) {
                
                if ((j > miny && j < maxy) &&( i > minx && i < maxx)) {
                    if (World.chunkHitbox_dict.ContainsKey(curpos)) {
                        LoadedChunksList.Add(curpos);
                        curchunkstate = World.chunkstate_dict[curpos];
                        drawChunkBox(curpos, Color.green);

                        if (curchunkstate.tilestate == 0) {
                            deleteevent.Raise2Vector2Int(curpos);
                            GameObject GO = ObjectPooler.SharedInstance.GetPooledObject(0);
                            if (GO != null) {
                                GO.SetActive(true);
                                // GO.GetComponent<ChunkColliderScript>().Test();
                                List<List<Vector2>> this1 = World.chunkHitbox_dict[curpos];
                                int thiscount = this1.Count;
                                if (thiscount != 0) {
                                    Debug.Log("FISH");
                                }

                                GO.GetComponent<ChunkColliderScript>().SetSolidPath(this1, curpos);
                            }
                            // curchunkstate.colliderChanged = false;                     
                            // curchunkstate.tilestate = 1;
                        }
                        World.chunkstate_dict[curpos] = curchunkstate;
                        //Chunks.fillChunkWithTiles(chunkpos, colorTileMap, basictile); //fills with basic tile
                    } else {
                        chunkgen(curpos);
                    }
                    chunkInit(curpos);

                } else {

                    if (World.chunkstate_dict.ContainsKey(curpos)) {
                        curchunkstate = World.chunkstate_dict[curpos];
                        if (curchunkstate.tilestate == 1) {
                            drawChunkBox(curpos, Color.magenta);
                            deleteevent.Raise2Vector2Int(curpos);
                            Chunks.fillChunkWithNull(curpos, colorTileMap);
                            curchunkstate.tilestate = 0;
                            World.chunkstate_dict[curpos] = curchunkstate;
                        }
                    }
                }
                tick = false;
                } else {
                    tick = true; 
                }

            }

        }
        

    }


    void UpdateChunks() {
        int result = 0;
        ChunkState curchunkstate; 
        // List<Vector2Int> keyList = new List<Vector2Int>(World.chunkstate_dict.Keys);
        List<Vector2Int> removekey = new List<Vector2Int>();

        foreach (Vector2Int key in SimulationChunkList) {
            curchunkstate = World.chunkstate_dict[key];
            if (curchunkstate.state == 1 || curchunkstate.state == 2) {
                drawChunkBox(key, new Color(1, 0, 0, 0.3f));
                if (UseJobs) {
                    result = UpdateChunkWithJob(key); //will be chunks later. 
                } else {
                    result = UpdateChunk(key); //will be chunks later. 
                }
                // 0 means no changes, 1 means changes
                if (result == 0) { 
                    if (curchunkstate.state == 1) {
                        result = 2; 
                    } else {
                        removekey.Add(key);
                    }
                }
                if (result != -1) {
                    curchunkstate.state = result; 
                }
            }
            World.chunkstate_dict[key] = curchunkstate; 
        }
        foreach (Vector2Int key in removekey) {
            SimulationChunkList.Remove(key);
        }
    }

    int UpdateChunk(Vector2Int cpos) {
        Vector3Int curcandidate;
        int curreturn = 0; //no changes
        if (!World.world_dict.ContainsKey(cpos)) {
            return -1;
        }
        for(int ii =0;ii < Mathf.Pow(Constants.CHUNK_SIZE, 2); ii++) {
            if (World.world_dict[cpos][ii].matter == Matter.Powder) {
                curcandidate = e_step.PowderStep(World.world_dict[cpos][ii]);
                if (curcandidate.z == 0) {
                    if (World.execution_dict.ContainsKey((Vector2Int)curcandidate)){
                        if (UnityEngine.Random.Range(0f, 1f) > 0.5f) {
                            World.execution_dict[(Vector2Int) curcandidate] = World.world_dict[cpos][ii].position;
                        }
                    } else {
                        World.execution_dict.Add((Vector2Int) curcandidate, World.world_dict[cpos][ii].position);
                    }
                    // Debug.LogError("Hmm" + World.world_dict[cpos][ii].position + " : " + curcandidate);
                    curreturn = 1; //changes
                }
            } else {

            }
            //  World.world_dict[cpos][ii].position   //original
            //  curcandidate  //destination
        }
        return curreturn; 
    }

    int UpdateChunkWithJob(Vector2Int cpos) {
        
        NativeArray<element_s> e_array = new NativeArray<element_s>((int)Mathf.Pow(Constants.CHUNK_SIZE, 2), Allocator.TempJob);
        NativeArray<Vector3Int> candidates = new NativeArray<Vector3Int>((int)Mathf.Pow(Constants.CHUNK_SIZE, 2), Allocator.TempJob); 

        int curreturn1 = 0; //no changes
        if (!World.world_dict.ContainsKey(cpos)) {
            return -1;
        }
        for(int ii =0;ii < Mathf.Pow(Constants.CHUNK_SIZE, 2); ii++) {
            e_array[ii] = World.world_dict[cpos][ii];
        }
        ChunkUpdateJob curjob = new ChunkUpdateJob {
            world_dict = e_array,
            // curreturn = curreturn1,
            _candidates = candidates,
        };

        JobHandle jobHandle = curjob.Schedule((int)Mathf.Pow(Constants.CHUNK_SIZE, 2), 100);
        jobHandle.Complete();

        Vector3Int curcandidate; 
        for(int ii =0;ii < Mathf.Pow(Constants.CHUNK_SIZE, 2); ii++) {
            World.world_dict[cpos][ii] = e_array[ii];
            curcandidate = candidates[ii];
            if (curcandidate.z == 0) {
                if (World.execution_dict.ContainsKey((Vector2Int)curcandidate)){
                    if (UnityEngine.Random.Range(0f, 1f) > 0.5f) {
                        World.execution_dict[(Vector2Int) curcandidate] = e_array[ii].position;
                    }
                } else {
                    World.execution_dict.Add((Vector2Int) curcandidate, e_array[ii].position);
                }
                curreturn1 = 1;
            }
        }
        candidates.Dispose();
        e_array.Dispose();
        return curreturn1; 
    }

    public struct ChunkUpdateJob: IJobParallelFor {
        public NativeArray<element_s> world_dict;
        // public int curreturn; 
        private Unity.Mathematics.Random _random;
        public NativeArray<Vector3Int> _candidates; 

        public void Execute(int ii) {
            _random = new Unity.Mathematics.Random(1);
            Vector3Int curcandidate;
            if (world_dict[ii].matter == Matter.Powder) {
                curcandidate = e_step.PowderStep(world_dict[ii]);
                    _candidates[ii] = curcandidate;
                    // if (curcandidate == Vector3Int.zero) {
                    //     Debug.Log("hmm");
                    // }
                    // positions[ii] = world_dict[ii].position;
            } else {
                _candidates[ii] = Vector3Int.one;
            }
        }
    }

    void ExecuteSwaps() {
        List<Vector2Int> keyList = new List<Vector2Int>(World.execution_dict.Keys); 
        foreach (Vector2Int fish in keyList) { //fish is destination, value is origin.
            Chunks.Swap(fish, World.execution_dict[fish], colorTileMap);
            // World.chunkstate_dict[key] = 1;
            
            // Chunks.Edge curedge = 
            Vector2Int key = World.execution_dict[fish]; //should be called value

            Chunks.Edge edge1, edge2;
            ChunkState curchunkstate = World.chunkstate_dict[Chunks.GetChunkPos(World.execution_dict[fish])]; 
            curchunkstate.needGenCollider = true;
            World.chunkstate_dict[Chunks.GetChunkPos(World.execution_dict[fish])] = curchunkstate; 

            curchunkstate = World.chunkstate_dict[Chunks.GetChunkPos(fish)]; 
            curchunkstate.needGenCollider = true;
            (edge1, edge2) = Chunks.EdgeType(key);
            curchunkstate.state = 1; 
            SimulationChunkList.Add(key);

            World.chunkstate_dict[Chunks.GetChunkPos(fish)] = curchunkstate;
            if (Chunks.GetCell(fish+ Vector2Int.left).IsFreeFalling <1) {
                Chunks.TryWakeCell(fish + Vector2Int.left);
            }
            if (Chunks.GetCell(fish+ Vector2Int.right).IsFreeFalling <1) {
                Chunks.TryWakeCell(fish + Vector2Int.right);
            }
            if (Chunks.GetCell(fish+ Vector2Int.down).IsFreeFalling <1) {
                Chunks.TryWakeCell(fish + Vector2Int.down);
            }

            if (Chunks.GetCell(fish+ Vector2Int.left +Vector2Int.up).IsFreeFalling <1) {
                Chunks.TryWakeCell(fish + Vector2Int.left +Vector2Int.up);
            }
            if (Chunks.GetCell(fish+ Vector2Int.right +Vector2Int.up).IsFreeFalling <1) {
                Chunks.TryWakeCell(fish + Vector2Int.right +Vector2Int.up);
            }




            if (edge1 == Chunks.Edge.up) {
                if (World.chunkstate_dict.ContainsKey(Chunks.GetChunkPos(key)+ new Vector2Int(0,Constants.CHUNK_SIZE))) { //up
                    // drawChunkBox(Chunks.GetChunkPos(key) + new Vector2Int(0,Constants.CHUNK_SIZE), new Color(0, 1, 0, 1));
                    curchunkstate = World.chunkstate_dict[Chunks.GetChunkPos(key) + new Vector2Int(0,Constants.CHUNK_SIZE)];
                    curchunkstate.state = 1; 
                    SimulationChunkList.Add(Chunks.GetChunkPos(key) + new Vector2Int(0,Constants.CHUNK_SIZE));
                    World.chunkstate_dict[Chunks.GetChunkPos(key) + new Vector2Int(0,Constants.CHUNK_SIZE)] = curchunkstate;
                }
            } else if (edge1 == Chunks.Edge.down) {
                if (World.chunkstate_dict.ContainsKey(Chunks.GetChunkPos(key) - new Vector2Int(0,Constants.CHUNK_SIZE))) {
                    curchunkstate = World.chunkstate_dict[Chunks.GetChunkPos(key) - new Vector2Int(0,Constants.CHUNK_SIZE)];
                    SimulationChunkList.Add(Chunks.GetChunkPos(key) - new Vector2Int(0,Constants.CHUNK_SIZE));
                    curchunkstate.state = 1; 
                    World.chunkstate_dict[Chunks.GetChunkPos(key) - new Vector2Int(0,Constants.CHUNK_SIZE)] = curchunkstate;
                }
            }

            if (edge2 == Chunks.Edge.left) {
                if (World.chunkstate_dict.ContainsKey(Chunks.GetChunkPos(key) - new Vector2Int(Constants.CHUNK_SIZE, 0))) {
                    curchunkstate = World.chunkstate_dict[Chunks.GetChunkPos(key) - new Vector2Int(Constants.CHUNK_SIZE, 0)];
                    SimulationChunkList.Add(Chunks.GetChunkPos(key) - new Vector2Int(Constants.CHUNK_SIZE, 0));
                    curchunkstate.state = 1; 
                    World.chunkstate_dict[Chunks.GetChunkPos(key) - new Vector2Int(Constants.CHUNK_SIZE, 0)] = curchunkstate;
                }
            } else if (edge2 == Chunks.Edge.right) {
                if (World.chunkstate_dict.ContainsKey(Chunks.GetChunkPos(key) + new Vector2Int(Constants.CHUNK_SIZE, 0))) {
                    curchunkstate = World.chunkstate_dict[Chunks.GetChunkPos(key) + new Vector2Int(Constants.CHUNK_SIZE, 0)];
                    SimulationChunkList.Add(Chunks.GetChunkPos(key) + new Vector2Int(Constants.CHUNK_SIZE, 0));
                    curchunkstate.state = 1; 
                    World.chunkstate_dict[Chunks.GetChunkPos(key) + new Vector2Int(Constants.CHUNK_SIZE, 0)] = curchunkstate;
                }
            }
            
        }
        World.execution_dict.Clear();
        // Debug.Log("Clear");
    }

    void drawChunkBox(Vector2Int pos1, Color color) {
        // Color color = new Color(0.7f, 0.1f, 0.1f);
        // Color color = new Color(1, 0, 0, 1);
        Vector3 pos =((Vector3) (Vector3Int) pos1) * Constants.PIXEL_SCALE;
        Vector3 pos2 = new Vector3(pos.x + Constants.CHUNK_SIZE * Constants.PIXEL_SCALE, pos.y +Constants.CHUNK_SIZE * Constants.PIXEL_SCALE, 0);
        // Debug.Log(pos1);

        Debug.DrawLine(new Vector3(pos.x, pos.y, 0),new Vector3(pos2.x, pos.y, 0), color, Constants.PERIOD); 
        Debug.DrawLine(new Vector3(pos2.x, pos.y, 0),new Vector3(pos2.x, pos2.y, 0), color, Constants.PERIOD); 
        Debug.DrawLine(new Vector3(pos2.x, pos2.y, 0),new Vector3(pos.x, pos2.y, 0), color, Constants.PERIOD); 
        Debug.DrawLine(new Vector3(pos.x, pos2.y, 0),new Vector3(pos.x, pos.y, 0), color, Constants.PERIOD); 
        // Gizmos.color = new Color(1, 0, 0, 0.5f);
        // Gizmos.DrawCube((Vector3)(Vector3Int) pos, new Vector3(Constants.CHUNK_SIZE, Constants.CHUNK_SIZE, 1));
    }

    public void MouseEvent(Vector3 pos1) {
        int radius = (int) pos1.z; 
        radius = 1; 
        Vector2Int pos = new Vector2Int((int)(pos1.x *8) ,(int) (pos1.y *8));
        Vector2Int curpos = Vector2Int.zero; 
                    // AddCell(curpos);

        for (int j = (int) pos.y - radius; j < pos.y + radius; j++) {
            for (int i = (int) pos.x - radius; i < pos.x + radius; i++) {
                curpos.x = i;
                curpos.y = j; 
                if (isInCircle(curpos, new  Vector2Int((int) pos.x, (int) pos.y), radius)) {
                    AddCell(curpos);
                }
            }
        }

    }

    public void AddCell(Vector2Int pos) {
        // Debug.Log(pos);
        if (World.chunkstate_dict.ContainsKey(Chunks.GetChunkPos(pos))) {
            ChunkState curchunkstate = World.chunkstate_dict[Chunks.GetChunkPos(pos)];
            curchunkstate.state = 1;
            World.chunkstate_dict[Chunks.GetChunkPos(pos)] = curchunkstate;
            Chunks.AddCell(e_gen.Sand(pos), colorTileMap); 
        }
    }


    bool isInCircle(Vector2Int pos, Vector2Int center, int radius) { 
        return ((pos - center).magnitude <= radius);
        // if (pos.x > center.x + radius || pos.x < center.x - radius || 
        // pos.y > center.y + radius || pos.y < center.y - radius) {
        //     return false; 
        // } else {
        //     pos.x = pos.x > center.x? pos.x : 2*center.x - pos.x;
        //     pos.y = pos.y > center.y? pos.y : 2*center.y - pos.y;
        //     float y = Mathf.Sqrt(radius*radius - Mathf.Pow(pos.x-center.x,2))+center.y; 
        //     return (pos.y <= y);
        // }
    }


/// <summary>
    /// Returns a Rect that defines the area where chunks can be loaded in.
    /// </summary>
    /// <returns></returns>
    private Rect GetChunkLoadBounds()
    {
        playerposition = player.transform.position; 
        // cammin = camera.ScreenToWorldPoint(Vector3.zero);
        // cammax = camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth, camera.pixelHeight));
        // cammin = new Vector3(cammin.x/ Constants.PIXEL_SCALE, cammin.y/ Constants.PIXEL_SCALE);
        // cammax = new Vector3(cammax.x/ Constants.PIXEL_SCALE, cammax.y/ Constants.PIXEL_SCALE);

        cammin = playerposition;
        cammax = playerposition + Vector3.up + Vector3.right;
        cammin = new Vector3((cammin.x- Constants.RENDER_DISTANCE)/ Constants.PIXEL_SCALE, (cammin.y-Constants.RENDER_DISTANCE)/ Constants.PIXEL_SCALE);
        cammax = new Vector3((cammax.x+ Constants.RENDER_DISTANCE)/ Constants.PIXEL_SCALE, (cammax.y+Constants.RENDER_DISTANCE)/ Constants.PIXEL_SCALE);


        // Vector3 regionStart = Camera.main.transform.position + 
        //     Vector3.left * Constants.RENDER_DISTANCE + 
        //     Vector3.down * Constants.RENDER_DISTANCE;
        // Vector3 regionEnd = Camera.main.transform.position +
        //     Vector3.right * Constants.RENDER_DISTANCE + 
        //     Vector3.up * Constants.RENDER_DISTANCE;

        // Convert to int for automatic flooring of coordinates
        int regionStartX = (int)cammin.x / Constants.CHUNK_SIZE;
        int regionStartY = (int)cammin.y / Constants.CHUNK_SIZE;
        int regionEndX = ((int)cammax.x + Constants.CHUNK_SIZE) / Constants.CHUNK_SIZE;
        int regionEndY = ((int)cammax.y + Constants.CHUNK_SIZE) / Constants.CHUNK_SIZE;
        Rect loadBoundaries = new Rect(regionStartX, regionStartY, regionEndX - regionStartX, regionEndY - regionStartY);
        Debug.DrawLine(cammin, cammax);
        return loadBoundaries;
    }

        /// <summary>
    /// Scans and loads chunks in view using a Rect as bounds.
    /// </summary>
    /// <param name="loadAll"></param>
    private IEnumerator PerformLoadChunks()
    {
        Rect loadBoundaries = GetChunkLoadBounds();
        List<Chunk> chunksToLoad = new List<Chunk>();
        for (int h = (int)loadBoundaries.xMax; h >= (int)loadBoundaries.xMin; h--)
        {
            for (int v = (int)loadBoundaries.yMax; v >= (int)loadBoundaries.yMin; v--)
            {
                // if ((h < 0 || h >= GenerationManager.Instance.worldWidth / GenerationManager.Instance.chunkSize) ||
                //     (v < 0 || v >= GenerationManager.Instance.worldHeight / GenerationManager.Instance.chunkSize))
                //     continue;
                // Vector3Int chunkPosition = new Vector3Int(h, v, 0);
                // Vector3Int worldPosition = new Vector3Int(
                //     h * GenerationManager.Instance.chunkSize,
                //     v * GenerationManager.Instance.chunkSize, 0);
                Vector2Int curpos = Chunks.GetChunkPos(new Vector2Int(h * Constants.CHUNK_SIZE, v * Constants.CHUNK_SIZE));
                List<List<Vector2>> curchunkmesh;

                if (loadBoundaries.Contains(new Vector2(curpos.x/Constants.CHUNK_SIZE, curpos.y/Constants.CHUNK_SIZE)) && !LoadedChunksList.Contains(curpos))
                {   
                    ChunkState curchunkstate; 
                    if (World.chunkstate_dict.ContainsKey(curpos)) {
                    } else {
                        chunkgen(curpos);
                    }
                    curchunkstate = World.chunkstate_dict[curpos];
                    if (curchunkstate.needGenCollider) {
                        curchunkmesh = Chunks.GetChunkMesh(curpos);
                        if (curchunkmesh.Count > 0) {
                            if (World.chunkHitbox_dict.ContainsKey(curpos)) {
                                World.chunkHitbox_dict[curpos] = curchunkmesh;
                                // curchunkstate.colliderChanged = false; 
                            } else {
                                World.chunkHitbox_dict.Add(curpos, curchunkmesh);
                            }
                            curchunkstate.needDrawCollider = true; 
                        }
                        curchunkstate.needGenCollider = false; 
                    }

                    drawChunkBox(curpos, Color.green);
                    if (curchunkstate.tilestate == 0) 

                    if (curchunkstate.needDrawCollider) {
                        deleteevent.Raise2Vector2Int(curpos);
                        GameObject GO = ObjectPooler.SharedInstance.GetPooledObject(0);
                        if (GO != null) {
                            GO.SetActive(true);
                            // GO.GetComponent<ChunkColliderScript>().Test();
                            List<List<Vector2>> this1 = World.chunkHitbox_dict[curpos];
                            int thiscount = this1.Count;
                            GO.GetComponent<ChunkColliderScript>().SetSolidPath(this1, curpos);
                            // if (thiscount == 0) {
                            //     Debug.LogError("error");
                            // }
                            curchunkstate.needDrawCollider = false; 
                        }
                        curchunkstate.tilestate = 1;
                    }
                    World.chunkstate_dict[curpos] = curchunkstate;
                    //Chunks.fillChunkWithTiles(chunkpos, colorTileMap, basictile); //fills with basic tile
                    chunkInit(curpos);
                    LoadedChunksList.Add(curpos);
                    yield return null;
                }
            }
        }
    }


    /// <summary>
    /// Unloads chunks that are outside the view, when we're not loading chunks.
    /// </summary>
    /// <returns></returns>
    private IEnumerator PerformUnloadChunks()
    {
        Rect loadBoundaries = GetChunkLoadBounds();
        ChunkState curchunkstate; 

        List<Vector2Int> chunkunload = new List<Vector2Int>();
        foreach(Vector2Int curpos in LoadedChunksList) {
            curchunkstate = World.chunkstate_dict[curpos];
            if (curchunkstate.tilestate == 1) {
                if (!loadBoundaries.Contains(new Vector2(curpos.x/Constants.CHUNK_SIZE, curpos.y/Constants.CHUNK_SIZE))) {
                    chunkunload.Add(curpos);
                }               
            }

        }


        foreach (Vector2Int curpos in chunkunload)
        {
            while (isUpdatingChunks)
                    yield return null;
            curchunkstate = World.chunkstate_dict[curpos];

            LoadedChunksList.Remove(curpos);
            drawChunkBox(curpos, Color.magenta);
            deleteevent.Raise2Vector2Int(curpos);
            Chunks.fillChunkWithNull(curpos, colorTileMap);

            curchunkstate.needDrawCollider = true; 
            curchunkstate.tilestate = 0;
            World.chunkstate_dict[curpos] = curchunkstate; 
            yield return null;
        }
    }



}
