using System.Collections;
using UnityEngine;
using Photon.Pun;

namespace Gameplay.Player
{
    public class PlayerAttackController : MonoBehaviour
    {
        public bool CanAttack;
        [SerializeField] Animator playerAnim;
        [SerializeField] float normalAttackCd = 1.3f, normalSpwanDelay = 0.7f;
        [SerializeField] Transform playerCam, shootPos;
        [SerializeField] GameObject playerAttacks;

        bool isAttacking = false;
        enum AttackTypes { Normal, Heavy };

        private void Update()
        {
            if (CanAttack)
            {
                InputHandler();
            }
        }

        //lmb norm atk, rmb heavey atk for now
        void InputHandler()
        {
            if (isAttacking) return;
            if (Input.GetMouseButtonDown(0))
            {
                isAttacking = true;
                playerAnim.SetLayerWeight(1, 1);
                playerAnim.SetTrigger("IsAttacking");
                StartCoroutine(ResetAttack(normalAttackCd));
                StartCoroutine(LaunchAttack(normalSpwanDelay, AttackTypes.Normal));
            }
        }

        IEnumerator ResetAttack(float _delay)
        {
            yield return new WaitForSeconds(_delay);
            playerAnim.SetLayerWeight(1, 0);
            isAttacking = false;
        }

        IEnumerator LaunchAttack(float _delay, AttackTypes _atk)
        {
            yield return new WaitForSeconds(_delay);
            var _Temp = PhotonNetwork.Instantiate("PlayerProjectile", shootPos.position,
                playerCam.rotation).GetComponent<ProjectileManager>();
            //var _Temp = Instantiate(playerAttacks, shootPos.position,
            //    playerCam.rotation).GetComponent<ProjectileManager>();

            _Temp.ProjectileType = _atk switch
            {
                AttackTypes.Normal => ProjectileManager.AvailableProjectiles.PlayerLight,
                _ => ProjectileManager.AvailableProjectiles.PlayerLight
            };
        }
    }
}
