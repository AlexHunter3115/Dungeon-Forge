using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DungeonForge.Utils
{
    using DungeonForge.AlgoScript;
    using System.Threading.Tasks;

    public static class DFGeneralUtil
    {
        public static int[,] childPosArry4Side = { { 0, -1 }, { -1, 0 }, { 1, 0 }, { 0, 1 } };
        public static int[,] childPosArry8Side = { { 0, -1 }, { 1, -1 }, { -1, -1 }, { -1, 0 }, { 1, 0 }, { 0, 1 }, { 1, 1 }, { -1, 1 } };

        public static float EuclideanDistance2D(Vector2 pointA, Vector2 pointB) => Vector2.Distance(pointA, pointB);

        public static float ManhattanDistance2D(Vector2 pointA, Vector2 pointB) => Mathf.Abs(pointA.x - pointB.x) + Mathf.Abs(pointA.y - pointB.y);

        public static float ManhattanDistance3D(Vector3 pointA, Vector3 pointB) => Mathf.Abs(pointA.x - pointB.x) + Mathf.Abs(pointA.y - pointB.y) + Mathf.Abs(pointA.z - pointB.z);

        public static float EuclideanDistance3D(Vector3 pointA, Vector3 pointB) => Vector3.Distance(pointA, pointB);

        /// <summary>
        /// Given a 2D position, find the corresponding Tile
        /// </summary>
        /// <param name="point"></param>
        /// <param name="gridArr"></param>
        /// <returns></returns>
        public static DFTile WorldPosToTile(Vector2 point, DFTile[,] gridArr)
        {
            float pointX = point.x;
            float pointY = point.y;

            float tileSize = 1;
            int tileX = Mathf.FloorToInt(pointX / tileSize);
            int tileY = Mathf.FloorToInt(pointY / tileSize);

            if (tileX < 0 || tileY < 0 || tileX >= gridArr.GetLength(0) || tileY >= gridArr.GetLength(1))
            {
                return null;
            }

            return gridArr[tileX, tileY];
        }

        /// <summary>
        /// Return a random item from the list given
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int ReturnRandomFromList<T>(List<T> list)
        {
            return list.Count == 1 ? 0 : Random.Range(0, list.Count);
        }

        /// <summary>
        /// given a 2d Grid sets the colour of each index to the one of the respective type
        /// </summary>
        /// <param name="gridArr"></param>
        public static void SetUpColorBasedOnType(DFTile[,] gridArr)
        {
            for (int y = 0; y < gridArr.GetLength(1); y++)
            {
                for (int x = 0; x < gridArr.GetLength(0); x++)
                {
                    switch (gridArr[x, y].tileType)
                    {
                        case DFTile.TileType.VOID:
                            gridArr[x, y].color = Color.white;
                            break;
                        case DFTile.TileType.FLOORROOM:

                            gridArr[x, y].color = Color.blue;
                            break;
                        case DFTile.TileType.WALL:

                            gridArr[x, y].color = Color.black;
                            break;
                        case DFTile.TileType.WALLCORRIDOR:

                            gridArr[x, y].color = Color.green;
                            break;
                        case DFTile.TileType.ROOF:
                            gridArr[x, y].color = Color.gray;
                            break;
                        case DFTile.TileType.FLOORCORRIDOR:

                            gridArr[x, y].color = Color.yellow;
                            break;
                        case DFTile.TileType.AVOID:

                            gridArr[x, y].color = Color.red;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// given a set of points finds the mid points of those points
        /// </summary>
        /// <param name="listOfPoints"></param>
        /// <returns></returns>
        public static Vector2 FindMiddlePoint(List<Vector2> listOfPoints)
        {
            var midPoint = new Vector2(0, 0);

            foreach (var point in listOfPoints)
            {
                midPoint.x += point.x;
                midPoint.y += point.y;
            }

            midPoint = new Vector2(midPoint.x / listOfPoints.Count, midPoint.y / listOfPoints.Count);

            return midPoint;
        }

        /// <summary>
        /// given a set of Tiles finds the mid points of those points
        /// </summary>
        /// <param name="listOfPoints"></param>
        /// <returns></returns>
        public static Vector2 FindMiddlePoint(List<DFTile> listOfPoints)
        {
            var midPoint = new Vector2(0, 0);

            foreach (var point in listOfPoints)
            {
                midPoint.x += point.position.x;
                midPoint.y += point.position.y;
            }

            midPoint = new Vector2(midPoint.x / listOfPoints.Count, midPoint.y / listOfPoints.Count);

            return midPoint;
        }

        /// <summary>
        /// Given a grid it resets all the elements
        /// </summary>
        /// <param name="gridArr"></param>
        public static void RestartGrid(DFTile[,] gridArr)
        {
            Parallel.For(0, gridArr.GetLength(1), y =>
            {
                for (int x = 0; x < gridArr.GetLength(0); x++)
                {
                    gridArr[x, y] = new DFTile();
                    gridArr[x, y].position = new Vector2Int(x, y);
                    gridArr[x, y].color = Color.white;
                }
            });
        }

        /// <summary>
        /// resets the colour,type and weight of a tile
        /// </summary>
        /// <param name="tile"></param>
        public static void ResetTile(DFTile tile) 
        {
            tile.tileType = DFTile.TileType.VOID;
            tile.color = Color.white;
            tile.tileWeight = 0;
        }

        #region Texture Return Colour Region
        /// <summary>
        /// Sets the colour of the pixel that is saved in the class instance
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="gridArr"></param>
        /// <returns></returns>
        public static Texture2D SetUpTextSelfCol(DFTile[,] gridArr)
        {
            Texture2D texture = new Texture2D(gridArr.GetLength(0), gridArr.GetLength(1));

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    texture.SetPixel(x, y, gridArr[x, y].color);
                }
            }
            texture.filterMode = FilterMode.Point;
            texture.Apply();

            return texture;
        }

        /// <summary>
        /// either black or white, if = 0 white if = 1 black
        /// </summary>
        /// <param name="gridArr"></param>
        /// <param name="black"></param>
        /// <returns></returns>
        public static Texture2D SetUpTextBiColAnchor(DFTile[,] gridArr, bool black = false)
        {
            Texture2D texture = new Texture2D(gridArr.GetLength(0), gridArr.GetLength(1));

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    Color color = new Color();

                    if (black)
                    {
                        color = ((gridArr[x, y].tileWeight) == 0 ? Color.white : Color.black);
                    }
                    else
                    {
                        color = ((gridArr[x, y].tileWeight) == 0 ? Color.white : Color.grey);
                    }

                    texture.SetPixel(x, y, color);
                }
            }
            texture.filterMode = FilterMode.Point;
            texture.Apply();

            return texture;
        }

        /// <summary>
        /// Set the shade of black and white with a given max and min weight then weight
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="gridArr"></param>
        /// <returns></returns>
        public static Texture2D SetUpTextBiColShade(DFTile[,] gridArr, float minWeight, float maxWeight, bool inverse = false)
        {
            Texture2D texture = new Texture2D(gridArr.GetLength(0), gridArr.GetLength(1));

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    float num = Mathf.InverseLerp(minWeight, maxWeight, gridArr[x, y].tileWeight);

                    if (inverse)
                        gridArr[x, y].color = new Color(1 - num, 1 - num, 1 - num, 1f);
                    else
                        gridArr[x, y].color = new Color(num, num, num, 1f);


                    texture.SetPixel(x, y, gridArr[x, y].color);
                }
            }
            texture.filterMode = FilterMode.Point;
            texture.Apply();

            return texture;
        }

        #endregion

    }


    [Serializable]
    public class SerializableTile
    {
        public SerialiableVector2Int position = new SerialiableVector2Int();
        public float tileWeight;
        public float cost = 0;

        public int tileType;

        public SerializableTile(Vector2Int position, float tileWeight, float cost, int tileType)
        {
            this.position = new SerialiableVector2Int(position.x, position.y);
            this.tileWeight = tileWeight;
            this.cost = cost;
            this.tileType = tileType;
        }
    }

    [Serializable]
    public struct SerialiableVector2Int
    {
        public int x;
        public int y;

        public SerialiableVector2Int(int rX, int rY)
        {
            x = rX;
            y = rY;
        }
    }

}