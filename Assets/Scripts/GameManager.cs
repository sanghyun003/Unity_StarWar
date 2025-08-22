using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UnitGenerator[] allBases;

    public void RestartGame()
    {
        // 모든 Base 초기화
        foreach (var baseGen in allBases)
        {
            BaseOwner startOwner = baseGen.Owner; // 필요하면 초기 상태 배열 저장
            int startUnits = 0; // 시작 유닛 수
            baseGen.ResetBase(startOwner, startUnits);
        }
    }

}
