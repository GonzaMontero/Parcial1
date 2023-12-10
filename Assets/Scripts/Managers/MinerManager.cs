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

            MineItem.OnMineDrained += (bool areMinesLeft, bool areWorkingMinesLeft) =>
            {
                Debug.Log("AYYYY");
            };
        }

        public void Update()
        {
            for(short i=0; i<PopulationTypes.Count; i++)
            {
                Parallel.ForEach(PopulationTypes[i].PopulationBag, ParallelOptions, currentPopulation =>
                {
                    currentPopulation.Update();
                });
            }
        }

        public void SetReturnToBase(bool shouldReturn)
        {
            returnToBase = !shouldReturn;
            OnReturnToBaseCalled();
        }
    }
}

