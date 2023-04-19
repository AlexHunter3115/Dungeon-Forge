using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DungeonForge.AlgoScript
{
    using DungeonForge.Utils;

    public class PCGManager : MonoBehaviour
    {
        public Dictionary<DFTile.TileType, float> tileTypeToCostDict = new Dictionary<DFTile.TileType, float>();

        public DFTile[,] gridArr = new DFTile[0, 0];

        public List<DFTile[,]> prevGridArray2D = new List<DFTile[,]>();
        private int maxBackUps = 3;

        private GameObject plane;
        public GameObject Plane
        {
            get { return plane; }
        }

        private IUndoInteraction undoInteraction;
        public IUndoInteraction UndoInteraction
        {
            set { undoInteraction = value; }
        }


        [Header("Dungeon Generation Settings")]
        [Tooltip("How wide the drawing canvas where the algorithms will take place will be")]
        [Range(50, 1000)]
        public int width = 125;

        [Tooltip("How tall the drawing canvas where the algorithms will take place will be")]
        [Range(50, 1000)]
        public int height = 125;

        [Tooltip("How tall the dungeon will be.")]
        [Range(3f, 8f)]
        public int RoomHeight = 4;

        public enum MainAlgo
        {
            VORONI = 0,
            RANDOM_WALK = 1,
            ROOM_GEN = 2,
            CELLULAR_AUTOMATA = 3,
            L_SYSTEM = 4,
            DELAUNAY = 5,
            WAVE_FUNCTION_COLLAPSE = 6,
            PERLIN_NOISE = 7,
            PERLIN_WORM = 8,
            DIAMOND_SQUARE = 9,
            DIFFUSION_LIMITATION_AGGREGATION = 10,
            GENERATE_YOUR_DUNGEON = 11
        }

        [Space(10)]
        [Tooltip("The main algorithm to start with, this depends on the type of dungeons prefered")]
        public MainAlgo mainAlgorithm;

        [Space(40)]
        [Header("The loaded gameobjects for the Generation will show here")]
        
        [SerializeField]
        public List<TileRuleSetPCG> FloorTiles = new List<TileRuleSetPCG>();
        [SerializeField]
        public List<TileRuleSetPCG> CeilingTiles = new List<TileRuleSetPCG>();
        [SerializeField]
        public List<TileRuleSetPCG> WallsTiles = new List<TileRuleSetPCG>();

        [Space(40)]
        [Header("This is where entities go for the chunk system")]
        public List<Transform> player = new List<Transform>();

        [Range(1, 3)]
        public int loadingRange = 1;

        private int chunkWidth = 10;
        public int ChunkWidth
        {
            get { return chunkWidth; }
            set { chunkWidth = value; }
        }

        private int chunkHeight = 10;
        public int ChunkHeight
        {
            get { return chunkHeight; }
            set { chunkHeight = value; }
        }

        [HideInInspector]
        public int CLength;
        [HideInInspector]
        public int CHeight;

        [HideInInspector]
        public string WeightRuleFileName = "";

        [HideInInspector]
        public float[] tileCosts = new float[0];

        [HideInInspector]
        public List<Chunk> chunks;

        [HideInInspector]
        public bool loadSectionOpen = false;

        private int currMainAlgoIDX = 11;
        public int CurrMainAlgoIDX
        {
            get { return currMainAlgoIDX; }
        }

        [HideInInspector]
        public string TileSetRuleFileName = "";

        [HideInInspector]
        public List<Transform> chunkObjs = new List<Transform>();

        private Transform parentObj;

        private void Update()
        {
            if (chunks.Count > 0)
            {
                CheckChunkRender();
            }
        }


        #region undo Button
        public void CreateBackUpGrid()
        {
            var prevGridArray2Dstack = new DFTile[gridArr.GetLength(0), gridArr.GetLength(1)];
            for (int y = 0; y < gridArr.GetLength(1); y++)
            {
                for (int x = 0; x < gridArr.GetLength(0); x++)
                {
                    prevGridArray2Dstack[x, y] = new DFTile(gridArr[x, y]);
                }
            }
            prevGridArray2D.Add(prevGridArray2Dstack);

            if (prevGridArray2D.Count >= maxBackUps + 1)
                prevGridArray2D.RemoveAt(0);
        }

        public bool LoadBackUpGrid()
        {
            if (prevGridArray2D.Count == 0)
                return false;

            var prevGridArray2DList = prevGridArray2D[prevGridArray2D.Count - 1];

            gridArr = new DFTile[prevGridArray2DList.GetLength(0), prevGridArray2DList.GetLength(1)];

            for (int y = 0; y < prevGridArray2DList.GetLength(1); y++)
            {
                for (int x = 0; x < prevGridArray2DList.GetLength(0); x++)
                {
                    gridArr[x, y] = new DFTile(prevGridArray2DList[x, y]);
                }
            }

            prevGridArray2D.RemoveAt(prevGridArray2D.Count - 1);

            Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(gridArr, 0, 1, true);

            if (undoInteraction != null)
                undoInteraction.DeleteLastSavedRoom();

            return true;
        }


        public void ClearUndos() => prevGridArray2D.Clear();


        #endregion


        #region algo Managment
        public void CreatePlane()
        {
            DeletePlane();
            plane = GameObject.CreatePrimitive(PrimitiveType.Plane);

            plane.transform.position = Vector3.zero;
            plane.name = "Canvas Generation";

            plane.transform.localScale = new Vector3(width/10f, 1, height/10f);

            plane.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 180.0f, 0.0f));

            gridArr = new DFTile[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    gridArr[x, y] = new DFTile();
                    gridArr[x, y].position = new Vector2Int(x, y);
                    gridArr[x, y].tileType = DFTile.TileType.VOID;
                }
            }
        }

        public void DeletePlane()
        {
            if (plane != null)
                DestroyImmediate(plane);

            ClearUndos();
        }

        public void LoadMainAlgo()
        {
            DelPrevAlgo();
            CreatePlane();
            currMainAlgoIDX = (int)mainAlgorithm;
            undoInteraction = null;

            parentObj = this.transform;
            switch (mainAlgorithm)
            {
                case MainAlgo.VORONI:
                    {
                        var comp = gameObject.AddComponent<VoronoiMA>();
                        comp.InspectorAwake();
                    }
                    break;
                case MainAlgo.RANDOM_WALK:
                    {
                        var comp = gameObject.AddComponent<RandomWalkMA>();
                        comp.InspectorAwake();
                    }
                    break;
                case MainAlgo.ROOM_GEN:
                    {
                        var comp = gameObject.AddComponent<RanRoomGenMA>();
                        comp.InspectorAwake();
                    }
                    break;
                case MainAlgo.CELLULAR_AUTOMATA:
                    {
                        var comp = gameObject.AddComponent<CellularAutomataMA>();
                        comp.InspectorAwake();
                    }
                    break;
                case MainAlgo.L_SYSTEM:
                    {
                        var comp = gameObject.AddComponent<LSystem>();
                        comp.InspectorAwake();
                    }
                    break;
                case MainAlgo.DELAUNAY:
                    {
                        var comp = gameObject.AddComponent<DelunaryMA>();
                        comp.InspectorAwake();
                    }
                    break;
                case MainAlgo.WAVE_FUNCTION_COLLAPSE:
                    {
                        gameObject.AddComponent<WFCRuleDecipher>();

                        var comp = gameObject.AddComponent<NewWFCAlog>();
                        comp.InspectorAwake();
                    }
                    break;
                case MainAlgo.PERLIN_NOISE:
                    {
                        var comp = gameObject.AddComponent<PerlinNoiseMA>();
                        comp.InspectorAwake();
                    }
                    break;
                case MainAlgo.PERLIN_WORM:
                    {
                        var comp = gameObject.AddComponent<PerlinWormsMA>();
                        comp.InspectorAwake();
                    }
                    break;
                case MainAlgo.DIAMOND_SQUARE:
                    {
                        var comp = gameObject.AddComponent<DiamondSquareMA>();
                        comp.InspectorAwake();
                    }
                    break;
                case MainAlgo.DIFFUSION_LIMITATION_AGGREGATION:
                    {
                        var comp = gameObject.AddComponent<DiffLimAggMA>();
                        comp.InspectorAwake();
                    }
                    break;
                case MainAlgo.GENERATE_YOUR_DUNGEON:
                    {
                        var comp = gameObject.AddComponent<LoadMapMA>();
                        comp.InspectorAwake();
                    }
                    break;
                default:
                    {
                        Debug.Log($"There was an issue with this setting");
                    }
                    break;
            }
        }

        public void DelPrevAlgo()
        {
            switch (currMainAlgoIDX)
            {
                case 0:
                    DestroyImmediate(gameObject.GetComponent<VoronoiMA>());
                    break;
                case 1:

                    DestroyImmediate(gameObject.GetComponent<RandomWalkMA>());

                    break;
                case 2:
                    DestroyImmediate(gameObject.GetComponent<RanRoomGenMA>());
                    break;
                case 3:
                    DestroyImmediate(gameObject.GetComponent<CellularAutomataMA>());
                    break;
                case 4:
                    DestroyImmediate(gameObject.GetComponent<LSystem>());
                    break;
                case 5:
                    DestroyImmediate(gameObject.GetComponent<DelunaryMA>());
                    break;
                case 6:
                    DestroyImmediate(gameObject.GetComponent<NewWFCAlog>());
                    DestroyImmediate(gameObject.GetComponent<WFCRuleDecipher>());

                    foreach (Transform child in transform)
                        DestroyImmediate(child.gameObject);

                    break;
                case 7:
                    DestroyImmediate(gameObject.GetComponent<PerlinNoiseMA>());
                    break;
                case 8:
                    DestroyImmediate(gameObject.GetComponent<PerlinWormsMA>());
                    break;
                case 9:
                    DestroyImmediate(gameObject.GetComponent<DiamondSquareMA>());
                    break;
                case 10:
                    DestroyImmediate(gameObject.GetComponent<DiffLimAggMA>());
                    break;
                case 11:
                    DestroyImmediate(gameObject.GetComponent<LoadMapMA>());
                    break;

                default:
                    break;
            }

            currMainAlgoIDX = 11;
        }

        public void Restart()
        {
            DFGeneralUtil.RestartGrid(gridArr);
            Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColAnchor(gridArr);

            prevGridArray2D.Clear();
        }

        #endregion


        #region Generation area
        private void CheckChunkRender()
        {
            var indexesToDraw = new HashSet<int>();

            foreach (var player in player)
            {
                int collidedIndex = -1;

                for (int i = 0; i < chunks.Count; i++)
                {
                    if (AABBCol(player.transform.position, chunks[i]))
                    {
                        collidedIndex = i;
                        break;
                    }
                }

                if (collidedIndex == -1)
                {
                    Debug.Log($"The player is out of bounds");
                }
                else
                {
                    int botCornerLeft = collidedIndex - CLength * loadingRange + loadingRange;
                    int botCornerRight = collidedIndex - CLength * loadingRange - loadingRange;

                    int chunksToDrawLength = (botCornerLeft - botCornerRight) + 1;

                    for (int i = 0; i < chunksToDrawLength; i++)
                    {
                        for (int j = 0; j < chunksToDrawLength; j++)
                        {
                            int newIndex = botCornerRight + j + (i * CLength);

                            if (newIndex < 0 || newIndex >= chunks.Count)
                                continue;
                            else
                                indexesToDraw.Add(botCornerRight + j + (i * CLength));
                        }
                    }
                }
            }

            foreach (var chunk in chunks)
            {
                if (indexesToDraw.Contains(chunk.index))
                {
                    chunk.mainParent.SetActive(true);
                }
                else
                {
                    chunk.mainParent.SetActive(false);
                }
            }
        }

        /// <summary>
        /// returns true if it collides
        /// </summary>
        /// <param name="player"></param>
        /// <param name="chunk"></param>
        /// <returns></returns>
        public bool AABBCol(Vector3 player, Chunk chunk)
        {
            if (player.x >= chunk.bottomLeft.x && player.x < chunk.topRight.x)
            {
                if (player.z >= chunk.bottomLeft.y && player.z < chunk.topRight.y)
                {
                    return true;
                }
            }

            return false;
        }

        public void FormObject(Mesh mesh)
        {
            GameObject newPart = new GameObject();
            newPart.transform.position = parentObj.position;
            newPart.transform.rotation = parentObj.rotation;
            newPart.transform.localScale = parentObj.localScale;
            newPart.transform.parent = parentObj;

            var filter = newPart.AddComponent<MeshFilter>();
            filter.mesh = mesh;

            var meshRenderer = newPart.AddComponent<MeshRenderer>();

            var collider = newPart.AddComponent<MeshCollider>();
            collider.convex = false;
        }

        public int RatioBasedChoice(List<TileRuleSetPCG> objects)
        {
            int totRatio = 0;

            for (int i = 0; i < objects.Count; i++)
            {
                totRatio += objects[i].occurance;
            }

            int ranNum = Random.Range(0, totRatio);

            int countRatio = 0;
            int savedIdx = 0;
            for (int i = 1; i < objects.Count; i++)
            {
                if (ranNum > countRatio && ranNum <= countRatio + objects[i].occurance)
                {
                    savedIdx = i;
                    break;
                }
                countRatio += objects[i].occurance;
            }

            return savedIdx;
        }

        public void ChunkCreate(int height, int width)
        {
            int maxWidth = gridArr.GetLength(0);
            int maxHeight = gridArr.GetLength(1);

            chunkObjs.Clear();

            Vector2Int BLhead = Vector2Int.zero;
            Vector2Int TRhead = Vector2Int.zero;

            int correctHeight = (maxHeight - 1) - TRhead.y >= height ? height : (maxHeight - 1) - TRhead.y;
            TRhead = new Vector2Int(0, TRhead.y + correctHeight);

            int timerCheck = 0;

            chunks = new List<Chunk>();
            while (true)
            {
                if (TRhead.x + 1 >= maxWidth)  // needs to go in the new line
                {
                    if (TRhead.y + 1 >= maxHeight)  // this checks if we are dont with the algo
                    {
                        break;
                    }

                    BLhead = new Vector2Int(0, TRhead.y);

                    correctHeight = (maxHeight - 1) - TRhead.y >= height ? height : (maxHeight - 1) - TRhead.y;

                    TRhead = new Vector2Int(0, TRhead.y + correctHeight + 1);
                    timerCheck = 0;
                }
                else
                {
                    timerCheck++;
                    int correctWidth = (maxWidth - 1) - TRhead.x >= width ? width : (maxWidth - 1) - TRhead.x;

                    TRhead = new Vector2Int(TRhead.x + correctWidth + 1, TRhead.y);

                    chunks.Add(new Chunk() { width = correctWidth, height = correctHeight });

                    var currChunk = chunks[chunks.Count - 1];
                    currChunk.topRight = TRhead;
                    currChunk.bottomLeft = BLhead;
                    currChunk.index = chunks.Count - 1;

                    BLhead = new Vector2Int(TRhead.x, BLhead.y);
                }
            }

            for (int i = 0; i < chunks.Count; i++)
            {
                var objRef = new GameObject();
                chunkObjs.Add(objRef.transform);
                objRef.transform.parent = parentObj;
                objRef.transform.name = i.ToString();
                objRef.isStatic = true;
                chunks[i].mainParent = objRef;

                int widthChunk = chunks[i].topRight.x - chunks[i].bottomLeft.x;
                int heightChunk = chunks[i].topRight.y - chunks[i].bottomLeft.y;

                for (int y = 0; y < heightChunk; y++)
                {
                    for (int x = 0; x < widthChunk; x++)
                    {
                        gridArr[x + chunks[i].bottomLeft.x, y + chunks[i].bottomLeft.y].idx = chunks[i].index;
                    }
                }
            }

            CLength = timerCheck;

        }

        public void DrawTileMapDirectionalWalls()
        {
            ChunkCreate(chunkWidth, chunkHeight);

            if (WallsTiles.Count == 0 || CeilingTiles.Count == 0 || FloorTiles.Count == 0)
            {
                EditorUtility.DisplayDialog("Invalid tile Rules given", "Please make sure you have loaded all of the tile object correctly and all the 3 lists have at least one object in them to use this generation method", "OK!");
                return;
            }

            for (int z = 0; z < RoomHeight; z++)  // this is the heihgt of the room
            {
                for (int y = 0; y < gridArr.GetLength(1); y++)
                {
                    for (int x = 0; x < gridArr.GetLength(0); x++)
                    {
                        if (z == 0 || z == RoomHeight - 1) //we draw everything as this is the ceiling and the floor       THIS IS WHERE THE CEILING SHOULD BE
                        {
                            if (gridArr[x, y].tileType != DFTile.TileType.VOID)
                            {
                                var objRef = Instantiate(FloorTiles.Count > 1 ? FloorTiles[RatioBasedChoice(FloorTiles)].objectPrefab : FloorTiles[0].objectPrefab, parentObj);
                                parentObj.GetChild(0);

                                objRef.transform.parent = chunkObjs[gridArr[x, y].idx].transform;
                                objRef.isStatic = true;
                                objRef.transform.position = new Vector3(x, z, y);
                            }
                        }

                        if (gridArr[x, y].tileType == DFTile.TileType.WALL || gridArr[x, y].tileType == DFTile.TileType.WALLCORRIDOR)
                        {
                            var checkVector = new Vector2Int(x + 1, y);// right check

                            if (checkVector.x < 0 || checkVector.y < 0 || checkVector.x >= gridArr.GetLength(0) || checkVector.y >= gridArr.GetLength(1))
                            {
                                var objRef = Instantiate(WallsTiles.Count > 1 ? WallsTiles[RatioBasedChoice(WallsTiles)].objectPrefab : WallsTiles[0].objectPrefab, parentObj);

                                objRef.transform.position = new Vector3(x, z, y);
                                objRef.transform.Rotate(0, 90, 0);
                                objRef.isStatic = true;
                                objRef.transform.parent = chunkObjs[gridArr[x, y].idx].transform;
                            }
                            else
                            {
                                if (gridArr[checkVector.x, checkVector.y].tileType == DFTile.TileType.VOID)
                                {
                                    var objRef = Instantiate(WallsTiles.Count > 1 ? WallsTiles[RatioBasedChoice(WallsTiles)].objectPrefab : WallsTiles[0].objectPrefab, parentObj);

                                    objRef.transform.position = new Vector3(x, z, y);
                                    objRef.transform.Rotate(0, 90, 0);
                                    objRef.isStatic = true;
                                    objRef.transform.parent = chunkObjs[gridArr[x, y].idx].transform;
                                }
                            }



                            checkVector = new Vector2Int(x - 1, y);// left check

                            if (checkVector.x < 0 || checkVector.y < 0 || checkVector.x >= gridArr.GetLength(0) || checkVector.y >= gridArr.GetLength(1))
                            {
                                var objRef = Instantiate(WallsTiles.Count > 1 ? WallsTiles[RatioBasedChoice(WallsTiles)].objectPrefab : WallsTiles[0].objectPrefab, parentObj);

                                objRef.transform.position = new Vector3(x, z, y);
                                objRef.transform.Rotate(0, 270, 0);
                                objRef.isStatic = true;
                                objRef.transform.parent = chunkObjs[gridArr[x, y].idx].transform;
                            }
                            else
                            {
                                if (gridArr[checkVector.x, checkVector.y].tileType == DFTile.TileType.VOID)
                                {
                                    var objRef = Instantiate(WallsTiles.Count > 1 ? WallsTiles[RatioBasedChoice(WallsTiles)].objectPrefab : WallsTiles[0].objectPrefab, parentObj);

                                    objRef.transform.position = new Vector3(x, z, y);
                                    objRef.transform.Rotate(0, 270, 0);
                                    objRef.isStatic = true;
                                    objRef.transform.parent = chunkObjs[gridArr[x, y].idx].transform;
                                }
                            }


                            checkVector = new Vector2Int(x, y + 1);// above check

                            if (checkVector.x < 0 || checkVector.y < 0 || checkVector.x >= gridArr.GetLength(0) || checkVector.y >= gridArr.GetLength(1))
                            {
                                var objRef = Instantiate(WallsTiles.Count > 1 ? WallsTiles[RatioBasedChoice(WallsTiles)].objectPrefab : WallsTiles[0].objectPrefab, parentObj);

                                objRef.transform.position = new Vector3(x, z, y);
                                objRef.isStatic = true;
                                objRef.transform.parent = chunkObjs[gridArr[x, y].idx].transform;
                                objRef.transform.Rotate(0, 0, 0);
                            }
                            else
                            {
                                if (gridArr[checkVector.x, checkVector.y].tileType == DFTile.TileType.VOID)
                                {
                                    var objRef = Instantiate(WallsTiles.Count > 1 ? WallsTiles[RatioBasedChoice(WallsTiles)].objectPrefab : WallsTiles[0].objectPrefab, parentObj);

                                    objRef.transform.position = new Vector3(x, z, y);
                                    objRef.transform.Rotate(0, 0, 0);
                                    objRef.isStatic = true;
                                    objRef.transform.parent = chunkObjs[gridArr[x, y].idx].transform;
                                }
                            }

                            checkVector = new Vector2Int(x, y - 1);// down check

                            if (checkVector.x < 0 || checkVector.y < 0 || checkVector.x >= gridArr.GetLength(0) || checkVector.y >= gridArr.GetLength(1))
                            {
                                var objRef = Instantiate(WallsTiles.Count > 1 ? WallsTiles[RatioBasedChoice(WallsTiles)].objectPrefab : WallsTiles[0].objectPrefab, parentObj);

                                objRef.transform.position = new Vector3(x, z, y);
                                objRef.transform.Rotate(0, 180, 0);
                                objRef.isStatic = true;
                                objRef.transform.parent = chunkObjs[gridArr[x, y].idx].transform;
                            }
                            else
                            {
                                if (gridArr[checkVector.x, checkVector.y].tileType == DFTile.TileType.VOID)
                                {
                                    var objRef = Instantiate(WallsTiles.Count > 1 ? WallsTiles[RatioBasedChoice(WallsTiles)].objectPrefab : WallsTiles[0].objectPrefab, parentObj);

                                    objRef.transform.position = new Vector3(x, z, y);
                                    objRef.transform.Rotate(0, 180, 0);
                                    objRef.isStatic = true;
                                    objRef.transform.parent = chunkObjs[gridArr[x, y].idx].transform;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void DrawTileMapBlockType()
        {
            ChunkCreate(chunkWidth, chunkHeight);

            if (WallsTiles.Count == 0 || CeilingTiles.Count == 0 || FloorTiles.Count == 0)
            {
                EditorUtility.DisplayDialog("Invalid tile Rules given", "Please make sure you have loaded all of the tile object correctly and all the 3 lists have at least one object in them to use this Generation method", "OK!");
                return;
            }

            for (int z = 0; z < RoomHeight; z++)  // this is the heihgt of the room
            {
                for (int y = 0; y < gridArr.GetLength(0); y++)
                //Parallel.For(0, gridArr.GetLength(1), y =>
                {
                    for (int x = 0; x < gridArr.GetLength(0); x++)
                    {
                        if (z == 0) //we draw everything as this is the ceiling and the floor       THIS IS WHERE THE CEILING SHOULD BE
                        {
                            if (gridArr[x, y].tileType != DFTile.TileType.VOID)
                            {
                                var objRef = Instantiate(FloorTiles.Count > 1 ? FloorTiles[RatioBasedChoice(FloorTiles)].objectPrefab : FloorTiles[0].objectPrefab, parentObj);
                                parentObj.GetChild(0);

                                objRef.transform.parent = chunkObjs[gridArr[x, y].idx].transform;
                                objRef.isStatic = true;
                                objRef.transform.position = new Vector3(x, z, y);
                            }
                        }
                        else if (z == RoomHeight - 1)
                        {
                            if (gridArr[x, y].tileType != DFTile.TileType.VOID)
                            {
                                var objRef = Instantiate(CeilingTiles.Count > 1 ? FloorTiles[RatioBasedChoice(CeilingTiles)].objectPrefab : CeilingTiles[0].objectPrefab, parentObj);
                                parentObj.GetChild(0);

                                objRef.transform.parent = chunkObjs[gridArr[x, y].idx].transform;
                                objRef.isStatic = true;
                                objRef.transform.position = new Vector3(x, z, y);
                            }
                        }
                        else
                        {
                            if (gridArr[x, y].tileType == DFTile.TileType.WALL || gridArr[x, y].tileType == DFTile.TileType.WALLCORRIDOR)
                            {
                                var objRef = Instantiate(WallsTiles.Count > 1 ? WallsTiles[RatioBasedChoice(WallsTiles)].objectPrefab : WallsTiles[0].objectPrefab, parentObj);

                                objRef.transform.position = new Vector3(x, z, y);
                                objRef.isStatic = true;
                                objRef.transform.parent = chunkObjs[gridArr[x, y].idx].transform;
                            }
                        }
                    }
                }
            }
        }

        #endregion


        #region classes

        public class TilesRuleSetPCG
        {
            public List<TileRuleSetPCG> FloorTiles = new List<TileRuleSetPCG>();
            public List<TileRuleSetPCG> CeilingTiles = new List<TileRuleSetPCG>();
            public List<TileRuleSetPCG> WallsTiles = new List<TileRuleSetPCG>();
        }

        [Serializable]
        public class TileRuleSetPCG
        {
            [Tooltip("Occurange is the ratio at which this object will spawn in respect to other objects if they are present")]
            public int occurance = 1;
            [Tooltip("Object to spawn")]
            public GameObject objectPrefab;
        }

        [Serializable]
        public class Chunk
        {
            public Vector2Int topRight = Vector2Int.zero;
            public Vector2Int bottomLeft = Vector2Int.zero;

            public int width;
            public int height;

            public int index = 0;
            public GameObject mainParent = null;
            public List<GameObject> listOfObjInChunk = new List<GameObject>();
        }

        #endregion
    }


}