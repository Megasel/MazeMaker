﻿#if UNITY_WEBGL
namespace InstantGamesBridge.Modules.Platform
{
    public enum PlatformMessage
    {
        GameReady,
        InGameLoadingStarted,
        InGameLoadingStopped,
        GameplayStarted,
        GameplayStopped,
        PlayerGotAchievement
    }
}
#endif