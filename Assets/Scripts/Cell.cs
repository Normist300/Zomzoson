using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.DebugUI;

public class Cell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, ISubscribable<GridController>
{
    public int xPos;
    public int yPos;
    public Vector2Int pos;
    public bool isEmptyCell = false;
    public bool isClicked = false;
    public bool isPreview = false;
    public bool isMounted = false; // Удаляет превью при заходе на смонтированную ячейку и обновляет currentMouseEnter
    public bool isAvailableForMount = false;
    [SerializeField] private Color highlightColor = Color.green;
    [SerializeField] private float intensity = 2.0f;
    [SerializeField] private Color highlightColorClicked = Color.green;
    [SerializeField] private float intensityClicked = 2.0f;
    public event Action<Vector2Int> eventReplaceCell;
    public event Action<Vector2Int> eventPreview;
    public event Action<Vector2Int> eventDisPreview;
    public event Action<Vector2Int> eventEnterMount;
    public UIController uiController;
    public GridController gridController;
    public GridController.CellSideType[] cellSideType = new GridController.CellSideType[4]; // 0 - вверх, 1 - право, 2 - вниз, 3 - лево

    private Renderer[] _renderers;
    private readonly List<Material> _materials = new();

    public void SetValues(Vector2Int newPos)
    {
        pos = newPos;
    }


    void Awake()
    {
        // Берём все Renderer в себе и детях
        _renderers = GetComponentsInChildren<Renderer>(includeInactive: false);
        foreach (var r in _renderers)
        {
            // Важно: у одного Renderer может быть несколько материалов
            _materials.AddRange(r.materials); // инстансы материалов на каждом объекте
        }
        uiController = FindFirstObjectByType<UIController>();
        gridController = FindFirstObjectByType<GridController>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Clicked {gameObject.name} with {eventData.button}");
        Debug.Log($"{pos.x}, {pos.y}");
        if (isPreview && eventData.button == PointerEventData.InputButton.Left)
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
        Debug.Log($"Exit {pos.x} {pos.y} with {eventData.button}");
    }

    public void Subscribe(GridController gridController)
    {
        eventReplaceCell += gridController.onReplaceCell;
        eventPreview += gridController.onPreview;
        eventDisPreview += gridController.onDisPreview;
        eventEnterMount += gridController.onEnterMount;
        gridController.subscribedCells.Add(this);
    }

    public void Unsubscribe(GridController gridController)
    {
        eventReplaceCell -= gridController.onReplaceCell;
        eventPreview -= gridController.onPreview;
        eventDisPreview -= gridController.onDisPreview;
        eventEnterMount -= gridController.onEnterMount;
        gridController.subscribedCells.Remove(this);
    }
}
