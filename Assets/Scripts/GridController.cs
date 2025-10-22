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
    public Material materialAvailablePreviewMode;

    public Vector2Int currentMouseEnter = new Vector2Int(-1, -1);

    public enum CellSideType
    {
        None,       // Пустая сторона
        Road,       // Дорога
        Nothing     // Ничего
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

    public void OnReplaceCell(Vector2Int pos)
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

    public bool IsCellAvailableForMount(Vector2Int pos) // Проверяем стороны на соответствие
    {
        bool[] sides = new bool[4];
        bool[] isSideNone = new bool[4];
        Cell cell = UIcontroller.currentGameObject.GetComponent<Cell>();

        
        if (pos.y + 1 <= grid.height)
        {
            Cell cellUp = grid.GetCell(new Vector2Int(pos.x, pos.y + 1)).GetComponent<Cell>();
            if (cellUp.cellSideType[2] == cell.cellSideType[0] || cellUp.cellSideType[2] == CellSideType.None)
            {
                sides[0] = true;
                if (cellUp.cellSideType[2] == CellSideType.None)
                {
                    isSideNone[0] = true;
                }
            }
        }
        else sides[0] = true;
        if (pos.x + 1 <= grid.width)
        {
            Cell cellRight = grid.GetCell(new Vector2Int(pos.x + 1, pos.y)).GetComponent<Cell>();
            if (cellRight.cellSideType[3] == cell.cellSideType[1] || cellRight.cellSideType[3] == CellSideType.None)
            {
                sides[1] = true;
                if (cellRight.cellSideType[3] == CellSideType.None)
                {
                    isSideNone[1] = true;
                }
            }
        }
        else sides[1] = true;
        if (pos.y - 1 >= 0)
        {
            Cell cellDown = grid.GetCell(new Vector2Int(pos.x, pos.y - 1)).GetComponent<Cell>();
            if (cellDown.cellSideType[0] == cell.cellSideType[2] || cellDown.cellSideType[0] == CellSideType.None)
            {
                sides[2] = true;
                if (cellDown.cellSideType[0] == CellSideType.None)
                {
                    isSideNone[2] = true;
                }
            }
        }
        else sides[2] = true;
        if (pos.x - 1 >= 0)
        {
            Cell cellLeft = grid.GetCell(new Vector2Int(pos.x - 1, pos.y)).GetComponent<Cell>();
            if (cellLeft.cellSideType[1] == cell.cellSideType[3] || cellLeft.cellSideType[1] == CellSideType.None)
            {
                sides[3] = true;
                if (cellLeft.cellSideType[1] == CellSideType.None)
                {
                    isSideNone[3] = true;
                }
            }
        }
        else sides[3] = true;

        if (isSideNone.All(n => n)) // Если все стороны None, то нельзя монтировать
        {
            return false;
        }
        return sides.All(s => s); // true, если все стороны подходят
    }

    public void OnPreview(Vector2Int pos)
    {
        if (UIcontroller.currentGameObject != null)
        {
            Material requiredMaterial;

            GameObject previewObject = Instantiate(UIcontroller.currentGameObject);
            Renderer[] rendsPasteObject = previewObject.GetComponentsInChildren<Renderer>();
            Cell cellPreview = previewObject.GetComponent<Cell>();

            // Определяем доступность размещения (проверяем соседние ячейки и их стороны)
            if (IsCellAvailableForMount(pos))
            {
                requiredMaterial = materialAvailablePreviewMode;
                cellPreview.isAvailableForMount = true;
            }
            else
            {
                requiredMaterial = materialPreviewMode;
            }
            cellPreview.isPreview = true;

            foreach (Renderer rend in rendsPasteObject)
            {
                rend.material = requiredMaterial;
            }
            grid.ReplaceCell(pos, previewObject);

            if (currentMouseEnter != new Vector2Int(-1, -1)) //Удаляем прошлую превью
            { 
                Cell pastClassCell = grid.GetCell(currentMouseEnter).GetComponent<Cell>();
                if (!pastClassCell.isMounted)
                {
                    OnDisPreview(currentMouseEnter);
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

    public void OnDisPreview(Vector2Int pos)
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
                OnDisPreview(currentMouseEnter);
                currentMouseEnter = pos;
            }
        }
        else
        {
            currentMouseEnter = pos;
        }
    }
}
