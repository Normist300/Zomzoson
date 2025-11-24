using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.DebugUI;

public class Cell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, ISubscribable<GridController>
{
    public Vector2Int pos;
    public bool isSckeletons = false;
    public bool isEmptyCell = false;
    public bool isClicked = false;
    public bool isPreview = false;
    public bool isMounted = false; // Удаляет превью при заходе на смонтированную ячейку и обновляет currentMouseEnter
    public bool isAvailableForMount = false;
    public event Action<Vector2Int> eventReplaceCell;
    public event Action<Vector2Int> eventPreview;
    public event Action<Vector2Int> eventDisPreview;
    public event Action<Vector2Int> eventEnterMount;
    public UIController uiController;
    public GridController gridController;
    public GridController.CellSideType[] typesSidesOfCell = new GridController.CellSideType[4]; // 0 - вверх, 1 - право, 2 - вниз, 3 - лево
    public GridController.Rotation currentRotation = GridController.Rotation.up;

    private Renderer[] _renderers;
    private readonly List<Material> _materials = new();

    public void SetValues(Vector2Int newPos)
    {
        pos = newPos;
    }


    void Awake()
    {
        // Берём все Renderer в себе и детях
        //_renderers = GetComponentsInChildren<Renderer>(includeInactive: false);
        //foreach (var r in _renderers)
        //{
        //    // Важно: у одного Renderer может быть несколько материалов
        //    _materials.AddRange(r.materials); // инстансы материалов на каждом объекте
        //}
        uiController = FindFirstObjectByType<UIController>();
        gridController = FindFirstObjectByType<GridController>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Clicked {gameObject.name} with {eventData.button}");
        Debug.Log($"{pos.x}, {pos.y}");
        if (isPreview && isAvailableForMount && eventData.button == PointerEventData.InputButton.Left)
        {
            eventReplaceCell.Invoke(pos);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log($"Enter {pos.x} {pos.y} with {eventData.button}");
        if (uiController.isSelected)
        {

            if (!isPreview && !isMounted)
            {
                eventPreview.Invoke(pos);
            }
            if (isMounted)
            {
                eventEnterMount.Invoke(pos);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log($"Exit {pos.x} {pos.y} with {eventData.button}");
    }

    public void Subscribe(GridController gridController)
    {
        eventReplaceCell += gridController.OnReplaceCell;
        eventPreview += gridController.OnPreview;
        eventEnterMount += gridController.OnEnterMount;
        gridController.subscribedCells.Add(this);
    }

    public void Unsubscribe(GridController gridController)
    {
        eventReplaceCell -= gridController.OnReplaceCell;
        eventPreview -= gridController.OnPreview;
        eventEnterMount -= gridController.OnEnterMount;
        gridController.subscribedCells.Remove(this);
    }
}
