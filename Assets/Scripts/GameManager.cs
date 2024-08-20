using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using InstantGamesBridge;
[System.Serializable]
public class GameState
{
    public List<ElementData> elements = new List<ElementData>();
    public List<CellData> cells = new List<CellData>();
    public int ballCount;
}

[System.Serializable]
public class ElementData
{
    public string type;
    public int level;
    public Vector2 position;
    public MazeElement.TriangleOrientation? orientation;
    public int cellIndex; 
}

[System.Serializable]
public class CellData
{
    public bool isEmpty;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public float shapesPrice = 1;
    public float ballsPrice = 10000;
    public float incomePrice = 100;

    public int shapesLevel = 1;
    public int ballsLevel = 1;
    public int incomelevel = 1;
    public float coins = 10;
    public float globalMultiplier = 1;
    public List<GameObject> shapePrefabs = new List<GameObject>();
    public GameObject ballPrefab;
    [Header("UI")]
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text shapesPriceText;
    [SerializeField] private TMP_Text ballsPriceText;
    [SerializeField] private TMP_Text incomePriceText;
    [SerializeField] private List<Sprite> bgSprites = new List<Sprite>();
    [SerializeField] private SpriteRenderer bg;
    public int actions = 0;
    int ballsCount = 0;
    int bgIndex = 0;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        LoadGame();
    }
    public void ShowInterstitial()
    {
        Bridge.advertisement.ShowInterstitial();
    }
    private void Start()
    {
        UpdateUi();
        StartCoroutine(GenerateBalls());
    }
    IEnumerator GenerateBalls()
    {
        yield return new WaitForSeconds(1);
        Instantiate(ballPrefab,new Vector3(0.041f, 5.96f,0),Quaternion.identity);
        ballsCount++;
        if (ballsCount < PlayerPrefs.GetInt("ballsLevel")) 
        {
            StartCoroutine(GenerateBalls());
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("All data deleted.");
        }
        if (actions == 100)
        {
            bgIndex++;
            if (bgIndex > bgSprites.Count)
                bgIndex = 0;
            PlayerPrefs.SetInt("bgIndex", bgIndex);
            
            bg.sprite = bgSprites[bgIndex];
            actions = 0;
        }
    }

    public void SaveGame()
    {
        GameState gameState = new GameState();
        Cell[] cells = FindObjectsOfType<Cell>();

        foreach (MazeElement element in FindObjectsOfType<MazeElement>())
        {
           
                Cell cell = element.GetComponentInParent<Cell>();
                int cellIndex = System.Array.IndexOf(cells, cell);

                ElementData elementData = new ElementData
                {
                    type = element.type.ToString(),
                    level = element.level,
                    position = element.transform.position,
                    orientation = element.type == MazeElement.ElementType.Triangle ? element.triangleOrientation : (MazeElement.TriangleOrientation?)null,
                    cellIndex = cellIndex
                };
                gameState.elements.Add(elementData);
            
        }

        foreach (Cell cell in cells)
        {
            gameState.cells.Add(new CellData { isEmpty = cell.isEmpty });
        }

        gameState.ballCount = FindObjectsOfType<BallController>().Length;

        string json = JsonUtility.ToJson(gameState, true);
        PlayerPrefs.SetString("GameState", json);

        PlayerPrefs.SetFloat("shapesPrice", shapesPrice);
        PlayerPrefs.SetFloat("ballsPrice", ballsPrice);
        PlayerPrefs.SetFloat("incomePrice", incomePrice);

        PlayerPrefs.SetInt("shapesLevel", shapesLevel);
        PlayerPrefs.SetInt("ballsLevel", ballsLevel);
        PlayerPrefs.SetInt("incomelevel", incomelevel);

        PlayerPrefs.SetFloat("coins", coins);
        PlayerPrefs.SetFloat("globalMultiplier", globalMultiplier);
        PlayerPrefs.SetInt("bgIndex", bgIndex);
       PlayerPrefs.Save();

        

       
    }

    public void LoadGame()
    {
        shapesPrice = PlayerPrefs.GetFloat("shapesPrice", 1);
        ballsPrice = PlayerPrefs.GetFloat("ballsPrice", 10000);
        incomePrice = PlayerPrefs.GetFloat("incomePrice", 100);
        coins = PlayerPrefs.GetFloat("coins", 10);
        globalMultiplier = PlayerPrefs.GetFloat("globalMultiplier", 1);
        shapesLevel = PlayerPrefs.GetInt("shapesLevel", 1);
        ballsLevel = PlayerPrefs.GetInt("ballsLevel", 1);
        incomelevel = PlayerPrefs.GetInt("incomelevel", 1);
        bgIndex = PlayerPrefs.GetInt("bgIndex", 0);
        UpdateUi();

        if (PlayerPrefs.HasKey("GameState"))
        {
            string json = PlayerPrefs.GetString("GameState");
            GameState gameState = JsonUtility.FromJson<GameState>(json);
            Cell[] cells = FindObjectsOfType<Cell>();

            for (int i = 0; i < gameState.cells.Count; i++)
            {
                cells[i].isEmpty = gameState.cells[i].isEmpty;
            }

            foreach (ElementData elementData in gameState.elements)
            {
                GameObject prefab = shapePrefabs[elementData.level - 1];
                GameObject newElement = Instantiate(prefab, elementData.position, Quaternion.identity);

                MazeElement mazeElement = newElement.GetComponent<MazeElement>();
                mazeElement.level = elementData.level;
                mazeElement.type = (MazeElement.ElementType)System.Enum.Parse(typeof(MazeElement.ElementType), elementData.type);

                if (mazeElement.type == MazeElement.ElementType.Triangle && elementData.orientation.HasValue)
                {
                    mazeElement.triangleOrientation = elementData.orientation.Value;
                }

                if (elementData.cellIndex >= 0 && elementData.cellIndex < cells.Length)
                {
                    Cell cell = cells[elementData.cellIndex];
                    newElement.GetComponent<DragAndDrop>().SnapToCell(cell);
                }
                else
                {
                    Debug.LogWarning($"Invalid cell index {elementData.cellIndex} for element at position {elementData.position}. Skipping this element.");
                }
                    
            }

           

        
        }
    }

    public void MergeElements(GameObject elementA, GameObject elementB)
    {
        if (elementA == elementB)
        {
            Debug.LogWarning("Попытка слияния элемента с самим собой предотвращена.");
            return;
        }

        MazeElement mazeElementA = elementA.GetComponent<MazeElement>();
        MazeElement mazeElementB = elementB.GetComponent<MazeElement>();

        if (mazeElementA != null && mazeElementB != null && mazeElementA.level == mazeElementB.level && mazeElementA.level < 10)
        {
            int nextLevel = mazeElementA.level + 1;

            Cell currentCellA = mazeElementA.GetComponent<DragAndDrop>().currentCell;

            Destroy(elementB);

            GameObject newElement = Instantiate(shapePrefabs[nextLevel - 1], currentCellA.transform.position, Quaternion.identity, currentCellA.transform);
            newElement.GetComponent<MazeElement>().level = nextLevel;

            newElement.GetComponent<DragAndDrop>().SnapToCell(currentCellA);
            newElement.transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0), 0.3f);
            Destroy(elementA);
        }
    }

    public void UpdateUi()
    {
        moneyText.text = "$" + formatMoneyText();
        shapesPriceText.text = "$" + shapesPrice.ToString("0.0");
        ballsPriceText.text = "$" + ballsPrice.ToString("0.0");
        incomePriceText.text = "$" + incomePrice.ToString("0.0");
    }

    string formatMoneyText()
    {
        string formattedNumber = coins.ToString("0.0");
        int dotIndex = formattedNumber.IndexOf('.');
        if (dotIndex >= 0 && formattedNumber.Length > dotIndex + 3)
        {
            formattedNumber = formattedNumber.Substring(0, dotIndex + 3);
        }

        return formattedNumber;
    }

    public void CalculatePrice(BottomButtons button)
    {
        coins -= button.price;
        switch (button.buttonType)
        {
            case BottomButtons.ButtonType.AddShape:
                button.price += 3;
                button.UpdateUi();
                shapesLevel++;
                shapesPrice = button.price;
                PlayerPrefs.SetInt("shapesLevel", shapesLevel);
                PlayerPrefs.SetFloat("shapesPrice", shapesPrice);
                break;
            case BottomButtons.ButtonType.AddBall:
                button.price *= (0.1f * ballsLevel) + 1.4f;
                button.UpdateUi();
                ballsLevel++;
                ballsPrice = button.price;
                PlayerPrefs.SetInt("ballsLevel", ballsLevel);
                PlayerPrefs.SetFloat("ballsPrice", ballsPrice);
                break;
            case BottomButtons.ButtonType.Income:
                button.price *= 1.04f + (0.1f * ballsLevel);
                button.UpdateUi();
                incomelevel++;
                incomePrice = button.price;
                PlayerPrefs.SetInt("incomelevel", incomelevel);
                PlayerPrefs.SetFloat("incomePrice", incomePrice);
                break;
        }
        PlayerPrefs.SetFloat("coins", coins);
        PlayerPrefs.SetFloat("globalMultiplier", globalMultiplier);
        SaveGame();
        UpdateUi();
    }
}
