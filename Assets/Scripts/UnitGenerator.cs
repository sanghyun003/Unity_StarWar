using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class UnitGenerator : MonoBehaviour
{
    [SerializeField] private int currentUnits = 0; //외부에서 수정불가
    public int CurrentUnits => currentUnits;       //외부에서 읽기 전용
    [SerializeField] private float productionInterval = 0.5f;
    [SerializeField] private int unitsPerInterval = 1;
    private float currentInterval;

    public UnitUIController uiController;

    public BaseOwner Owner = BaseOwner.Neutral;

    private void Start()
    {
        currentInterval = productionInterval;
        UpdateUnitUI();
        StartCoroutine(ProduceUnits());
    }

    IEnumerator ProduceUnits()
    {
        while (gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(currentInterval);
            
            if (Owner != BaseOwner.Neutral)
            {
                currentUnits += unitsPerInterval;
                UpdateUnitUI();
            }
        }
    }

    public bool TrySendUnits(int amount)
    {
        if (currentUnits >= amount)
        {
            currentUnits -= amount;
            UpdateUnitUI();
            return true;
        }
        return false;
    }

    public void AddUnits(int amount)
    {
        currentUnits += amount;
        UpdateUnitUI();
    }

    public void ReceiveUnit(BaseOwner incomingOwner, int amount = 1)
    {
        if (Owner == BaseOwner.Neutral)
        {
            Owner = incomingOwner;
            currentUnits = amount;

            currentInterval = productionInterval * 2;

            UpdateUnitUI();

            return;
        }
        if (Owner == incomingOwner)
        {
            AddUnits(amount);
        }
        else
        {
            currentUnits -= amount;
            if (currentUnits <= 0)
            {
                Owner = incomingOwner;
                currentUnits = 1; // 점령 후 최소 1 유닛
                currentInterval = productionInterval;
            }
            UpdateUnitUI();
        }
    }

    private void UpdateUnitUI()
    {
        uiController?.UpdateUnitCount(currentUnits);
        uiController?.UpdateBaseColor(Owner);
    }
}
public enum BaseOwner
{
    Player,
    Enemy,
    Neutral
}
