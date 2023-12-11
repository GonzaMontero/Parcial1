public enum MinerStates
{
    Mine,
    Return,
    Collect,
    Idle
}

public enum MinerFlags
{
    OnFindTarget,
    OnNearTarget,
    OnInventoryFull,
    OnMineDepleted,
    OnEmergency,
    OnReachDeposit,
    OnIdle
}

public enum FoodStates
{
    Supply,
    Return,
    Idle
}

public enum FoodFlags
{
    OnReturnToDeposit,
    OnSupply,
    OnIdle,
    OnEmergency
}

public enum SlotStates
{
    Open,
    Close,
    Ready,
    Obstacle
}