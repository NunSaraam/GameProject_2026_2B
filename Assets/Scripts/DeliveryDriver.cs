using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DeliveryDriver : MonoBehaviour
{
    [Header("배달원 설정")]
    public float moveSpeed = 8f;
    public float rotationSpeed = 10.0f;

    [Header("상태")]
    public float currentMoney = 0;
    public float batteryLevel = 100f;
    public int deliveryCount = 0;

    [System.Serializable]
    public class DriverEvents
    {
        [Header("이동 Event")]
        public UnityEvent OnMoveStarted;
        public UnityEvent OnMoveStoped;

        [Header("상태 변화 Event")]
        public UnityEvent<float> OnMoneyChanged;
        public UnityEvent<float> OnLowBatteryChanged;
        public UnityEvent<int> OnDeliveryCountChanged;

        [Header("경고 Event")]
        public UnityEvent OnLowBattery;
        public UnityEvent OnLowBatteryEmpty;
        public UnityEvent OnDeliveryCompleted;
    }

    public DriverEvents driverEvents;

    public bool isMoving = false;

    private void Start()
    {
        driverEvents.OnMoneyChanged?.Invoke(currentMoney);
        driverEvents.OnLowBatteryChanged?.Invoke(batteryLevel);
        driverEvents.OnDeliveryCountChanged?.Invoke(deliveryCount);
    }

    private void Update()
    {
        HandleMovement();
    }


    void ChangeBattery(float amount)
    {
        float oldBattery = batteryLevel;
        batteryLevel += amount;
        batteryLevel = Mathf.Clamp(batteryLevel, 0, 100);

        driverEvents.OnLowBatteryChanged?.Invoke(batteryLevel);

        if (oldBattery > 20f && batteryLevel <= 20f)
        {
            driverEvents.OnLowBattery?.Invoke();
        }
        if (oldBattery > 0 && batteryLevel <= 0)
        {
            driverEvents.OnLowBatteryEmpty?.Invoke();
        }
    }

    void HandleMovement()
    {
        if (batteryLevel <= 0)
        {
            if (isMoving)
            {
                StopMoving();
            }
            return;
        }

        Vector2 input = Keyboard.current != null ? new Vector2(
            (Keyboard.current.dKey.isPressed ? 1 : 0) -
            (Keyboard.current.aKey.isPressed ? 1 : 0),
            (Keyboard.current.wKey.isPressed ? 1 : 0) -
            (Keyboard.current.sKey.isPressed ? 1 : 0)
            ) : Vector2.zero;

        Vector3 moveDirction = new Vector3(input.x, 0f, input.y);

        if (moveDirction.magnitude > .1f)
        {
            if (!isMoving)
            {
                StartMoving();
            }

            moveDirction = moveDirction.normalized;
            transform.Translate(moveDirction * moveSpeed * Time.deltaTime, Space.World);


            if (moveDirction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            ChangeBattery(-Time.deltaTime * 3.0f);
        }
        else
        {
            if (isMoving)
            {
                StopMoving();
            }
        }
    }

    void StartMoving()
    {
        isMoving = true;
        driverEvents.OnMoveStarted?.Invoke();
    }

    void StopMoving()
    {
        isMoving = false;
        driverEvents.OnMoveStoped?.Invoke();
    }

    public void AddMoney(float amount)
    {
        currentMoney += amount;
        driverEvents.OnMoneyChanged?.Invoke(currentMoney);
    }

    public void CompleteDelivery()
    {
        deliveryCount++;
        float reward = Random.Range(3000, 8000);
        AddMoney(reward);
        driverEvents.OnDeliveryCountChanged?.Invoke(deliveryCount);
        driverEvents.OnDeliveryCompleted?.Invoke();
    }

    public void ChargeBattery()
    {
        ChangeBattery(100f - batteryLevel);
    }

    public string GetStatusText()
    {
        return $"돈 : {currentMoney:F0} 원 | 배터리 : {batteryLevel:F1}% | 배달 : {deliveryCount} 건";
    }

    public bool CanMove()
    {
        return batteryLevel > 0;
    }
}
