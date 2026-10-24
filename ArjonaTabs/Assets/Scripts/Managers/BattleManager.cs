using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    private readonly List<TeamMember> units = new();
    public event Action<int, bool> OnUnitPlaced;
    private bool isBlue;

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterUnit(TeamMember unit)
    {
        if (unit == null || units.Contains(unit))
            return;

        StartCoroutine(RegisterUnitNextFrame(unit));
    }

    private IEnumerator RegisterUnitNextFrame(TeamMember unit)
    {
        yield return null;

        if (unit == null || unit.stats == null)
            yield break;

        units.Add(unit);

        bool isBlue = unit.CurrentTeam == TeamMember.Team.Blue;
        OnUnitPlaced?.Invoke(unit.stats.costCurrency, isBlue);
    }

    public void UnregisterUnit(TeamMember unit)
    {
        units.Remove(unit);
    }

    public TeamMember FindClosest(TeamMember unit, bool allies)
    {
        if (unit == null)
        {
            return null;
        }

        TeamMember closest = null;
        float closestDistance = float.MaxValue;

        foreach (TeamMember other in units)
        {
            if (other == null || other == unit)
            {
                continue;
            }

                bool sameTeam = other.CurrentTeam == unit.CurrentTeam;

            // allies = true  -> solo aliados
            // allies = false -> solo enemigos
            if (sameTeam != allies)
            {
                continue;
            }

            float distance = (other.Position - unit.Position).sqrMagnitude;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = other;
            }
        }

        return closest;
    }
    public TeamMember FindClosestEnemy(TeamMember unit) => FindClosest(unit, false);
    public TeamMember FindClosestAlly(TeamMember unit) => FindClosest(unit, true);
}
