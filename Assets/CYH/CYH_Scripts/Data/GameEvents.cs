using System;

/// <summary>
/// 게임 전역에서 사용되는 이벤트를 정의하는 클래스입니다.
/// </summary>
public static class GameEvents
{
    // 저장 요청 이벤트 : GameEvents.RequestSave() 를 호출하면 저장 이벤트 발생
    public static event Action OnSaveRequested;
    public static void RequestSave() => OnSaveRequested?.Invoke();
}