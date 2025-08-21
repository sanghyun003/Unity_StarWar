using System.Collections;
using UnityEngine;

public class UnitGenerator : MonoBehaviour
{
    [SerializeField] private int currentUnits = 0;
    [SerializeField] private float productionInterval = 0.5f;
    [SerializeField] private int unitsPerInterval = 1;

    public UnitUIController uiController;

    private void Start()
    {
        UpdateUnitUI();
        StartCoroutine(ProduceUnits());
    }

    IEnumerator ProduceUnits()
    {
        while (gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(productionInterval);
            currentUnits += unitsPerInterval;
            UpdateUnitUI();
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

    private void UpdateUnitUI()
    {
        uiController?.UpdateUnitCount(currentUnits);
    }
}
