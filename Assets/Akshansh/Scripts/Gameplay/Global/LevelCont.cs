using UnityEngine;
using System.Collections;
using Photon.Pun;

public class LevelCont : MonoBehaviour
{
    [SerializeField] GameObject gameoverUI;
    [SerializeField] SkyboxManager skyMang;
    [SerializeField] float CycleDuration = 0.1f;
    PhotonView ownerView;
    RPCManager playerRpc;
    bool isDay =true;
    [SerializeField] string enemyObj;
    [SerializeField] Transform[] groupSpwanPos;
    [SerializeField] int minEnemyGroups = 10, minEnemyAmmount = 3, maxEnemyAmmount = 8;

    bool isClient;
    private void Start()
    {
        SetPlayer();
        if(isClient)
        {
            StartCoroutine(SwitchCycle());
            GenerateEnemyGroups();
        }
    }
    void GenerateEnemyGroups()
    {
        int _groupAmmount = Random.Range(minEnemyGroups, groupSpwanPos.Length);
        for(int i =0;i<_groupAmmount;i++)
        {
            var _tempEnemyCounts = Random.Range(minEnemyAmmount, maxEnemyAmmount);
            for(int j =0;j<_tempEnemyCounts;j++)
            {
                var _temp = PhotonNetwork.Instantiate(enemyObj, groupSpwanPos[i].position,Quaternion.identity).GetComponent<
                    EnemyController>();
                _temp.WanderCenter = groupSpwanPos[i].position;
            }
        }
    }
    public void GameOver()
    {
        gameoverUI.SetActive(true);
    }
    public void LoadScene(int index)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(index);
        PhotonNetwork.Disconnect();
        PhotonNetwork.Reconnect();
    }
    void SetPlayer()
    {
        GameObject _temp = PhotonNetwork.Instantiate("Player",new Vector3(0,2,0),Quaternion.identity);
        if(_temp.GetComponent<PhotonView>().IsMine)
        {
            _temp.transform.GetChild(0).gameObject.SetActive(true);//camera
            ownerView = _temp.GetComponent<PhotonView>();
            isClient = PhotonNetwork.IsMasterClient;
            playerRpc = _temp.GetComponent<RPCManager>();
        }
    }
    
    IEnumerator SwitchCycle()
    {
        yield return new WaitForSeconds(CycleDuration*60);
        isDay = !isDay;
        playerRpc.UpdateDay(isDay);
        StartCoroutine(SwitchCycle());
    }

    public void SetCycle(bool _state)
    {
        skyMang.SetDaytime(_state);
        print("Day time is: "+_state);
    }
}
