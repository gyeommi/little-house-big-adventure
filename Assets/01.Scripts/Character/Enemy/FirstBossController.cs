
using System.Collections;
using UnityEngine;

public class FirstBossController : BossController
{
    public enum AttackType
    {
        BookRain,
        Charge,
        Sweep
    }

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 8f;
    [SerializeField] private Transform player;
    [SerializeField] private float chargeRange = 100f;

    [Header("Book Rain")]
    [SerializeField] private BookTrapManager bookTrapManager;

    [Header("Boss Animation")]
    [SerializeField] private BossAnimator bossAnimator;

    [Header("Charge Attack")]
    [SerializeField] private float chargeSpeed = 10f;
    [SerializeField] private float chargeDistance = 6f;
    [SerializeField] private float chargeDuration = 0.7f;
    [SerializeField] private float chargeHitRadius = 0.8f;
    [SerializeField] private int chargeDamage = 1;
    [SerializeField] private float chargeSweepDelay = 3f;
    [SerializeField] private float returnDuration = 0.5f;

    [Header("Sweep Attack")]
    [SerializeField] private float sweepDelay = 0.45f;
    [SerializeField] private float sweepRecoveryTime = 0.5f;
    [SerializeField] private float sweepRadius = 3f;
    [SerializeField] private float sweepAngle = 120f;
    [SerializeField] private int sweepDamage = 1;
    [SerializeField] private Transform sweepOrigin;

    private float lastAttackTime;
    private AttackType currentAttackType;
    private Coroutine attackCoroutine;
    private bool isAttacking;

    public AttackType CurrentAttackType => currentAttackType;
    public Transform Player => player;
    public float ChargeRange => chargeRange;
    public bool IsAttacking => isAttacking;

    protected override void Awake()
    {
        base.Awake();

        lastAttackTime = -attackCooldown;

        if (bossAnimator == null)
        {
            bossAnimator = GetComponent<BossAnimator>();
        }
    }

    public bool CanAttack()
    {
        return !isAttacking && Time.time >= lastAttackTime + attackCooldown;
    }

    /// <summary>
    /// 공격 종류를 결정한다.
    /// forceSweep이 true면 쿨다운과 관계없이 Sweep을 선택한다.
    /// </summary>
    public bool StartBossAttack(bool forceSweep = false)
    {
        if (!CanAttack())
            return false;

        if (player == null)
        {
            Debug.LogWarning("보스의 Player가 연결되지 않았습니다.");
            return false;
        }

        Vector3 bossPosition = transform.position;
        Vector3 playerPosition = player.position;

        bossPosition.y = 0f;
        playerPosition.y = 0f;

        float distance = Vector3.Distance(
            bossPosition,
            playerPosition
        );

        Debug.Log($"보스와 플레이어 거리: {distance:F2}");

        if (forceSweep)
        {
            currentAttackType = AttackType.Sweep;
        }
        else if (distance <= chargeRange)
        {
            currentAttackType = AttackType.Charge;
        }
        else
        {
            currentAttackType = AttackType.BookRain;
        }

        Debug.Log($"공격 선택: {currentAttackType}");

        return true;
    }

    /// <summary>
    /// 공격 종료 및 쿨다운 시작
    /// </summary>
    public void NotifyAttackFinished()
    {
        lastAttackTime = Time.time;
        isAttacking = false;
        attackCoroutine = null;

        Debug.Log($"공격 종료. {attackCooldown}초 후 다음 공격 가능");
    }

    /// <summary>
    /// 책 폭격 공격 시작
    /// </summary>
    public void StartBookRain()
    {
        if (isAttacking)
            return;

        if (bookTrapManager == null)
        {
            Debug.LogError("BookTrapManager가 연결되지 않았습니다.");
            return;
        }

        isAttacking = true;

        // 책 폭격 애니메이션 시작
        bossAnimator?.PlayBookRain(true);

        // 책 폭격 시작
        if (!bookTrapManager.StartBossBookRain())
        {
            bossAnimator?.PlayBookRain(false);
            NotifyAttackFinished();
            return;
        }

        Debug.Log("Boss Attack: BookRain");
    }

    /// <summary>
    /// 플레이어 방향으로 돌진한다.
    /// </summary>
    public void StartChargeAttack()
    {
        if (isAttacking || player == null)
            return;

        isAttacking = true;
        attackCoroutine = StartCoroutine(ChargeRoutine());
    }

    private IEnumerator ChargeRoutine()
    {
        // 돌진하기 전 보스의 원래 위치 저장
        Vector3 originalPosition = transform.position;

        // 플레이어 방향으로 돌진
        Vector3 direction = GetFlatDirectionToPlayer();

        if (direction.sqrMagnitude < 0.001f)
            direction = transform.forward;

        FaceDirection(direction);

        if (bossAnimator != null)
            bossAnimator.PlayCharge();

        float elapsed = 0f;
        float movedDistance = 0f;
        bool hasHitPlayer = false;

        // 1. Charge
        while (elapsed < chargeDuration &&
               movedDistance < chargeDistance)
        {
            float step = Mathf.Min(
                chargeSpeed * Time.deltaTime,
                chargeDistance - movedDistance
            );

            transform.position += direction * step;

            movedDistance += step;
            elapsed += Time.deltaTime;

            // 돌진 중 플레이어에게 한 번만 피해
            if (!hasHitPlayer && IsPlayerInChargeRange())
            {
                hasHitPlayer = true;
                DealDamageToPlayer(chargeDamage);
            }

            yield return null;
        }

        // 2. Charge 종료 후 3초 대기
        yield return new WaitForSeconds(chargeSweepDelay);

        // 3. Sweep은 거리와 관계없이 무조건 실행
        Vector3 sweepDirection = GetFlatDirectionToPlayer();

        if (sweepDirection.sqrMagnitude > 0.001f)
            FaceDirection(sweepDirection);

        if (bossAnimator != null)
            bossAnimator.PlaySweep();

        // 팔을 휘두르기 전 준비 시간
        yield return new WaitForSeconds(sweepDelay);

        // Sweep 공격 판정
        if (player != null && IsPlayerInSweepRange())
        {
            DealDamageToPlayer(sweepDamage);
        }

        // 공격 후딜레이
        yield return new WaitForSeconds(sweepRecoveryTime);

        // 4. Sweep 종료 후 원래 위치로 복귀
        Vector3 returnStartPosition = transform.position;
        float returnElapsed = 0f;

        while (returnElapsed < returnDuration)
        {
            returnElapsed += Time.deltaTime;

            float t = returnDuration > 0f
                ? Mathf.Clamp01(returnElapsed / returnDuration)
                : 1f;

            transform.position = Vector3.Lerp(
                returnStartPosition,
                originalPosition,
                t
            );

            yield return null;
        }

        // 위치 오차 없이 원래 위치에 정확히 복귀
        transform.position = originalPosition;

        // 전체 공격 시퀀스 종료 및 쿨다운 시작
        NotifyAttackFinished();
    }

    /// <summary>
    /// 보스 앞쪽을 휩쓰는 공격
    /// </summary>
    public void StartSweepAttack()
    {
        if (isAttacking || player == null)
            return;

        isAttacking = true;
        attackCoroutine = StartCoroutine(SweepRoutine());
    }

    private IEnumerator SweepRoutine()
    {
        Vector3 direction = GetFlatDirectionToPlayer();

        if (direction.sqrMagnitude > 0.001f)
            FaceDirection(direction);

        if (bossAnimator != null)
            bossAnimator.PlaySweep();

        // 팔을 휘두르기 전 준비 시간
        yield return new WaitForSeconds(sweepDelay);

        // 공격 판정은 한 번만 실행
        if (player != null && IsPlayerInSweepRange())
        {
            DealDamageToPlayer(sweepDamage);
        }

        // 공격 후딜레이
        yield return new WaitForSeconds(sweepRecoveryTime);

        NotifyAttackFinished();
    }

    private Vector3 GetFlatDirectionToPlayer()
    {
        if (player == null)
            return Vector3.zero;

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        return direction.normalized;
    }

    private void FaceDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.001f)
            return;

        transform.rotation = Quaternion.LookRotation(direction);
    }

    private bool IsPlayerInChargeRange()
    {
        if (player == null)
            return false;

        Vector3 bossPosition = transform.position;
        Vector3 playerPosition = player.position;

        bossPosition.y = 0f;
        playerPosition.y = 0f;

        return Vector3.Distance(bossPosition, playerPosition)
               <= chargeHitRadius;
    }

    private bool IsPlayerInSweepRange()
    {
        if (player == null)
            return false;

        Vector3 origin = sweepOrigin != null
            ? sweepOrigin.position
            : transform.position;

        Vector3 toPlayer = player.position - origin;
        toPlayer.y = 0f;

        if (toPlayer.magnitude > sweepRadius)
            return false;

        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        if (toPlayer.sqrMagnitude < 0.001f)
            return true;

        float angle = Vector3.Angle(
            forward,
            toPlayer.normalized
        );

        return angle <= sweepAngle * 0.5f;
    }

    /// <summary>
    /// 플레이어의 BaseCharacterHealth를 이용해 피해를 준다.
    /// </summary>
    private void DealDamageToPlayer(int damage)
    {
        if (player == null)
            return;

        PlayerHealth playerHealth =
            player.GetComponentInParent<PlayerHealth>();

        if (playerHealth == null)
        {
            Debug.LogWarning(
                "플레이어에서 PlayerHealth를 찾지 못했습니다."
            );
            return;
        }

        playerHealth.TakeDamage(damage);

        Debug.Log($"보스 공격 적중! 피해량: {damage}");
    }


    /// <summary>
    /// 책 폭격 애니메이션
    /// </summary>
    public void PlayBookRainAnimation(bool isActive)
    {
        if (bossAnimator != null)
            bossAnimator.PlayBookRain(isActive);
    }

    /// <summary>
    /// 스턴 애니메이션
    /// </summary>
    public void PlayStunAnimation()
    {
        if (bossAnimator != null)
            bossAnimator.PlayStun();
    }

    /// <summary>
    /// 사망 애니메이션
    /// </summary>
    public void PlayDeadAnimation()
    {
        if (bossAnimator != null)
            bossAnimator.PlayDead();
    }

}
