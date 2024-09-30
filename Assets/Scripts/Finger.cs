using DG.Tweening;
using UnityEngine;

public class Finger : MonoBehaviour
{
    [SerializeField] Transform target;

    // Start is called before the first frame update
    void Start()
    {

       
        transform.DOMove(target.position, 2).SetLoops(-1, LoopType.Restart);
    }

    
}
