using UnityEngine;
using UnityEngine.Events;

public class Building : MonoBehaviour
{
    [Header("건물 정보")]
    public BuildingType buildingType;
    public string buildingName = "건물";

    [System.Serializable]
    public class BuildingEvents
    {
        public UnityEvent<string> OnDriverEntered;
        public UnityEvent<string> OnDriverExited;
        public UnityEvent<BuildingType> OnServiceUsed;

    }

    public BuildingEvents buildingEvents;

    private DeliveryOrderSystem orderSystem;

    private void Start()
    {
        SetupBuilding();
        orderSystem = FindFirstObjectByType<DeliveryOrderSystem>();
        CreateNameTag();
    }

    void HandleDriverService(DeliveryDriver driver)
    {
        switch (buildingType)
        {
            case BuildingType.Restaurant:
                if (orderSystem != null)
                {
                    orderSystem.OnDriverEnteredRestaurant(this);
                }
                break;

            case BuildingType.Customer:
                if (orderSystem != null)
                {
                    orderSystem.OnDriverEnteredCustomer(this);
                }
                else
                {
                    driver.CompleteDelivery();
                }
                break;

            case BuildingType.ChargingStation:
                Debug.Log($"{buildingName} 에서 배터리를 충전 했습니다.");
                driver.ChargeBattery();
                break;
        }

        buildingEvents.OnServiceUsed?.Invoke(buildingType);
    }

    private void OnTriggerEnter(Collider other)
    {
        DeliveryDriver driver = other.GetComponent<DeliveryDriver>();

        if (driver != null)
        {
            buildingEvents.OnDriverEntered?.Invoke(buildingName);
            HandleDriverService(driver);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        DeliveryDriver driver = other.GetComponent<DeliveryDriver>();

        if (driver != null)
        {
            buildingEvents.OnDriverExited?.Invoke(buildingName);
            Debug.Log($"{buildingName}을 떠났습니다.");
        }
    }

    void SetupBuilding()
    {
        Renderer renderer = GetComponent<Renderer>();

        if (renderer != null)
        {
            Material mat = renderer.material;

            switch (buildingType)
            {
                case BuildingType.Restaurant:
                    mat.color = Color.red;
                    break;

                case BuildingType.Customer:
                    mat.color = Color.green;
                    break;

                case BuildingType.ChargingStation:
                    mat.color = Color.yellow;
                    break;
            }
        }

        Collider col = GetComponent<Collider>();
        if (col != null) { col.isTrigger = true; }
    }

    void CreateNameTag()
    {
        GameObject nameTag = new GameObject("NameTag");
        nameTag.transform.SetParent(transform);
        nameTag.transform.localPosition = Vector3.up * 1.5f;

        TextMesh textMesh = nameTag.AddComponent<TextMesh>();
        textMesh.text = buildingName;
        textMesh.characterSize = .2f;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.color = Color.white;
        textMesh.fontSize = 20;

        nameTag.AddComponent<Billboard>();
    }
}
