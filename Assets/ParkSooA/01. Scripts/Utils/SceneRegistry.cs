using System.Collections.Generic;

public static class SceneRegistry
{
    public static readonly Dictionary<SceneType, string> sceneNames = new();        // 씬 종류, 이름 딕셔너리

    // 생성자
    static SceneRegistry()
    {
        // 초기화
        sceneNames.Clear();
        // 집
        sceneNames.Add(SceneType.Home, "Home_Scene_P");
        // 지하철
        sceneNames.Add(SceneType.Platform, "Platform_Scene_P");
    }

    // 씬 이름 반환 함수
    public static bool GetSceneName(SceneType type, out string name)
    {
        // 씬 종류가 있다면 true와 씬 이름을, 없다면 false와 null을 반환
        return sceneNames.TryGetValue(type, out name);
    }
}