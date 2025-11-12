using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class UIController : MonoBehaviour, IController
{
    private List<Click> subscribedClicks = new();
    int currentUIPosition = -1;
    public GameObject currentGameObject;
    public GameObject mainCamera;
    public bool isSelected;
    public GridController gridController;
    public Dictionary<int, GameObject> uiTiles = new();
    public Dictionary<GameObject, int> numberOfCells = new();
    [NonSerialized] public Dictionary<int, Click> tiles = new();
    public Vector3 SpawnPositionUiCell;


    public GameObject uiRoad;
    public GameObject uiTurnRoad;
    public GameObject uiCrossroad;

    private void Awake()
    {
        numberOfCells[uiRoad] = 5;
        numberOfCells[uiTurnRoad] = 4;
        numberOfCells[uiCrossroad] = 2;
    }

    private void OnEnable()
    {
        GameObject[] goTiles;
        // Заполнение словаря uiTiles
        goTiles = GameObject.FindGameObjectsWithTag("UITile");
        if (goTiles.Length != 0)
        {
            foreach (var goTile in goTiles)
            {
                uiTiles[goTile.GetComponent<Click>().UIPositon] = goTile;
            }
        }

        // Подключение ивентов со всех экземпляров
        Click[] tilesArray = GetComponentsInChildren<Click>()
                               .Where(t => t.CompareTag("UITile"))
                               .ToArray();

        foreach (var tile in tilesArray)
        {
            tile.Subscribe(this);
            subscribedClicks.Add(tile);
            tiles[tile.UIPositon] = tile; // Заполняем словарь tiles
        }
        

        gridController.Subscribe(this);
    }

    private void OnDisable()
    {
        // Отключение ивентов со всех экземпляров
        foreach (var tile in subscribedClicks)
        {
            tile.Unsubscribe(this);
        }
        subscribedClicks.Clear();

        gridController.Unsubscribe(this);
    }

    void Start()
    {
        isSelected = false;
        // Находим Main Camera
        mainCamera = GameObject.Find("Main Camera");
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera не найдена!");
            return;
        }
        SpawnRandomUICell(false);
    }

    void Update()
    {
    }

    public void OnSelectTile(int UIPosition, GameObject newGameObject)
    {
        currentGameObject = newGameObject;
        if (currentUIPosition == -1)
        {
            currentUIPosition = UIPosition;
            tiles[currentUIPosition].SetHighlightClicked(true);
        }
        else
        {
            tiles[currentUIPosition].SetHighlightClicked(false);
            tiles[currentUIPosition].isClicked = false;
            currentUIPosition = UIPosition;
            tiles[currentUIPosition].SetHighlightClicked(true);
        }
        isSelected = true;
    }

    public void OnDisableTile()
    {
        tiles[currentUIPosition].SetHighlightClicked(false);
        tiles[currentUIPosition].isClicked = false;
        currentUIPosition = -1;
        currentGameObject = null;
        isSelected = false;
    }

    public UnityEvent eventClearPreview;
    public void OnClearPreview()
    {
        eventClearPreview?.Invoke();
    }

    public GameObject GetRandomUICellPrefab()
    {
        // Функция также меняет веса в numberOfCells

        // 1️ Считаем общее количество всех экземпляров
        int total = 0;
        foreach (var kvp in numberOfCells)
            total += kvp.Value;

        // 2️ Выбираем случайное число от 1 до total включительно
        int randomValue = Random.Range(1, total + 1);

        // 3️ Проходим по словарю, уменьшая randomValue,
        // пока не попадём в диапазон конкретного объекта
        foreach (var kvp in numberOfCells)
        {
            randomValue -= kvp.Value;
            if (randomValue <= 0)
                numberOfCells[kvp.Key] = Math.Max(1, kvp.Value - 1); // Уменьшаем вес, но не ниже 1
                return kvp.Key;
        }
        
        return null;
    }

    public void SpawnRandomUICell(bool Selected = true)
    {
        GameObject _randomUiCellPrefab = GetRandomUICellPrefab();
        GameObject randomUiCell = Instantiate(_randomUiCellPrefab, transform);
        randomUiCell.transform.SetParent(mainCamera.transform);
        randomUiCell.transform.localPosition = SpawnPositionUiCell;
        randomUiCell.transform.rotation = Quaternion.Euler(0, 0, 0);
        uiTiles[1] = randomUiCell; // Пока что позиция 1

        Click classUiCell = randomUiCell.GetComponent<Click>();
        classUiCell.Subscribe(this);
        classUiCell.UIPositon = 0;
        tiles[classUiCell.UIPositon] = classUiCell; // Обновляем словрь tiles
        if (Selected)
        {
            classUiCell.OnPointerClick(null);
        }
    }

    public void OnAfterMount()
    {
        Destroy(uiTiles[1]);
        uiTiles.Remove(1);
    }
}
