using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ExcelManager : MonoBehaviour
{
    public static ExcelManager Instance { get; private set; }
    public event Action OnDataLoaded;
    public bool IsLoaded { get; private set; }

    [Header("Google Sheets CSV URL")]
    [SerializeField] private string csvUrl;

    private readonly Dictionary<string, UnitData> unitsData = new Dictionary<string, UnitData>();

    public struct UnitData
    {
        public float health;
        public float damage;
        public float speed;
        public float attackCooldown;
        public float range;
        public int costCurrency;
        public float heal;
        public float defense;
        public float projectileSpeed;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        StartCoroutine(DownloadCSV());
    }

    public bool TryGetUnitData(string unitType, out UnitData data)
    {
        return unitsData.TryGetValue(unitType.Trim().ToLower(), out data);
    }

    IEnumerator DownloadCSV()
    {
        UnityWebRequest www = UnityWebRequest.Get(GetCsvExportUrl(csvUrl));
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error downloading CSV: " + www.error);
        }
        else
        {
            ParseValues(www.downloadHandler.text);
            IsLoaded = true;
            OnDataLoaded?.Invoke();
        }
    }

    // Accepts the normal "/edit" link of the sheet and turns it into the CSV export link
    string GetCsvExportUrl(string url)
    {
        int editIndex = url.IndexOf("/edit", StringComparison.Ordinal);
        if (editIndex < 0)
        {
            return url;
        }

        string gid = "0";
        int gidIndex = url.IndexOf("gid=", StringComparison.Ordinal);
        if (gidIndex >= 0)
        {
            gid = url.Substring(gidIndex + 4).Split('&', '#')[0];
        }

        return url.Substring(0, editIndex) + "/export?format=csv&gid=" + gid;
    }

    void ParseValues(string csvData)
    {
        string[] lines = csvData.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length == 0)
        {
            return;
        }

        // Columns are found by their header name, so adding or moving columns in the sheet doesn't break anything
        // The header is the row whose first cell is "class", so title rows above it are ignored
        int headerRow = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            if (SplitCsvLine(lines[i])[0].Trim().ToLower() == "class")
            {
                headerRow = i;
                break;
            }
        }

        List<string> header = SplitCsvLine(lines[headerRow]);
        Dictionary<string, int> columns = new Dictionary<string, int>();
        for (int i = 0; i < header.Count; i++)
        {
            // Anything after " - " or "(" is treated as a note, e.g. "speed - m/s" or "defense (%)"
            string name = header[i];
            int noteStart = name.IndexOfAny(new[] { '-', '(', '[' });
            if (noteStart > 0)
            {
                name = name.Substring(0, noteStart);
            }

            name = name.Trim().ToLower();
            if (name.Length > 0 && !columns.ContainsKey(name))
            {
                columns[name] = i;
            }
        }

        for (int i = headerRow + 1; i < lines.Length; i++)
        {
            List<string> fields = SplitCsvLine(lines[i]);
            string unitName = fields[0].Trim().ToLower();

            if (string.IsNullOrEmpty(unitName))
            {
                continue;
            }

            UnitData data = new UnitData
            {
                health = GetFloat(fields, columns, "health"),
                damage = GetFloat(fields, columns, "damage"),
                speed = GetFloat(fields, columns, "speed"),
                attackCooldown = GetFloat(fields, columns, "attackcooldown"),
                range = GetFloat(fields, columns, "range"),
                costCurrency = Mathf.RoundToInt(GetFloat(fields, columns, "costcurrency")),
                heal = GetFloat(fields, columns, "heal"),
                defense = GetFloat(fields, columns, "defense"),
                projectileSpeed = GetFloat(fields, columns, "projectilespeed", "proyectilespeed"),
            };

            unitsData[unitName] = data;
            Debug.Log($"[Success] {unitName} loaded -> Health: {data.health}, Damage: {data.damage}, Speed: {data.speed}, Cooldown: {data.attackCooldown}, Range: {data.range}, Cost: {data.costCurrency}, Heal: {data.heal}, Defense: {data.defense}, Projectile speed: {data.projectileSpeed}");
        }
    }

    // Splits a CSV line keeping quoted values like "1,5" together
    List<string> SplitCsvLine(string line)
    {
        List<string> fields = new List<string>();
        StringBuilder current = new StringBuilder();
        bool inQuotes = false;

        foreach (char c in line.Trim('\r', '\n'))
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        fields.Add(current.ToString());
        return fields;
    }

    // Accepts both "1.5" and "1,5". Several names can be given for the same column (e.g. different spellings)
    float GetFloat(List<string> fields, Dictionary<string, int> columns, params string[] names)
    {
        foreach (string column in names)
        {
            if (columns.TryGetValue(column, out int index) && index < fields.Count)
            {
                string value = fields[index].Trim().Replace(',', '.');
                float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float result);
                return result;
            }
        }

        Debug.LogWarning($"Column '{names[0]}' not found in the CSV");
        return 0;
    }
}
