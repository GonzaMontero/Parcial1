using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace AI.Managers
{
    public class MapManager : MonoBehaviour
    {
        public List<MineItem> AllMinesOnMap;
        public List<MineItem> AllWorkedMines;

        [Header("Other Handlers")]
        public VoronoiHandler VoronoiHandler;

        [Header("Points of Interest & Map Settings")]
        public Vector3Int MinerSpawnPosition;
        public int MinesCount;
        public Vector2Int MapSize;

        [Header("Parent Transforms")]
        public Transform MinesParents;
        public Transform WallParent;

        [Header("Easy Access Data")]
        private List<Vector2Int> buildingPos = new();
        public List<Vector2Int> BuildingPos => buildingPos;

        private List<Vector2Int> minesPos = new();
        public List<Vector2Int> MinesPos => minesPos;

        private List<Vector2Int> minePositions;
        public List<Vector2Int> MinePositions => minePositions;

        private List<GameObject> minesList = new();
        public List<GameObject> MinesList => minesList;

        private GridSlot[] map;
        public GridSlot[] Map => map;

        private Vector2Int depositPos;
        public Vector2Int DepositPos => depositPos;

        public void Init(Vector2Int mapSize, GameObject mineGO)
        {
            InitBuildings();
            InitMap(mapSize);

            VoronoiHandler.Config();

            for (int i = 0; i < MinesCount; i++)
            {
                SpawnMine(mineGO);
            }

            UpdateSectors();
        }

        private void InitBuildings()
        {
            buildingPos.Add(depositPos);

            for (int i = 0; i < minesList.Count; i++)
            {
                Vector2Int pos = new Vector2Int((int)minesList[i].transform.position.x, (int)minesList[i].transform.position.y);
                buildingPos.Add(pos);
            }

            for (int i = 0; i < WallParent.childCount; i++)
            {
                Transform wall = WallParent.GetChild(i);
                Vector2Int pos = new Vector2Int((int)wall.transform.position.x, (int)wall.transform.position.y);
                buildingPos.Add(pos);
            }
        }

        private void InitMap(Vector2Int mapSize)
        {
            map = new GridSlot[mapSize.x * mapSize.y];
            GridUtils.GridSize = new Vector2Int(mapSize.x, mapSize.y);
            int id = 0;

            for (int i = 0; i < mapSize.x; i++)
            {
                for (int j = 0; j < mapSize.y; j++)
                {
                    map[id] = new GridSlot(id, new Vector2Int(j, i));
                    map[id].SetWeight(Random.Range(1, 6));

                    for (int k = 0; k < buildingPos.Count; k++)
                    {
                        if (map[id].position == buildingPos[k])
                        {
                            map[id].currentState = SlotStates.Obstacle;
                        }
                    }

                    id++;
                }
            }
        }

        private void SpawnMine(GameObject mineGO)
        {
            int x = Random.Range(1, GridUtils.GridSize.x - 1);
            int y = Random.Range(1, GridUtils.GridSize.y - 1);
            Vector2Int pos = new Vector2Int(x, y);

            for (int i = 0; i < buildingPos.Count; i++)
            {
                if (pos == buildingPos[i])
                {
                    SpawnMine(mineGO);
                    return;
                }
            }

            Vector3 posVec3 = new Vector3(pos.x, pos.y, 0);
            var go = Instantiate(mineGO, posVec3, Quaternion.identity, MinesParents);

            minesList.Add(go);
            buildingPos.Add(pos);
            minesPos.Add(pos);
        }

        private GridSlot[] GetMap()
        {
            return map;
        }

        public void UpdateSectors()
        {
            List<(Vector2, float)> minesPos = new List<(Vector2, float)>();
            foreach (var mine in minesList)
            {
                float weight = Random.Range(1, 6);
                minesPos.Add((mine.transform.position, weight));
            }
            VoronoiHandler.UpdateSectors(minesPos);
        } 
    }
}

