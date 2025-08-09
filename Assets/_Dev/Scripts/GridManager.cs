using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MKDev
{
    public class GridManager : Singleton<GridManager>
    {
        [SerializeField] private GridObject pfGrid;

        [SerializeField] private GridWall pfWall;
        [SerializeField] private GameObject pfCornerWall;

        [SerializeField] Transform[] envCubesHorizontal;
        [SerializeField] Transform[] envCubesVertical;

        public List<GridObject> AllGrids;

        private void Start()
        {
        }

        public void Initialize(GridData gridData)
        {
            float xStartPos = transform.position.x - ((gridData.XSize - 1) * 0.5f);
            float zStartPos = transform.position.z - ((gridData.YSize - 1) * 0.5f);

            for (int i = 0; i < gridData.YSize; i++)
            {
                for (int j = 0; j < gridData.XSize; j++)
                {
                    Vector3 spawnPos = new Vector3(xStartPos + j, 0f, zStartPos + i);
                    GridObject grid = Instantiate(pfGrid, spawnPos, pfGrid.transform.rotation, this.transform);
                    grid.GridPosition = new Vector2Int(j, i);

                    grid.gameObject.name = $"Grid|{j}x{i}|";

                    AllGrids.Add(grid);
                }
            }



            foreach (var item in AllGrids)
            {
                if (!item.TryGetNode(Vector2Int.right, out _))
                {
                    GridWall g = Instantiate(pfWall, item.transform.position + (Vector3.right + Vector3.back) * 0.5f, Quaternion.Euler(-90f, 0f, 180f), this.transform);
                    g.Direction = Direction.Right;
                    g.YPos = item.GridPosition.y;
                    item.Walls.Add(g);
                }
                if (!item.TryGetNode(Vector2Int.left, out _))
                {
                    GridWall g = Instantiate(pfWall, item.transform.position + (Vector3.left + Vector3.forward) * 0.5f, Quaternion.Euler(-90f, 0f, 0f), this.transform);
                    g.Direction = Direction.Left;
                    g.YPos = item.GridPosition.y;
                    item.Walls.Add(g);
                }
                if (!item.TryGetNode(Vector2Int.up, out _))
                {
                    GridWall g = Instantiate(pfWall, item.transform.position + (Vector3.right + Vector3.forward) * 0.5f, Quaternion.Euler(-90f, 0f, 90f), this.transform);
                    g.Direction = Direction.Top;
                    g.XPos = item.GridPosition.x;
                    item.Walls.Add(g);
                }
                if (!item.TryGetNode(Vector2Int.down, out _))
                {
                    GridWall g = Instantiate(pfWall, item.transform.position + (Vector3.left + Vector3.back) * 0.5f, Quaternion.Euler(-90f, 0f, -90f), this.transform);
                    g.Direction = Direction.Down;
                    g.XPos = item.GridPosition.x;
                    item.Walls.Add(g);
                }

                if (item.GridPosition.x == 0 && item.GridPosition.y == 0)
                {
                    Instantiate(pfCornerWall, item.transform.position + (Vector3.left + Vector3.back) * 0.5f, Quaternion.Euler(-90f, 0f, -180f), this.transform);
                }
                if (item.GridPosition.x == gridData.XSize - 1 && item.GridPosition.y == 0)
                {
                    Instantiate(pfCornerWall, item.transform.position + (Vector3.right + Vector3.back) * 0.5f, Quaternion.Euler(-90f, 0f, 90f), this.transform);
                }
                if (item.GridPosition.x == 0 && item.GridPosition.y == gridData.YSize - 1)
                {
                    Instantiate(pfCornerWall, item.transform.position + (Vector3.left + Vector3.forward) * 0.5f, Quaternion.Euler(-90f, 0f, -90f), this.transform);
                }
                if (item.GridPosition.x == gridData.XSize - 1 && item.GridPosition.y == gridData.YSize - 1)
                {
                    Instantiate(pfCornerWall, item.transform.position + (Vector3.right + Vector3.forward) * 0.5f, Quaternion.Euler(-90f, 0f, 0f), this.transform);
                }
            }

            float xScale = 25.96f - ((gridData.XSize - 4) * 1);
            float yScale = 25.96f - ((gridData.YSize - 4) * 1);

            foreach (var item in envCubesHorizontal)
            {
                Vector3 scale = item.transform.localScale;
                scale.x = xScale;
                item.transform.localScale = scale;
            }
            foreach (var item in envCubesVertical)
            {
                Vector3 scale = item.transform.localScale;
                scale.x = yScale;
                item.transform.localScale = scale;
            }
        }

        public GridObject GetGrid(int x, int y)
        {
            return AllGrids.First(g => g.GridPosition.x == x && g.GridPosition.y == y);
        }

        public void CreateBlock(BlockData blockData)
        {
            Block block = Instantiate(GameAssets.i.GetBlock(blockData.Type));

            GridChecker placement = block.GetGridCheckers().First(x => x.IsPlacement);
            GridObject gridObject = GetGrid(blockData.BlockPos.x, blockData.BlockPos.y);

            Vector3 offset = block.transform.position - placement.transform.position;
            Vector3 pos = gridObject.transform.position + offset;
            pos.y = 0.03f;
            block.transform.position = pos;

            block.Color = blockData.Color;
            block.IsLocked = blockData.IsLocked;
            block.MoveToUnlock = blockData.UnlockWithMoves;
            block.IsHorizontal = blockData.IsHorizontal;
            block.IsVertical = blockData.IsVertical;

            GameManager.i.AllBlocks.Add(block);
        }

        public void CreateGrinders(GrinderData grinderData)
        {
            List<BlockDestroyer> grinders = new List<BlockDestroyer>();
            foreach (var item in grinderData.GridPos)
            {
                if (GetGrid(item.x, item.y).Walls.Any(x => x.Direction == grinderData.Direction))
                {
                    BlockDestroyer bd = GetGrid(item.x, item.y).Walls.First(x => x.Direction == grinderData.Direction).transform.GetChild(0).gameObject.GetComponent<BlockDestroyer>();
                    bd.gameObject.SetActive(true);
                    grinders.Add(bd);
                }
            }

            foreach (var item in grinders)
            {
                item.ConnectedDestroyers = grinders;
                item.Color = grinderData.Color;
            }
        }
    }

    [System.Serializable]
    public class GridData
    {
        public int XSize;
        public int YSize;
    }

}