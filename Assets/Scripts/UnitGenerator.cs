using System.Collections;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class UnitGenerator : MonoBehaviour
{
    public TextMeshProUGUI unitText; // 화면에 유닛 수를 표시할 TextMeshPro UI
    [SerializeField] private int currentUnits = 0; // 현재 보유 중인 유닛 수
    [SerializeField] private float productionInterval = 0.5f; // 유닛 생산 주기 (초)
    [SerializeField] private int unitsPerInterval = 1; // 주기마다 생산되는 유닛 수

    [SerializeField] private float lineWidth = 0.2f;
    [SerializeField] private Color lineDefaultColor = Color.white;
    [SerializeField] private Color lineActiveColor = Color.green;

    [SerializeField] private Transform unitIconPrefab; //날아갈 유닛 아이콘

    private LineRenderer lineRenderer; // 드래그 시 표시할 선
    private Camera mainCamera; // 메인 카메라 참조
    private bool isDragging = false;

    void Start()
    {
        UpdateUnitText(); // 시작 시 UI 초기화
        StartCoroutine(ProduceUnits()); // 유닛 생산 코루틴 실행

        mainCamera = Camera.main; // 메인 카메라 캐싱
        SetupLineRenderer();

    }

    private void SetupLineRenderer()
    {
        
        // === LineRenderer 설정 ===
        lineRenderer = gameObject.AddComponent<LineRenderer>(); // 컴포넌트 동적 추가
        lineRenderer.positionCount = 2; // 시작점과 끝점
        lineRenderer.startWidth = lineWidth; // 선 시작 두께
        lineRenderer.endWidth = lineWidth;   // 선 끝 두께
        lineRenderer.enabled = false;   // 처음에는 보이지 않도록 설정

        // 선이 보이도록 기본 머티리얼과 색상 지정
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineDefaultColor;
        lineRenderer.endColor = lineDefaultColor;
    }

    IEnumerator MoveUnitToTarget(Transform unitImage, Vector3 targetPos, System.Action onArrive)
    {
        Debug.Log("이동 시작");
        float speed = 5f;

        // 이동 방향 계산
        Vector3 direction = (targetPos - unitImage.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 2D 회전 적용 (Z축 기준으로만 회전)
        unitImage.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        while (Vector3.Distance(unitImage.position, targetPos) > 0.05f)
        {
            unitImage.position = Vector3.MoveTowards(unitImage.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }
        
        Debug.Log("도착");
        unitImage.position = targetPos;
        onArrive?.Invoke();
    }
    /// 일정 시간마다 유닛을 생산하는 코루틴
    IEnumerator ProduceUnits()
    {
        while (true)
        {
            yield return new WaitForSeconds(productionInterval); // 주기 대기
            currentUnits += unitsPerInterval; // 유닛 추가
            UpdateUnitText(); // UI 갱신
        }
    }

    /// 화면의 유닛 수 UI를 갱신
    void UpdateUnitText()
    {
        if (unitText != null)
        {
            unitText.text = "Units: " + currentUnits;
        }
        else
        {
            unitText.text = "0";
        }
    }

    /// 다른 기지에서 유닛을 받을 때 호출
    public void AddUnits(int amount)
    {
        currentUnits += amount;
        UpdateUnitText();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            TryStartDrag(Input.mousePosition);

        if (isDragging && Input.GetMouseButton(0))
            UpdateDrag(Input.mousePosition);

        if (isDragging && Input.GetMouseButtonUp(0))
            EndDrag(Input.mousePosition);
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
                TryStartDrag(t.position);

            if (isDragging && (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary))
                UpdateDrag(t.position);

            if (isDragging && (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled))
                EndDrag(t.position);
        }
    }

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
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(screenPos);
        mouseWorldPos.z = 0;
        lineRenderer.SetPosition(1, mouseWorldPos);
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

        if (hit != null)
        {
            UnitGenerator targetBase = hit.GetComponent<UnitGenerator>();
            if (targetBase != null && targetBase != this)
            {
                int unitsToSend = 1;
                    if (unitsToSend > 0 && currentUnits >= unitsToSend)
                {
                    currentUnits -= unitsToSend;
                    UpdateUnitText();

                    // 유닛 아이콘 생성
                    Transform unitIcon = Instantiate(unitIconPrefab, transform.position, Quaternion.identity);

                    // 유닛 아이콘을 목표까지 부드럽게 이동시키고 도착 시 유닛 추가
                    StartCoroutine(MoveUnitToTarget(unitIcon, targetBase.transform.position, () =>
                    {
                        targetBase.AddUnits(unitsToSend);
                        Destroy(unitIcon.gameObject);
                    }));
                }
            }
        }
    }
}
