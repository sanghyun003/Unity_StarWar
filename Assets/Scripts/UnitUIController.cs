using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UnitUIController : MonoBehaviour
{
    public TextMeshProUGUI unitText; // 화면에 유닛 수를 표시할 TextMeshPro UI
    public Image baseImage;

    public void UpdateUnitCount(int count)
    {
        if (unitText != null)
            unitText.text = "Units: " + count;
    } 

    public void UpdateBaseColor(BaseOwner owner)
    {
        if (baseImage == null) return;

        switch (owner)
        {
            case BaseOwner.Player:
                baseImage.color = Color.blue;
                break;
            case BaseOwner.Enemy:
                baseImage.color = Color.red;
                break;
            case BaseOwner.Neutral:
                baseImage.color = Color.gray;
                break;
        }
    }
}
