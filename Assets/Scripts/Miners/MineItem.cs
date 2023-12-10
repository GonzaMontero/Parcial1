using AI.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineItem : MonoBehaviour
{
    public int FoodAmount = 20;
    public int MineralAmount = 20;

    public static Action<bool, bool> OnMineDrained;

    public bool Worked = false;
    public bool Empty = false;

    public int TryMine(int amountToMine)
    {
        Worked = true;

        if (!Empty)
        {
            int taken = MineralAmount - amountToMine;

            if(taken <= 0)
            {
                amountToMine += taken;
                SetEmpty();
            }

            SetMineralAmount(MineralAmount - amountToMine);

            return amountToMine;
        }

        return 0;
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

    private void SetEmpty()
    {
        Empty = true;
        //MapManager.instance.Mines.remove(this);
        Destroy(gameObject);
        //OnMineDrained?.Invoke(MapManager.instance.minesavailable.count > 0, MapManager.instance.minesworked.count > 0);
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
