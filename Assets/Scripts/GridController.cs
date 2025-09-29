using NUnit.Framework.Constraints;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GridController : MonoBehaviour
{
    Cell[] cells;
    private List<Cell> subscribedClicks = new();

    [SerializeField] private Grid grid;
    [SerializeField] private UIController UIcontroller;

    private void Start()
    {
        grid.eventGridReady += onGridReady;
    }

    private void OnEnable()
    {
        // Подключение ивентов со всех экземпляров
        cells = GetComponentsInChildren<Cell>()
                               .Where(t => t.CompareTag("Cell"))
                               .ToArray();

        foreach (var cell in cells)
        {
            cell.eventReplaceCell += onReplaceCell;
            subscribedClicks.Add(cell);
        }
    }

    private void OnDisable()
    {
        // Отключение ивентов со всех экземпляров
        foreach (var tile in subscribedClicks)
            tile.eventReplaceCell -= onReplaceCell;

        subscribedClicks.Clear();
    }

    void onGridReady()
    {
        OnEnable();
    }

    void onReplaceCell(int x, int y)
    {
        if (UIcontroller.currentGameObject != null)
        {
            grid.ReplaceCell(x, y, UIcontroller.currentGameObject);
        }
        else Debug.LogWarning("UIcontroller.currentGameObject == null");

    }
}
