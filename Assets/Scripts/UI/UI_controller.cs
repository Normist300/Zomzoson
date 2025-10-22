using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIController : MonoBehaviour
{
    private List<Click> subscribedClicks = new();
    int currentUIPosition = -1;
    public GameObject currentGameObject;
    public bool isSelected;
    Click[] tiles;

    private void OnEnable()
    {
        // Подключение ивентов со всех экземпляров
        tiles = GetComponentsInChildren<Click>()
                               .Where(t => t.CompareTag("UITile"))
                               .ToArray();

        foreach (var tile in tiles)
        {
            tile.eventSelectTile += onSelectTile;
            tile.eventDisableTile += onDiasableTile;
            subscribedClicks.Add(tile);
        }
    }

    private void OnDisable()
    {
        // Отключение ивентов со всех экземпляров
        foreach (var tile in subscribedClicks)
        {
            tile.eventSelectTile -= onSelectTile;
            tile.eventDisableTile -= onDiasableTile;
        }
        subscribedClicks.Clear();
    }

    void Start()
    {
        isSelected = false;
    }

    void Update()
    {
    }

    void onSelectTile(int UIPosition, GameObject newGameObject)
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

    void onDiasableTile()
    {
        tiles[currentUIPosition].SetHighlightClicked(false);
        tiles[currentUIPosition].isClicked = false;
        currentUIPosition = -1;
        currentGameObject = null;
        isSelected = false;
    }
}
