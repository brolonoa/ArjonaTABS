using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
        public int health;
        public int damage;
        public float speed;
        public float attackCooldown;
        public float range;
        public int costCurrency;
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

        // Start from 1 to skip the header
        for (int i = 1; i < lines.Length; i++)
        {
            string[] fields = lines[i].Trim().Split(',');

            if (fields.Length >= 7)
            {
                string unitName = fields[0].Trim().ToLower();
                UnitData data = new UnitData();
                int.TryParse(fields[1].Trim(), out data.health);
                int.TryParse(fields[2].Trim(), out data.damage);
                float.TryParse(fields[3].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out data.speed);
                float.TryParse(fields[4].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out data.attackCooldown);
                float.TryParse(fields[5].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out data.range);
                int.TryParse(fields[6].Trim(), out data.costCurrency);

                unitsData[unitName] = data;
                Debug.Log($"[Success] {unitName} loaded -> Health: {data.health}, Damage: {data.damage}, Speed: {data.speed}, Cooldown: {data.attackCooldown}, Range: {data.range}, Cost: {data.costCurrency}");
            }
        }
    }
}
