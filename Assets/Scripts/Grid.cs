using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Grid : MonoBehaviour
{
    public event Action eventGridReady;

    // Размер сетки
    public int width = 10;
    public int height = 10;
    public int indentX = 10;
    public int indentZ = 10;

    // Размер ячеек
    public float cellSize = 1f;

    // Префаб для клеток (например, куб)
    public GameObject cellSpace;
    public GameObject cellFirst;
    public GridController gridController;

    // Вложенный словарь для хранения объектов сетки
    private Dictionary<int, Dictionary<int, GameObject>> gridCells = new Dictionary<int, Dictionary<int, GameObject>>();

    private Vector3 objectPosition;

    void Start()
    {
        GenerateGrid();
        eventGridReady.Invoke();
        GameObject goCellFirst = InstanForMount(cellFirst);
        ReplaceCell(new Vector2Int(3, 3), goCellFirst);
    }

    public GameObject InstanForMount(GameObject go)
    {
        GameObject instanGo = Instantiate(go);
        Cell cellInstanGo = instanGo.GetComponent<Cell>();
        cellInstanGo.isMounted = true;
        return instanGo;
    }

    // Генерация сетки
    void GenerateGrid()
    {
        Vector3 objectPosition = transform.position;
        for (int x = 0; x < width; x++)
        {
            gridCells[x] = new Dictionary<int, GameObject>();

            for (int z = 0; z < height; z++)
            {
                // Вычисление позиции для каждой ячейки
                Vector3 position = new Vector3(x * cellSize + objectPosition.x + indentX, objectPosition.y,
                                               z * cellSize + objectPosition.z + indentZ);

                // Создание ячейки
                GameObject cell = Instantiate(cellSpace, position, Quaternion.identity, transform);
                Cell classCell = cell.GetComponent<Cell>();
                if (classCell != null)
                {
                    classCell.SetValues(new Vector2Int(x, z));
                }
                else
                {
                    Debug.Log("Не найден класс Cell");
                }
                // Сохраняем ссылку на созданный объект в словарь
                gridCells[x][z] = cell;
            }
        }

        // Пример вывода в консоль всех созданных объектов
        Debug.Log("Created " + gridCells.Count + " rows of grid cells.");
    }

    // Пример метода для получения объекта по координатам
    public GameObject GetCell(Vector2Int pos)
    {
        if (gridCells.ContainsKey(pos.x) && gridCells[pos.x].ContainsKey(pos.y))
        {
            return gridCells[pos.x][pos.y];
        }
        else
        {
            return null; // Если ячейка не найдена
        }
    }

    public void AddCell (Vector2Int pos, GameObject newGoCell)
    {
        if (GetCell(pos) == null)
        {
            Vector3 objectPosition = transform.position;
            Vector3 position = new Vector3(pos.x * cellSize + objectPosition.x + indentX, objectPosition.y,
                                           pos.y * cellSize + objectPosition.z + indentZ);
            if (newGoCell.scene.IsValid())
            {
                newGoCell.transform.position = position;
                gridCells[pos.x][pos.y] = newGoCell;
                Cell newGoClassCell = newGoCell.gameObject.GetComponent<Cell>();
                newGoClassCell.Subscribe(gridController);
                newGoClassCell.SetValues(pos);
                newGoCell.transform.SetParent(transform);
                Debug.Log($"Создана ячейка в ({pos}), {newGoClassCell.isPreview}");
            }
            else
            {
                GameObject goCell = Instantiate(newGoCell, position, Quaternion.identity, transform);
                gridCells[pos.x][pos.y] = goCell;
                Cell classCell = goCell.GetComponent<Cell>();
                classCell.Subscribe(gridController);
                classCell.SetValues(pos);
                Debug.Log($"Создана ячейка в ({pos}), {classCell.isPreview}");
            }
            
        }
        else
        {
            Debug.Log($"Место для ячейки ({pos}) занято");
        }
    }

    // Пример метода для удаления всех ячеек
    public void DestroyAllCells()
    {
        foreach (var row in gridCells)
        {
            foreach (var cell in row.Value)
            {
                Destroy(cell.Value); // Удаление ячейки
            }
        }

        gridCells.Clear(); // Очищаем словарь после удаления
        Debug.Log("All cells destroyed.");
    }

    public void DestroyCell(Vector2Int pos)
    {
        Destroy(GetCell(pos));
        gridCells[pos.x].Remove(pos.y);
    }

    public void ReplaceCell(Vector2Int pos, GameObject newCell)
    {
        if (gridCells.ContainsKey(pos.x) && gridCells[pos.x].ContainsKey(pos.y))
        {
            DestroyCell(pos);
            AddCell(pos, newCell);
        }
        else
        {
            Debug.Log("Не найдена ячейка"); // Если ячейка не найдена
        }
    }
}
