using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace DungeonForge.AlgoScript
{

    public class NewWFCAlog : MonoBehaviour
    {

        [SerializeField]
        public WFCTile[,] arrayOfWFCTiles = new WFCTile[0, 0];
        private int xSize = 20;
        private int ySize = 20;

        public bool outskirtsCheck = false;
        public int indexOutskirts = 0;

        private WFCRuleDecipher rulesInst;

        private PCGManager pcgManager;
        public PCGManager PcgManager
        {
            get { return pcgManager; }
        }

        public void InspectorAwake()
        {
            pcgManager = this.transform.GetComponent<PCGManager>();
            rulesInst = this.transform.GetComponent<WFCRuleDecipher>();
        }

        public void RunWFCAlgo()
        {
            DestroyKids();

            rulesInst = this.transform.GetComponent<WFCRuleDecipher>();

            ySize = pcgManager.height;
            xSize = pcgManager.width;

            arrayOfWFCTiles = new WFCTile[xSize, ySize];

            for (int y = 0; y < arrayOfWFCTiles.GetLength(1); y++)
            {
                for (int x = 0; x < arrayOfWFCTiles.GetLength(0); x++)
                {
                    arrayOfWFCTiles[x, y] = new WFCTile(rulesInst.tileSet.Count());
                    arrayOfWFCTiles[x, y].coord = new Vector2Int(x, y);
                }
            }

            if (outskirtsCheck)
                SetOutSkirts();

            var ranStart = new Vector2Int(Random.Range(0, xSize), Random.Range(0, ySize));
            arrayOfWFCTiles[ranStart.x, ranStart.y].solved = true;// choosen idx should be the indx of the item choosen

            int ranNum = Random.Range(0, arrayOfWFCTiles[ranStart.x, ranStart.y].AllowedObjectsIDXs.Count);

            arrayOfWFCTiles[ranStart.x, ranStart.y].choosenIDX = arrayOfWFCTiles[ranStart.x, ranStart.y].AllowedObjectsIDXs[ranNum];

            var spawnedAsset = Instantiate(rulesInst.tileSet[arrayOfWFCTiles[ranStart.x, ranStart.y].choosenIDX], this.transform);
            spawnedAsset.transform.position = new Vector3(ranStart.x, 0, ranStart.y);
            spawnedAsset.transform.name = $"{ranStart.x} {ranStart.y} {arrayOfWFCTiles[ranStart.x, ranStart.y].choosenIDX}";
            arrayOfWFCTiles[ranStart.x, ranStart.y].AllowedObjectsIDXs.Clear();
            ResetNeighbours(arrayOfWFCTiles, ranStart, arrayOfWFCTiles[ranStart.x, ranStart.y].choosenIDX);

            bool finishedAlgo = false;

            while (!finishedAlgo)
            {
                finishedAlgo = true;
                int lowestSuperposition = 999;
                Vector2Int coordOfLowestSuperposition = new Vector2Int(0, 0);

                for (int y = 0; y < arrayOfWFCTiles.Length; y++)
                {
                    for (int x = 0; x < arrayOfWFCTiles.GetLength(0); x++)
                    {
                        if (!arrayOfWFCTiles[x, y].solved)
                        {
                            finishedAlgo = false;
                            if (arrayOfWFCTiles[x, y].AllowedObjectsIDXs.Count < lowestSuperposition)
                            {
                                lowestSuperposition = arrayOfWFCTiles[x, y].AllowedObjectsIDXs.Count;
                                coordOfLowestSuperposition = new Vector2Int(x, y);
                            }
                        }
                    }
                }

                if (!finishedAlgo)
                {
                    // get the choosen idx
                    if (arrayOfWFCTiles[coordOfLowestSuperposition.x, coordOfLowestSuperposition.y].AllowedObjectsIDXs.Count == 1)
                    {
                        arrayOfWFCTiles[coordOfLowestSuperposition.x, coordOfLowestSuperposition.y].choosenIDX = arrayOfWFCTiles[coordOfLowestSuperposition.x, coordOfLowestSuperposition.y].AllowedObjectsIDXs[0];
                    }
                    else
                    {
                        arrayOfWFCTiles[coordOfLowestSuperposition.x, coordOfLowestSuperposition.y].choosenIDX = arrayOfWFCTiles[coordOfLowestSuperposition.x, coordOfLowestSuperposition.y].AllowedObjectsIDXs[Random.Range(0, arrayOfWFCTiles[coordOfLowestSuperposition.x, coordOfLowestSuperposition.y].AllowedObjectsIDXs.Count)];
                    }

                    spawnedAsset = Instantiate(rulesInst.tileSet[arrayOfWFCTiles[coordOfLowestSuperposition.x, coordOfLowestSuperposition.y].choosenIDX], this.transform);
                    spawnedAsset.transform.position = new Vector3(coordOfLowestSuperposition.x, 0, coordOfLowestSuperposition.y);
                    spawnedAsset.transform.name = $"{coordOfLowestSuperposition.x} {coordOfLowestSuperposition.y} {arrayOfWFCTiles[coordOfLowestSuperposition.x, coordOfLowestSuperposition.y].choosenIDX}";
                    arrayOfWFCTiles[coordOfLowestSuperposition.x, coordOfLowestSuperposition.y].AllowedObjectsIDXs.Clear();
                    ResetNeighbours(arrayOfWFCTiles, coordOfLowestSuperposition, arrayOfWFCTiles[coordOfLowestSuperposition.x, coordOfLowestSuperposition.y].choosenIDX);

                    arrayOfWFCTiles[coordOfLowestSuperposition.x, coordOfLowestSuperposition.y].solved = true;
                }
            }
        }

        private void SetOutSkirts()
        {
            WFCTileRule ruleRef = null;

            foreach (var rule in rulesInst.ruleSet)
            {
                if (rule.assetIdx == indexOutskirts)
                {
                    ruleRef = rule;
                    break;
                }
            }

            for (int x = 0; x < xSize; x++)
            {
                arrayOfWFCTiles[x, 0].NeighbourAllowed(ruleRef.allowedObjAbove);
            }

            for (int x = 0; x < xSize; x++)
            {
                arrayOfWFCTiles[x, ySize - 1].NeighbourAllowed(ruleRef.allowedObjBelow);
            }

            for (int y = 0; y < ySize; y++)
            {
                arrayOfWFCTiles[0, y].NeighbourAllowed(ruleRef.allowedObjRight);
            }

            for (int y = 0; y < ySize; y++)
            {
                arrayOfWFCTiles[xSize - 1, y].NeighbourAllowed(ruleRef.allowedObjLeft);
            }
        }

        public void ResetNeighbours(WFCTile[,] arrayOfWFCTiles, Vector2Int mainTileCoord, int mainTileIDX)
        {

            WFCTileRule ruleRef = null;

            foreach (var rule in rulesInst.ruleSet)
            {
                if (rule.assetIdx == mainTileIDX)
                {
                    ruleRef = rule;
                    break;
                }
            }


            if (mainTileCoord.x + 1 < xSize)   // the one on the right  exists
            {
                if (!arrayOfWFCTiles[mainTileCoord.x + 1, mainTileCoord.y].solved)
                    arrayOfWFCTiles[mainTileCoord.x + 1, mainTileCoord.y].NeighbourAllowed(ruleRef.allowedObjRight);
            }

            if (mainTileCoord.x - 1 >= 0)   // the one on the left  exists
            {
                if (!arrayOfWFCTiles[mainTileCoord.x - 1, mainTileCoord.y].solved)
                    arrayOfWFCTiles[mainTileCoord.x - 1, mainTileCoord.y].NeighbourAllowed(ruleRef.allowedObjLeft);
            }

            if (mainTileCoord.y + 1 < ySize)   // the one on the right  exists
            {
                if (!arrayOfWFCTiles[mainTileCoord.x, mainTileCoord.y + 1].solved)
                    arrayOfWFCTiles[mainTileCoord.x, mainTileCoord.y + 1].NeighbourAllowed(ruleRef.allowedObjAbove);
            }

            if (mainTileCoord.y - 1 >= 0)   // the one on the right  exists
            {
                if (!arrayOfWFCTiles[mainTileCoord.x, mainTileCoord.y - 1].solved)
                    arrayOfWFCTiles[mainTileCoord.x, mainTileCoord.y - 1].NeighbourAllowed(ruleRef.allowedObjBelow);
            }

        }

        public void DestroyKids()
        {
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }

        public class WFCTile
        {
            public Vector2Int coord;
            public bool solved = false;
            public int choosenIDX = 9999;
            public List<int> AllowedObjectsIDXs = new List<int>();


            public WFCTile(int num)
            {
                for (int i = 0; i < num; i++)
                {
                    AllowedObjectsIDXs.Add(i);
                }
            }

            public void NeighbourAllowed(List<int> neighbourAllowedIDXs)
            {
                for (int i = AllowedObjectsIDXs.Count; i-- > 0;)
                {
                    bool isThere = false;

                    foreach (var allowedIDX in neighbourAllowedIDXs)
                    {
                        if (AllowedObjectsIDXs[i] == allowedIDX)
                        {
                            isThere = true;
                            break;
                        }
                    }


                    if (!isThere)
                    {
                        AllowedObjectsIDXs.RemoveAt(i);
                    }

                }


                if (AllowedObjectsIDXs.Count == 0)
                {
                    AllowedObjectsIDXs.Add(0);
                }
            }
        }
    }
}