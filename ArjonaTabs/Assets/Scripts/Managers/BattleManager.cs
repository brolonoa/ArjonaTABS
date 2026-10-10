using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    private readonly List<TeamMember> units = new();

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterUnit(TeamMember unit)
    {
        if (!units.Contains(unit))
        {
            units.Add(unit);
        }
    }

    public void UnregisterUnit(TeamMember unit)
    {
        units.Remove(unit);
    }

    public TeamMember FindClosestEnemy(TeamMember unit)
    {
        TeamMember closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (TeamMember other in units)
        {
            if (other == null || other == unit)
            {
                continue;
            }

            if (other.CurrentTeam == unit.CurrentTeam)
            {
                continue;
            }

            float distance = (other.Position - unit.Position).sqrMagnitude;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = other;
            }
        }

        return closestEnemy;
    }
}
