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
    public bool isMounted = false;
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

    public void SetHighlight(bool on)
    {
        if (isClicked) { return; } // Если уже кликнут, то выделять не надо

        if (on)
        {
            foreach (var m in _materials)
            {
                m.EnableKeyword("_EMISSION");
                m.SetColor("_EmissionColor", highlightColor * intensity);
            }
        }
        else
        {
            foreach (var m in _materials)
            {
                m.DisableKeyword("_EMISSION");
            }
        }
    }

    public void SetHighlightClicked(bool on)
    {
        if (on)
        {
            if (isClicked && // Если выделен + включена эмиссия, то выходим
                _materials[0].IsKeywordEnabled("_EMISSION")) { return; }
            foreach (var m in _materials)
            {
                m.EnableKeyword("_EMISSION");
                m.SetColor("_EmissionColor", highlightColorClicked * intensityClicked);
            }
        }
        else
        {
            foreach (var m in _materials)
            {
                m.DisableKeyword("_EMISSION");
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isPreview)
        {
            Debug.Log($"Clicked {gameObject.name} with {eventData.button}");
            Debug.Log($"{pos.x}, {pos.y}");
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
        if (uiController.isSelected)
        {
            
        }
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
