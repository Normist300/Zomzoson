using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class GridController : MonoBehaviour, IController
{
    ISubscribable<GridController>[] iSubCells;
    ISubscribable<GridController> wasdGod;
    public List<Cell> subscribedCells = new();

    [SerializeField] private Grid grid;
    [SerializeField] private UIController UIcontroller;
    public Material materialPreviewMode;
    public Material materialAvailablePreviewMode;

    public Vector2Int currentMouseEnter = new Vector2Int(-1, -1);
    public Rotation currentRotation = Rotation.up;

    public enum Rotation // Куда смотрит вверхняя сторона ячейки
    {
        up,     // 0
        right,  // 90
        down,   // 180
        left    // 270
    }

    public enum CellSideType
    {
        None,       // Ничего
        Road,       // Дорога
        Nothing     // Как правило поле
    }

    private void Start()
    {
        grid.eventGridReady += onGridReady;
    }

    private void OnEnable()
    {
        // Подписываем WASDGod
        wasdGod = FindFirstObjectByType<WASDGod>();
        wasdGod.Subscribe(this);
    }

    private void OnDisable()
    {
        // Отключение ивентов со всех экземпляров
        foreach (var cell in subscribedCells.ToList()) // ToList() чтобы не было ошибки
        {
            cell.Unsubscribe(this);
        }
        subscribedCells.Clear();


        // Отписываем WASDGod
        wasdGod.Unsubscribe(this);
        wasdGod = null;
    }

    void onGridReady()
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

    public void RotateTypeSideOfCell(Cell classCell)
    {
        Rotation currentCellRotation = classCell.currentRotation;
        Rotation currentGridRotation = currentRotation;
        int deltaRotation = Math.Abs(currentCellRotation - currentGridRotation);
        CellSideType newCellSideType;
        // Поворот типов сторон ячейки
        if (deltaRotation > 0)
        {
            for (int i = 0; i < 4; i++)
            {
                int newIndex = (i + deltaRotation) % 4;
                newCellSideType = classCell.typeSideOfCell[i];
                classCell.typeSideOfCell[newIndex] = newCellSideType;
            }
        }
        classCell.currentRotation = currentGridRotation;
    }

    public bool IsCellAvailableForMount(Vector2Int pos) // Проверяем стороны на соответствие
    {
        bool[] sides = new bool[4];
        bool[] isSideNone = new bool[4];
        Cell cell = UIcontroller.currentGameObject.GetComponent<Cell>();


        if (pos.y + 1 <= grid.height - 1)
        {
            Cell cellUp = grid.GetCell(new Vector2Int(pos.x, pos.y + 1)).GetComponent<Cell>();
            if (cellUp.typeSideOfCell[2] == cell.typeSideOfCell[0] || cellUp.typeSideOfCell[2] == CellSideType.None)
            {
                sides[0] = true;
                if (cellUp.typeSideOfCell[2] == CellSideType.None)
                {
                    isSideNone[0] = true;
                }
            }
        }
        else
        {
            sides[0] = true;
            isSideNone[0] = true;
        }
        if (pos.x + 1 <= grid.width - 1)
        {
            Cell cellRight = grid.GetCell(new Vector2Int(pos.x + 1, pos.y)).GetComponent<Cell>();
            if (cellRight.typeSideOfCell[3] == cell.typeSideOfCell[1] || cellRight.typeSideOfCell[3] == CellSideType.None)
            {
                sides[1] = true;
                if (cellRight.typeSideOfCell[3] == CellSideType.None)
                {
                    isSideNone[1] = true;
                }
            }
        }
        else
        {
            sides[1] = true;
            isSideNone[1] = true;
        }
        if (pos.y - 1 >= 0)
        {
            Cell cellDown = grid.GetCell(new Vector2Int(pos.x, pos.y - 1)).GetComponent<Cell>();
            if (cellDown.typeSideOfCell[0] == cell.typeSideOfCell[2] || cellDown.typeSideOfCell[0] == CellSideType.None)
            {
                sides[2] = true;
                if (cellDown.typeSideOfCell[0] == CellSideType.None)
                {
                    isSideNone[2] = true;
                }
            }
        }
        else
        {
            sides[2] = true;
            isSideNone[2] = true;
        }
        if (pos.x - 1 >= 0)
        {
            Cell cellLeft = grid.GetCell(new Vector2Int(pos.x - 1, pos.y)).GetComponent<Cell>();
            if (cellLeft.typeSideOfCell[1] == cell.typeSideOfCell[3] || cellLeft.typeSideOfCell[1] == CellSideType.None)
            {
                sides[3] = true;
                if (cellLeft.typeSideOfCell[1] == CellSideType.None)
                {
                    isSideNone[3] = true;
                }
            }
        }
        else
        {
            sides[3] = true;
            isSideNone[3] = true;
        }

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
            
            // Заполнить массив сторон значениями None
            cellPreview.typeSideOfCell = new CellSideType[4]
            {
                CellSideType.None,
                CellSideType.None,
                CellSideType.None,
                CellSideType.None
            };

            RotateTypeSideOfCell(cellPreview); // Поворачиваем для корректной проверки в IsCellAvailableForMount
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

    public void OnRotateCell()
    {
        // Меняем текущую ротацию по часовой стрелке
        currentRotation = (Rotation)(((int)currentRotation + 1) % 4);
        grid.RotateCell(currentMouseEnter);
    }
}
