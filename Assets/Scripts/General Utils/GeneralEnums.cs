public enum MinerStates
{
    Mine,
    Return,
    Collect
}

public enum MinerFlags
{
    OnFindTarget,
    OnNearTarget,
    OnInventoryFull,
    OnMineDepleted,
    OnEmergency,
    OnReachDeposit
}

public enum FoodStates
{
    Supply,
    Return
}

public enum FoodFlags
{
    OnReturnToDeposit,
    OnSupply
}

public enum SlotStates
{
    Open,
    Close,
    Ready,
    Obstacle
}