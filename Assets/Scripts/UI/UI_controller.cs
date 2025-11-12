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
    public bool isSelected;
    public GridController gridController;
    public Dictionary<int, GameObject> uiTiles = new();
    public Dictionary<GameObject, int> numberOfCells = new();
    public Vector3 SpawnPositionUiCell;
    Click[] tiles;


    public GameObject uiRoad;
    public GameObject uiTurnRoad;
    public GameObject uiCrossroad;

    private void Awake()
    {
        numberOfCells[uiRoad] = 5;
        numberOfCells[uiTurnRoad] = 4;
        numberOfCells[uiCrossroad] = 2;

        SpawnRandomUICell(false);
    }

    private void OnEnable()
    {
        GameObject[] goTiles;
        // Заполнение словаря uiTiles
        goTiles = GameObject.FindGameObjectsWithTag("UITile");
        foreach (var goTile in goTiles)
        {
            uiTiles[goTile.GetComponent<Click>().UIPositon] = goTile;
        }

        // Подключение ивентов со всех экземпляров
        tiles = GetComponentsInChildren<Click>()
                               .Where(t => t.CompareTag("UITile"))
                               .ToArray();

        foreach (var tile in tiles)
        {
            tile.Subscribe(this);
            subscribedClicks.Add(tile);

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
        GameObject randomUiCell = GetRandomUICellPrefab();
        randomUiCell = Instantiate(randomUiCell, transform);
        randomUiCell.transform.localPosition = SpawnPositionUiCell;
        Click classUiCell = randomUiCell.GetComponent<Click>();
        classUiCell.Subscribe(this);
        classUiCell.UIPositon = 1;
        if (Selected)
        {
            classUiCell.OnPointerClick(null);
        }

        uiTiles[1] = randomUiCell; // Пока что позиция 1
    }

    public void OnAfterMount()
    {
        Destroy(uiTiles[1]);
        uiTiles.Remove(1);
    }
}
