using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Managers
{
    public class SimulationManager : MonoBehaviour
    {
        [Header("Main Simulation Config")]
        public GameObject DepositGOPrefab;
        public GameObject MinerGOPrefab;
        public GameObject MineGOPrefab;
        public GameObject TownHallGOPrefab;
        public Vector2Int mapSize;
        
        [Header("Other Managers")]
        public MinerManager MinerManager;
        public MapManager MapManager;

        private void Start()
        {
            Vector2Int depositPosVec2 = new Vector2Int((int)DepositGOPrefab.transform.position.x, (int)DepositGOPrefab.transform.position.y);
            Vector2Int restPosVec2 = new Vector2Int((int)TownHallGOPrefab.transform.position.x, (int)TownHallGOPrefab.transform.position.y);

            MinerManager.Init(depositPosVec2, restPosVec2);

            MapManager.Init(mapSize, MineGOPrefab);
        }

        private void Update()
        {
            MinerManager.UpdateMiners();
        }

        private void OnDrawGizmos()
        {
            if (MapManager.Map == null)
                return;

            foreach (GridSlot slot in MapManager.Map)
            {
                Vector3 worldPosition = new Vector3(slot.position.x, slot.position.y, 0.0f);

                Gizmos.color = slot.currentState == SlotStates.Obstacle ? Color.red : Color.green;
                Gizmos.DrawWireSphere(worldPosition, 0.2f);
            }
        }

        public void SpawnMiner()
        {
            var go = Instantiate(MinerGOPrefab, MapManager.MinerSpawnPosition, Quaternion.identity, transform);
            var miner = go.GetComponent<Miner>();
            miner.Init(MapManager.DepositPos, MinerManager.RestPosition, miner.transform.position, 
                GetDeltaTime, GetMine, OnEmptyMine, ref MinerManager.OnUpdateWeight, MapManager.BuildingPos, MapManager.MinesPos);

            MinerManager.Miners.Add(miner);
        }

        private float GetDeltaTime()
        {
            return MinerManager.DeltaTime;
        }

        private void OnEmptyMine(Vector2Int minePos)
        {
            Vector2Int pos;
            for (int i = 0; i < MapManager.MinesList.Count; i++)
            {
                pos = new Vector2Int((int)MapManager.MinesList[i].transform.position.x, 
                    (int)MapManager.MinesList[i].transform.position.y);
                if (minePos == pos)
                {
                    MapManager.BuildingPos.Remove(pos);
                    MapManager.MinesPos.Remove(pos);
                    Destroy(MapManager.MinesList[i]);
                    MapManager.MinesList.RemoveAt(i);
                    MapManager.UpdateSectors();

                    Parallel.ForEach(MinerManager.Miners, MinerManager.ParallelOptions, miner =>
                    {
                        if (miner.CurrentMine == minePos)
                        {
                            miner.UpdateMine(GetMine());
                        }
                    });
                    break;
                }
            }
        }

        private Vector2Int GetMine()
        {
            int index = Random.Range(0, MapManager.MinesList.Count);

            Vector2Int pos = new Vector2Int((int)MapManager.MinesList[index].transform.position.x, 
                (int)MapManager.MinesList[index].transform.position.y);

            return pos;
        }

        private Vector2Int GetMine(Vector2 minerPos)
        {
            return MapManager.VoronoiHandler.GetNearestMine(minerPos);
        }
    }
}

