using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CameraFollowScript : NetworkBehaviour
{
    [SerializeField] private Transform targetTransform; // Цель (игрок)
    [SerializeField] private float movingSpeed = 5f;    // Скорость перемещения камеры

    private Camera mainCamera; // Ссылка на основную камеру

    void Start()
    {
        // Проверяем, локальный ли это игрок
        if (!isLocalPlayer) return;

        // Получаем основную камеру
        mainCamera = Camera.main;

        // Устанавливаем начальную позицию камеры на игрока
        if (mainCamera != null && targetTransform != null)
        {
            mainCamera.transform.position = new Vector3(
                targetTransform.position.x,
                targetTransform.position.y,
                targetTransform.position.z - 10
            );

            // Привязываем камеру к этому скрипту (если не хотим использовать FixedUpdate)
            mainCamera.transform.SetParent(null); // Отсоединяем от иерархии игрока
        }
    }

    void Update()
    {
        // Следим только за локальным игроком
        if (!isLocalPlayer || targetTransform == null || mainCamera == null) return;

        // Новая позиция для камеры
        Vector3 target = new Vector3(
            targetTransform.position.x,
            targetTransform.position.y,
            targetTransform.position.z - 10
        );

        // Плавное движение камеры к цели
        Vector3 pos = Vector3.Lerp(mainCamera.transform.position, target, movingSpeed * Time.deltaTime);

        // Применяем новую позицию
        mainCamera.transform.position = pos;
    }
}