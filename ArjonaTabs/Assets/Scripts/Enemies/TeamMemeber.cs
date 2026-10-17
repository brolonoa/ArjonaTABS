using System;
using UnityEngine;

public class TeamMember : MonoBehaviour
{
    [SerializeField] private Transform movementTransform;
    private EnemyHp hp;
    public Vector3 Position => movementTransform.position;
    
    public UnitStats stats;
    public enum Team
    {
        Blue,
        Red
    }

    [SerializeField] private Team team;

    public Team CurrentTeam => team;

    private void Awake()
    {
        hp = GetComponentInChildren<EnemyHp>();
        stats = GetComponent<UnitStats>();
    }

    public bool IsHealer()
    {
        return stats.IsHealer;
    }

    public void SetTeam(Team newTeam)
    {
        team = newTeam;
    }

    public bool IsEnemy(TeamMember other)
    {
        return other != null && other.CurrentTeam != team;
    }

    public void TakeDamage(float damage)
    {
        hp.TakeDamage(damage);
        Debug.Log(damage);
    }

    private void OnEnable()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.RegisterUnit(this);
        }
    }

    private void OnDisable()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.UnregisterUnit(this);
        }

    }
    public void Heal(float amount)
    {
        if (hp == null)
            return;

        hp.Heal(amount);
    }
    public float HealthPercentage => hp != null ? hp.HealthPercentage : 1f;

    public bool NeedsHealing => hp != null && hp.HealthPercentage < 1f;
}

    

