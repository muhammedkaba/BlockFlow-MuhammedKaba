using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MKDev
{
    public class BlockDestroyer : MonoBehaviour
    {
        public Direction Direction;

        [SerializeField] ParticleSystem grindParticle;

        public int XPos = -1;
        public int YPos = -1;

        public BlockColor Color;

        public List<BlockDestroyer> ConnectedDestroyers;

        private GridWall ownerWall;

        private async void Start()
        {
            await UniTask.DelayFrame(1);

            ownerWall = GetComponentInParent<GridWall>();

            this.Direction = ownerWall.Direction;
            this.XPos = ownerWall.XPos;
            this.YPos = ownerWall.YPos;

            this.SetColor(this.Color);
        }

        public void SetColor(BlockColor color)
        {
            Debug.Log(color.ToString());

            this.Color = color;
            Material mat = GameAssets.i.BlockColorMaterials[(int)color];

            List<Material> mats = new List<Material>() { mat, mat };
            GetComponent<MeshRenderer>().SetMaterials(mats);

            grindParticle.GetComponent<ParticleSystemRenderer>().material = mat;
        }

        public void TryDestroyBlock(Block block)
        {
            if (block.Color != this.Color) return;

            if (Direction == Direction.Right || Direction == Direction.Left)
            {
                if (!block.IsDestroyable(ConnectedDestroyers.Select(x => x.YPos).ToList(), DestroyerDirection.Horizontal))
                {
                    return;
                }
            }
            else
            {
                if (!block.IsDestroyable(ConnectedDestroyers.Select(x => x.XPos).ToList(), DestroyerDirection.Vertical))
                {
                    return;
                }
            }


            DestroyBlock(block);
        }

        public async UniTaskVoid DestroyBlock(Block block)
        {
            BlockMovement.i.RemoveBlockSelection();

            foreach (var item in block.GetComponentsInChildren<Collider>())
            {
                item.enabled = false;
            }

            await UniTask.NextFrame();


            float pos = (this.Direction == Direction.Top || this.Direction == Direction.Down) ? block.SizeOnGrids.y : block.SizeOnGrids.x;

            Vector3[] directions = { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };
            Vector3[] offsetDirections = { Vector3.right, Vector3.back, Vector3.left, Vector3.forward };

            Vector3 startPos = transform.position;

            if (ConnectedDestroyers.Count > 1)
            {
                startPos = ConnectedDestroyers.Select(x => x.transform.position).Aggregate(Vector3.zero, (current, pos) => current += pos) / ConnectedDestroyers.Count;
            }

            block.transform.position = startPos + (offsetDirections[(int)this.Direction] * 0.5f) - (directions[(int)this.Direction] * pos * 0.5f);
            Vector3 movePos = startPos + (offsetDirections[(int)this.Direction] * 0.5f) + (directions[(int)this.Direction] * pos * 0.8f);

            foreach (var item in ConnectedDestroyers)
            {
                item.grindParticle.Play();
            }

            _ = SoundManager.i.PlaySoundLoop(SFXType.GrindingBlock, 1f);
            await block.transform.DOMove(movePos, 1f).SetEase(Ease.Linear).ToUniTask();

            foreach (var item in ConnectedDestroyers)
            {
                item.grindParticle.Stop();
            }

            GameManager.i.BlockDestroyed.Invoke(block);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out GridChecker gc))
            {
                if (gc.MyBlock)
                {
                    TryDestroyBlock(gc.MyBlock);
                }
            }
        }
    }

    public enum Direction
    {
        Top,
        Right,
        Down,
        Left
    }
}
