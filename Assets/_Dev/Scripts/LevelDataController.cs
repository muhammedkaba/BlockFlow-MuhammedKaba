using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MKDev
{
    public class LevelDataController : MonoBehaviour
    {
        public LevelData CurrentLevelData;
        public int Level;

        private void Start()
        {
            Application.targetFrameRate = 60;

            int currentLevel = PlayerPrefs.GetInt("Level", 1);

            if (currentLevel > 5)
            {
                currentLevel = Random.Range(2, 6);
            }

            if (Level != -1)
            {
                currentLevel = Level;
            }

            TextAsset jsonFile = Resources.Load<TextAsset>($"Levels/Level{currentLevel}");
            CurrentLevelData = JsonUtility.FromJson<LevelData>(jsonFile.text);

            LoadLevel();
        }

        public void LoadLevel()
        {
            GameManager.i.GameTime = CurrentLevelData.LevelDuration;
            GridManager.i.Initialize(CurrentLevelData.GridData);
            foreach (var item in CurrentLevelData.BlockData)
            {
                GridManager.i.CreateBlock(item);
            }
            foreach (var item in CurrentLevelData.GrinderData)
            {
                GridManager.i.CreateGrinders(item);
            }
        }

        public void SaveLevel()
        {
            string levelToJson = JsonUtility.ToJson(CurrentLevelData);
            System.IO.File.WriteAllText(Application.persistentDataPath + $"/Level{Level}.json", levelToJson);
        }

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(LevelDataController))]
    public class LevelDataControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var levelDataController = (LevelDataController)target;

            if (GUILayout.Button("Save"))
            {
                levelDataController.SaveLevel();
            }

            if (GUILayout.Button("Load"))
            {
                levelDataController.LoadLevel();
            }
        }
    }

#endif


    [System.Serializable]
    public class LevelData
    {
        public int LevelDuration;
        public GridData GridData;
        public BlockData[] BlockData;
        public GrinderData[] GrinderData;
    }

    [System.Serializable]
    public class BlockData
    {
        public BlockColor Color;
        public BlockType Type;
        public Vector2Int BlockPos;
        public bool IsLocked;
        public int UnlockWithMoves;
        public bool IsHorizontal;
        public bool IsVertical;
    }

    [System.Serializable]
    public class GrinderData
    {
        public Vector2Int[] GridPos;
        public Direction Direction;
        public BlockColor Color;
    }
}
