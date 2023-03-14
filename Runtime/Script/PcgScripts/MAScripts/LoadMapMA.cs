using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;


namespace DungeonForge.AlgoScript
{

    using DungeonForge.Utils;

    public class LoadMapMA : MonoBehaviour
    {

        public GameObject debris;


        [HideInInspector]
        public PCGManager PcgManager;

        [HideInInspector]
        public List<List<DFTile>> rooms = new List<List<DFTile>>();

        public void InspectorAwake()
        {
            PcgManager = this.transform.GetComponent<PCGManager>();
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


    }
}