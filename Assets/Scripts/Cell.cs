using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool isEmpty = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("MazeElement"))
        {
            isEmpty = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("MazeElement"))
        {
            isEmpty = true;
        }
    }

}
