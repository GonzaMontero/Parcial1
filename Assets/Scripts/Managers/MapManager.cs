using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace AI.Managers
{
    public class MapManager : MonoBehaviour
    {
        [Header("Entity Useful Data")]
        public List<MineItem> AllMinesOnMap;
        public List<MineItem> AllWorkedMines;
        public static MapManager Instance;

        [Header("Points of Interest & Map Settings")]
        public Vector3Int MinerSpawnPosition;
        public int MinesCount;
        public Vector2Int MapSize;

        [Header("Useful Prefabs")]
        public MineItem MineItemPrefab;

        [Header("Parent Transforms")]
        public Transform MinesParents;
        public Transform WallParent;

        [Header("Easy Access Data")]
        private List<Vector2Int> buildingPos = new();
       
        public GridSlot[] Map;

        public Vector2Int DepositPos;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this.gameObject);

            InitBuildings();
            InitMap(new Vector2Int(MapSize.x, MapSize.y));

            for (int i = 0; i < MinesCount; i++)
            {
                SpawnMine(MineItemPrefab);
            }
        }

        private void InitBuildings()
        {
            buildingPos.Add(DepositPos);

            for (int i = 0; i < WallParent.childCount; i++)
            {
                Transform wall = WallParent.GetChild(i);
                Vector2Int pos = new Vector2Int((int)wall.transform.position.x, (int)wall.transform.position.y);
                buildingPos.Add(pos);
            }
        }

        private void InitMap(Vector2Int mapSize)
        {
            Map = new GridSlot[mapSize.x * mapSize.y];
            GridUtils.GridSize = new Vector2Int(mapSize.x, mapSize.y);
            int id = 0;

            for (int i = 0; i < mapSize.x; i++)
            {
                for (int j = 0; j < mapSize.y; j++)
                {
                    Map[id] = new GridSlot(id, new Vector2Int(j, i));
                    Map[id].SetWeight(Random.Range(1, 6));

                    for (int k = 0; k < buildingPos.Count; k++)
                    {
                        if (Map[id].position == buildingPos[k])
                        {
                            Map[id].currentState = SlotStates.Obstacle;
                        }
                    }

                    id++;
                }
            }
        }

        public MineItem GetItemOnPosition(Vector2Int position)
        {
            return AllMinesOnMap.Single(m => m.MinePosition == position);
        }

        private void SpawnMine(MineItem mineGO)
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
            Map[GridUtils.PositionToIndex(new Vector2Int((int)posVec3.x, (int)posVec3.y))].currentState = SlotStates.Obstacle;

            var go = Instantiate(mineGO, posVec3, Quaternion.identity, MinesParents);
            go.MinePosition = new Vector2Int((int)posVec3.x, (int)posVec3.y);
            AllMinesOnMap.Add(go);
            buildingPos.Add(pos);
        }
    }
}

