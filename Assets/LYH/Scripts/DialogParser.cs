using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Rendering;

// 데이터 구조 정의
[System.Serializable]
public class DialogLine
{
    public int index;
    public string charName;
    public string imgName;
    public string dialog;
    public string animation;
    public string location;
    public string sound;
    public string bgmusic;
    public string bgImg;
    public string extra1;
}
public class SummaryLine
{
    public string SceneName;
    public string Summary;
}

public class DialogParser : MonoBehaviour
{
    [SerializeField] private TextAsset[] dialogDatas = new TextAsset[21];
    [SerializeField] private TextAsset dialogData;
    [SerializeField] private TextAsset summaryData;
    [SerializeField] private TextMeshProUGUI charNameText;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private TextMeshProUGUI summary;

    [SerializeField] private GameObject dialogUI;
    [SerializeField] private GameObject cantskipUI;
    [SerializeField] private GameObject playerDialog;

    [SerializeField] private Image blackTransition;
    [SerializeField] private Image effect;

    [SerializeField] private RectTransform logBoxSize;

    [SerializeField] private TMP_InputField changeNameUI;
    [SerializeField] private TextMeshProUGUI changeNameInstruction;
    [SerializeField] private TextAsset badNames;

    [SerializeField] private TextMeshProUGUI playerDialogBox;
    [SerializeField] private TextMeshProUGUI logText;

    [SerializeField] private AudioSource soundPlayer;
    [SerializeField] private AudioSource bgSoundPlayer;

    [SerializeField] private Image[] charImgs = new Image[5];
    [SerializeField] private Image bgImg;

    [SerializeField] private Sprite[] charSprites = new Sprite[14];
    [SerializeField] private Sprite[] bgSprites = new Sprite[3];
    [SerializeField] private Sprite[] effects = new Sprite[5];

    [SerializeField] private AudioClip[] sounds = new AudioClip[20];
    [SerializeField] private AudioClip[] bgSounds = new AudioClip[2];

    [SerializeField] private String[] charNames = new string[20];

    [SerializeField] private Button nextButton;

    [SerializeField] private string dialogName;
    // foxName 입력값 저장해놓는 변수 -> TODO SetPlayerName(foxNameInput) 해줄 필요가 있습니다.
    // 플레이어가 정해주는 구미호의 이름 -> 저장된 데이터에서 가져오는 것과 데이터에 저장하는 두가지가 구현되어야 합니다.
    [SerializeField] private string foxName;
    [SerializeField] private int spacing;
    [SerializeField] private float fadeDuration = 0.5f;

    private List<DialogLine> dialogLines = new List<DialogLine>();
    private List<SummaryLine> summaries = new List<SummaryLine>();
    private List<String> notValidNames = new List<String>();

    private int currentIndex = 0;
    private int charIndex = 0;
    private bool isNextClicked = false;
    
    public string foxNameInput;

    void Start()
    {
        // ----------------------------#############################
        // 플레이어 이름 입력받기
        foxName = PlayerController.Instance.GetPlayerName();

        // dialogName = 입력값 (추가완료)
        dialogName = SceneChangeManager.Instance._stageInfo[PlayerController.Instance.GetPlayerSceneIndex()];
        Debug.Log($"dialogName : {dialogName} / SceneIndex : {PlayerController.Instance.GetPlayerSceneIndex()}");
        // csv 파싱
        for (int i = 0; i < dialogDatas.Length; i++)
        {
            if (dialogDatas[i].name == dialogName)
            {
                dialogData = dialogDatas[i];
            }
        }
        nextButton.onClick.AddListener(() => isNextClicked = true);
        changeNameUI.onEndEdit.AddListener(changeFoxName);
        LoadDialog();
        ShowLine(0);
        for (int i = 0; i < summaries.Count; i++)
        {
            if (summaries[i].SceneName == dialogName)
            {
                summary.text = summaries[i].Summary;
            }
        }
    }

    void LoadDialog()
    {
        string[] lines = dialogData.text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        // 첫 줄은 헤더이므로 생략
        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');

            // CSV 필드가 누락되었을 경우 방지
            while (values.Length < 10)
            {
                System.Array.Resize(ref values, 10);
            }

            DialogLine line = new DialogLine
            {
                index = int.Parse(values[0]),
                charName = values[1],
                imgName = values[2],
                dialog = values[3].Replace("`", ","), // ` 를 ,로 변환
                animation = values[4],
                location = values[5],
                sound = values[6],
                bgmusic = values[7],
                bgImg = values[8],
                extra1 = values[9],
            };

            dialogLines.Add(line);
        }

        Debug.Log($"총 {dialogLines.Count}개의 대사가 로드되었습니다.");

        string[] summaryLines = summaryData.text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        // 첫 줄은 헤더이므로 생략
        for (int i = 1; i < summaryLines.Length; i++)
        {
            string[] values = summaryLines[i].Split(',');

            // CSV 필드가 누락되었을 경우 방지
            while (values.Length < 2)
            {
                System.Array.Resize(ref values, 2);
            }

            SummaryLine line = new SummaryLine
            {
                SceneName = values[0],
                Summary = values[1].Replace("ENTER","\n").Replace("`", ",").Replace("foxName",foxName),
            };

            summaries.Add(line);
        }

        Debug.Log($"총 {summaries.Count}개의 줄거리가 로드되었습니다.");
        // ---------------------------------------------------------------
        string[] badNameCsv = badNames.text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < badNameCsv.Length; i++)
        {
            notValidNames.Add(badNameCsv[i]);
        }
    }

    void ShowLine(int index) // 다음 대사를 표시하는 버튼을 눌렀을때 행동하는 함수
    {
        if (index < 0 || index >= dialogLines.Count)
        {
            Debug.LogWarning("대사 인덱스 범위를 벗어남");
            return;
        }
        // ----------------------------------------------------------------------------------------------------------------
        DialogLine line = dialogLines[index];
        // ----------------------------------------------------------------------------------------------------------------
        charIndex = 20;
        for (int i = 0; i < charNames.Length; i++)
        {
            if (charNames[i] == line.charName)
            {
                charIndex = i;
            }
        }
        if (charIndex <= 1)
        {
            charIndex = 0;
        }
        else if (charIndex <= 3)
        {
            charIndex = 1;
        }
        else if (charIndex <= 7)
        {
            charIndex = 2;
        }
        else if (charIndex <= 11)
        {
            charIndex = 3;
        }
        else if (charIndex == 12)
        {
            charIndex = 4;
        }
        else if (charIndex == 13)
        {
            charIndex = 5;
        }
        else if (charIndex == 14)
        {
            charIndex = 2;
        }
        else
        {
            charIndex = -1;
        }
        Debug.Log($"{line.charName}, {charIndex}");
        // ----------------------------------------------------------------------------------------------------------------
        // 브금 재생 기능
        if (line.bgmusic != "")
        {
            if (line.bgmusic == "stop")
            {
                Debug.Log("브금 정지");
                bgSoundPlayer.Stop();
            }
            else
            {
                for (int i = 0; i < bgSounds.Length; i++)
                {
                    if (bgSounds[i].name == line.bgmusic && line.bgmusic != bgSoundPlayer.clip.name)
                    {
                        Debug.Log("브금 재생");
                        bgSoundPlayer.clip = bgSounds[i];
                        bgSoundPlayer.Play();
                    }
                }
            }
        }
        // ----------------------------------------------------------------------------------------------------------------
        // 배경지정
        if (charIndex != -1)
        {
            for (int i = 0; i < bgSprites.Length; i++)
            {
                if (bgSprites[i].name == line.bgImg)
                {
                    bgImg.sprite = bgSprites[i];
                    Debug.Log(line.bgImg);
                }
            }
        }
        // ----------------------------------------------------------------------------------------------------------------
        // 대사 관리
        if (charIndex == 0) // 주인공 (플레이어)의 대사가 나올 경우
        {
            // 모든 이미지 비활성화
            for (int i = 0; i < charImgs.Length; i++)
            {
                charImgs[i].gameObject.SetActive(false);
            }
            // 대화창을 숨기고 플레이어 선택 버튼을 생성
            playerDialog.SetActive(true);
            dialogUI.SetActive(false);
            // line 입력 (대사 / 사운드)
            playerDialogBox.text = line.dialog.Replace("foxName", foxName);
            for (int i = 0; i < sounds.Length; i++)
            {
                if (sounds[i].name == line.sound && line.sound != "")
                {
                    soundPlayer.clip = sounds[i];
                    soundPlayer.Play();
                }
            }
        }
        else if (charIndex == -1) // 이펙트 분류
        {
            // 모든 캐릭터 이미지 비활성화
            for (int i = 0; i < charImgs.Length; i++)
            {
                charImgs[i].gameObject.SetActive(false);
            }
            if (line.charName == "sound") // 사운드 + 이펙트만 재생할 경우
            {
                // Debug.Log(line.sound);
                StartCoroutine(startSound(line.sound, line.imgName));
            }
            else if (line.charName == "background") // 배경 컷씬
            {
                if (index < dialogLines.Count - 1) StartCoroutine(startBackground(line.bgImg, dialogLines[index + 1].bgImg));
                else StartCoroutine(startBackground(line.bgImg, line.bgImg));
            }
        }
        else if (charIndex == 4) // 내레이션 분류
        {
            // 모든 캐릭터 이미지 비활성화
            for (int i = 0; i < charImgs.Length; i++)
            {
                charImgs[i].gameObject.SetActive(false);
            }
            playerDialog.SetActive(false);
            dialogUI.SetActive(true);
            charNameText.text = "";
            dialogText.text = line.dialog.Replace("foxName", foxName);
        }
        else if (charIndex == 5) // 이름 지어주기
        {
            // 모든 캐릭터 이미지 비활성화
            for (int i = 0; i < charImgs.Length; i++)
            {
                charImgs[i].gameObject.SetActive(false);
            }
            // UI 모두 가리고
            playerDialog.SetActive(false);
            dialogUI.SetActive(false);
            // 여우 이름 입력받는 UI 실행
            changeNameUI.gameObject.SetActive(true);
        }
        else // 주인공 이외의 캐릭터 대사가 나올 경우 (이미지 표시 & 대사 & 이름 변경)
        {
            // 모든 이미지 비활성화 후 해당 인덱스만 활성화
            for (int i = 0; i < charImgs.Length; i++)
            {
                charImgs[i].gameObject.SetActive(i == charIndex - 1);
            }
            // 플레이어 선택 버튼을 숨기고, 대화창을 나타나게 함
            playerDialog.SetActive(false);
            dialogUI.SetActive(true);

            // line 입력 (이름 / 대사 / 이미지 / 위치 / 애니메이션 / 사운드)
            charNameText.text = line.charName.Replace("foxName", foxName);
            dialogText.text = line.dialog.Replace("foxName", foxName);
            // 이미지 지정
            for (int i = 0; i < charSprites.Length; i++)
            {
                if (charSprites[i].name == line.imgName)
                {
                    charImgs[charIndex - 1].sprite = charSprites[i];
                }
            }
            // 위치 지정
            SetLocation(line.location, charIndex - 1);
            // 애니메이션이 있을 경우 애니메이션 발동
            if (line.animation != "")
            {
                StartCoroutine(startAnimation(charIndex, line.animation));// animation 타입에 맞춰서 animation1() 등 함수 발동
            }
            // 사운드 있을 경우 사운드 발동
            for (int i = 0; i < sounds.Length; i++)
            {
                if (sounds[i].name == line.sound && line.sound != "")
                {
                    soundPlayer.clip = sounds[i];
                    soundPlayer.Play();
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
                logText.text += "      ";
                if (dialogLines[i].charName != "" && dialogLines[i].charName != "sound")
                {
                    logText.text += dialogLines[i].charName.Replace("foxName", foxName);
                    logText.text += ": ";
                }
                logText.text += dialogLines[i].dialog.Replace("foxName", foxName);
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
            int curIndex = PlayerController.Instance.GetPlayerSceneIndex();
            if (curIndex == 5 || curIndex == 14) SceneChangeManager.Instance.LoadPrevScene();
            else SceneChangeManager.Instance.LoadNextScene();
        }
    }

    public void NextScene()
    {
        if (dialogData.name == "Stage1-1_Last")
        {
            cantskipUI.SetActive(true);
        }
        else
        {
            int curIndex = PlayerController.Instance.GetPlayerSceneIndex();
            if (curIndex == 5 || curIndex == 14) SceneChangeManager.Instance.LoadPrevScene();
            else SceneChangeManager.Instance.LoadNextScene();
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
    public void changeFoxName(string name)
    {
        bool validName = true;
        // 사용불가능 이름 리스트 필터
        for (int i = 0; i < notValidNames.Count; i++)
        {
            if (name.Contains(notValidNames[i])) validName = false;
            changeNameInstruction.text = "잘못된 이름입니다.\n신중히 결정해주세요.\n(Enter로 확정)";
        }
        // 이름 길이 제한
        int byteCount = System.Text.Encoding.UTF8.GetByteCount(name);
        if (byteCount > 20)
        {
            validName = false;
            changeNameInstruction.text = "이름이 너무 깁니다.\n다른 이름으로 정해주세요.\n(Enter로 확정)";
            return;
        }

        if (validName)
        {
            foxName = name;
            Debug.Log(name);
            // ----------------------------#############################
            // 플레이어 이름값 저장
            PlayerController.Instance.SetPlayerName(foxName);

            // UI 끄고 다음 대사 출력
            changeNameUI.gameObject.SetActive(false);
            NextLine();
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
    private IEnumerator startSound(string name, string imgName)
    {
        // 모든 이미지 비활성화
        for (int i = 0; i < charImgs.Length; i++)
        {
            charImgs[i].gameObject.SetActive(false);
        }

        playerDialog.SetActive(false);
        dialogUI.SetActive(false);
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == name)
            {
                soundPlayer.clip = sounds[i];
            }
        }

        for (int i = 0; i < effects.Length; i++)
        {
            if (effects[i].name == imgName)
            {
                effect.sprite = effects[i];
            }
        }
        Debug.Log($"사운드명: {name}");
        soundPlayer.Play();

        if (imgName != "")
        {
            effect.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            effect.gameObject.SetActive(false);
        }
    }
    private IEnumerator startBackground(string bgName, string nextBgName)
    {
        isNextClicked = false;
        playerDialog.SetActive(false);
        dialogUI.SetActive(false);
        blackTransition.gameObject.SetActive(true);

        Color startColor = blackTransition.color;
        Sprite sprite = bgImg.sprite;
        float elapsed = 0f;

        // 페이드 인
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            blackTransition.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }
        // 중간 대기 & 배경 전환
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < bgSprites.Length; i++)
        {
            if (bgSprites[i].name == bgName)
            {
                bgImg.sprite = bgSprites[i];
            }
        }
        // 페이드 아웃
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            blackTransition.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        // 버튼 클릭 대기
        while (!isNextClicked)
        {
            yield return null;
        }

        // 페이드 인
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            blackTransition.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }
        // 트랜지션 다음에 나올 이미지가 다를 경우에 추가 트랜지션 효과
        if (bgName != nextBgName)
        {
            // 중간 대기 & 배경 원상복구
            yield return new WaitForSeconds(0.2f);
            for (int i = 0; i < bgSprites.Length; i++)
            {
                if (bgSprites[i].name == nextBgName)
                {
                    bgImg.sprite = bgSprites[i];
                }
            }
            // 페이드 아웃
            elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                blackTransition.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                yield return null;
            }
        }
        blackTransition.gameObject.SetActive(false);
        isNextClicked = false;
        NextLine();
    }

    private IEnumerator startAnimation(int charIndex, string animationType)
    {
        Debug.Log(animationType + ": 실행");

        if (animationType == "tremble")
        {
            RectTransform target = charImgs[charIndex - 1].GetComponent<RectTransform>();
            Vector2 originalPos = target.anchoredPosition;

            float distance = 50f;
            float duration = 0.5f; // 총 왕복 시간
            float halfDuration = duration / 2f;

            // 위로 이동 (부드럽게)
            float elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / halfDuration;
                target.anchoredPosition = Vector2.Lerp(originalPos, originalPos + Vector2.up * distance, t);
                yield return null;
            }

            // 아래로 이동 (부드럽게)
            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / halfDuration;
                target.anchoredPosition = Vector2.Lerp(originalPos + Vector2.up * distance, originalPos, t);
                yield return null;
            }

            target.anchoredPosition = originalPos;
        }

        yield return null;
    }

}