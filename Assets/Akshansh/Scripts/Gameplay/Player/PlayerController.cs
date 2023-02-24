using Photon.Pun;
using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerController : MonoBehaviour
    {
        //handles player related events like collision and movements.

        //serialized Fields
        [SerializeField] Transform playerCam;
        [SerializeField] float moveSpeed = 2f, rotSenstivit = 3f, minMouseClamp = -40f, maxMouseClamp = 40f;

        //private fields
        PhotonView view;
        UIManager uiMang;

        bool isActivePlayer = false, canRotateCam = true;
        Vector3 tempMoveDir;
        Vector2 tempMouseRot;

        #region Private Functions
        private void Start()
        {
            view = GetComponent<PhotonView>();
            if (view.IsMine)
            {
                isActivePlayer = true;
                playerCam.gameObject.SetActive(true);
                uiMang = FindObjectOfType<UIManager>();
                uiMang.SetCursorVisibility(false);
            }
        }


        private void FixedUpdate()
        {
            MoveInDirection(tempMoveDir);
        }
        private void Update()
        {
            InputHandler();
        }

        /// <summary>
        /// Handles player inputs
        /// </summary>
        void InputHandler()
        {
            if (!isActivePlayer)
                return;

            //player movement
            tempMoveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));//handled by move in direction fuc

            //mouse rotation
            if (canRotateCam)
            {
                MouseLookHandler();
            }
        }

        /// <summary>
        /// moves player in give vector's direction
        /// </summary>
        /// <param name="_dir"></param>
        void MoveInDirection(Vector3 _dir)
        {
            if (_dir != Vector3.zero)
            {
                transform.Translate(moveSpeed * Time.deltaTime * _dir.normalized);
            }
        }

        /// <summary>
        /// Handles mouse rotation
        /// </summary>
        void MouseLookHandler()
        {
            tempMouseRot.x -= Input.GetAxis("Mouse Y");
            tempMouseRot.y += Input.GetAxis("Mouse X");
            //body
            transform.eulerAngles = new Vector3(0, tempMouseRot.y, 0);
            //camera
            tempMouseRot.x = Mathf.Clamp(tempMouseRot.x, minMouseClamp, maxMouseClamp);
            playerCam.localEulerAngles = new Vector3(tempMouseRot.x, 0, 0);
        }
        #endregion
    }
}
