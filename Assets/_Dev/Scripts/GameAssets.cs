using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MKDev
{
    public class GameAssets : MonoBehaviour
    {
        private static GameAssets _i;

        public static GameAssets i
        {
            get
            {
                if (_i == null) _i = Instantiate(Resources.Load<GameAssets>("GameAssets"));
                return _i;
            }
        }


        public GridChecker PfGridChecker;

        public Material[] BlockColorMaterials;

        public Material LockedBlockMaterial;

        public Block[] PfBlocks;

        public Block GetBlock(BlockType blockType)
        {
            return PfBlocks.First(x => x.Type == blockType);
        }

    }
}
