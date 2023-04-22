using System.Collections.Generic;
using UnityEngine;


namespace DungeonForge.AlgoScript
{
    using DungeonForge.Utils;

    public class RanRoomGenMA : MonoBehaviour, IUndoInteraction
    {
        private PCGManager pcgManager;
        public PCGManager PcgManager
        {
            get { return pcgManager; }
        }

        private int maxWidth;
        public int MaxWidth
        {
            get { return maxWidth; }
            set { maxWidth = value; }
        }

        private int minWidth;
        public int MinWidth
        {
            get { return minWidth; }
            set { minWidth = value; }
        }

        private int maxHeight;
        public int MaxHeight
        {
            get { return maxHeight; }
            set { maxHeight = value; }
        }


        private int minHeight;
        public int MinHeight
        {
            get { return minHeight; }
            set { minHeight = value; }
        }

        private int numOfRoom;
        public int NumOfRoom
        {
            get { return numOfRoom; }
            set { numOfRoom = value; }
        }

        private bool BPS;
        public bool BPSg
        {
            get { return BPS; }
            set { BPS = value; }
        }

        private bool started;
        public bool Started
        {
            get { return started; }
            set { started = value; }
        }


        public List<RoomsClass> roomList = new List<RoomsClass>();
        private List<BoundsInt> roomsListBPSAlgo = new List<BoundsInt>();

        public List<BoundsInt> RoomList
        {
            get { return roomsListBPSAlgo; }
        }

        [HideInInspector]
        public bool pathType = false;
       

        private int minSizeRoom = 40;
        public int MinSizeRoom
        {
            get { return minSizeRoom; }
            set { minSizeRoom = value; }
        }


        public List<List<DFTile>> rooms = new List<List<DFTile>>();
        public List<Edge> edges = new List<Edge>();


        public enum UI_STATE
        {
            MAIN_ALGO,
            CA,
            ROOM_GEN,
            EXTRA_ROOM_GEN,
            CORRIDOR_MAKING,
            GENERATE
        }
        [HideInInspector]
        public UI_STATE state = UI_STATE.MAIN_ALGO;


        [HideInInspector]
        public bool allowedBack = false;
        [HideInInspector]
        public bool allowedForward = false;
        [HideInInspector]
        public int currStateIndex = 0;


        public enum PathFindingType
        {
            A_STAR,
            DJISTRA,
            BFS,
            DFS
        }
        private PathFindingType pathFindingType;
        public PathFindingType PathfindingType
        {
            get { return pathFindingType; }
            set { pathFindingType = value; }
        }



        public void InspectorAwake()
        {
            pcgManager = transform.GetComponent<PCGManager>();
            roomList.Clear();
            roomsListBPSAlgo.Clear();
            pcgManager.UndoInteraction = this;
        }


        /// <summary>
        /// from the remaining rooms set up the weights
        /// </summary>
        /// <param name="gridArr"></param>
        public void SetUpWeights(DFTile[,] gridArr)
        {
            foreach (var room in roomList)
            {
                for (int i = 0; i < room.tileCoords.Count; i++)
                {
                    gridArr[room.tileCoords[i].x, room.tileCoords[i].y].tileWeight = 1;
                    gridArr[room.tileCoords[i].x, room.tileCoords[i].y].tileType = DFTile.TileType.FLOORROOM;
                }
            }
        }


        #region Random Room Gen

        public void RandomRoomGen(DFTile[,] gridArr)
        {
            int tries = numOfRoom * 4;


            while (tries > 0)
            {
                tries--;


                if (roomList.Count >= numOfRoom) { break; }


                Vector2Int ranStartPoint = new Vector2Int(Random.Range(0, gridArr.GetLength(0)), Random.Range(0, gridArr.GetLength(1)));

                RoomsClass currRoom = new RoomsClass();

                int ranWidth = Random.Range(minWidth, maxWidth);
                if (Random.value > 0.5f)
                    ranWidth *= -1;


                int ranHeight = Random.Range(minHeight, MaxHeight);
                if (Random.value > 0.5f)
                    ranHeight *= -1;



                if (ranWidth < 0)
                {
                    if (ranStartPoint.x + ranWidth < 0)
                        continue;


                    currRoom.maxX = ranStartPoint.x;
                    currRoom.minX = ranStartPoint.x + ranWidth;
                }
                else
                {

                    if (ranStartPoint.x + ranWidth > gridArr.GetLength(0) - 1)
                        continue;

                    currRoom.minX = ranStartPoint.x;
                    currRoom.maxX = ranStartPoint.x + ranWidth;


                }



                if (ranHeight < 0)
                {
                    if (ranStartPoint.y + ranHeight < 0)
                        continue;

                    currRoom.maxY = ranStartPoint.y;
                    currRoom.minY = ranStartPoint.y + ranHeight;

                }
                else
                {

                    if (ranStartPoint.y + ranHeight > gridArr.GetLength(1) - 1)
                        continue;

                    currRoom.minY = ranStartPoint.y;
                    currRoom.maxY = ranStartPoint.y + ranHeight;

                }

                bool toAdd = true;

                for (int i = 0; i < roomList.Count; i++)
                {
                    if (AABBCol(currRoom, roomList[i]))
                    {
                        toAdd = false;
                        break;
                    }
                }


                if (toAdd)
                {
                    WorkoutRegion(currRoom);

                    roomList.Add(currRoom);
                }
            }
        }


        /// <summary>
        /// returns true when something is hit ---- return false when nothing is hit
        /// </summary>
        /// <param name="newRoom"></param>
        /// <param name="oldRoom"></param>
        /// <returns></returns>
        public bool AABBCol(RoomsClass newRoom, RoomsClass oldRoom)
        {
            if (newRoom.minX >= oldRoom.minX && newRoom.minX <= oldRoom.maxX || newRoom.maxX >= oldRoom.minX && newRoom.maxX <= oldRoom.maxX)
            {
                if (newRoom.minY >= oldRoom.minY && newRoom.minY <= oldRoom.maxY || newRoom.maxY >= oldRoom.minY && newRoom.maxY <= oldRoom.maxY)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        public void WorkoutRegion(RoomsClass room)
        {
            room.tileCoords = new List<Vector2Int>();

            for (int y = room.minY; y < room.maxY + 1; y++)
            {
                for (int x = room.minX; x < room.maxX + 1; x++)
                {
                    room.tileCoords.Add(new Vector2Int(x, y));
                }
            }
        }

        #region BPS

        public void BPSRoomGen(DFTile[,] gridArr, bool solve = true)
        {
            BoundsInt map = new BoundsInt();

            map.min = new Vector3Int(0, 0, 0);
            map.max = new Vector3Int(gridArr.GetLength(0) - 1, 0, gridArr.GetLength(1) - 1);

            roomsListBPSAlgo = BPSAlgo2d(map);

            if (solve)
            {
                while (roomsListBPSAlgo.Count > numOfRoom)
                {
                    int ranIDX = Random.Range(0, roomsListBPSAlgo.Count - 1);

                    roomsListBPSAlgo.RemoveAt(ranIDX);
                }

                BoundsToWeights();
            }
            else
            {
                BoundsToWeights();
            }
        }

        public void BoundsToWeights()
        {
            roomList.Clear();

            foreach (var room in roomsListBPSAlgo)
            {
                roomList.Add(new RoomsClass() { maxX = room.xMax, maxY = room.zMax, minX = room.xMin, minY = room.zMin });
                WorkoutRegion(roomList[roomList.Count - 1]);
            }
        }

        public List<BoundsInt> BPSAlgo2d(BoundsInt toSplit)
        {
            Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();

            roomsQueue.Enqueue(toSplit);

            while (roomsQueue.Count > 0)
            {
                var room = roomsQueue.Dequeue();

                if (room.size.z >= minHeight && room.size.x >= minWidth)
                {
                    if (Random.value < 0.5f)
                    {
                        if (room.size.z >= minHeight * 2 + 1)
                        {
                            SplitHori(minHeight, room, roomsQueue);
                        }
                        else if (room.size.x >= minWidth * 2 + 1)
                        {
                            SplitVert(minWidth, room, roomsQueue);
                        }
                        else
                        {
                            roomsListBPSAlgo.Add(room);
                        }
                    }
                    else
                    {
                        if (room.size.x >= minWidth * 2 + 1)
                        {
                            SplitVert(minWidth, room, roomsQueue);
                        }
                        else if (room.size.z >= minHeight * 2 + 1)
                        {
                            SplitHori(minHeight, room, roomsQueue);
                        }
                        else
                        {
                            roomsListBPSAlgo.Add(room);
                        }
                    }
                }
            }
            return roomsListBPSAlgo;
        }

        public void SplitVert(int minWidth, BoundsInt room, Queue<BoundsInt> roomQue)
        {
            int minX = room.min.x;
            int maxX = room.max.x;

            int adjustedMinX = minX + minWidth;
            int adjustedMaxX = maxX - minWidth;

            var ranPosition = Random.Range(adjustedMinX, adjustedMaxX);

            BoundsInt roomLeft = new BoundsInt();

            roomLeft.min = new Vector3Int(room.min.x, 0, room.min.z);
            roomLeft.max = new Vector3Int(ranPosition, 0, room.max.z);


            BoundsInt roomRight = new BoundsInt();

            roomRight.min = new Vector3Int(ranPosition, 0, room.min.z);
            roomRight.max = new Vector3Int(room.max.x, 0, room.max.z);

            roomQue.Enqueue(roomRight);
            roomQue.Enqueue(roomLeft);

        }

        public void SplitHori(int minHeight, BoundsInt room, Queue<BoundsInt> roomQue)
        {
            int minY = room.min.z;
            int maxY = room.max.z;

            int adjustedMinY = minY + minHeight;
            int adjustedMaxY = maxY - minHeight;

            var ranPosition = Random.Range(adjustedMinY, adjustedMaxY);

            BoundsInt roomTop = new BoundsInt();

            roomTop.min = new Vector3Int(room.min.x, 0, ranPosition);
            roomTop.max = new Vector3Int(room.max.x, 0, room.max.z);

            BoundsInt roomBot = new BoundsInt();

            roomBot.min = new Vector3Int(room.min.x, 0, room.min.z);
            roomBot.max = new Vector3Int(room.max.x, 0, ranPosition);

            roomQue.Enqueue(roomBot);
            roomQue.Enqueue(roomTop);

        }

        #endregion

        public void RanRoom(bool usingBPS, bool usingCA)
        {
            pcgManager.Restart();

            if (!usingBPS)
            {
                roomsListBPSAlgo.Clear();
                foreach (var room in roomList)
                {
                    roomsListBPSAlgo.Add(new BoundsInt() { xMin = room.minX, xMax = room.maxX, zMin = room.minY, zMax = room.maxY });
                }
            }

            foreach (var room in roomsListBPSAlgo)
            {
                var gridArrRoom = new DFTile[0, 0];

                if (usingCA)
                {
                    gridArrRoom = DFAlgoBank.CompartimentalisedCA(room);
                }
                else
                {
                    gridArrRoom = DFAlgoBank.CompartimentalisedRandomWalk(room);
                }

                for (int y = 0; y < gridArrRoom.GetLength(1); y++)
                {
                    for (int x = 0; x < gridArrRoom.GetLength(0); x++)
                    {
                        if (gridArrRoom[x, y].tileWeight == 1)
                        {
                            pcgManager.gridArr[x + room.xMin, y + room.zMin].tileWeight = 1;
                        }
                        else
                        {
                            pcgManager.gridArr[x + room.xMin, y + room.zMin].tileWeight = 0;
                        }
                    }
                }
            }

            PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColAnchor(PcgManager.gridArr, true);

        }

        public void DeleteLastSavedRoom()
        {
            if (state == UI_STATE.EXTRA_ROOM_GEN)
                rooms.RemoveAt(rooms.Count - 1);
        }
    }


    public class RoomsClass
    {
        public int minX;
        public int maxX;
        public int minY;
        public int maxY;

        public List<Vector2Int> tileCoords = new List<Vector2Int>();

        public List<RoomsClass> childRooms = new List<RoomsClass>();

    }
}