
using UnityEngine; // Unity에서 제공하는 기능을 사용하기 위해 필요

// 이 스크립트를 추가한 오브젝트에 Rigidbody2D가 없으면 자동으로 추가
[RequireComponent(typeof(Rigidbody2D))]

// 이 스크립트를 추가한 오브젝트에 EnemyHealth가 없으면 자동으로 추가
[RequireComponent(typeof(EnemyHealth))]

// 슬라임 보스의 이동과 페이즈 전환을 관리하는 클래스
public class BossSlimeController : MonoBehaviour
{
    // ===== 1. 플레이어 설정 =====

    [Header("Target")] // Inspector에 Target이라는 제목 표시

    // 슬라임이 추적할 플레이어의 위치 정보
    [SerializeField] private Transform player;


    // ===== 2. 이동 설정 =====

    [Header("Movement")] // Inspector에 Movement 제목 표시

    // 슬라임의 기본 이동속도 (초당 2유닛)
    [SerializeField] private float moveSpeed = 2f;

    // 플레이어와 이 거리 이하로 가까워지면 이동을 멈춤
    [SerializeField] private float stopDistance = 0.1f;


    // ===== 3. 2페이즈 설정 =====

    [Header("Phase 2")] // Inspector에 Phase 2 제목 표시

    // 슬라임의 체력이 150 이하가 되면 2페이즈 시작
    [SerializeField] private int phase2Health = 150;

    // 2페이즈에서 사용하는 이동속도 (초당 3유닛)
    [SerializeField] private float phase2Speed = 3f;


    // ===== 4. 내부에서 사용할 변수 =====

    // 슬라임의 물리 이동을 제어하는 컴포넌트를 저장
    private Rigidbody2D rb;

    // 슬라임의 현재 체력을 확인하기 위한 컴포넌트를 저장
    private EnemyHealth health;

    // 2페이즈가 시작되었는지 확인하는 변수
    // false = 1페이즈, true = 2페이즈
    private bool isPhase2;


    // ===== 5. Awake 함수 =====

    // 오브젝트가 초기화될 때 한 번 실행되는 함수
    private void Awake()
    {
        // 슬라임에 부착된 Rigidbody2D 컴포넌트를 가져옴
        rb = GetComponent<Rigidbody2D>();

        // 슬라임에 부착된 EnemyHealth 컴포넌트를 가져옴
        health = GetComponent<EnemyHealth>();
    }


    // ===== 6. Start 함수 =====

    // 스크립트가 활성화된 후 첫 프레임의 Update 전에 실행
    private void Start()
    {
        // Inspector에서 플레이어를 직접 지정하지 않았다면
        if (player == null)
        {
            // Player 태그가 붙어 있는 오브젝트를 검색
            GameObject playerObject =
                GameObject.FindWithTag("Player");

            // Player 태그를 가진 오브젝트를 찾았다면
            if (playerObject != null)
            {
                // 해당 오브젝트의 Transform을 player에 저장
                player = playerObject.transform;
            }
        }
    }


    // ===== 7. Update 함수 =====

    // 게임이 실행되는 동안 매 프레임 호출되는 함수
    private void Update()
    {
        // 아래 조건을 모두 만족하는지 확인
        // 1. 아직 2페이즈가 아님
        // 2. 슬라임이 살아 있음 (체력이 0보다 큼)
        // 3. 체력이 150 이하로 감소함
        if (!isPhase2 &&
            health.CurrentHealth > 0 &&
            health.CurrentHealth <= phase2Health)
        {
            // 2페이즈 상태를 true로 변경
            isPhase2 = true;

            // Unity Console에 2페이즈 시작 메시지 출력
            Debug.Log("슬라임 보스 2페이즈 시작!");
        }
    }


    // ===== 8. FixedUpdate 함수 =====

    // 물리 연산 주기에 맞춰 반복 실행되는 함수
    // Rigidbody2D 이동은 여기에서 처리하는 것이 적절함
    private void FixedUpdate()
    {
        // 플레이어가 없거나 슬라임이 죽었다면
        if (player == null || health.CurrentHealth <= 0)
        {
            // 아래 이동 코드를 실행하지 않고 종료
            return;
        }

        // 슬라임 자신의 현재 위치를 가져옴
        Vector2 currentPosition = rb.position;

        // 플레이어의 현재 위치를 가져옴
        Vector2 targetPosition = player.position;

        // 슬라임과 플레이어 사이의 거리를 계산
        float distance = Vector2.Distance(
            currentPosition, // 슬라임 위치
            targetPosition   // 플레이어 위치
        );

        // 플레이어가 정지 거리 이내에 있다면
        if (distance <= stopDistance)
        {
            // 더 이상 플레이어에게 접근하지 않음
            return;
        }

        // 플레이어가 있는 방향을 계산
        // (목표 위치 - 현재 위치) = 플레이어 방향 벡터
        // normalized = 방향은 유지하고 길이는 1로 만듦
        Vector2 direction =
            (targetPosition - currentPosition).normalized;

        // 2페이즈라면 phase2Speed 사용
        // 1페이즈라면 moveSpeed 사용
        float speed = isPhase2 ? phase2Speed : moveSpeed;

        // 현재 위치에서 목표 위치를 향해 이동할 다음 위치 계산
        Vector2 nextPosition = Vector2.MoveTowards(
            currentPosition,             // 슬라임 현재 위치
            targetPosition,              // 플레이어 목표 위치
            speed * Time.fixedDeltaTime   // 이번 물리 프레임의 이동 거리
        );

        // Rigidbody2D를 이용해 계산한 위치로 이동
        rb.MovePosition(nextPosition);
    }
}
