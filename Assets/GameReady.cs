using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameReady : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InstantGamesBridge.Bridge.platform.SendMessage(InstantGamesBridge.Modules.Platform.PlatformMessage.GameReady);
    }

    
}
