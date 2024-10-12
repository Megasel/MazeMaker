using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;
    private Vector3 originalPosition;
    public Cell currentCell; 
    private Collider2D[] colliders;
    private MazeElement mazeElement;
    Tutorial tutorial;
    [SerializeField] private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
    private TMP_Text text;
    private void Start()
    {
        originalPosition = transform.position;
        currentCell = FindNearestCell();
        colliders = GetComponentsInChildren<Collider2D>();
        mazeElement = GetComponent<MazeElement>(); 
        tutorial = FindAnyObjectByType<Tutorial>();
        if (currentCell != null)
        {
            currentCell.isEmpty = false;
        }
    }
    private void OnEnable()
    {
        text = GetComponentInChildren<TMP_Text>();
        foreach (SpriteRenderer spr in GetComponentsInChildren<SpriteRenderer>()) { 
            spriteRenderers.Add(spr);   
        
        
        }
    }
    private void OnMouseDown()
    {
        SetCollidersEnabled(false);
        mazeElement.enabled = false; 
        offset = transform.position - GetMouseWorldPosition();
        isDragging = true;
        text.GetComponent<Renderer>().sortingOrder += 100;
        foreach (SpriteRenderer spriteRenderer in spriteRenderers) 
        {
            spriteRenderer.sortingOrder += 100;
        }
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPosition() + offset;
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;
        Cell nearestCell = FindNearestCell();
        text.GetComponent<Renderer>().sortingOrder -= 100;
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.sortingOrder -= 100;
        }
        if (nearestCell != null)
        {
            MazeElement existingElement = nearestCell.GetComponentInChildren<MazeElement>();

            if (nearestCell.isEmpty || nearestCell == currentCell)
            {
                if (GameManager.instance.isTutorialCompleted == 0 && tutorial.currentStep == nearestCell.tutorialStep && nearestCell)
                {
                    tutorial.NextStep();
                    SnapToCell(nearestCell);
                }
                else if(GameManager.instance.isTutorialCompleted == 0 && tutorial.currentStep != nearestCell.tutorialStep && nearestCell)
                {
                    transform.position = originalPosition;
                }
                else
                {
                    SnapToCell(nearestCell);
                    
                }
                 
            }
            else if (existingElement != null)
            {
                MazeElement thisElement = GetComponent<MazeElement>();
               
                if (existingElement.level == thisElement.level && (PlayerPrefs.GetInt("tutorialCompleted") == 0 && tutorial.currentStep == 6 || PlayerPrefs.GetInt("tutorialCompleted") == 1))
                {
                    print("swap");
                    currentCell.isEmpty = true;

                        GameManager.instance.MergeElements(existingElement.gameObject, gameObject);
                    
                    
                }
                else
                {

                    SwapElements(existingElement);
                }
            }
            else
            {
                transform.position = originalPosition;
            }
        }
        else
        {
            transform.position = originalPosition;
        }

        SetCollidersEnabled(true);
        mazeElement.enabled = true; 
        GameManager.instance.actions++;
        GameManager.instance.ShowInterstitial();
    }

    private void SetCollidersEnabled(bool enabled)
    {
        foreach (var collider in colliders)
        {
            collider.enabled = enabled;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
   
    private Cell FindNearestCell()
    {
        Cell[] cells = FindObjectsOfType<Cell>();
        Cell nearestCell = null;
        float minDistance = Mathf.Infinity;

        foreach (Cell cell in cells)
        {
            float distance = Vector2.Distance(transform.position, cell.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestCell = cell;
            }
        }

        if (nearestCell != null && minDistance <= 1.0f)
        {
            return nearestCell;
        }
        else
        {
            return null;
        }
    }

    public void SnapToCell(Cell cell)
    {
        if (currentCell != null && currentCell != cell)
        {
            currentCell.isEmpty = true;
        }
        originalPosition = cell.transform.position;
        transform.position = cell.transform.position;

        transform.SetParent(cell.transform);

        transform.localScale = Vector3.one;

        cell.isEmpty = false;
        currentCell = cell;
        GameManager.instance.SaveGame();
    }


    private void SwapElements(MazeElement otherElement)
    {
        // Get the current cells of both elements
        Cell otherCell = otherElement.GetComponent<DragAndDrop>().currentCell;
        Cell tempCell = currentCell;

        // Set the original cell to empty
        currentCell.isEmpty = true;

        // Snap this element to the other cell
        SnapToCell(otherCell);

        // Snap the other element to this cell
        otherElement.GetComponent<DragAndDrop>().SnapToCell(tempCell);

        // After snapping, set the new current cell as not empty
        otherCell.isEmpty = false;
        tempCell.isEmpty = false;

        // Save the game state after the swap
        GameManager.instance.SaveGame();
    }

}
