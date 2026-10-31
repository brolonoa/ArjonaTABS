using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

// Temporary test tool: plays a list of battles automatically in the Progra scene and writes the results
public class BattleTester : MonoBehaviour
{
    const string MarkerPath = "Library/battle_tests.txt";

    private readonly List<string[]> matchups = new List<string[]>();
    private string outputPath;
    private int repetitions = 3;
    private float timeScale = 3f;
    private float timeLimit = 90f;
    private bool trace;
    private bool noAvoid;

    // Writes where every unit is, who it targets and its distance to it
    private void Trace(List<TeamMember> units, float t)
    {
        var sb = new System.Text.StringBuilder($"t={t:0.00}");
        foreach (TeamMember u in units.Where(u => u != null))
        {
            EnemyController c = u.GetComponentInChildren<EnemyController>();
            EnemyHp hp = u.GetComponentInChildren<EnemyHp>();
            UnityEngine.AI.NavMeshAgent ag = u.GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
            string tgt = c != null && c.currentTarget != null ? c.currentTarget.name.Replace("(Clone)", "") : "-";
            float d = c != null && c.currentTarget != null ? Vector3.Distance(c.currentTarget.Position, c.transform.position) : -1;
            sb.Append($" | {u.name.Replace("(Clone)", "")} hp={hp.currentHealth:0.0} pos=({c.transform.position.x:0.0},{c.transform.position.z:0.0}) tgt={tgt} d={d:0.00} stop={ag.isStopped} v={ag.velocity.magnitude:0.0}");
        }
        File.AppendAllText(outputPath + ".trace.txt", sb + "\n");
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        if (!File.Exists(MarkerPath) || SceneManager.GetActiveScene().name != "Progra")
        {
            return;
        }

        new GameObject("BattleTester").AddComponent<BattleTester>();
    }

    private void Start()
    {
        foreach (string raw in File.ReadAllLines(MarkerPath))
        {
            string line = raw.Trim();
            if (line.StartsWith("output=")) outputPath = line.Substring(7);
            else if (line.StartsWith("repetitions=")) repetitions = int.Parse(line.Substring(12));
            else if (line.StartsWith("timescale=")) timeScale = float.Parse(line.Substring(10), System.Globalization.CultureInfo.InvariantCulture);
            else if (line.StartsWith("trace=1")) trace = true;
            else if (line.StartsWith("noavoid=1")) noAvoid = true;            else if (line.Contains("|")) matchups.Add(line.Split('|'));
        }

        File.WriteAllText(outputPath, "armyA;armyB;rep;blueSide;winner;duration;survivors\n");
        StartCoroutine(RunAll());
    }

    private IEnumerator RunAll()
    {
        float wait = 0f;
        while ((ExcelManager.Instance == null || !ExcelManager.Instance.IsLoaded) && wait < 15f)
        {
            wait += Time.unscaledDeltaTime;
            yield return null;
        }

        Dictionary<char, GameObject[]> prefabs = FindPrefabs();
        Dictionary<TeamMember.Team, PlacementSystem> placements = FindPlacements();
        Time.timeScale = timeScale;

        // Nothing is drawn while testing, so the graphics card can't crash; the battles run the same
        foreach (Camera cam in FindObjectsByType<Camera>(FindObjectsSortMode.None))
        {
            cam.enabled = false;
        }

        foreach (string[] m in matchups)
        {
            for (int rep = 0; rep < repetitions; rep++)
            {
                bool swap = rep % 2 == 1;
                string blueArmy = swap ? m[1] : m[0];
                string redArmy = swap ? m[0] : m[1];

                List<TeamMember> spawned = new List<TeamMember>();
                spawned.AddRange(Spawn(blueArmy, TeamMember.Team.Blue, prefabs, placements));
                spawned.AddRange(Spawn(redArmy, TeamMember.Team.Red, prefabs, placements));

                yield return new WaitForSeconds(0.5f);
                GameManager.Instance.StartCombat();

                float start = Time.time;
                if (trace)
                {
                    File.AppendAllText(outputPath + ".trace.txt", $"=== {blueArmy} (blue) vs {redArmy} (red)\n");
                    foreach (TeamMember u in spawned)
                    {
                        string victim = u.name.Replace("(Clone)", "");
                        u.GetComponentInChildren<EnemyHp>().OnDamaged += dmg => File.AppendAllText(outputPath + ".trace.txt", $"HIT t={Time.time - start:0.00} {victim} -{dmg:0.00}\n");
                    }
                }
                int blue = 1, red = 1;
                while (Time.time - start < timeLimit)
                {
                    yield return new WaitForSeconds(0.25f);
                    if (trace) Trace(spawned, Time.time - start);
                    blue = spawned.Count(u => u != null && u.CurrentTeam == TeamMember.Team.Blue);
                    red = spawned.Count(u => u != null && u.CurrentTeam == TeamMember.Team.Red);
                    if (blue == 0 || red == 0) break;
                }

                string winnerSide = blue > 0 && red == 0 ? "blue" : red > 0 && blue == 0 ? "red" : "draw";
                string winner = winnerSide == "draw" ? "draw" : (winnerSide == "blue") != swap ? "A" : "B";
                string survivors = string.Join(" ", spawned.Where(u => u != null).Select(u =>
                {
                    EnemyHp hp = u.GetComponentInChildren<EnemyHp>();
                    return u.name.Replace("(Clone)", "") + ":" + (hp != null ? Mathf.RoundToInt(hp.currentHealth) : 0);
                }));
                File.AppendAllText(outputPath, $"{m[0]};{m[1]};{rep};{blueArmy};{winner};{(Time.time - start):0.0};{survivors}\n");

                foreach (TeamMember u in spawned.Where(u => u != null)) Destroy(u.gameObject);
                foreach (PotionProjectile p in FindObjectsByType<PotionProjectile>(FindObjectsSortMode.None)) Destroy(p.gameObject);
                yield return new WaitForSeconds(0.5f);
            }
        }

        File.AppendAllText(outputPath, "DONE\n");
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private List<TeamMember> Spawn(string army, TeamMember.Team team, Dictionary<char, GameObject[]> prefabs, Dictionary<TeamMember.Team, PlacementSystem> placements)
    {
        List<TeamMember> result = new List<TeamMember>();
        PlacementSystem placement = placements[team];
        Grid grid = (Grid)typeof(PlacementSystem).GetField("grid", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(placement);
        TeamGrid teamGrid = FindObjectsByType<TeamGrid>(FindObjectsInactive.Include, FindObjectsSortMode.None).First(g => g.Team == team);
        Bounds area = teamGrid.GetComponentInChildren<Collider>(true).bounds;

        char[] units = army.Split('+').Select(s => s.Trim()[0]).ToArray();
        for (int i = 0; i < units.Length; i++)
        {
            Vector3 world = area.center + Vector3.right * ((i - (units.Length - 1) / 2f) * 2.5f);
            Vector3 pos = grid.CellToWorld(grid.WorldToCell(world));
            GameObject prefab = prefabs[units[i]][team == TeamMember.Team.Blue ? 0 : 1];
            GameObject unit = Instantiate(prefab, pos, prefab.transform.rotation);
            TeamMember member = unit.GetComponent<TeamMember>();
            member.SetTeam(team);
            // Test option: units walk through each other, so nobody gets blocked by an ally
            if (noAvoid)
            {
                foreach (UnityEngine.AI.NavMeshAgent ag in unit.GetComponentsInChildren<UnityEngine.AI.NavMeshAgent>())
                    ag.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.NoObstacleAvoidance;
            }
            result.Add(member);
        }

        return result;
    }

    private static Dictionary<char, GameObject[]> FindPrefabs()
    {
        Dictionary<char, GameObject[]> result = new Dictionary<char, GameObject[]>();
        FieldInfo blue = typeof(UiButton).GetField("bluePrefab", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo red = typeof(UiButton).GetField("redPrefab", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo type = typeof(UiButton).GetField("unitType", BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (UiButton b in FindObjectsByType<UiButton>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            string t = ((string)type.GetValue(b)).ToLower();
            char key = t.StartsWith("sol") ? 'S' : t.StartsWith("tan") ? 'T' : t.StartsWith("ran") ? 'R' : t.StartsWith("vam") ? 'V' : '?';
            result[key] = new[] { (GameObject)blue.GetValue(b), (GameObject)red.GetValue(b) };
        }
        return result;
    }

    private static Dictionary<TeamMember.Team, PlacementSystem> FindPlacements()
    {
        Dictionary<TeamMember.Team, PlacementSystem> result = new Dictionary<TeamMember.Team, PlacementSystem>();
        FieldInfo tg = typeof(PlacementSystem).GetField("teamGrid", BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (PlacementSystem p in FindObjectsByType<PlacementSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            result[((TeamGrid)tg.GetValue(p)).Team] = p;
        }
        return result;
    }
}
