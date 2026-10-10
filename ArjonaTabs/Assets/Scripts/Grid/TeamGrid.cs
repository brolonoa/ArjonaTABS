
using UnityEngine;

public class TeamGrid : MonoBehaviour
{
    [SerializeField] private TeamMember.Team team;

    public TeamMember.Team Team => team;
}
