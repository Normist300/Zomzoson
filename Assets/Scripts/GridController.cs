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
            grid.ReplaceCell(pos.x, pos.y, goForFunc);
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
            grid.ReplaceCell(pos.x, pos.y, pasteObject);

            if (currentMouseEnter != new Vector2Int(-1, -1))
            {
                onDisPreview(currentMouseEnter);
                currentMouseEnter = new Vector2Int(pos.x, pos.y);
            }
            else
            {
                currentMouseEnter = new Vector2Int(pos.x, pos.y);
            }
        }
        else Debug.LogWarning("UIcontroller.currentGameObject == null");
    }

    public void onDisPreview(Vector2Int pos)
    {
         if (UIcontroller.currentGameObject != null)
        {
            grid.ReplaceCell(pos.x, pos.y, grid.cellSpace);
        }
        else Debug.LogWarning("UIcontroller.currentGameObject == null");
    }

    public void onEnterMount(Vector2Int pos)
    {
        //if (currentMouseEnter != new Vector2Int(-1, -1))
        //{
        //    GameObject pastCell = grid.GetCell(x, y)
        //    if 
        //    onDisPreview(currentMouseEnter.x, currentMouseEnter.y);
        //    currentMouseEnter = new Vector2Int(x, y);
        //}
        //else
        //{
        //    currentMouseEnter = new Vector2Int(x, y);
        //}
    }
}
