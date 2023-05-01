using Gameplay.Player;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyController : MonoBehaviour
{
    public enum AvailableEnemyTypes { Mage, Skeleton, Spider };
    public AvailableEnemyTypes CurtType;
    public Vector3 WanderCenter;
    [SerializeField] int playerDamage = 25;
    [SerializeField] int Hp = 100;
    public enum AvailableAttackTypes { Fireball };

    [System.Serializable]
    public struct AttackDataHolder
    {
        public float AttackCD;
        public float AttackEffectDelay;
        public GameObject AttackProjectile;
        public Transform AttackSpwanPos;
        public string BulletPrefabName;
        public bool UsePunInstantiate;
        public AvailableAttackTypes AttackType;
        public float AttackDamage;
    }
    [SerializeField]
    AttackDataHolder[] attackPatterns;

    [SerializeField]
    float idleSpeed = 2f, attackSpeed = 3f, atttackRange = 4f
        , sightDist = 5f, wanderRadius = 5f, minIdle = 4f, maxIdle = 6f;
    [SerializeField] bool IsActive = true;
    [SerializeField] Transform[] eyePos;

    bool isAttacking, gettingPos = false, canAttack = true;
    NavMeshAgent agent;
    PlayerController[] playerConts;
    [SerializeField] Animator anim;
    Transform curtTarget;
    RPCManager rpcManager;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = isAttacking ? attackSpeed : idleSpeed;
        agent.stoppingDistance = isAttacking ? atttackRange : 0.2f;
        if (!isAttacking)
        {
            agent.SetDestination(GetRandomPos());
        }
        rpcManager = FindObjectOfType<RPCManager>();
    }

    private void Update()
    {
        if (IsActive)
        {
            switch (CurtType)
            {
                case AvailableEnemyTypes.Mage:
                    MageMovementMang();
                    break;
                default:
                    break;
            }
        }
    }

    Vector3 GetRandomPos()
    {
        Vector3 _pos = Vector3.one;
        _pos.x = Random.Range(1, -2) * Random.Range(-wanderRadius, wanderRadius);
        _pos.z = Random.Range(1, -2) * Random.Range(-wanderRadius, wanderRadius);
        _pos.y = 0;
        return _pos+WanderCenter;
    }

    void MageMovementMang()
    {
        if (!isAttacking)
        {
            //wander around radius
            if (agent.remainingDistance < agent.stoppingDistance)
            {
                //reached location
                if (!gettingPos)
                {
                    gettingPos = true;
                    anim.SetBool("idle_normal", true);
                    anim.SetBool("move_forward", false);
                    StartCoroutine(SetPosDelayed(Random.Range(minIdle,
                        maxIdle)));
                }
            }
            else
            {
                //if some action needed on idle
            }
            SightMang();
        }
        else
        {
            var _Temp = Quaternion.Slerp(Quaternion.Euler(0, transform.eulerAngles.y, 0),
                Quaternion.LookRotation(curtTarget.position - transform.position), agent.angularSpeed * Time.deltaTime);//look at player
            _Temp.x = 0; _Temp.z = 0;
            transform.rotation = _Temp;
            if (agent.remainingDistance < agent.stoppingDistance)
            {
                //attack player 
                if (canAttack)
                {
                    switch (CurtType)
                    {
                        case AvailableEnemyTypes.Mage:
                            anim.SetBool("attack_short_001", true);
                            anim.SetBool("move_forward_fast", false);
                            break;
                        default:
                            break;
                    }
                    StartCoroutine(ResetAttack(SetRandomAttack()));
                }
            }
            else
            {
                if (!canAttack)//finish current attack before moving
                    return;
                switch (CurtType)
                {
                    case AvailableEnemyTypes.Mage:
                        anim.SetBool("attack_short_001", false);
                        anim.SetBool("move_forward_fast", true);
                        break;
                    default:
                        break;
                }
                curtTarget = !curtTarget ? GetNearestPlayer() : curtTarget;
            }
        }
    }
    float SetRandomAttack()
    {
        var _tempAtk = attackPatterns[Random.Range(0, attackPatterns.Length)];
        StartCoroutine(SpwanAttackDelayed(_tempAtk.AttackEffectDelay, _tempAtk));
        return _tempAtk.AttackCD;
    }

    void SightMang()
    {
        foreach (var v in eyePos)
        {
            if (Physics.Raycast(v.position, v.transform.forward, out RaycastHit _hit, sightDist))
            {
                if (_hit.collider.CompareTag("Player"))
                {
                    isAttacking = true;
                    playerConts = FindObjectsOfType<PlayerController>();
                    agent.stoppingDistance = atttackRange;
                    agent.speed = attackSpeed;
                    curtTarget = GetNearestPlayer();
                    agent.SetDestination(curtTarget.position);
                    StartCoroutine(TrackPlayerDelayed(0));
                    switch (CurtType)
                    {
                        case AvailableEnemyTypes.Mage:
                            anim.SetBool("idle_normal", false);
                            anim.SetBool("idle_combat", true);

                            break;
                        default:
                            break;
                    }
                }
            }

            Debug.DrawRay(v.position, v.transform.forward * sightDist, Color.red);
        }
    }

    IEnumerator SetPosDelayed(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        agent.SetDestination(GetRandomPos());
        switch (CurtType)
        {
            case AvailableEnemyTypes.Mage:
                anim.SetBool("idle_normal", false);
                anim.SetBool("move_forward", true);
                break;
        }
        gettingPos = false;
    }
    IEnumerator TrackPlayerDelayed(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        curtTarget = GetNearestPlayer();
        agent.SetDestination(curtTarget.position);
    }
    IEnumerator SpwanAttackDelayed(float _delay, AttackDataHolder _atk)
    {
        yield return new WaitForSeconds(_delay);
        _atk.AttackSpwanPos.transform.LookAt(curtTarget);
        if (!_atk.UsePunInstantiate)
        {
            Instantiate(_atk.AttackProjectile, _atk.AttackSpwanPos.position, _atk.AttackSpwanPos.rotation);
            //generate atk particles here if needed
        }
        else
        {
            PhotonNetwork.Instantiate(_atk.BulletPrefabName, _atk.AttackSpwanPos.position, _atk.AttackSpwanPos.rotation);
        }
    }
    IEnumerator ResetAttack(float _delay)
    {
        canAttack = false;
        yield return new WaitForSeconds(_delay);
        canAttack = true;
        agent.SetDestination(curtTarget.position);
    }
    Transform GetNearestPlayer()
    {
        var _tempPos = new List<float>();
        foreach (var v in playerConts)
        {
            _tempPos.Add(Vector3.Distance(transform.position, v.transform.position));
        }
        for (int i = 0; i < _tempPos.Count; i++)
        {
            for (int j = 0; j < _tempPos.Count; j++)
            {
                if (_tempPos[i] > _tempPos[j])
                {
                    var _pos = _tempPos[i];
                    var _trans = playerConts[i];
                    _tempPos[i] = _tempPos[j];
                    playerConts[i] = playerConts[j];
                    _tempPos[j] = _pos;
                    playerConts[j] = _trans;
                }
            }
        }
        return playerConts[0].transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ProjectileManager>().ProjectileType == ProjectileManager.AvailableProjectiles.PlayerLight)
        {
            print("enemy hurt");
            Hp -= playerDamage;
            if (other.GetComponent<PhotonView>().IsMine)
                PhotonNetwork.Destroy(other.gameObject);
            if (Hp <= 0)
            {
                PhotonNetwork.Destroy(gameObject);
                print("I am dead");
            }
        }
    }
}
