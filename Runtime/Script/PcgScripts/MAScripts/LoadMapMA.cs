

namespace DungeonForge.AlgoScript
{
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using UnityEditor;
    using UnityEngine;

    using Random = UnityEngine.Random;
    using DungeonForge.Utils;
    using System;
    using static DungeonForge.AlgoScript.PCGManager;

    public class LoadMapMA : MonoBehaviour
    {
        [SerializeField]
        public List<TileRuleSetPCG> mapRandomObjects = new List<TileRuleSetPCG>();

        [HideInInspector]
        public bool generatedMap = false;


        [HideInInspector]
        public PCGManager pcgManager;

        [HideInInspector]
        public List<List<DFTile>> rooms = new List<List<DFTile>>();


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


        public DFTile[,] LoadDataCall(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                EditorUtility.DisplayDialog("Error", "The file name given is not valie", "OK");
                return null;
            }

            string filePath = Application.dataPath + "/Resources/Resources_Algorithms/Saved_Gen_Data/" + fileName;

            if (File.Exists(filePath))
            {
                byte[] data = File.ReadAllBytes(filePath);
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream(data);
                SerializableTile[,] serializableMap = (SerializableTile[,])formatter.Deserialize(stream);

                DFTile[,] map = new DFTile[serializableMap.GetLength(0), serializableMap.GetLength(1)];
                for (int i = 0; i < serializableMap.GetLength(1); i++)
                {
                    for (int j = 0; j < serializableMap.GetLength(0); j++)
                    {
                        map[j, i] = new DFTile(serializableMap[j, i].position, serializableMap[j, i].tileWeight, serializableMap[j, i].cost, serializableMap[j, i].tileType);

                        if (map[j, i].tileType == DFTile.TileType.WALLCORRIDOR)
                            map[j, i].tileType = DFTile.TileType.WALL;

                        if (map[j, i].tileType == DFTile.TileType.FLOORCORRIDOR)
                            map[j, i].tileType = DFTile.TileType.FLOORROOM;
                    }
                }

                return map;
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "The file name given is not valie", "OK");
            }
            return null;
        }

        public void AddOnGridData(DFTile[,] gridToAddOn, bool wallDominance) 
        {

            for (int y = 0; y < pcgManager.gridArr.GetLength(1); y++)
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
                                    if (pcgManager.gridArr[x,y].tileType == DFTile.TileType.VOID) 
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
                                    if (pcgManager.gridArr[x, y].tileType == DFTile.TileType.VOID )
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
            }

        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (state == UI_STATE.SELF_EDITING) 
            {
                Gizmos.DrawSphere(new Vector3(pointerPosition.x - pcgManager.gridArr.GetLength(0)/2, 0, pointerPosition.y - pcgManager.gridArr.GetLength(1) / 2), 0.5f);
            }
            else if (state == UI_STATE.GENERATE) 
            {
                Gizmos.DrawSphere(new Vector3(0, heigthPoissant, 0),0.5f);
            }
        }
    }
}