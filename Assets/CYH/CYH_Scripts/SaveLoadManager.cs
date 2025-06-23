using System;
using System.IO;
using UnityEngine;

/// <summary>
/// JSON �������� ���� �����͸� �����ϰ� �ҷ����� �Ŵ��� Ŭ�����Դϴ�.
/// </summary>
public class SaveLoadManager : MonoBehaviour
{
    public GameData GameData;

    public const string FileName = "SaveFile";  // ���̺� ���ϸ�
    public string DataPath => Path.Combine(Application.dataPath, $"CYH/CYH_SaveFiles/{FileName}");  // ���̺� ���� ���� ���

    [field: SerializeField] public int ElapsedSeconds { get; private set; } // �������� �� �� ��� �ð�(��) - �׽�Ʈ��

    [Header("�� ��� �ð�")]
    [SerializeField] private int _elapsedMinutes;   // �������� �� �� ��� �ð�(��)
    public int ElapsedMinutes
    {
        get { return _elapsedMinutes; }
    }

    // merge �� Singleton Ŭ���� ��� ���� �� ���� �����Դϴ�.
    #region Singleton
    private static SaveLoadManager _instance;
    public static SaveLoadManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // ���� ������ �ε�
        //LoadData();
    }
    #endregion

    private void Update()
    {
        // �׽�Ʈ��
        if (Input.GetKeyDown(KeyCode.M))
        {
            SaveData();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            LoadData();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            DeleteData();
        }
    }

    /// <summary>
    /// ������ ��ο� �����͸� JSON �������� �����մϴ�.
    /// </summary>
    public void SaveData()
    {
        // ���� ��� ���� üũ
        if (Directory.Exists(Path.GetDirectoryName($"{DataPath}")) == false)
        {
            Directory.CreateDirectory(Path.GetDirectoryName($"{DataPath}"));
        }

        GameData.SavedTime = DateTime.Now;  // ���� �ð� = ���� �ð�
        
        string json = JsonUtility.ToJson(GameData, true);
        Debug.Log(json);
        File.WriteAllText($"{DataPath}", json);
        Debug.Log("SaveData");
        Debug.Log($"������ ���� �ð�: {GameData.SavedTime}");
    }

    /// <summary>
    /// ������ ��ο��� JSON ������ �����͸� �ҷ��� ��ü�� ��ȯ�մϴ�.
    /// ���� ���ӿ����� Awake���� ȣ��� �����Դϴ�.
    /// </summary>
    public bool LoadData()
    {
        // ������ ���� ���� üũ
        if (!ExistData())
            return false;

        string json = File.ReadAllText($"{DataPath}");
        GameData = JsonUtility.FromJson<GameData>(json);
        Debug.Log("LoadData");

        // �׽�Ʈ��
        ElapsedTime();
        _elapsedMinutes = 0;
        ElapsedSeconds = 0;

        return true;
    }

    /// <summary>
    /// ������ ��ο� �ִ� ���� ������ �����մϴ�.
    /// </summary>
    public bool DeleteData()
    {
        // �ʱ�ȭ�� Json���� ��� ��Ȳ�� ���

        // ������ ���� ���� üũ
        if (!ExistData())
            return false;

        // ���� ����
        File.Delete($"{DataPath}");
        Debug.Log("DeleteData");

        return true;
    }

    /// <summary>
    /// ������ ��ο� ���� ������ �����ϴ��� Ȯ���մϴ�.
    /// </summary>
    public bool ExistData()
    {
        // ���� ��� ���� üũ
        if (Directory.Exists(Path.GetDirectoryName($"{DataPath}")) == false)
            return false;

        return File.Exists($"{DataPath}");
    }

    /// <summary>
    /// ����� �ð��� ���� �ð��� ���̸� ����� ��� �ð��� �� ������ �����մϴ�.
    /// ����� �׽�Ʈ �뵵�� �� ������ �������� ����մϴ�.
    /// </summary>
    public void ElapsedTime()
    {
        DateTime gameStartTime = DateTime.Now;
        TimeSpan elapsedTime = gameStartTime - GameData.SavedTime;

        // �� ������ ������ �� ������ ����
        //_elapsedMinutes = (int)elapsedTime.TotalMinutes;
        //Debug.Log($"���� ���� �ð�: {gameStartTime}");
        //Debug.Log($"���� ���� �� ��� �ð�: {ElapsedMinutes}��");

        // �׽�Ʈ�� �� ���� üũ
        ElapsedSeconds = (int)elapsedTime.TotalSeconds;
        Debug.Log($"���� ���� �ð�: {gameStartTime}");
        Debug.Log($"���� ���� �� ��� �ð�: {ElapsedSeconds}��");
    }
}