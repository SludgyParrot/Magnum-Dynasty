public enum InputButton
{
    AButton,
    BButton,
    XButton,
    YButton,
    RightButton,
    LeftButton,
    DPadUp,
    DPadDown,
    DPadLeft,
    DPadRight,
    StartButton,
    OptionsButton
}

public enum InputFloat
{
   LeftTrigger,
   RightTrigger
}

public enum InputVector
{
    LeftJoystick,
    RightJoystick
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

public enum ScreenUIType
{
    None,
    MusicPlayerPreview,
    MatchHUD,
    CollactableHUD,
    TutorialRightHUD,
    TutorialLeftHUD,
    WeaponsMenuHUD
}

public enum UIViewState
{
    Show,
    Hide
}

public enum ScreenUITransitionType
{
    Default,
    Fade,
    Translate
}

public enum TranslatePropertyType
{
    None,
    All,
    Position,
    Rotation,
    Scale
}

public enum AnimationType
{
    Default,
    Loop,
    Randomized
}

public enum RaceStatus
{
    Started,
    InProgress,
    Completed
}