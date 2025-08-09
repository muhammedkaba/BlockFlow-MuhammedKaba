using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MKDev
{
    public class GridChecker : MonoBehaviour
    {
        [SerializeField] private LayerMask gridLayer;

        public Block MyBlock;

        public bool IsPlacement = false;

        private void Start()
        {
            this.MyBlock = GetComponentInParent<Block>();
        }

        public bool IsPlaceable(out GridObject gridObject)
        {
            gridObject = null;

            RaycastHit rayHit;

            Physics.Raycast(transform.position, Vector3.down, out rayHit, 4f, gridLayer);

            if (rayHit.collider == null) return false;

            if (rayHit.collider.TryGetComponent(out GridObject g))
            {
                gridObject = g;
                return true;
            }
            return false;
        }
    }
}
