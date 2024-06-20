using System.IO;
using UnityEngine;

[System.Serializable]
public class Stat
{
    public string statName;
    public string statType;
    public string statSprite;
    public float statValue;

    public Stat(string statName, string statType, string statSprite, float statValue)
    {
        this.statName = statName;
        this.statSprite = statSprite;
        this.statType = statType;
        this.statValue = statValue;
    }
}

public class StatCardInfo : MonoBehaviour
{
    [SerializeField] public Stat[] statInfos;

    private class StatJson
    {
        public Stat[] stats;

        public StatJson(Stat[] stats)
        {
            this.stats= stats;
        }
    }

    [ContextMenu("MakeJsonFile")]
    public void MakeJsonFile()
    {
        StatJson statJson = new StatJson(GetStatArray(11));      // �׷��� item�迭�� ������ Ŭ������ �ϳ� ���� ���� ��ü�� ������ �� �ְ� ����

        // ToJson(obj, bool): bool���� true�� ��� �������� ����, false�� ��� �ּҰ����� obj���� ���
        string json = JsonUtility.ToJson(statJson, true);       // Json�� ��ü �ϳ��� ���尡��
        SaveFile("statData", json);
    }

    private Stat[] GetStatArray(int count)
    {
        Stat[] stats = new Stat[count];

        for (int i = 0; i < stats.Length; i++)
        {
            Stat newStat = new Stat("������", "ü��", "���� �̹���", 1);
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

    [ContextMenu("RoadJsonFile")]
    public void RoadJsonFile()
    {
        string json = LoadFile("statData");
        object convert = JsonUtility.FromJson(json, typeof(StatJson));
        StatJson statJson = convert as StatJson;
        Stat[] stats = statJson.stats;

        this.statInfos = stats;
        Debug.Log("������ ������ �ε� �Ϸ�");
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
