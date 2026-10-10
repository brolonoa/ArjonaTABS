using UnityEngine;

public class TeamMember : MonoBehaviour
{
    [SerializeField] private Transform movementTransform;

    public Vector3 Position => movementTransform.position;
    public enum Team
    {
        Blue,
        Red
    }

    [SerializeField] private Team team;

    public Team CurrentTeam => team;

    public void SetTeam(Team newTeam)
    {
        team = newTeam;
    }

    public bool IsEnemy(TeamMember other)
    {
        return other != null && other.CurrentTeam != team;
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
}

    

