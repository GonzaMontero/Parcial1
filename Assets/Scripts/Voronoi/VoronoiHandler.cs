using System.Collections.Generic;
using UnityEngine;
using AI.Managers;

namespace AI.Voronoi
{
    public class VoronoiHandler
    {
        #region Cached Values
        private List<int /*closest mine id*/> closestMineToNode;

        private List<Dictionary< int /* mine id*/, int /*cost to mine*/>> possiblePathCosts;

        private List<Mines> allMinesOnMap => MapManager.Instance.AllMines;
        private List<Mines> allWorkedMinesOnMap => MapManager.Instance.WorkedMines;

        private PathingAlternatives pathingAlternatives;
        #endregion

        #region Villager Voronois
        public void InitVillagerVoronoi()
        {
            closestMineToNode = new List<int>();
            possiblePathCosts = new List<Dictionary< int , int>>();

            pathingAlternatives = new PathingAlternatives();

            int id = 0;
            int tempCheapestDomain = int.MaxValue;
            int tempCheapestCost = int.MaxValue;

            int mineIndex = 0;

            for (short x = 0; x < GridUtils.GridSize.x; x++)
            {
                for(short y = 0; y < GridUtils.GridSize.y; y++)
                {
                    tempCheapestDomain = int.MaxValue;
                    tempCheapestCost = int.MaxValue;

                    for(short m = 0; m < allMinesOnMap.Count; m++)
                    {
                        mineIndex = GridUtils.PositionToIndex(allMinesOnMap[m].Position);

                        pathingAlternatives.GetPath(MapManager.Instance.Map, MapManager.Instance.Map[id],
                        MapManager.Instance.Map[mineIndex], out int totalCost);

                        if(totalCost < tempCheapestCost)
                        {
                            tempCheapestCost = totalCost;
                            tempCheapestDomain = mineIndex;
                        }

                        var dict = new Dictionary<int, int>();
                        dict.Add(mineIndex, totalCost);

                        possiblePathCosts.Add(dict);
                    }

                    closestMineToNode.Add(tempCheapestDomain);

                    id++;
                }
            }
        }

        public void UpdateVillagerVoronoi()
        {
            int id = 0;

            int tempCheapestDomain = int.MaxValue;
            int tempCheapestCost = int.MaxValue;

            int mineIndex = 0;

            for (short x = 0; x < GridUtils.GridSize.x; x++)
            {
                for (short y = 0; y < GridUtils.GridSize.y; y++)
                {
                    tempCheapestDomain = int.MaxValue;
                    tempCheapestCost = int.MaxValue;

                    for (short m = 0; m < allMinesOnMap.Count; m++)
                    {
                        mineIndex = GridUtils.PositionToIndex(allMinesOnMap[m].Position);

                        possiblePathCosts[id].TryGetValue(mineIndex, out int totalCost);

                        if (totalCost < tempCheapestCost)
                        {
                            tempCheapestCost = totalCost;
                            tempCheapestDomain = mineIndex;
                        }
                    }

                    closestMineToNode[id] = tempCheapestDomain;

                    id++;
                }
            }
        }
        #endregion
    }
}
