using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MKDev
{
    public class GridObject : MonoBehaviour
    {
        public Vector2Int GridPosition;

        public List<GridWall> Walls;

        public bool TryGetNode(Vector2Int direction, out GridObject grid)
        {
            if (GridManager.i.AllGrids.Any(x => x.GridPosition == this.GridPosition + direction))
            {
                grid = GridManager.i.AllGrids.First(x => x.GridPosition == this.GridPosition + direction);
                return true;
            }
            grid = null;
            return false;
        }
    }
}
