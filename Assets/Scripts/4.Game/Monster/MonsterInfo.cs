using System.IO;
using UnityEngine;

[System.Serializable]
public class MonsterStat
{
    public string monsterName;
    public float monsterMaxHp;
    public float monsterDamage;
    public float monsterSpeed;
    public float monsterDefence;
    public float monsterExp;
    public int monsterCoin;

    public MonsterStat(string statName, float monsterMaxHp, float monsterDamage, float monsterSpeed, float monsterDefence, float monsterExp, int monsterCoin)
    {
        this.monsterName = statName;
        this.monsterMaxHp = monsterMaxHp;
        this.monsterDamage = monsterDamage;
        this.monsterSpeed = monsterSpeed;
        this.monsterDefence = monsterDefence;
        this.monsterExp = monsterExp;
        this.monsterCoin = monsterCoin;
    }
}

public class MonsterInfo : Singleton<MonsterInfo>
{
    [SerializeField] public MonsterStat[] monsterInfos;

    int count;

    private class MonsterStatJson
    {
        public MonsterStat[] stats;

        public MonsterStatJson(MonsterStat[] stats)
        {
            this.stats = stats;
        }
    }

    [ContextMenu("MakeJsonFile")]
    public void MakeJsonFile()
    {
        count = 5;
        MonsterStatJson statJson = new MonsterStatJson(GetStatArray());      // �׷��� item�迭�� ������ Ŭ������ �ϳ� ���� ���� ��ü�� ������ �� �ְ� ����

        // ToJson(obj, bool): bool���� true�� ��� �������� ����, false�� ��� �ּҰ����� obj���� ���
        string json = JsonUtility.ToJson(statJson, true);       // Json�� ��ü �ϳ��� ���尡��
        SaveFile("monsterStatData", json);
    }

    private MonsterStat[] GetStatArray()
    {
        MonsterStat[] stats = new MonsterStat[count];

        for (int i = 0; i < stats.Length; i++)
        {
            MonsterStat newStat = new MonsterStat("����", 0, 0, 0, 0, 0, 0);
            stats[i] = newStat;
        }

        return stats;
    }

    private void SaveFile(string fileName, string text)
    {
        string path = string.Format("{0}/{1}.txt", Application.dataPath, fileName);
        StreamWriter sw = new StreamWriter(path);
        sw.Write(text);
        sw.Close();

        Debug.Log("���� ���� �Ϸ�");
    }

    [ContextMenu("AddJsonFile")]
    public void AddJsonFile()
    {
        MonsterStatJson statJson = new MonsterStatJson(AddStatArray(1));      // �׷��� item�迭�� ������ Ŭ������ �ϳ� ���� ���� ��ü�� ������ �� �ְ� ����

        // ToJson(obj, bool): bool���� true�� ��� �������� ����, false�� ��� �ּҰ����� obj���� ���
        string json = JsonUtility.ToJson(statJson, true);       // Json�� ��ü �ϳ��� ���尡��
        SaveFile("monsterStatData", json);
    }

    private MonsterStat[] AddStatArray(int add)
    {
        count += add;

        MonsterStat[] stats = new MonsterStat[count];

        string json = LoadFile("monsterStatData");
        object convert = JsonUtility.FromJson(json, typeof(MonsterStatJson));
        MonsterStatJson statJson = convert as MonsterStatJson;
        MonsterStat[] beforeStats = statJson.stats;

        for (int i = 0; i < beforeStats.Length; i++)
        {
            stats[i] = beforeStats[i];
        }

        for (int j = beforeStats.Length; j < stats.Length; j++)
        {
            MonsterStat newStat = new MonsterStat("����", 0, 0, 0, 0, 0, 0);
            stats[j] = newStat;
        }

        return stats;
    }

    [ContextMenu("RoadJsonFile")]
    public void RoadJsonFile()
    {
        string json = LoadFile("monsterStatData");
        object convert = JsonUtility.FromJson(json, typeof(MonsterStatJson));
        MonsterStatJson statJson = convert as MonsterStatJson;
        MonsterStat[] stats = statJson.stats;

        this.monsterInfos = stats;
        Debug.Log("���� ������ �ε� �Ϸ�");
    }

    private string LoadFile(string fileName)
    {
        string path = string.Format("{0}/{1}.txt", Application.dataPath, fileName);
        StreamReader sr = new StreamReader(path);
        string readToEnd = sr.ReadToEnd();
        sr.Close();

        return readToEnd;
    }
}
