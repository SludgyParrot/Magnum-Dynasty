public enum InputAction
{
    Any,
    Accelerate,
    Brake,
    Steer
}

public enum DriveType
{
    AWD,
    FWD,
    RWD
}

public enum RacePointType
{
    None,
    CheckPoint,
    Acceleration,
    Brake,
    
}

public enum MisteryBoxType
{
    None,
    MachineGun,
    Ammo,
    Explosives,
    RepairTool,
    Points
}

public enum VehicleLightType
{
   None,
   Head,
   HighBeam,
   Brake,
   SignalLeft,
   SignalRight,
   Parking

}

public enum SensorType
{
    None,
    FrontMidSensor,
    FrontLeftSensor,
    FrontRightSensor,
    FrontLeftAngledSensor,
    FrontRightAngledSensor,
    LeftSideSensor,
    RightSideSensor
}

public enum AudioType
{
    Ambience,
    Environment,
    Music,
    Fx,
    Narrator
}

public enum MusicGroupType
{
    RapMusic,
    HouseMusic,
    KwaitoMusic,
    RockMusic
}

public enum CameraViewType
{
    Default,
    Menu,
    Racing,
    RacingCockpit
}

public enum TriggerEventEntry
{
    Custom,
    OnAwake,
    OnStart,
    OnEnabled,
    OnDisabled,
    OnIntervals
}

public enum TooltipType
{
    None,
    MatchStartTooltip,
    MatchEndTooltip
}

public enum ScreenState
{
    Focus,
    Blur
}

public enum ScreenSortingLayer
{
    Default,
    ForeGround,
    Background
}