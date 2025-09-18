using UnityEngine;

public class GridGizmo : MonoBehaviour
{
    // Размер сетки
    public int width = 10;
    public int height = 10;

    // Размер ячеек
    public float cellSize = 1f;

    void OnDrawGizmos()
    {
        // Получаем позицию объекта, к которому прикреплён скрипт
        Vector3 objectPosition = transform.position;

        // Устанавливаем цвет гизмос для сетки
        Gizmos.color = Color.green;

        // Рисуем горизонтальные линии
        for (int x = 0; x <= width; x++)
        {
            // Локальная позиция в сетке относительно объекта
            Vector3 start = objectPosition + new Vector3(x * cellSize, 0, 0);
            Vector3 end = objectPosition + new Vector3(x * cellSize, 0, height * cellSize);
            Gizmos.DrawLine(start, end);
        }

        // Рисуем вертикальные линии
        for (int z = 0; z <= height; z++)
        {
            // Локальная позиция в сетке относительно объекта
            Vector3 start = objectPosition + new Vector3(0, 0, z * cellSize);
            Vector3 end = objectPosition + new Vector3(width * cellSize, 0, z * cellSize);
            Gizmos.DrawLine(start, end);
        }

        // Отображаем размеры сетки как текст
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(objectPosition + new Vector3((width * cellSize) / 2, 0, (height * cellSize) / 2), new Vector3(width * cellSize, 0, height * cellSize));
    }
}