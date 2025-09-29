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

    // Вложенный словарь для хранения объектов сетки
    private Dictionary<int, Dictionary<int, GameObject>> gridCells = new Dictionary<int, Dictionary<int, GameObject>>();

    private Vector3 objectPosition;

    void Start()
    {
        GenerateGrid();
        eventGridReady.Invoke();
        ReplaceCell(3, 3, cellFirst);
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
                    classCell.SetValues(x, z);
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
    public GameObject GetCell(int x, int y)
    {
        if (gridCells.ContainsKey(x) && gridCells[x].ContainsKey(y))
        {
            return gridCells[x][y];
        }
        else
        {
            return null; // Если ячейка не найдена
        }
    }

    public void AddCell (int x, int y, GameObject newCell)
    {
        if (GetCell(x, y) != null || GetCell(x, y) != cellSpace)
        {
            Vector3 objectPosition = transform.position;
            Vector3 position = new Vector3(x * cellSize + objectPosition.x + indentX, objectPosition.y,
                                           y * cellSize + objectPosition.z + indentZ);
            GameObject cell = Instantiate(newCell, position, Quaternion.identity, transform);
        }
        else
        {
            Debug.Log("Место для ячейки занято");
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

    public void DestroyCell(int x, int y)
    {
        Destroy(GetCell(x, y));
    }

    public void ReplaceCell(int x, int y, GameObject newCell)
    {
        if (gridCells.ContainsKey(x) && gridCells[x].ContainsKey(y))
        {
            DestroyCell(x, y);
            AddCell(x, y, newCell);
        }
        else
        {
            Debug.Log("Не найдена ячейка"); // Если ячейка не найдена
        }
    }
}
