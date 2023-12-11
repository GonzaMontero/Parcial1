using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using UnityEngine;
using AI.Entities;
using AI.Voronoi;
using System.Collections.Generic;

namespace AI.Managers
{
    [System.Serializable]
    public class PopulationType
    {
        public string PopulationName;
        public OtherVoronoiHandler PopulationVoronoiHandler;
        public GameObject PopulationPrefab;
        public ConcurrentBag<AIEntity> PopulationBag;
        public int PopulationCount;
    }

    public class MinerManager : MonoBehaviour
    {
        public static MinerManager Instance;

        public List<PopulationType> PopulationTypes;
        public ParallelOptions ParallelOptions;

        public static Action<bool> OnReturnToBaseCalled;

        public Vector2Int DepositPosition;

        public bool ReturnToBase = false;

        public float DeltaTime = 0;

        public void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this.gameObject);
        }

        public void Start()
        {
            ParallelOptions = new ParallelOptions();
            ParallelOptions.MaxDegreeOfParallelism = 6;

            List<(Vector2, float)> mineList = new List<(Vector2, float)>();
            List<Vector2Int> minesPos = new List<Vector2Int>();

            foreach (MineItem m in MapManager.Instance.AllMinesOnMap)
            {
                Vector2 minePos = new Vector2(m.MinePosition.x, m.MinePosition.y);
                minesPos.Add(new Vector2Int((int)minePos.x, (int)minePos.y));
                mineList.Add((minePos, m.MineWeight));
            }
            PopulationTypes[0].PopulationVoronoiHandler.UpdateSectors(mineList);

            for (int i = 0; i < PopulationTypes.Count; i++)
            {
                PopulationTypes[i].PopulationBag = new ConcurrentBag<AIEntity>();
                PopulationTypes[i].PopulationVoronoiHandler.Config();
                for (short p = 0; p < PopulationTypes[i].PopulationCount; p++)
                {
                    var GO = Instantiate(PopulationTypes[i].PopulationPrefab, MapManager.Instance.MinerSpawnPosition,
                        Quaternion.identity, this.transform);
                    GO.gameObject.GetComponent<AIEntity>().InitMap(MapManager.Instance.BuildingsPos, minesPos);
                    GO.gameObject.GetComponent<AIEntity>().Init((Vector2Int)MapManager.Instance.MinerSpawnPosition);
                    PopulationTypes[i].PopulationBag.Add(GO.gameObject.GetComponent<AIEntity>());
                }
            }

            SetActionEvents();
        }

        public void Update()
        {
            DeltaTime = Time.deltaTime;

            for (short i = 0; i < PopulationTypes.Count; i++)
            {
                foreach (var p in PopulationTypes[i].PopulationBag)
                {
                    if (p.updatePos)
                    {
                        p.transform.position = p.GetData().Position;
                        p.updatePos = false;
                    }
                }
            }

            for (short i = 0; i < PopulationTypes.Count; i++)
            {
                Parallel.ForEach(PopulationTypes[i].PopulationBag, ParallelOptions, currentPopulation =>
                {
                    currentPopulation.UpdateMiner();
                });
            }
        }

        public void SetReturnToBase()
        {
            ReturnToBase = !ReturnToBase;
            OnReturnToBaseCalled(ReturnToBase);
        }

        private void SetActionEvents()
        {
            MineItem.OnMineDrained += (bool areMinesLeft, bool areWorkingMinesLeft) =>
            {
                List<(Vector2, float)> mineList = new List<(Vector2, float)>();

                if (areMinesLeft)
                {
                    foreach (MineItem m in MapManager.Instance.AllMinesOnMap)
                    {
                        Vector2 minePos = new Vector2(m.MinePosition.x, m.MinePosition.y);
                        mineList.Add((minePos, m.MineWeight));
                    }
                    PopulationTypes[0].PopulationVoronoiHandler.UpdateSectors(mineList);

                    Parallel.ForEach(PopulationTypes[0].PopulationBag, ParallelOptions, currentPopulation =>
                    {
                        currentPopulation.fsm.ForceCurrentState((int)MinerStates.Collect);
                    });
                }
                else
                {
                    Parallel.ForEach(PopulationTypes[0].PopulationBag, ParallelOptions, currentPopulation =>
                    {
                        currentPopulation.fsm.ForceCurrentState((int)MinerStates.Return);
                    });
                }

                mineList.Clear();

                if (areWorkingMinesLeft)
                {
                    foreach (MineItem m in MapManager.Instance.AllWorkedMines)
                    {
                        Vector2 minePos = new Vector2(m.MinePosition.x, m.MinePosition.y);
                        mineList.Add((minePos, m.MineWeight));
                    }
                    PopulationTypes[1].PopulationVoronoiHandler.UpdateSectors(mineList);

                    Parallel.ForEach(PopulationTypes[1].PopulationBag, ParallelOptions, currentPopulation =>
                    {
                        currentPopulation.fsm.ForceCurrentState((int)FoodStates.Supply);
                    });
                }
                else
                {
                    Parallel.ForEach(PopulationTypes[1].PopulationBag, ParallelOptions, currentPopulation =>
                    {
                        currentPopulation.fsm.ForceCurrentState((int)FoodStates.Return);
                    });
                }
            };

            MineItem.OnMineStartedWorking += () =>
            {
                List<(Vector2, float)> mineList = new List<(Vector2, float)>();

                foreach (MineItem m in MapManager.Instance.AllWorkedMines)
                {
                    Vector2 minePos = new Vector2(m.MinePosition.x, m.MinePosition.y);
                    mineList.Add((minePos, m.MineWeight));
                }
                PopulationTypes[1].PopulationVoronoiHandler.UpdateSectors(mineList);

                Parallel.ForEach(PopulationTypes[1].PopulationBag, ParallelOptions, currentPopulation =>
                {
                    currentPopulation.fsm.ForceCurrentState((int)FoodStates.Supply);
                });
            };

            MineItem.OnMineEnded += () =>
            {
                List<(Vector2, float)> mineList = new List<(Vector2, float)>();

                foreach (MineItem m in MapManager.Instance.AllWorkedMines)
                {
                    Vector2 minePos = new Vector2(m.MinePosition.x, m.MinePosition.y);
                    mineList.Add((minePos, m.MineWeight));
                }

                PopulationTypes[1].PopulationVoronoiHandler.UpdateSectors(mineList);

                Parallel.ForEach(PopulationTypes[1].PopulationBag, ParallelOptions, currentPopulation =>
                {
                    currentPopulation.fsm.ForceCurrentState((int)FoodStates.Return);
                });
            };
        }
    }
}

