using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Managers;
using System;

public class UIController : MonoBehaviour
{
    public SimulationManager SimulationManager;

    public TMP_InputField WeighInput;
    public TMP_InputField XInput;
    public TMP_InputField YInput;

    private Vector2Int slotPositionToAffect;
    private int slotWeightToApply;

    public void UpdateWeight()
    {
        slotPositionToAffect = new Vector2Int(Convert.ToInt32(XInput.text), Convert.ToInt32(YInput.text));
        slotWeightToApply = Convert.ToInt32(WeighInput.text);

        SimulationManager.MinerManager.UpdateWeight(slotPositionToAffect, slotWeightToApply);
    }

    public void SpawnMiner()
    {
        SimulationManager.SpawnMiner();
    }

    public void HeadToDeposit()
    {
        SimulationManager.MinerManager.AbruptExit();
    }
}
