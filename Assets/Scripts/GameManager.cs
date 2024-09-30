using InstantGamesBridge;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    public float shapesPrice = 0;
    public float ballsPrice = 525;
    public float incomePrice = 31;

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
    [SerializeField] ParticleSystem particleEffect;
    public int isTutorialCompleted;
    public Tutorial tutorial;
    DragAndDrop dragAndDrop;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        //DontDestroyOnLoad(gameObject);
        tutorial = GetComponent<Tutorial>();
        
    }
    public void ShowInterstitial()
    {
        Bridge.advertisement.ShowInterstitial();
    }
   
    private void Start()
    {
        dragAndDrop = FindAnyObjectByType<DragAndDrop>();
        LoadGame();
        tutorial = FindAnyObjectByType<Tutorial>();
        UpdateUi();
        StartCoroutine(GenerateBalls());
    }
    
    IEnumerator GenerateBalls()
    {
        yield return new WaitForSeconds(1);
        Instantiate(ballPrefab, new Vector3(0.041f, 5.96f, 0), Quaternion.identity);
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
        if (Input.GetKeyDown(KeyCode.Y))
        {
            SaveCells();
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
    public void SaveCells()
    {
        GameState gameState = new GameState();
        Cell[] cells = FindObjectsOfType<Cell>();
        //print("COUNT: " + FindObjectsOfType<MazeElement>().Length);
        foreach (MazeElement element in FindObjectsOfType<MazeElement>())
        {
            Cell cell = element.GetComponentInParent<Cell>();
            if (cell != null)
            {
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
            else
            {
                Debug.LogWarning($"Element {element.name} is not attached to any cell.");
            }
        }

        foreach (Cell cell in cells)
        {
            gameState.cells.Add(new CellData { isEmpty = cell.isEmpty });
        }

        gameState.ballCount = FindObjectsOfType<BallController>().Length;
        string json = JsonUtility.ToJson(gameState, true);
        //print(json);
        PlayerPrefs.SetString("GameState", json);
    }

    public void SaveGame()
    {
        
        SaveCells();
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
    public string FormatNumber(float number)
    {
        if (number >= 1000000)
        {
            return (number / 1000000f).ToString("0.0") + "M";
        }
        else if (number >= 1000)
        {
            return (number / 1000f).ToString("0.0") + "K";
        }
        else
        {
            return number.ToString("0.00");
        }
    }
    public void LoadGame()
    {
        shapesPrice = PlayerPrefs.GetFloat("shapesPrice", 0);
        ballsPrice = PlayerPrefs.GetFloat("ballsPrice", 525);
        incomePrice = PlayerPrefs.GetFloat("incomePrice", 31);
        coins = PlayerPrefs.GetFloat("coins", 300);
        globalMultiplier = PlayerPrefs.GetFloat("globalMultiplier", 1);
        shapesLevel = PlayerPrefs.GetInt("shapesLevel", 1);
        ballsLevel = PlayerPrefs.GetInt("ballsLevel", 1);
        incomelevel = PlayerPrefs.GetInt("incomelevel", 1);
        bgIndex = PlayerPrefs.GetInt("bgIndex", 0);
        isTutorialCompleted = PlayerPrefs.GetInt("tutorialCompleted", 0);
        UpdateUi();

        if (PlayerPrefs.HasKey("GameState"))
        {
            string json = PlayerPrefs.GetString("GameState");
            GameState gameState = JsonUtility.FromJson<GameState>(json);
            Cell[] cells = FindObjectsOfType<Cell>();

            for (int i = 0; i < gameState.cells.Count; i++)
            {
                if (i < cells.Length)
                {
                    cells[i].isEmpty = gameState.cells[i].isEmpty;
                }
                else
                {
                    Debug.LogWarning($"Saved cell index {i} is out of bounds for current cells array.");
                }
            }

            foreach (ElementData elementData in gameState.elements)
            {
                if (elementData.cellIndex >= 0 && elementData.cellIndex < cells.Length)
                {
                    Cell cell = cells[elementData.cellIndex];
                    GameObject prefab = shapePrefabs[elementData.level - 1];
                    GameObject newElement = Instantiate(prefab, elementData.position, Quaternion.identity);

                    MazeElement mazeElement = newElement.GetComponent<MazeElement>();
                    mazeElement.level = elementData.level;
                    mazeElement.type = (MazeElement.ElementType)System.Enum.Parse(typeof(MazeElement.ElementType), elementData.type);

                    if (mazeElement.type == MazeElement.ElementType.Triangle && elementData.orientation.HasValue)
                    {
                        mazeElement.triangleOrientation = elementData.orientation.Value;
                    }

                    DragAndDrop dragAndDrop = newElement.GetComponent<DragAndDrop>();
                    if (dragAndDrop != null)
                    {
                        dragAndDrop.SnapToCell(cell); // Place element correctly
                    }
                    else
                    {
                        Debug.LogWarning("DragAndDrop component is missing on the element prefab.");
                    }
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
            if(tutorial.currentStep == 6 && isTutorialCompleted == 0)
            {
                tutorial.TutorialComplete();
                isTutorialCompleted = 1;
                PlayerPrefs.SetInt("tutorialCompleted", 1);
            }
            //newElement.transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0), 0.3f);
            particleEffect.transform.position = elementB.transform.position;
            particleEffect.Play();
            
            mazeElementA.GetComponent<DragAndDrop>().currentCell = null;
            mazeElementB.GetComponent<DragAndDrop>().currentCell = null;
            Destroy(elementA);
            StartCoroutine(SaveMerge());
        }
        
    }
    IEnumerator SaveMerge()
    {
        yield return new WaitForSeconds(0.5f);
        SaveCells();

    }
    public void UpdateUi()
    {
        moneyText.text = "$" + FormatNumber(coins);
        shapesPriceText.text = "$" + FormatNumber(shapesPrice);
        ballsPriceText.text = "$" + FormatNumber(ballsPrice);
        incomePriceText.text = "$" + FormatNumber(incomePrice);
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
         float[] shapesPrices = new float[] { 0,0,0,0, 28, 36, 48, 78, 82, 88, 98, 112, 126, 162, 174, 184, 192, 200, 210, 239, 253, 272, 299, 434, 542, 616, 686, 711, 727, 764, 814, 888, 932, 999, 1100, 1200, 1300 };
    coins -= button.price;
        switch (button.buttonType)
        {
            case BottomButtons.ButtonType.AddShape:
                if (shapesLevel < shapesPrices.Length)
                {
                    button.price = shapesPrices[shapesLevel];
                    shapesPrice = button.price;
                }
                else
                {
                    button.price = button.price * 1.02f;
                }
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
