
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
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private Transform player;
    [SerializeField] private float chargeRange = 5f;

    [Header("Book Rain")]
    [SerializeField] private BookTrapManager bookTrapManager;

    private float lastAttackTime;
    private AttackType currentAttackType;

    public AttackType CurrentAttackType => currentAttackType;
    public Transform Player => player;
    public float ChargeRange => chargeRange;

    protected override void Awake()
    {
        base.Awake();

        // 게임 시작 직후 첫 공격 가능
        lastAttackTime = -attackCooldown;
    }

    /// <summary>
    /// 다음 공격을 시작할 수 있는지 확인한다.
    /// </summary>
    public bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown;
    }

    /// <summary>
    /// 공격 종류를 결정한다.
    /// forceSweep이 true면 무조건 Sweep 공격을 선택한다.
    /// </summary>
    public bool StartBossAttack(bool forceSweep = false)
    {
        // 강제 Sweep이 아니라면 쿨다운 확인
        if (!forceSweep && !CanAttack())
            return false;

        if (forceSweep)
        {
            currentAttackType = AttackType.Sweep;
        }
        else
        {
            currentAttackType = SelectAttackType();
        }

        Debug.Log($"공격 선택: {currentAttackType}");

        return true;
    }

    /// <summary>
    /// 공격 종료 시 다음 공격까지의 쿨다운을 시작한다.
    /// </summary>
    public void NotifyAttackFinished()
    {
        lastAttackTime = Time.time;

        Debug.Log($"공격 종료. {attackCooldown}초 후 다음 공격 가능");
    }

    /// <summary>
    /// 플레이어와의 거리에 따라 공격을 선택한다.
    /// 가까우면 돌진, 멀면 책 폭격.
    /// </summary>
    private AttackType SelectAttackType()
    {
        if (player == null)
        {
            Debug.LogWarning(
                "FirstBossController: Player가 설정되지 않았습니다."
            );

            return AttackType.BookRain;
        }

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        Debug.Log($"보스와 플레이어 거리: {distance:F2}");

        return distance <= chargeRange
            ? AttackType.Charge
            : AttackType.BookRain;
    }

    /// <summary>
    /// 책 폭격 공격
    /// </summary>
    public void StartBookRain()
    {
        if (bookTrapManager == null)
        {
            Debug.LogWarning(
                "FirstBossController: BookTrapManager가 설정되지 않았습니다."
            );
            return;
        }

        bookTrapManager.StartBossBookRain();
    }

    /// <summary>
    /// 플레이어를 향한 돌진 공격
    /// </summary>
    public void StartChargeAttack()
    {
        // TODO: 돌진 공격 시작
    }

    /// <summary>
    /// 주변을 휩쓰는 공격
    /// </summary>
    public void StartSweepAttack()
    {
        // TODO: 휩쓸기 공격 시작
    }
}
