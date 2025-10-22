using NUnit.Framework.Constraints;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GridController : MonoBehaviour, IController
{
    ISubscribable<GridController>[] iSubCells;
    public List<Cell> subscribedCells = new();

    [SerializeField] private Grid grid;
    [SerializeField] private UIController UIcontroller;
    public Material materialPreviewMode;

    public Vector2Int currentMouseEnter = new Vector2Int(-1, -1);

    public enum CellSideType
    {
        None,       // Пустая сторона
        Road        // Дорога
    }

    private void Start()
    {
        grid.eventGridReady += onGridReady;
    }

    private void OnEnable()
    {
        // Подключение ивентов со всех экземпляров
        iSubCells = GetComponentsInChildren<Cell>()
                               .Where(t => t.CompareTag("Cell"))
                               .ToArray();

        foreach (var cell in iSubCells)
        {
            cell.Subscribe(this);
        }
    }

    private void OnDisable()
    {
        // Отключение ивентов со всех экземпляров
        foreach (var cell in subscribedCells.ToList()) // ToList() чтобы не было ошибки
        {
            cell.Unsubscribe(this);
        }
        subscribedCells.Clear();
    }

    void onGridReady()
    {
        OnEnable();
    }

    public void onReplaceCell(Vector2Int pos)
    {
        if (UIcontroller.currentGameObject != null)
        {
            GameObject goForFunc = Instantiate(UIcontroller.currentGameObject);
            Cell goCell = goForFunc.GetComponent<Cell>();
            goCell.isMounted = true;
            grid.ReplaceCell(pos, goForFunc);
        }
        else Debug.LogWarning("UIcontroller.currentGameObject == null");

    }

    public void onPreview(Vector2Int pos)
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
            grid.ReplaceCell(pos, pasteObject);

            if (currentMouseEnter != new Vector2Int(-1, -1))
            {
                Cell pastClassCell = grid.GetCell(currentMouseEnter).GetComponent<Cell>();
                if (!pastClassCell.isMounted)
                {
                    onDisPreview(currentMouseEnter);
                    currentMouseEnter = pos;
                }
                else
                {
                    currentMouseEnter = pos;
                }
            }
            else
            {
                currentMouseEnter = pos;
            }
        }
        else Debug.LogWarning("UIcontroller.currentGameObject == null");
    }

    public void onDisPreview(Vector2Int pos)
    {
         if (UIcontroller.currentGameObject != null)
        {
            grid.ReplaceCell(pos, grid.cellSpace);
        }
        else Debug.LogWarning("UIcontroller.currentGameObject == null");
    }

    public void onEnterMount(Vector2Int pos)
    // Удаляет превью при заходе на смонтированную ячейку и обновляет currentMouseEnter
    {
        if (currentMouseEnter != new Vector2Int(-1, -1))
        {
            Cell pastClassCell = grid.GetCell(currentMouseEnter).GetComponent<Cell>();
            if (pastClassCell.isPreview)
            {
                onDisPreview(currentMouseEnter);
                currentMouseEnter = pos;
            }
        }
        else
        {
            currentMouseEnter = pos;
        }
    }
}
