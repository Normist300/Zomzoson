using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GridController : MonoBehaviour
{
    private List<Cell> subscribedClicks = new();
    Cell[] cells;

    [SerializeField] private Grid grid;
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
            tile.OnSelectTile -= SelectTile;

        subscribedClicks.Clear();
    }

    void onReplaceCell(int x, int y)
    {

    }
}
