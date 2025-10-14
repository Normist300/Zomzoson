using NUnit.Framework.Constraints;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GridController : MonoBehaviour, IController
{
    Cell[] cells;
    public List<Cell> subscribedCells = new();

    [SerializeField] private Grid grid;
    [SerializeField] private UIController UIcontroller;
    public Material materialPreviewMode;

    public Vector2Int currentPreview = new Vector2Int(-1, -1);

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
            cell.eventPreview += onPreview;
            cell.eventDisPreview += onDisPreview;
            subscribedCells.Add(cell);
        }
    }

    private void OnDisable()
    {
        // Отключение ивентов со всех экземпляров
        foreach (var cell in subscribedCells)
        {
            cell.eventReplaceCell -= onReplaceCell;
            cell.eventPreview -= onPreview;
            cell.eventDisPreview -= onDisPreview;
        }
        subscribedCells.Clear();
    }

    void onGridReady()
    {
        OnEnable();
    }

    public void onReplaceCell(int x, int y)
    {
        if (UIcontroller.currentGameObject != null)
        {
            GameObject goForFunc = Instantiate(UIcontroller.currentGameObject);
            Cell goCell = goForFunc.GetComponent<Cell>();
            goCell.isMounted = true;
            grid.ReplaceCell(x, y, goForFunc);
        }
        else Debug.LogWarning("UIcontroller.currentGameObject == null");

    }

    public void onPreview(int x, int y)
    {
        if (UIcontroller.currentGameObject != null)
        {
            GameObject pasteObject = Instantiate(UIcontroller.currentGameObject);
            Renderer[] rendsPasteObject = pasteObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in rendsPasteObject)
            {
                rend.material = materialPreviewMode;
            }
            Cell classCellPreviewObject = pasteObject.GetComponent<Cell>();
            classCellPreviewObject.isPreview = true; 
            grid.ReplaceCell(x, y, pasteObject);

            if (currentPreview != new Vector2Int(-1, -1))
            {
                onDisPreview(currentPreview.x, currentPreview.y);
                currentPreview = new Vector2Int(x, y);
            }
            else
            {
                currentPreview = new Vector2Int(x, y);
            }
        }
        else Debug.LogWarning("UIcontroller.currentGameObject == null");
    }

    public void onDisPreview(int x, int y)
    {
         if (UIcontroller.currentGameObject != null)
        {
            grid.ReplaceCell(x, y, grid.cellSpace);
        }
        else Debug.LogWarning("UIcontroller.currentGameObject == null");
    }
}
