using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DungeonForge.AlgoScript
{
    using DungeonForge.Utils;

    public class LSystem : MonoBehaviour
    {
        //https://www.gamedeveloper.com/design/kastle-dungeon-generation-using-l-systems

        private List<Vector3Int> points = new List<Vector3Int>();
        private Vector3Int head;

        private string endingWord;
        private int currDirection = 0;

        [HideInInspector]
        public List<List<DFTile>> rooms = new List<List<DFTile>>();

        [Space(30)]
        [Header("This is the starting word")]
        public string axium;
        [Header("How many times should the algorithms run")]
        public int iterations = 2;


        [Space(30)]
        [Header("Name of the file containing the rulesets")]
        public string fileName = "";


        [Space(30)]
        [Header("----- RULES -----")]
        public int A_dist = 0;
        public int B_dist = 0;
        public int C_dist = 0;


        [Space(30)]
        public List<string> A_RuleSet = new List<string>();
        public List<string> B_RuleSet = new List<string>();
        public List<string> C_RuleSet = new List<string>();
        public List<string> S_RuleSet = new List<string>();
        public List<string> L_RuleSet = new List<string>();
        public List<string> P_RuleSet = new List<string>();
        public List<string> N_RuleSet = new List<string>();

        public List<LSystemMacrosBuildings> roomMacros = new List<LSystemMacrosBuildings>();

        [Space(30)]
        [Header("Corridor Width")]
        [Range(2,5)]
        public int corridorWidth = 3;
        [Space(10)]
        [Header("Should the paths be allowed to be diagonal or not?")]
        public bool diagonalPaths;
        [Space(20)]
        [HideInInspector]
        public bool generated = false;

        [HideInInspector]
        public bool loaded = false;


        private PCGManager pcgManager;
        public PCGManager PcgManager
        {
            get { return pcgManager; }
        }


        public void InspectorAwake()
        {
            head = Vector3Int.zero;
            pcgManager = transform.GetComponent<PCGManager>();
        }

        private string RunLSystem(string axium)
        {
            string currentWord = axium;

            for (int i = 0; i < iterations; i++)
            {
                Queue<string> newWordQue = new Queue<string>();

                for (int x = 0; x < currentWord.Length; x++)
                {
                    switch (currentWord[x])
                    {
                        case 'A':

                            if (A_RuleSet.Count == 0)
                                newWordQue.Enqueue(currentWord[x].ToString());

                            else if (A_RuleSet.Count == 1)
                                newWordQue.Enqueue(A_RuleSet[0]);

                            else
                                newWordQue.Enqueue(A_RuleSet[Random.Range(0, A_RuleSet.Count - 1)]);

                            break;

                        case 'B':

                            if (B_RuleSet.Count == 0)
                                newWordQue.Enqueue(currentWord[x].ToString());

                            else if (B_RuleSet.Count == 1)
                                newWordQue.Enqueue(B_RuleSet[0]);

                            else
                                newWordQue.Enqueue(B_RuleSet[Random.Range(0, B_RuleSet.Count - 1)]);

                            break;

                        case 'C':

                            if (C_RuleSet.Count == 0)
                                newWordQue.Enqueue(currentWord[x].ToString());

                            else if (C_RuleSet.Count == 1)
                                newWordQue.Enqueue(C_RuleSet[0]);

                            else
                                newWordQue.Enqueue(C_RuleSet[Random.Range(0, C_RuleSet.Count - 1)]);

                            break;

                        case 'S':

                            if (S_RuleSet.Count == 0)
                                newWordQue.Enqueue(currentWord[x].ToString());

                            else if (S_RuleSet.Count == 1)
                                newWordQue.Enqueue(S_RuleSet[0]);

                            else
                                newWordQue.Enqueue(S_RuleSet[Random.Range(0, S_RuleSet.Count - 1)]);

                            break;

                        case 'L':

                            if (L_RuleSet.Count == 0)
                                newWordQue.Enqueue(currentWord[x].ToString());

                            else if (L_RuleSet.Count == 1)
                                newWordQue.Enqueue(L_RuleSet[0]);

                            else
                                newWordQue.Enqueue(L_RuleSet[Random.Range(0, L_RuleSet.Count - 1)]);

                            break;

                        case 'P':

                            if (P_RuleSet.Count == 0)
                                newWordQue.Enqueue(currentWord[x].ToString());

                            else if (P_RuleSet.Count == 1)
                                newWordQue.Enqueue(P_RuleSet[0]);

                            else
                                newWordQue.Enqueue(P_RuleSet[Random.Range(0, P_RuleSet.Count - 1)]);

                            break;

                        case 'N':

                            if (N_RuleSet.Count == 0)
                                newWordQue.Enqueue(currentWord[x].ToString());

                            else if (N_RuleSet.Count == 1)
                                newWordQue.Enqueue(N_RuleSet[0]);

                            else
                                newWordQue.Enqueue(N_RuleSet[Random.Range(0, N_RuleSet.Count - 1)]);

                            break;


                        default:
                            break;
                    }
                }

                currentWord = string.Empty;
                while (newWordQue.Count > 0)
                {
                    currentWord = currentWord + newWordQue.Dequeue();
                }

            }

            return currentWord.ToString();
        }

        public void RunIteration()
        {
            head = new Vector3Int(pcgManager.width / 2, 0, pcgManager.height / 2);

            pcgManager.Restart();

            points.Clear();
            currDirection = 0;
            endingWord = RunLSystem(axium);
            ProcessSentence();
            SetUpCorridors();
            DrawMap();
            pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(pcgManager.gridArr,0,1,true);
        }

        private void ProcessSentence()
        {
            Stack<Vector3Int> lastPositions = new Stack<Vector3Int>();

            for (int i = 0; i < endingWord.Length; i++)
            {
                bool found = true;

                switch (endingWord[i])
                {
                    case 'A':
                        MoveHead(A_dist);
                        if (!points.Contains(head))
                            points.Add(head);
                        break;

                    case 'B':
                        MoveHead(B_dist);
                        if (!points.Contains(head))
                            points.Add(head);
                        break;

                    case 'C':
                        MoveHead(C_dist);
                        if (!points.Contains(head))
                            points.Add(head);
                        break;


                    case 'S':
                        lastPositions.Push(head);
                        break;

                    case 'L':
                        head = lastPositions.Pop();
                        break;


                    case '+':

                        if (currDirection + 1 >= 5)
                        {
                            currDirection = 1;
                        }
                        else
                        {
                            currDirection += 1;
                        }

                        break;

                    case '-':


                        if (currDirection - 1 <= 0)
                        {
                            currDirection = 4;
                        }
                        else
                        {
                            currDirection -= 1;
                        }


                        break;

                    default:
                        found = false;
                        break;
                }

                if (!found) 
                {
                    for (int j = 0; j < roomMacros.Count; j++)
                    {
                        if (roomMacros[j].character == endingWord[i]) 
                        {
                            switch (roomMacros[j].roomType)
                            {
                                case LSystemMacrosBuildings.TYPE_OF_ROOM.CIRCLE_ROOM:
                                    {
                                      var room = DFAlgoBank.CreateCircleRoom(pcgManager.gridArr, new Vector2Int(head.x, head.z), roomMacros[j].widthORRadius, actuallyDraw: true);

                                      rooms.Add(room);
                                    }
                                    break;

                                case LSystemMacrosBuildings.TYPE_OF_ROOM.RECTANGLE_ROOM:
                                    {
                                        var room  = DFAlgoBank.CreateSquareRoom(roomMacros[j].widthORRadius, roomMacros[j].height, new Vector2Int(head.x, head.z), pcgManager.gridArr, true);

                                        rooms.Add(room);
                                    }
                                    break;

                                case LSystemMacrosBuildings.TYPE_OF_ROOM.RANDOM_ROOM:
                                    {
                                        var roomBounds = new BoundsInt() { xMin = head.x - roomMacros[j].widthORRadius / 2, xMax = head.x + roomMacros[j].widthORRadius / 2, zMin = head.y - roomMacros[j].height / 2, zMax = head.y + roomMacros[j].height / 2 };
                                       
                                        var room = DFAlgoBank.CompartimentalisedCA(roomBounds);

                                        for (int y = 0; y < room.GetLength(1); y++)
                                        {
                                            for (int x = 0; x < room.GetLength(0); x++)
                                            {

                                                if (x< 0 || y<0 || x >= room.GetLength(0) || y >= room.GetLength(1)) { continue; }

                                                if (room[x, y].tileWeight == 1)
                                                {
                                                    pcgManager.gridArr[x + roomBounds.xMin, y + roomBounds.zMin].tileWeight = 1;
                                                }
                                                else
                                                {
                                                    pcgManager.gridArr[x + roomBounds.xMin, y + roomBounds.zMin].tileWeight = 0;
                                                }
                                            }
                                        }
                                    }
                                    break;

                                default:
                                    break;
                            }

                            break;
                        }
                    }
                }
            }
        }

        private void MoveHead(int moveAmount)
        {
            switch (currDirection)
            {
                case 1:
                    head += new Vector3Int(moveAmount, 0, 0);
                    break;


                case 2:
                    head += new Vector3Int(0, 0, moveAmount);
                    break;


                case 3:
                    head += new Vector3Int(-moveAmount, 0, 0);
                    break;


                case 4:
                    head += new Vector3Int(0, 0, -moveAmount);
                    break;


                default:
                    break;
            }
        }

        private void SetUpCorridors()
        {
            for (int i = 0; i < points.Count; i++)
            {
                if (i != points.Count - 1)
                {
                    var path = DFAlgoBank.A_StarPathfinding2D(pcgManager.gridArr, new Vector2Int(points[i].x, points[i].z), new Vector2Int(points[i + 1].x, points[i + 1].z), diagonalPaths);

                    foreach (var tile in path.Item1)
                    {
                        tile.tileWeight = 1;
                        tile.tileType = DFTile.TileType.FLOORCORRIDOR;
                    }
                }
            }
        }

        private void DrawMap() 
        {
            DFAlgoBank.SetUpTileCorridorTypesUI(pcgManager.gridArr, corridorWidth);
        }

        [Serializable]
        public class LSystemMacrosBuildings
        {
            public char character;

            public int widthORRadius;
            public int height;

            public enum TYPE_OF_ROOM
            {
                CIRCLE_ROOM,
                RECTANGLE_ROOM,
                RANDOM_ROOM
            }
            public TYPE_OF_ROOM roomType;

        }
    }
}