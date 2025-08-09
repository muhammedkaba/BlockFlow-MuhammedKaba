using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.AI;

namespace MKDev
{
    public class Block : MonoBehaviour
    {
        public Vector2Int SizeOnGrids;

        public BlockColor Color;
        public BlockType Type;
        public bool IsLocked = false;
        public int MoveToUnlock = -1;
        [SerializeField] private TextMeshProUGUI lockText;

        public bool IsHorizontal = false;
        public bool IsVertical = false;
        [SerializeField] private GameObject horizontalImage;
        [SerializeField] private GameObject verticalImage;

        private Rigidbody rb;
        private GridChecker[] gridCheckers;

        private void Awake()
        {
            rb = this.GetComponent<Rigidbody>();
            gridCheckers = this.GetComponentsInChildren<GridChecker>();
            rb.isKinematic = true;
        }

        public async void Start()
        {
            await UniTask.DelayFrame(1);
            PlaceObject();

            SetColor(this.Color);

            Canvas objectsCanvas = GetComponentInChildren<Canvas>();

            lockText = objectsCanvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            horizontalImage = objectsCanvas.transform.GetChild(1).gameObject;
            verticalImage = objectsCanvas.transform.GetChild(2).gameObject;

            SetupLockText();

            if (!IsLocked)
            {
                if (IsHorizontal) horizontalImage.SetActive(true);
                if (IsVertical) verticalImage.SetActive(true);
            }
        }
#if UNITY_EDITOR
        public void GenerateGridCheckers()
        {
            foreach (var item in GetComponentsInChildren<GridChecker>())
            {
                if (item.TryGetComponent(out GridChecker _))
                {
                    DestroyImmediate(item.gameObject);
                }
            }

            float xStartPos = transform.position.x - ((SizeOnGrids.x - 1) * 0.5f);
            float zStartPos = transform.position.z - ((SizeOnGrids.y - 1) * 0.5f);

            GridChecker pfGridChecker = GameAssets.i.PfGridChecker;
            for (int i = 0; i < SizeOnGrids.y; i++)
            {
                for (int j = 0; j < SizeOnGrids.x; j++)
                {

                    Vector3 spawnPos = new Vector3(xStartPos + j, transform.position.y + 0.5f, zStartPos + i);
                    GridChecker instantiatedObject = PrefabUtility.InstantiatePrefab(pfGridChecker, this.transform) as GridChecker;

                    instantiatedObject.transform.position = spawnPos;
                    instantiatedObject.transform.rotation = pfGridChecker.transform.rotation;
                    //GridChecker grid = Instantiate(pfGridChecker, spawnPos, pfGridChecker.transform.rotation, this.transform);
                    //grid.GridPosition = new Vector2Int(j, i);

                    //AllGrids.Add(grid);
                }
            }
        }
#endif

        public Rigidbody GetRB() => rb;

        public void PlaceObject()
        {
            if (gridCheckers.All(x => x.IsPlaceable(out GridObject _)))
            {
                gridCheckers[0].IsPlaceable(out GridObject gridObject);

                Vector3 offset = transform.position - gridCheckers[0].transform.position;
                Vector3 pos = gridObject.transform.position + offset;
                pos.y = 0.03f;
                transform.position = pos;
            }
        }

        public void SetColor(BlockColor color)
        {
            if (this.IsLocked)
            {
                GetComponent<Renderer>().material = GameAssets.i.LockedBlockMaterial;
                return;
            }
            GetComponent<Renderer>().material = GameAssets.i.BlockColorMaterials[(int)color];

        }

        public void ReduceLock()
        {
            MoveToUnlock--;
            SetupLockText();
            if (MoveToUnlock == 0)
            {
                UnlockBlock();
            }
        }

        public void UnlockBlock()
        {
            this.IsLocked = false;
            this.SetColor(this.Color);
            if (IsHorizontal) horizontalImage.SetActive(true);
            if (IsVertical) verticalImage.SetActive(true);

            _ = SoundManager.i.PlaySound(SFXType.BlockUnlocked);
            _ = ParticleManager.i.PlayParticle(transform.position + Vector3.up, ParticleType.BlockUnlocked);
        }

        public GridChecker[] GetGridCheckers()
        {
            return gridCheckers;
        }

        public void SetupLockText()
        {
            lockText.text = MoveToUnlock > 0 ? $"{MoveToUnlock.ToString()}" : "";
        }

        public bool IsDestroyable(List<int> grids, DestroyerDirection direction)
        {
            List<GridObject> topOnGrids = new List<GridObject>();

            foreach (var item in gridCheckers)
            {
                if (item.IsPlaceable(out GridObject g))
                {
                    topOnGrids.Add(g);
                }
            }

            return direction == DestroyerDirection.Horizontal ? topOnGrids.All(x => grids.Contains(x.GridPosition.y)) : topOnGrids.All(x => grids.Contains(x.GridPosition.x));
        }

    }

    public enum DestroyerDirection
    {
        Horizontal,
        Vertical
    }

    public enum BlockColor
    {
        Red,
        Blue,
        Green,
        Purple
    }

    public enum BlockType
    {
        Square1x1,
        Line2x1,
        L2x2,
        Square2x2,
        L2x3,
        L2x3R,
        Z2x3,
        Line3x1,
        T3x2,
        Plus3x3
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Block))]
    public class BlockEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var block = (Block)target;

            if (GUILayout.Button("Generate Grid Checkers"))
            {
                block.GenerateGridCheckers();
            }
        }
    }
#endif

}


