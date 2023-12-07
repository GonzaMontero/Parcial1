using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Managers
{
    public class MinerManager : MonoBehaviour
    {
        public ConcurrentBag<Miner> Miners = new();
        public ParallelOptions ParallelOptions;
        public Vector2Int DepositPosition;
        public Vector2Int RestPosition;
        public float DeltaTime;
        public Action OnUpdateWeight;

        public void Init(Vector2Int depositPosition, Vector2Int restPosition)
        {
            ParallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 6 };

            DepositPosition = depositPosition;
            RestPosition = restPosition;
        }

        public void UpdateMiners()
        {
            DeltaTime -= Time.deltaTime;

            foreach (var miner in Miners)
            {
                if (miner.updatePos)
                {
                    miner.transform.position = miner.CurrentPos;
                    miner.updatePos = false;
                }
            }

            Parallel.ForEach(Miners, ParallelOptions, miner => { miner.UpdateMiner(); });
        }

        public void UpdateWeight(Vector2Int gridSlot, int slotWeight)
        {
            Parallel.ForEach(Miners, ParallelOptions, miner => { miner.UpdateWeight(gridSlot, slotWeight); });
        }

        public void AbruptExit()
        {
            Parallel.ForEach(Miners, ParallelOptions, miner => { miner.ExitMiner(); });
        }       
    }
}

