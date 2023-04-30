namespace DungeonForge.AlgoScript
{
    using System.Collections.Generic;
    using UnityEngine;
    using DungeonForge.Utils;
    using static DungeonForge.AlgoScript.PCGManager;
    using System.Threading.Tasks;

    public class LoadMapMA : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Populate this list with the ranomd debries to spawn around the map when using the poissant algorithm")]
        public List<TileRuleSetPCG> mapRandomObjects = new List<TileRuleSetPCG>();

        [HideInInspector]
        public bool generatedMap = false;


        [HideInInspector]
        public PCGManager pcgManager;

        [HideInInspector]
        public List<List<DFTile>> rooms = new List<List<DFTile>>();

        [HideInInspector]
        public List<Edge> edges = new List<Edge>();

        [HideInInspector]
        public bool singleStringSelected = true;
        [HideInInspector]
        public string fileName = "";
        [HideInInspector]
        public List<string> stringList = new List<string>();
        [HideInInspector]
        public int stringListSize = 1;
        [HideInInspector]
        public bool manualEditingMode = false;

        [HideInInspector]
        public int currStateIndex;

        [HideInInspector]
        public bool allowedBack = false;

        [HideInInspector]
        public bool allowedForward = false;

        [HideInInspector]
        public Vector2Int pointerPosition = new Vector2Int(0,0);


        [HideInInspector]
        public float heigthPoissant = 0;


        public enum UI_STATE
        {
            PICKING_FILE,
            SELF_EDITING,
            GENERATE
        }

        [HideInInspector]
        public UI_STATE state;


        public void InspectorAwake()
        {
            pcgManager = this.transform.GetComponent<PCGManager>();
        }

        [HideInInspector]
        public List<GameObject> generatedPoissantObjList = new List<GameObject>();

        public void AddOnGridData(DFTile[,] gridToAddOn, bool wallDominance) 
        {
            Parallel.For(0, pcgManager.gridArr.GetLength(1), y =>
            {
                for (int x = 0; x < pcgManager.gridArr.GetLength(0); x++)
                {
                    if (x >= gridToAddOn.GetLength(0) || y >= gridToAddOn.GetLength(1))
                    {
                        continue;
                    }

                    switch (gridToAddOn[x, y].tileType)
                    {
                        case DFTile.TileType.VOID:
                            {

                                break;
                            }

                        case DFTile.TileType.WALL:
                            {
                                if (wallDominance)
                                {
                                    pcgManager.gridArr[x, y] = new DFTile(gridToAddOn[x, y]);
                                }
                                else
                                {
                                    if (pcgManager.gridArr[x, y].tileType == DFTile.TileType.VOID)
                                    {
                                        pcgManager.gridArr[x, y] = new DFTile(gridToAddOn[x, y]);
                                    }
                                }

                                break;
                            }
                        case DFTile.TileType.WALLCORRIDOR:
                            {
                                if (wallDominance)
                                {
                                    pcgManager.gridArr[x, y] = new DFTile(gridToAddOn[x, y]);
                                }
                                else
                                {
                                    if (pcgManager.gridArr[x, y].tileType == DFTile.TileType.VOID)
                                    {
                                        pcgManager.gridArr[x, y] = new DFTile(gridToAddOn[x, y]);
                                    }
                                }

                                break;
                            }

                        case DFTile.TileType.FLOORROOM:
                            {

                                if (!wallDominance)
                                {
                                    if (pcgManager.gridArr[x, y].tileType == DFTile.TileType.VOID || pcgManager.gridArr[x, y].tileType == DFTile.TileType.WALL || pcgManager.gridArr[x, y].tileType == DFTile.TileType.WALLCORRIDOR)
                                    {
                                        pcgManager.gridArr[x, y] = new DFTile(gridToAddOn[x, y]);
                                    }
                                }
                                else
                                {
                                    if (pcgManager.gridArr[x, y].tileType == DFTile.TileType.VOID)
                                    {
                                        pcgManager.gridArr[x, y] = new DFTile(gridToAddOn[x, y]);
                                    }
                                }

                                break;
                            }
                        case DFTile.TileType.FLOORCORRIDOR:
                            {

                                if (!wallDominance)
                                {
                                    if (pcgManager.gridArr[x, y].tileType == DFTile.TileType.VOID || pcgManager.gridArr[x, y].tileType == DFTile.TileType.WALL || pcgManager.gridArr[x, y].tileType == DFTile.TileType.WALLCORRIDOR)
                                    {
                                        pcgManager.gridArr[x, y] = new DFTile(gridToAddOn[x, y]);
                                    }
                                }
                                else
                                {
                                    if (pcgManager.gridArr[x, y].tileType == DFTile.TileType.VOID)
                                    {
                                        pcgManager.gridArr[x, y] = new DFTile(gridToAddOn[x, y]);
                                    }
                                }
                                break;
                            }

                        default:
                            break;
                    }
                }
            });
        }
        
        private void OnDrawGizmos()
        {
            if (state == UI_STATE.SELF_EDITING) 
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(new Vector3(pointerPosition.x - pcgManager.gridArr.GetLength(0)/2, 0, pointerPosition.y - pcgManager.gridArr.GetLength(1) / 2), 0.5f);
            }
            else if (state == UI_STATE.GENERATE && generatedMap)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(new Vector3(0, heigthPoissant, 0),0.5f);
            }
        }

        public void DeleteAllGenObjects() 
        {
            if (generatedPoissantObjList.Count > 0) 
            {
                foreach (GameObject obj in generatedPoissantObjList)
                {
                    DestroyImmediate(obj);
                }
            }

            generatedPoissantObjList.Clear();
        }

    }
}