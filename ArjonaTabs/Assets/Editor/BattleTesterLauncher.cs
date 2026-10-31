using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Temporary: opens Progra and enters Play mode once so BattleTester can run the battles
[InitializeOnLoad]
public static class BattleTesterLauncher
{
    const string MarkerPath = "Library/battle_tests.txt";
    const string ScenePath = "Assets/Scenes/Progra.unity";

    static BattleTesterLauncher()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        EditorApplication.delayCall += TryStart;
        EditorApplication.update += Poll;
    }

    static double nextCheck;
    static bool warned;

    // Checks for a new test list every 2 seconds, so no recompile is needed to start
    static void Poll()
    {
        if (EditorApplication.timeSinceStartup < nextCheck)
        {
            return;
        }

        nextCheck = EditorApplication.timeSinceStartup + 2;
        TryStart();
    }

    static void TryStart()
    {
        if (!File.Exists(MarkerPath) || EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || SessionState.GetBool("BattleTesterStarted", false))
        {
            return;
        }

        // Compile any changed script first: a recompile during Play mode would break the tests
        AssetDatabase.Refresh();
        if (EditorApplication.isCompiling)
        {
            return;
        }

        var active = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (active.path != ScenePath)
        {
            if (active.isDirty)
            {
                if (!warned) Debug.LogWarning("[BattleTester] Guarda la escena abierta (Ctrl+S) para empezar las pruebas.");
                warned = true;
                return;
            }
            EditorSceneManager.OpenScene(ScenePath);
        }

        SessionState.SetBool("BattleTesterStarted", true);
        Debug.Log("[BattleTester] Empezando las pruebas de combate");
        EditorApplication.EnterPlaymode();
    }

    static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode && SessionState.GetBool("BattleTesterStarted", false))
        {
            SessionState.SetBool("BattleTesterStarted", false);
            if (File.Exists(MarkerPath)) File.Delete(MarkerPath);
            Debug.Log("[BattleTester] Pruebas terminadas");
        }
    }
}
