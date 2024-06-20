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
        StatJson statJson = new StatJson(GetStatArray(11));      // 그래서 item배열을 가지는 클래스를 하나 만들어서 여러 객체를 저장할 수 있게 만듦

        // ToJson(obj, bool): bool값이 true인 경우 가독성이 좋게, false인 경우 최소값으로 obj값을 출력
        string json = JsonUtility.ToJson(statJson, true);       // Json은 객체 하나만 저장가능
        SaveFile("statData", json);
    }

    private Stat[] GetStatArray(int count)
    {
        Stat[] stats = new Stat[count];

        for (int i = 0; i < stats.Length; i++)
        {
            Stat newStat = new Stat("강인함", "체력", "스탯 이미지", 1);
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

        Debug.Log("파일 저장 완료");
    }

    [ContextMenu("RoadJsonFile")]
    public void RoadJsonFile()
    {
        string json = LoadFile("statData");
        object convert = JsonUtility.FromJson(json, typeof(StatJson));
        StatJson statJson = convert as StatJson;
        Stat[] stats = statJson.stats;

        this.statInfos = stats;
        Debug.Log("아이템 데이터 로드 완료");
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
