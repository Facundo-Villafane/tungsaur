using UnityEngine;
using UnityEditor;
using System.IO;

namespace CDG.Editor
{
    /// <summary>
    /// Unity Editor tool to automatically create the recommended folder structure for ScriptableObjects
    /// Menu: Tools > CDG > Create ScriptableObject Folders
    /// </summary>
    public class FolderStructureCreator : EditorWindow
    {
        private bool createLevels = true;
        private bool createStages = true;
        private bool createWaves = true;
        private bool createBosses = true;
        private bool createCinematics = true;
        private bool createTutorial = true;
        private bool createDialogues = true;

        private int numberOfLevels = 2;

        [MenuItem("Tools/CDG/Create ScriptableObject Folders")]
        public static void ShowWindow()
        {
            GetWindow<FolderStructureCreator>("Folder Creator");
        }

        private void OnGUI()
        {
            GUILayout.Label("ScriptableObject Folder Structure Creator", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "This will create the recommended folder structure for your ScriptableObjects.\n" +
                "Existing folders will not be affected.",
                MessageType.Info
            );

            GUILayout.Space(10);

            GUILayout.Label("Number of Levels:", EditorStyles.label);
            numberOfLevels = EditorGUILayout.IntSlider(numberOfLevels, 1, 10);

            GUILayout.Space(10);

            GUILayout.Label("Folders to Create:", EditorStyles.boldLabel);
            createLevels = EditorGUILayout.Toggle("Levels", createLevels);
            createStages = EditorGUILayout.Toggle("Stages", createStages);
            createWaves = EditorGUILayout.Toggle("Waves", createWaves);
            createBosses = EditorGUILayout.Toggle("Bosses", createBosses);
            createCinematics = EditorGUILayout.Toggle("Cinematics", createCinematics);
            createTutorial = EditorGUILayout.Toggle("Tutorial", createTutorial);
            createDialogues = EditorGUILayout.Toggle("Dialogues", createDialogues);

            GUILayout.Space(20);

            if (GUILayout.Button("Create Folder Structure", GUILayout.Height(30)))
            {
                CreateFolders();
            }
        }

        private void CreateFolders()
        {
            string basePath = "Assets/ScriptableObjects";
            string dialoguesPath = "Assets/Dialogues";

            // Create base ScriptableObjects folder
            CreateFolder("Assets", "ScriptableObjects");

            int foldersCreated = 0;

            // Levels
            if (createLevels)
            {
                CreateFolder(basePath, "Levels");
                foldersCreated++;
            }

            // Stages
            if (createStages)
            {
                CreateFolder(basePath, "Stages");
                for (int i = 1; i <= numberOfLevels; i++)
                {
                    CreateFolder($"{basePath}/Stages", $"Level{i}");
                }
                CreateFolder($"{basePath}/Stages", "Tutorial");
                foldersCreated++;
            }

            // Waves
            if (createWaves)
            {
                CreateFolder(basePath, "Waves");
                CreateFolder($"{basePath}/Waves", "Easy");
                CreateFolder($"{basePath}/Waves", "Medium");
                CreateFolder($"{basePath}/Waves", "Hard");
                foldersCreated++;
            }

            // Bosses
            if (createBosses)
            {
                CreateFolder(basePath, "Bosses");
                for (int i = 1; i <= numberOfLevels; i++)
                {
                    CreateFolder($"{basePath}/Bosses", $"Level{i}");
                }
                foldersCreated++;
            }

            // Cinematics
            if (createCinematics)
            {
                CreateFolder(basePath, "Cinematics");
                CreateFolder($"{basePath}/Cinematics", "Intros");
                CreateFolder($"{basePath}/Cinematics", "Outros");
                CreateFolder($"{basePath}/Cinematics", "Bosses");
                CreateFolder($"{basePath}/Cinematics", "Stages");
                foldersCreated++;
            }

            // Tutorial
            if (createTutorial)
            {
                CreateFolder(basePath, "Tutorial");
                foldersCreated++;
            }

            // Dialogues
            if (createDialogues)
            {
                CreateFolder("Assets", "Dialogues");
                CreateFolder(dialoguesPath, "Cinematics");
                CreateFolder(dialoguesPath, "Tutorial");
                CreateFolder(dialoguesPath, "NPCs");
                foldersCreated++;
            }

            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog(
                "Folder Structure Created",
                $"Successfully created {foldersCreated} folder categories!\n\n" +
                $"Base path: {basePath}\n" +
                $"Dialogue path: {dialoguesPath}",
                "OK"
            );

            Debug.Log($"[FolderStructureCreator] Created folder structure with {numberOfLevels} levels");
        }

        private void CreateFolder(string parentPath, string folderName)
        {
            string fullPath = $"{parentPath}/{folderName}";

            // Check if folder already exists
            if (!AssetDatabase.IsValidFolder(fullPath))
            {
                AssetDatabase.CreateFolder(parentPath, folderName);
                Debug.Log($"[FolderStructureCreator] Created folder: {fullPath}");
            }
            else
            {
                Debug.Log($"[FolderStructureCreator] Folder already exists: {fullPath}");
            }
        }
    }
}
