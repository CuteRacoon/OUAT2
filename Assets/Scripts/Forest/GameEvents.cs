using System;
using UnityEditor;
public static class GameEvents
{
    public static event Action CanDisplayLampBar;
    public static event Action CannotDisplayLampBar;
    public static event Action LampStateChanging;
    public static event Action BarIsNull;
    public static event Action NeedToStopSprint;
    public static event Action NeedToStartSprint;

    public static void RaiseCanDisplayLampBar() => CanDisplayLampBar?.Invoke();
    public static void RaiseCannotDisplayLampBar() => CannotDisplayLampBar?.Invoke();
    public static void RaiseLampStateChanging() => LampStateChanging?.Invoke();
    public static void RaiseBarIsNull() => BarIsNull?.Invoke();

    public static void RaiseNeedToStopSprint() => NeedToStopSprint?.Invoke();
    public static void RaiseNeedToStartSprint() => NeedToStartSprint?.Invoke();
}
