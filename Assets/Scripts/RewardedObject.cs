using DG.Tweening;
using InstantGamesBridge;
using InstantGamesBridge.Modules.Advertisement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RewardedObject : MonoBehaviour
{
    [SerializeField] List<Transform> points = new List<Transform>();
    [SerializeField] List<GameObject> shapePrefabs = new List<GameObject>();
    [SerializeField] private BottomButtons bottomButtons;
    [SerializeField] GameObject shape;
    int posIndex = 0;
    int randShape;
    bool isClick = false;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartFly());
    }
    private void OnEnable()
    {
        Bridge.advertisement.rewardedStateChanged += Advertisement_rewardedStateChanged;
        
    }

    private void Advertisement_rewardedStateChanged(InstantGamesBridge.Modules.Advertisement.RewardedState obj)
    {
        if (obj == RewardedState.Opened )
        {
            foreach(AudioSource aud in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
            {
                aud.enabled = false;    
            }
            Time.timeScale = 0;
        }
        if (obj == RewardedState.Closed )
        {
            foreach (AudioSource aud in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
            {
                aud.enabled = true;
            }
            Time.timeScale = 1;
        }

        if (obj == InstantGamesBridge.Modules.Advertisement.RewardedState.Rewarded && isClick)
        {
            isClick = false ;
            Debug.Log("Spawn");
            bottomButtons.AddShapeFromRewarded(true,randShape);
            GameManager.instance.SaveGame();
        }
    }

    private void OnMouseDown()
    {
        isClick = true;
        Bridge.advertisement.ShowRewarded();

    }
    IEnumerator StartFly()
    {
        List<int> elements = new List<int>();
        int maxLevelOfElement = 0;
        foreach (MazeElement element in FindObjectsByType<MazeElement>(FindObjectsSortMode.None).ToList()) {

            elements.Add(element.level);
            
        
        }
        if(elements.Count > 0 ) 
            maxLevelOfElement = elements.Max();
        if (maxLevelOfElement > 4) {
            randShape = Random.Range(0, maxLevelOfElement);

        }
        else
        {
            randShape = 3;
        }
         
        yield return new WaitForSeconds(60);
        GameObject shape = Instantiate(shapePrefabs[randShape], transform.position,Quaternion.identity,transform);
        Destroy(shape.GetComponent<MazeElement>());
        Destroy(shape.GetComponent<DragAndDrop>());
        foreach (Collider2D col in shape.GetComponentsInChildren<Collider2D>())
        {
            Destroy(col);
        }
        //shape.GetComponent<DragAndDrop>().enabled = false;
        Collider2D[] colliders = shape.GetComponents<Collider2D>();

        // ��������� ������ �� ���
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }
        shape.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        posIndex = 0;
        Move(0,shape);
    }
    void Move(int index,GameObject shape)
    {
        if (posIndex < points.Count)
        {
            transform.DOLocalMove(points[posIndex].position, 1).SetEase(Ease.OutSine).OnComplete(() => Move(posIndex,shape));
            posIndex++;

        }

            
        else
        {
            transform.position = points[0].position;
            Destroy(shape);
            StartCoroutine(StartFly());
        }
           
    }
}
