using MKDev;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MKDev
{
    public class BlockMovement : Singleton<BlockMovement>
    {
        public LayerMask BlockLayer;
        public LayerMask GridLayer;

        private Block currentSelectedBlock;
        private Vector3 currentSelectedBlockOffset;

        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
        }


        private void Update()
        {
            if (GameManager.i.GameEnded) return;

            if (Input.GetMouseButtonDown(0))
            {
                if (currentSelectedBlock == null)
                {
                    Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, BlockLayer))
                    {
                        Debug.Log(hit.collider.name);
                        if (hit.collider.TryGetComponent(out GridChecker gc))
                        {
                            if (gc.MyBlock.IsLocked) return;
                            Block b = gc.MyBlock;
                            currentSelectedBlock = b;
                            currentSelectedBlockOffset = b.transform.position - hit.point;
                            b.GetComponent<Outline>().enabled = true;
                            b.GetRB().isKinematic = false;
                            _ = SoundManager.i.PlaySound(SFXType.BlockSelect);
                        }
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                RemoveBlockSelection();
            }
        }

        private void FixedUpdate()
        {
            if (GameManager.i.GameEnded) return;
            if (Input.GetMouseButton(0))
            {
                if (currentSelectedBlock != null)
                {
                    Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, GridLayer))
                    {
                        Debug.Log(hit.collider.name);
                        Vector3 blockMovePos = hit.point + currentSelectedBlockOffset;
                        Vector3 direction = (blockMovePos - currentSelectedBlock.transform.position);
                        float speed = 10f;

                        direction.y = 0f;

                        //y safety
                        Vector3 blockPos = currentSelectedBlock.transform.position;
                        blockPos.y = 0.1f;
                        currentSelectedBlock.transform.position = blockPos;

                        if (currentSelectedBlock.IsHorizontal) direction.z = 0f;
                        if (currentSelectedBlock.IsVertical) direction.x = 0f;


                        if (direction.magnitude > 0.1f)
                        {
                            currentSelectedBlock.GetComponent<Rigidbody>().velocity = direction * speed;
                        }
                        else
                        {
                            currentSelectedBlock.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        }

                        //Vector3 movePos = Vector3.Lerp(currentSelectedBlock.transform.position, blockMovePos, Time.fixedDeltaTime * 125f);

                        //currentSelectedBlock.GetComponent<Rigidbody>().MovePosition(movePos);
                    }
                }
            }
        }

        public void RemoveBlockSelection()
        {
            if (currentSelectedBlock != null)
            {
                currentSelectedBlock.GetComponent<Outline>().enabled = false;
                currentSelectedBlock.GetRB().isKinematic = true;
                currentSelectedBlock.PlaceObject();
                currentSelectedBlock = null;
                _ = SoundManager.i.PlaySound(SFXType.BlockSelect);
            }
        }
    }
}
