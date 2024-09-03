using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBall : MonoBehaviour
{
    [SerializeField] GameObject ballTubeCollider;
    public void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Ball"))
            ballTubeCollider.SetActive(true); 
    }
}
