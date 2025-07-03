using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using JetBrains.Annotations;
using System.Runtime.CompilerServices;

// 데이터 구조 정의
[System.Serializable]
public class DialogLine
{
    public int index;
    public string charName;
    public int charIndex;
    public string imgName;
    public string dialog;
    public string animation;
    public string location;
    public string sound;
    public string bgmusic;
    public string bgImg;
    public string extra1;
}

public class DialogParser : MonoBehaviour
{
    [SerializeField] private TextAsset dialogData;
    [SerializeField] private TextMeshProUGUI charNameText;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private GameObject dialogUI;
    [SerializeField] private GameObject playerDialog;
    [SerializeField] private RectTransform logBoxSize;
    [SerializeField] private TextMeshProUGUI playerDialogBox;
    [SerializeField] private TextMeshProUGUI logText;
    [SerializeField] private AudioSource soundPlayer;
    [SerializeField] private Image[] charImgs = new Image[5];
    [SerializeField] private Sprite[] charSprites = new Sprite[14];
    [SerializeField] private AudioClip[] sounds = new AudioClip[20];

    [SerializeField] private string foxName; // 플레이어가 정해주는 구미호의 이름 -> 저장된 데이터에서 가져오는 것과 데이터에 저장하는 두가지가 구현되어야 합니다.
    [SerializeField] private int spacing;

    private List<DialogLine> dialogLines = new List<DialogLine>();
    private int currentIndex = 0;

    void Start()
    {
        LoadDialog();
        ShowLine(0);
    }

    void LoadDialog()
    {
        string[] lines = dialogData.text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        // 첫 줄은 헤더이므로 생략
        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');

            // CSV 필드가 누락되었을 경우 방지
            while (values.Length < 11)
            {
                System.Array.Resize(ref values, 11);
            }

            DialogLine line = new DialogLine
            {
                index = int.Parse(values[0]),
                charName = values[1].Replace("구미호", foxName),
                charIndex = int.Parse(values[2]),
                imgName = values[3],
                dialog = values[4].Replace("`", ",").Replace("구미호", foxName), // ` 를 ,로 변환
                animation = values[5],
                location = values[6],
                sound = values[7],
                bgmusic = values[8],
                bgImg = values[9],
                extra1 = values[10],
            };

            dialogLines.Add(line);
        }

        Debug.Log($"총 {dialogLines.Count}개의 대사가 로드되었습니다.");
    }

    void ShowLine(int index) // 다음 대사를 표시하는 버튼을 눌렀을때 행동하는 함수
    {
        if (index < 0 || index >= dialogLines.Count)
        {
            Debug.LogWarning("대사 인덱스 범위를 벗어남");
            return;
        }

        DialogLine line = dialogLines[index];

        // 주인공 (플레이어)의 대사가 나올 경우
        if (line.charIndex == 0)
        {
            // 모든 이미지 비활성화
            for (int i = 0; i < charImgs.Length; i++)
            {
                charImgs[i].gameObject.SetActive(false);
            }
            // 대화창을 숨기고 플레이어 선택 버튼을 생성
            playerDialog.SetActive(true);
            dialogUI.SetActive(false);
            // line 입력 (대사 / 연출 / 사운드 / 브금 / 배경)
            playerDialogBox.text = line.dialog;
        }
        else if (line.charIndex == -1) // 이펙트 분류
        {
            if (line.charName == "sound") // 사운드 + 이펙트만 재생할 경우
            {
                playerDialog.SetActive(false);
                dialogUI.SetActive(false);
                StartCoroutine(startSound());
            }
            else if (line.charName == "background") // 배경 컷씬
            {
                // 대화창 모두 숨기고
                // coroutine
                // 검은색 트랜지션 1회
                // 3초 디스플레이
                // 검은색 트랜지션 1회
                // NextLine();
            }
            else if (line.charName == "animation")
            {
                // animation 타입에 맞춰서 animation1() 등 함수 발동
            }
        }
        else // 주인공 이외의 캐릭터 대사가 나올 경우 (이미지 표시 & 대사 & 이름 변경)
        {
            // 모든 이미지 비활성화 후 해당 인덱스만 활성화
            for (int i = 0; i < charImgs.Length; i++)
            {
                charImgs[i].gameObject.SetActive(i == line.charIndex - 1);
            }
            // 플레이어 선택 버튼을 숨기고, 대화창을 나타나게 함
            playerDialog.SetActive(false);
            dialogUI.SetActive(true);

            // line 입력 (이름 / 대사 / 이미지 / 위치 / 애니메이션 / 사운드 / 브금 / 배경)
            charNameText.text = line.charName;
            dialogText.text = line.dialog;
            SetLocation(line.location, line.charIndex);

            for (int i=0; i < charSprites.Length; i++)
            {
                if (charSprites[i].name == line.imgName)
                {
                    charImgs[line.charIndex].sprite = charSprites[i];
                }
            }
        }
    }

    public void showLog() // 자동으로 로그 텍스트 입력 및 텍스트 박스 사이즈 조정
    {
        int horiSize = 50;
        logText.text = "";

        // Debug.Log(logBoxSize.sizeDelta);
        for (int i = 0; i < currentIndex + 1; i++)
        {
            if (dialogLines[i].dialog != "")
            {
                logText.text += "\n";
                logText.text += "\n";
                if (dialogLines[i].charName != "")
                {
                    logText.text += dialogLines[i].charName;
                    logText.text += ": ";
                }
                logText.text += dialogLines[i].dialog;
                horiSize += spacing;
            }
        }
        logBoxSize.sizeDelta = new Vector2(logBoxSize.sizeDelta.x, horiSize);
    }

    // 다음 대사로 넘어가는 예시 (나중에 버튼에 연결)
    public void NextLine()
    {
        if (currentIndex < dialogLines.Count - 1)
        {
            currentIndex++;
            ShowLine(currentIndex);
            // 사라지는 등장인물 보이지 않게
        }
        else
        {
            Debug.Log("모든 대사 종료");
            Debug.Log("다음 씬으로 전환");
        }
    }
    public void PreviousLine()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            ShowLine(currentIndex);
        }
        else
        {
            Debug.Log("첫 대사 출력 완료");
        }
    }
    void SetLocation(string location, int imgNum)
    {
        if (location == "left")
        {
            charImgs[imgNum].rectTransform.anchoredPosition = new Vector2(-450f, charImgs[imgNum].rectTransform.anchoredPosition.y);
        }
        else if (location == "right")
        {
            charImgs[imgNum].rectTransform.anchoredPosition = new Vector2(450f, charImgs[imgNum].rectTransform.anchoredPosition.y);
        }
        else if (location == "center")
        {
            charImgs[imgNum].rectTransform.anchoredPosition = new Vector2(0f, charImgs[imgNum].rectTransform.anchoredPosition.y);
        }
    }
    private IEnumerator startSound()
    {
        // soundPlayer.audioSource.audioClip = 
        Debug.Log("effect Played");
        yield return new WaitForSeconds(2f);
        NextLine();
    }
}