using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBall : MonoBehaviour
{
    [SerializeField] GameObject ballTubeCollider;
    public void OnRayHit()
    {
        ballTubeCollider.SetActive(true); 
    }
}
