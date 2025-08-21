using System.Collections;
using UnityEngine;

public static class UnitIconMover
{
    [SerializeField] private static float moveSpeed = 5f;
    [SerializeField] private static float arrivalThreshold = 0.05f;

    public static IEnumerator MoveUnitToTarget(Transform unitImage, Vector3 targetPos, System.Action onArrive)
    {
        Vector3 direction = (targetPos - unitImage.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        unitImage.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        while (Vector3.Distance(unitImage.position, targetPos) > arrivalThreshold)
        {
            unitImage.position = Vector3.MoveTowards(unitImage.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        unitImage.position = targetPos;
        onArrive?.Invoke();
    }
}
