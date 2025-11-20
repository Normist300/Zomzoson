using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class WASDGod : MonoBehaviour, ISubscribable<GridController>
{
    public float moveSpeed = 5f;  // Скорость перемещения
    public float scrollSpeed = 2f; // Скорость перемещения по оси Y (вверх/вниз) при прокрутке колесика мыши

    // Пределы движения по вертикали
    public float minY = 20f;  // Минимальное значение для оси Y
    public float maxY = 100f; // Максимальное значение для оси Y
    public float minYVelocity = 0.2f;
    public float maxYVelocity = 1f;

    public float currentScroll = 0f;
    public float smoothScroll = 10f;

    private InputAction rotateCell;

    public event Action eventRotateCell;

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }

    private void Awake()
    {
        rotateCell = InputSystem.actions.FindAction("RotateCell");
    }

    void Update()
    {
        // Получаем ввод от пользователя для перемещения по осям X и Z
        float moveHorizontal = Input.GetAxis("Horizontal");  // A/D или стрелки влево/вправ (клавиша A = -1, D = 1)
        float moveVertical = Input.GetAxis("Vertical");  // W/S или стрелки вверх/вниз (клавиша W = 1, S = -1)

        // Получаем ввод от колесика мыши для перемещения по оси Y (вверх/вниз)
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        // Плавное сглаживание
        currentScroll = Mathf.Lerp(currentScroll, scrollInput, Time.deltaTime * smoothScroll);


        // Рассчитываем движение по осям X, Y и Z
        float yVelocity = Mathf.InverseLerp(minY, maxY, transform.position.y); // Нормализуем сначала координатно
        yVelocity = Mathf.Lerp(minYVelocity, maxYVelocity, yVelocity); // А потом до нужного коэфицента

        float yMovement = -currentScroll * scrollSpeed * yVelocity;

        Vector3 movement = new Vector3(moveHorizontal * moveSpeed * yVelocity,
                                       yMovement,
                                       moveVertical * moveSpeed * yVelocity) * Time.deltaTime;

        // Получаем текущую позицию объекта
        Vector3 newPosition = transform.position + movement;

        // Ограничиваем движение по оси Y
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        // Применяем новую позицию
        transform.position = newPosition;
        
        
        if (rotateCell.WasPerformedThisFrame())
        {
            Debug.Log("RotateCell action performed");
            eventRotateCell.Invoke();
        }
    }

    public void Subscribe(GridController gridController)
    {
        eventRotateCell += gridController.OnRotateCell;
    }

    public void Unsubscribe(GridController gridController)
    {
        eventRotateCell -= gridController.OnRotateCell;
    }
}
