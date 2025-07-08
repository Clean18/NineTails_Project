using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpCanvas : MonoBehaviour
{
    // BaseUI 상속한 UI를 스택 구조로 관리
    private Stack<BaseUI> stack = new Stack<BaseUI>();

    public void ShowUI(BaseUI ui)
    {
        // 팝업창이 있을시
        if (stack.Count > 0)
        {
            BaseUI top = stack.Pop(); // 기존에 있던 팝업 제거
            Destroy(top.gameObject);  
        }
        // 새로운 UI를 스택에 추가
        stack.Push(ui);  
    }

    public void CloseUI()
    {
        // 팝업창이 열려있지 않으면 동작하지않음
        if (stack.Count == 0)
            return;

        // 현재 UI를 스택에서 제거
        BaseUI top = stack.Pop();
        Destroy(top.gameObject);

        // 이전 팝업창이 있다면 다시 표시
        if (stack.Count > 0)
        {
            top = stack.Peek();
            top.gameObject.SetActive(true);
        }
    }
    public Stack<BaseUI> GetStack()
    {
        return stack;
    }
}
