using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using EasyButtons;

public class Click : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Color highlightColor = Color.green;
    [SerializeField] private float intensity = 2.0f;
    [SerializeField] private Color highlightColorClicked = Color.green;
    [SerializeField] private float intensityClicked = 2.0f;

    private Renderer[] _renderers;
    private readonly List<Material> _materials = new();


    void Awake()
    {
        // Берём все Renderer в себе и детях
        _renderers = GetComponentsInChildren<Renderer>(includeInactive: false);
        foreach (var r in _renderers)
        {
            // Важно: у одного Renderer может быть несколько материалов
            _materials.AddRange(r.materials); // инстансы материалов на каждом объекте
        }
    }

    public void SetHighlight(bool on)
    {
        if (highlightColorClicked * intensityClicked == _materials[0].GetColor("_EmissionColor")) { return; } // Если цвет == как выбранный, то выходим

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
            if (highlightColorClicked * intensityClicked == _materials[0].GetColor("_EmissionColor")) { return; } // Выходим, если цвет уже выделен
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
        Debug.Log($"Clicked {gameObject.name} with {eventData.button}");
        var r = GetComponent<Renderer>();
        SetHighlightClicked(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var r = GetComponent<Renderer>();
        Debug.Log($"Enter {gameObject.name} with {eventData.button}");
        SetHighlight(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var r = GetComponent<Renderer>();
        Debug.Log($"Exit {gameObject.name} with {eventData.button}");
        SetHighlight(false);
    }
}
