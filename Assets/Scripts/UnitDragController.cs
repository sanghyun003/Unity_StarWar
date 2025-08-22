using UnityEngine;
using System.Collections;

public class UnitDragController : MonoBehaviour
{
    [Header("Line Renderer")]
    [SerializeField] private float lineWidth = 0.2f;
    [SerializeField] private Color lineDefaultColor = Color.white;
    [SerializeField] private Color lineActiveColor = Color.green;

    [Header("Unit Icon")]
    [SerializeField] private Transform[] unitIconPrefab;

    private LineRenderer lineRenderer;
    private Camera mainCamera;
    private bool isDragging = false;
    private UnitGenerator unitGenerator;

    private void Start()
    {
        unitGenerator = GetComponent<UnitGenerator>();
        mainCamera = Camera.main;
        SetupLineRenderer();
    }

    private void SetupLineRenderer()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.enabled = false;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineDefaultColor;
        lineRenderer.endColor = lineDefaultColor;
    }

    private void Update()
    {
        if (Application.isMobilePlatform)
            HandleTouchInput();
        else
            HandleMouseInput();
    }

    #region Mouse Input
    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) TryStartDrag(Input.mousePosition);
        if (isDragging && Input.GetMouseButton(0)) UpdateDrag(Input.mousePosition);
        if (isDragging && Input.GetMouseButtonUp(0)) EndDrag(Input.mousePosition);
    }
    #endregion

    #region Touch Input
    private void HandleTouchInput()
    {
        if (Input.touchCount == 0) return;

        Touch t = Input.GetTouch(0);
        if (t.phase == TouchPhase.Began) TryStartDrag(t.position);
        if (isDragging && (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)) UpdateDrag(t.position);
        if (isDragging && (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)) EndDrag(t.position);
    }
    #endregion

    private void TryStartDrag(Vector2 screenPos)
    {
        Vector2 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        Collider2D hit = Physics2D.OverlapPoint(worldPos);

        if (hit != null && hit.gameObject == gameObject)
        {
            isDragging = true;
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position);
            lineRenderer.startColor = lineActiveColor;
            lineRenderer.endColor = lineActiveColor;
        }
    }

    private void UpdateDrag(Vector2 screenPos)
    {
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        worldPos.z = 0;
        lineRenderer.SetPosition(1, worldPos);
    }

    private void EndDrag(Vector2 screenPos)
    {
        isDragging = false;
        lineRenderer.enabled = false;
        lineRenderer.startColor = lineDefaultColor;
        lineRenderer.endColor = lineDefaultColor;

        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        worldPos.z = 0;

        Collider2D hit = Physics2D.OverlapPoint(worldPos);
        if (hit != null && hit.TryGetComponent<UnitGenerator>(out var targetBase) && targetBase != unitGenerator)
        {
            int unitsToSend = unitGenerator.CurrentUnits;

            if (unitsToSend > 0 && unitGenerator.TrySendUnits(unitsToSend))
            {
                StartCoroutine(SendUnitsRoutine(unitsToSend, targetBase,unitGenerator.Owner));
            }
        }
    }
    private IEnumerator SendUnitsRoutine(int unitsToSend, UnitGenerator targetBase, BaseOwner owner)
    {
        Vector3 startPos = transform.position;
        Vector3 dir = (targetBase.transform.position - startPos).normalized;
        Vector3 spawnPos = startPos + dir * 0.5f;

        float delay = 0.1f; // 유닛 간 출발 간격 (조절 가능)

        for (int i = 0; i < unitsToSend; i++)
        {
            int k = 0;
            if (owner == BaseOwner.Player) k = 0;
            else k = 1;

            Transform unitIcon = Instantiate(unitIconPrefab[k], spawnPos, Quaternion.identity);

            StartCoroutine(UnitIconMover.MoveUnitToTarget(unitIcon, targetBase.transform.position, () =>
            {
                targetBase.ReceiveUnit(unitGenerator.Owner, 1);
                Destroy(unitIcon.gameObject);
            }));
 
            yield return new WaitForSeconds(delay); // 다음 유닛은 조금 늦게 출발
        }
    }
}
