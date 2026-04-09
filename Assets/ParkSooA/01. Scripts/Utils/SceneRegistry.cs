using System.Collections.Generic;

public static class SceneChanger
{
    public static readonly HashSet<string> sceneNames;      // 씬 이름 해시셋

    static SceneChanger()
    {
        sceneNames = new HashSet<string>()
        {
            { "UIScene_P" },
            { "PlatformScene_P" }
        };
    }

    //public static void ChanceScene(string name, out bool value)
    //{
    //}
}