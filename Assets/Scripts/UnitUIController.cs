using UnityEngine;
using TMPro;
using System.Collections;

public class UnitUIController : MonoBehaviour
{
    public TextMeshProUGUI unitText;   // 유닛 수 표시
    public SpriteRenderer baseSprite;  // 기지 색 바꾸는 SpriteRenderer

    private Color neutralColor;        // 중립 시작 색 저장


    private void Start()
    {
        SetNeutralColor();
    }

    public void UpdateUnitCount(int count)
    {
        if (unitText != null)
            unitText.text = $"Units: {count}";
    }

    // 중립 상태일 때 색 복원
    public void SetNeutralColor()
    {
        if (baseSprite != null && baseSprite.sprite != null)
        {
            baseSprite.enabled = true;  // SpriteRenderer 켜기
            Color color = neutralColor;
            color.a = 1f;
            color = Color.white;
            baseSprite.color = color;
        }
    }

    // 주인 색으로 변경
    public void SetOwnerColor(Color ownerColor)
    {
        if (baseSprite != null)
        {
            ownerColor.a = 1f;
            baseSprite.enabled = true;
            baseSprite.color = ownerColor;
        }
    }
}
