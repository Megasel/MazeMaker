using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Petal : MonoBehaviour
{
    [SerializeField]BoxCollider2D boxCollider;
    //[SerializeField] List<GameObject> dollarObj = new List<GameObject>();

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("MazeElement"))
    //    {
    //        boxCollider.enabled = false;
    //        foreach (GameObject obj in dollarObj)
    //        {
    //            obj.SetActive(false);
    //        }
    //    }
    //}
    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("MazeElement"))
    //    {
    //        Debug.Log("!");
    //        foreach (GameObject obj in dollarObj)
    //        {
    //            obj.SetActive(true);
    //        }
    //        boxCollider.enabled = true;

    //    }
    //}
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "gipotenuza" )
        {
            boxCollider.enabled = false;

        }

    }
    private void OnTriggerStay2D(Collider2D other)
    {
        
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "gipotenuza" )
        {
            print(collision.gameObject.name);
            //boxCollider.enabled = true;
        }
    }
}
