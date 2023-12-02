public enum MinerStates
{
    Mining,
    GoToMine,
    GoToDeposit,
    Idle,
    GoToRest,
    _Count
}

public enum MinerFlags
{
    OnFullInventory,
    OnReachMine,
    OnReachDeposit,
    OnEmptyMine,
    OnAbruptReturn,
    OnGoBackToWork,
    OnIdle,
    _Count
}

public enum SlotStates
{
    Open,
    Close,
    Ready,
    Obstacle
}