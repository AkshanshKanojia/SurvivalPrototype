using Photon.Pun;
using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerController : MonoBehaviour
    {
        //handles player related events like collision and movements.

        //serialized Fields
        [SerializeField] Transform playerCam;
        [SerializeField] bool canSprint = true;
        [SerializeField]
        float moveSpeed = 2f, rotSenstivit = 3f, minMouseClamp = -40f, maxMouseClamp = 40f, sprintSpeed = 3f,
            maxStaminaAvailable = 10f, staminaConsumptionRate = 1, staminaRestoreDelay = 2f, staminaRestoreRate = 1;
        [SerializeField] Animator playerAnim;

        //private fields
        PhotonView view;
        UIManager uiMang;

        float tempSpeed;//used to store default speed
        float curntStamina, staminaRestoreTimer = 0;
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
                uiMang = FindObjectOfType<UIManager>();
                uiMang.SetCursorVisibility(false);
                tempSpeed = moveSpeed;
                curntStamina = maxStaminaAvailable;
            }
            else
            {
                playerCam.gameObject.SetActive(false);
                GetComponent<PlayerAttackController>().CanAttack = false;
            }
        }


        private void FixedUpdate()
        {
            if (isActivePlayer)
                MoveInDirection(tempMoveDir);
        }
        private void Update()
        {
            InputHandler();
            if (isActivePlayer)
            {
                RegenPropHandler();
            }
        }

        /// <summary>
        /// Handles player inputs
        /// </summary>
        void InputHandler()
        {
            if (!isActivePlayer)
                return;
            //custom inputs
            if (Input.GetKeyDown(KeyCode.F3))
            {
                uiMang.SetCursorVisibility(true);
            }

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
            if (_dir != Vector3.zero)//only if moving
            {
                //sprint
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    if (curntStamina > 0)
                    {
                        //wait before restoring stamina
                        staminaRestoreTimer = 0;
                        moveSpeed = sprintSpeed;
                        curntStamina -= Time.deltaTime * staminaConsumptionRate;
                    }
                    else
                    {
                        moveSpeed = tempSpeed;
                    }
                }
                else if (moveSpeed == sprintSpeed)//if already sprinting
                {
                    moveSpeed = tempSpeed;
                }

                //apply movement
                transform.Translate(moveSpeed * Time.deltaTime * _dir.normalized);
                playerAnim.SetBool("IsWalking", true);
            }
            else
            {
                playerAnim.SetBool("IsWalking", false);
            }
        }

        /// <summary>
        /// controls properties which restore overtiem like mana and stamina.
        /// </summary>
        void RegenPropHandler()
        {
            if (staminaRestoreTimer < staminaRestoreDelay)
            {
                staminaRestoreTimer += Time.deltaTime;
            }
            else
            {
                if (curntStamina < maxStaminaAvailable)
                {
                    curntStamina += Time.deltaTime * staminaRestoreRate;
                }
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

        #region Public Functions
        public float GetStamina()
        {
            return curntStamina;
        }
        public void AddToStamina(float _value)
        {
            curntStamina += _value;
            curntStamina = Mathf.Clamp(curntStamina, 0, maxStaminaAvailable);
        }
        #endregion
    }
}
