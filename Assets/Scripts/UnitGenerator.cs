using System.Collections;
using UnityEngine;

public class UnitGenerator : MonoBehaviour
{
    [SerializeField] private int currentUnits = 0;
    public int CurrentUnits => currentUnits;

    [SerializeField] private float productionInterval = 0.5f;
    [SerializeField] private int unitsPerInterval = 1;
    private float currentInterval;

    public UnitUIController uiController;

    public BaseOwner Owner = BaseOwner.Neutral;

    [Header("Owner Colors (set in Inspector)")]
    public Color playerColor;
    public Color enemyColor;

    private void Start()
    {
        currentInterval = productionInterval;
        UpdateUnitUI();
        StartCoroutine(ProduceUnits());
    }

    IEnumerator ProduceUnits()
    {
        while (enabled)
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
        Color ownerColor = incomingOwner == BaseOwner.Player ? playerColor : enemyColor;
        if (Owner == BaseOwner.Neutral)
        {
            Owner = incomingOwner;
            currentUnits = amount;

            // 중립 점령 시 주인 색으로 변경
            if (uiController != null)
            {
                uiController.SetOwnerColor(ownerColor);
            }

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
                currentUnits = 1;
                currentInterval = productionInterval *2;
                uiController.SetOwnerColor(ownerColor);
            }
            UpdateUnitUI();
        }
    }

    private void UpdateUnitUI()
    {
        uiController?.UpdateUnitCount(currentUnits);

        if (Owner == BaseOwner.Neutral) uiController?.SetNeutralColor();
    }
    
    public void ResetBase(BaseOwner initialOwner, int initialUnits)
    {
        Owner = initialOwner;
        currentUnits = initialUnits;

        // 생산 간격 초기화
        currentInterval = productionInterval;

        // UI 초기화
        if (uiController != null)
        {
            if (Owner == BaseOwner.Neutral)
                uiController.SetNeutralColor();
            else if (Owner == BaseOwner.Player)
                uiController.SetOwnerColor(playerColor);
            else if (Owner == BaseOwner.Enemy)
                uiController.SetOwnerColor(enemyColor);

            uiController.UpdateUnitCount(currentUnits);
        }
    }

}

public enum BaseOwner
{
    Player,
    Enemy,
    Neutral
}
