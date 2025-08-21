using UnityEngine;
using TMPro;

public class UnitUIController : MonoBehaviour
{
    public TextMeshProUGUI unitText; // 화면에 유닛 수를 표시할 TextMeshPro UI

    public void UpdateUnitCount(int count)
    {
        if(unitText != null)
            unitText.text = "Units: " + count;
    } 
}
