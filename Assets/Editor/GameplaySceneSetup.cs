#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Blitz.App;
using Blitz.Core;
using Blitz.Gameplay;
using Blitz.Gameplay.Input;
using Blitz.Gameplay.Minigames;
using Blitz.Gameplay.Navigation;
using Blitz.Gameplay.Content;
using Blitz.Gameplay.Table;
using Blitz.UI.Views;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Blitz.Editor
{
    public static class GameplaySceneSetup
    {
        const string SourceOffline = "Assets/Scenes/30_Gameplay_Offline.unity";
        const string CoreScene = "Assets/Scenes/30_Gameplay_Core.unity";
        const string BlitzScene = "Assets/Scenes/31_Minigame_Blitz.unity";
        const string FantasmaScene = "Assets/Scenes/32_Minigame_Fantasma.unity";
        const string CatalogPath = "Assets/ScriptableObjects/Minigames/MinigameCatalog.asset";
        const string DifficultyCatalogPath = "Assets/ScriptableObjects/Difficulty/DifficultyCatalog.asset";

        [MenuItem("Blitz/Setup Leaderboard Bootstrap")]
        public static void SetupLeaderboardBootstrap()
        {
            const string menuScene = "Assets/Scenes/10_MainMenu.unity";
            if (!File.Exists(menuScene))
            {
                Debug.LogError($"Missing scene: {menuScene}");
                return;
            }

            EditorSceneManager.OpenScene(menuScene, OpenSceneMode.Single);
            if (Object.FindAnyObjectByType<LeaderboardBootstrap>() != null)
            {
                Debug.Log("LeaderboardBootstrap already present in main menu.");
                return;
            }

            var go = new GameObject("LeaderboardBootstrap");
            go.AddComponent<LeaderboardBootstrap>();
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            Debug.Log("Added LeaderboardBootstrap to 10_MainMenu.");
        }

        [MenuItem("Blitz/Setup Difficulty Catalog")]
        public static void SetupDifficultyCatalog()
        {
            Directory.CreateDirectory("Assets/ScriptableObjects/Difficulty");
            var catalog = EnsureDifficultyCatalog();
            WireDifficultyCatalogReferences(catalog);
            AssetDatabase.SaveAssets();
            Debug.Log("Difficulty catalog and scene references updated.");
        }

        [MenuItem("Blitz/Setup Gameplay Scenes (Core + Additive)")]
        public static void SetupAll()
        {
            if (!File.Exists(SourceOffline))
            {
                Debug.LogError($"Missing source scene: {SourceOffline}");
                return;
            }

            Directory.CreateDirectory("Assets/ScriptableObjects/Minigames");

            var source = EditorSceneManager.OpenScene(SourceOffline, OpenSceneMode.Single);

            var table = Object.FindAnyObjectByType<TableRuntimeRegistry>();
            if (table == null)
            {
                Debug.LogError("TableRuntimeRegistry not found in offline scene.");
                return;
            }

            var tableRoot = table.gameObject;

            SaveMinigameScene(BlitzScene, tableRoot, go => go.AddComponent<BlitzOnomatopoeicoMinigame>());
            SaveMinigameScene(FantasmaScene, tableRoot, go =>
            {
                go.AddComponent<FantasmaLadraoMinigame>();
                go.AddComponent<FantasmaWorldGrabInput>();
            });

            EditorSceneManager.OpenScene(SourceOffline, OpenSceneMode.Single);
            table = Object.FindAnyObjectByType<TableRuntimeRegistry>();
            if (table != null)
                Object.DestroyImmediate(table.gameObject);

            var quickStart = Object.FindAnyObjectByType<Blitz.App.OfflineQuickStart>();
            if (quickStart != null)
                Object.DestroyImmediate(quickStart);

            var difficultyCatalog = EnsureDifficultyCatalog();
            var localMatch = Object.FindAnyObjectByType<LocalMatchSession>();
            if (localMatch != null)
            {
                var matchGo = localMatch.gameObject;
                if (matchGo.GetComponent<MinigameServicesHost>() == null)
                    matchGo.AddComponent<MinigameServicesHost>();

                var orchestrator = matchGo.GetComponent<OfflineMinigameOrchestrator>();
                if (orchestrator == null)
                    orchestrator = matchGo.AddComponent<OfflineMinigameOrchestrator>();

                var catalog = EnsureCatalog();
                var so = new SerializedObject(orchestrator);
                so.FindProperty("_catalog").objectReferenceValue = catalog;
                so.FindProperty("_difficultyCatalog").objectReferenceValue = difficultyCatalog;
                so.FindProperty("_servicesHost").objectReferenceValue = matchGo.GetComponent<MinigameServicesHost>();
                so.FindProperty("_session").objectReferenceValue = localMatch;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), CoreScene);
            WireDifficultyCatalogReferences(difficultyCatalog);
            UpdateBuildSettings();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Gameplay scenes: core + Blitz + Fantasma created/updated.");
        }

        static void SaveMinigameScene(string path, GameObject tableRoot, System.Action<GameObject> configureRoot)
        {
            var clone = Object.Instantiate(tableRoot);
            clone.name = tableRoot.name;
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            SceneManager.MoveGameObjectToScene(clone, scene);
            configureRoot(clone);
            EditorSceneManager.SaveScene(scene, path);
            EditorSceneManager.CloseScene(scene, true);
        }

        static DifficultyCatalog EnsureDifficultyCatalog()
        {
            var catalog = AssetDatabase.LoadAssetAtPath<DifficultyCatalog>(DifficultyCatalogPath);
            if (catalog == null)
                catalog = ScriptableObject.CreateInstance<DifficultyCatalog>();

            var easy = EnsureDifficultyProfile(
                "Assets/ScriptableObjects/Difficulty/DifficultyEasy.asset",
                DifficultyIds.Easy,
                "Fácil",
                totalRounds: 5,
                grabWindowSeconds: 3f,
                matchSeedOffset: 0);

            var medium = EnsureDifficultyProfile(
                "Assets/ScriptableObjects/Difficulty/DifficultyMedium.asset",
                DifficultyIds.Medium,
                "Médio",
                totalRounds: 4,
                grabWindowSeconds: 2.75f,
                matchSeedOffset: 1);

            var hard = EnsureDifficultyProfile(
                "Assets/ScriptableObjects/Difficulty/DifficultyHard.asset",
                DifficultyIds.Hard,
                "Difícil",
                totalRounds: 3,
                grabWindowSeconds: 2.5f,
                matchSeedOffset: 2);

            var catalogSo = new SerializedObject(catalog);
            var listProp = catalogSo.FindProperty("_entries");
            listProp.ClearArray();
            listProp.InsertArrayElementAtIndex(0);
            listProp.GetArrayElementAtIndex(0).objectReferenceValue = easy;
            listProp.InsertArrayElementAtIndex(1);
            listProp.GetArrayElementAtIndex(1).objectReferenceValue = medium;
            listProp.InsertArrayElementAtIndex(2);
            listProp.GetArrayElementAtIndex(2).objectReferenceValue = hard;
            catalogSo.ApplyModifiedPropertiesWithoutUndo();

            if (!AssetDatabase.Contains(catalog))
                AssetDatabase.CreateAsset(catalog, DifficultyCatalogPath);

            EditorUtility.SetDirty(catalog);
            return catalog;
        }

        static DifficultyProfile EnsureDifficultyProfile(
            string path,
            string id,
            string displayName,
            int totalRounds,
            float grabWindowSeconds,
            int matchSeedOffset)
        {
            var profile = AssetDatabase.LoadAssetAtPath<DifficultyProfile>(path);
            if (profile == null)
                profile = ScriptableObject.CreateInstance<DifficultyProfile>();

            var so = new SerializedObject(profile);
            so.FindProperty("_difficultyId").stringValue = id;
            so.FindProperty("_displayName").stringValue = displayName;
            so.FindProperty("_totalRounds").intValue = totalRounds;
            so.FindProperty("_grabWindowSeconds").floatValue = grabWindowSeconds;
            so.FindProperty("_matchSeedOffset").intValue = matchSeedOffset;
            so.ApplyModifiedPropertiesWithoutUndo();

            if (!AssetDatabase.Contains(profile))
                AssetDatabase.CreateAsset(profile, path);

            EditorUtility.SetDirty(profile);
            return profile;
        }

        static void WireDifficultyCatalogReferences(DifficultyCatalog catalog)
        {
            if (!File.Exists(CoreScene))
                return;

            var core = EditorSceneManager.OpenScene(CoreScene, OpenSceneMode.Single);
            var orchestrator = Object.FindAnyObjectByType<OfflineMinigameOrchestrator>();
            if (orchestrator != null)
            {
                var so = new SerializedObject(orchestrator);
                so.FindProperty("_difficultyCatalog").objectReferenceValue = catalog;
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorSceneManager.SaveScene(core);
            }

            const string menuScene = "Assets/Scenes/10_MainMenu.unity";
            if (!File.Exists(menuScene))
                return;

            var menu = EditorSceneManager.OpenScene(menuScene, OpenSceneMode.Single);
            var menuView = Object.FindAnyObjectByType<MainMenuView>();
            if (menuView != null)
            {
                var so = new SerializedObject(menuView);
                so.FindProperty("_difficultyCatalog").objectReferenceValue = catalog;
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorSceneManager.SaveScene(menu);
            }
        }

        static MinigameCatalog EnsureCatalog()
        {
            var catalog = AssetDatabase.LoadAssetAtPath<MinigameCatalog>(CatalogPath);
            if (catalog == null)
                catalog = ScriptableObject.CreateInstance<MinigameCatalog>();

            var blitz = EnsureDescriptor(
                "Assets/ScriptableObjects/Minigames/BlitzOnomatopoeico.asset",
                MinigameIds.BlitzOnomatopoeico,
                SceneNames.MinigameBlitz,
                "Blitz Onomatopoeico",
                useBlitzGrabDriver: true);

            var fantasma = EnsureDescriptor(
                "Assets/ScriptableObjects/Minigames/FantasmaLadrao.asset",
                MinigameIds.FantasmaLadrao,
                SceneNames.MinigameFantasma,
                "Fantasma Ladrão",
                useBlitzGrabDriver: false);

            var catalogSo = new SerializedObject(catalog);
            var listProp = catalogSo.FindProperty("_entries");
            listProp.ClearArray();
            listProp.InsertArrayElementAtIndex(0);
            listProp.GetArrayElementAtIndex(0).objectReferenceValue = blitz;
            listProp.InsertArrayElementAtIndex(1);
            listProp.GetArrayElementAtIndex(1).objectReferenceValue = fantasma;
            catalogSo.ApplyModifiedPropertiesWithoutUndo();

            if (!AssetDatabase.Contains(catalog))
                AssetDatabase.CreateAsset(catalog, CatalogPath);

            EditorUtility.SetDirty(catalog);
            return catalog;
        }

        static MinigameDescriptor EnsureDescriptor(
            string path,
            string id,
            string sceneName,
            string displayName,
            bool useBlitzGrabDriver)
        {
            var descriptor = AssetDatabase.LoadAssetAtPath<MinigameDescriptor>(path);
            if (descriptor == null)
                descriptor = ScriptableObject.CreateInstance<MinigameDescriptor>();

            var so = new SerializedObject(descriptor);
            so.FindProperty("_minigameId").stringValue = id;
            so.FindProperty("_additiveSceneName").stringValue = sceneName;
            so.FindProperty("_displayName").stringValue = displayName;
            so.FindProperty("_useBlitzGrabDriver").boolValue = useBlitzGrabDriver;
            so.ApplyModifiedPropertiesWithoutUndo();

            if (!AssetDatabase.Contains(descriptor))
                AssetDatabase.CreateAsset(descriptor, path);

            EditorUtility.SetDirty(descriptor);
            return descriptor;
        }

        static void UpdateBuildSettings()
        {
            var scenes = new List<EditorBuildSettingsScene>
            {
                new("Assets/Scenes/10_MainMenu.unity", true),
                new(CoreScene, true),
                new(BlitzScene, true),
                new(FantasmaScene, true),
                new("Assets/Scenes/40_Results.unity", true),
                new("Assets/Scenes/50_Leaderboard.unity", true),
                new("Assets/Scenes/SampleScene.unity", true),
            };

            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
#endif
