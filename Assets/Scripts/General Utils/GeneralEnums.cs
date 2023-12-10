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

public enum SlotStates
{
    Open,
    Close,
    Ready,
    Obstacle
}