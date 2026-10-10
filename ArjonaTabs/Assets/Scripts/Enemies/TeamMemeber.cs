using UnityEngine;

public class TeamMember : MonoBehaviour
{
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
}
