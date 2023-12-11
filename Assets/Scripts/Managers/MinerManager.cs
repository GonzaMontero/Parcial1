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
        public VoronoiHandler PopulationVoronoiHandler;
        public GameObject PopulationPrefab;
        public ConcurrentBag<AIEntity> PopulationBag;
        public int PopulationCount;
    }

    public class MinerManager : MonoBehaviour
    {
        public static MinerManager Instance;

        public List<PopulationType> PopulationTypes;
        public ParallelOptions ParallelOptions;

        public static Action OnReturnToBaseCalled;

        public Vector2Int DepositPosition;

        public bool ReturnToBase = false;

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

            for(int i=0;i<PopulationTypes.Count;i++)
            {
                PopulationTypes[i].PopulationBag = new ConcurrentBag<AIEntity>();
                PopulationTypes[i].PopulationVoronoiHandler = new VoronoiHandler();
                PopulationTypes[i].PopulationVoronoiHandler.SetupVoronoi(MapManager.Instance.AllMinesOnMap);
                for(short p = 0; p < PopulationTypes[i].PopulationCount; p++)
                {
                    var GO = Instantiate(PopulationTypes[i].PopulationPrefab, MapManager.Instance.MinerSpawnPosition, 
                        Quaternion.identity, this.transform);
                    GO.gameObject.GetComponent<Miner>().Init((Vector2Int)MapManager.Instance.MinerSpawnPosition);
                    PopulationTypes[i].PopulationBag.Add(GO.gameObject.GetComponent<Miner>());
                }
            }

            MineItem.OnMineDrained += (bool areMinesLeft, bool areWorkingMinesLeft) =>
            {
                Debug.Log("AYYYY");
            };
        }

        public void Update()
        {
            for(short i = 0; i < PopulationTypes.Count; i++)
            {
                foreach(var p in PopulationTypes[i].PopulationBag)
                {
                    if (p.updatePos)
                    {
                        p.transform.position = p.GetData().Position;
                        p.updatePos = false;
                    }
                }
            }

            for(short i=0; i<PopulationTypes.Count; i++)
            {
                Parallel.ForEach(PopulationTypes[i].PopulationBag, ParallelOptions, currentPopulation =>
                {
                    currentPopulation.UpdateMiner();
                });
            }
        }

        public void SetReturnToBase(bool shouldReturn)
        {
            ReturnToBase = !shouldReturn;
            OnReturnToBaseCalled();
        }
    }
}

