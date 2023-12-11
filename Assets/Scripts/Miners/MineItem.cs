using AI.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineItem : MonoBehaviour
{
    public Vector2Int MinePosition;

    public int MineWeight;

    public int FoodAmount = 10;
    public int MineralAmount = 20;

    public static Action<bool, bool> OnMineDrained;
    public static Action OnMineStartedWorking;
    public static Action OnMineEnded;

    public bool Worked = false;
    public bool Empty = false;

    private int workingMiners = 0;

    public int TryMine(int amountToMine)
    {    
        if (!Empty)
        {
            int taken = MineralAmount - amountToMine;

            if(taken <= 0)
            {
                amountToMine += taken;
                Empty = true;
            }

            SetMineralAmount(MineralAmount - amountToMine);

            return amountToMine;
        }

        return 0;
    }

    public void StartWorking()
    {
        if (Worked == false)
        {
            Worked = true;
            MapManager.Instance.AllWorkedMines.Add(this);
            OnMineStartedWorking?.Invoke();
            workingMiners++;
            MineWeight++;
        }
    }

    public void StopWorking()
    {
        workingMiners--;
        MineWeight--;

        if(workingMiners <= 0)
        {
            workingMiners = 0;
            MapManager.Instance.AllWorkedMines.Remove(this);
            OnMineEnded?.Invoke();
        }
    }

    public void Update()
    {
        if (Empty)
        {
            MapManager.Instance.AllMinesOnMap.Remove(this);
            MapManager.Instance.AllWorkedMines.Remove(this);
            Destroy(gameObject);
            OnMineDrained?.Invoke(MapManager.Instance.AllMinesOnMap.Count > 0, MapManager.Instance.AllWorkedMines.Count > 0);
        }
    }

    public bool TryEat()
    {
        if (FoodAmount > 0)
        {
            FoodAmount--;
            return true;
        }
        else
            return false;
    }

    private void SetMineralAmount(int amount)
    {
        if(amount < 0)
        {
            amount = 0;
        }

        this.MineralAmount = amount;
    }
}
