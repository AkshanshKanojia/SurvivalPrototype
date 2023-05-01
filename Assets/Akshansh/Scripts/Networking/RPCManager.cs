using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RPCManager : MonoBehaviour
{
    //private 
    PhotonView view;
    LevelCont levelCont;
    
    #region Private Methods
    private void Start()
    {
        view = GetComponent<PhotonView>();
        levelCont = FindObjectOfType<LevelCont>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //view.RPC("TestRPC",RpcTarget.All,"bang");
        }
    }
    public void UpdateDay(bool _state)
    {
        view.RPC("SetDay", RpcTarget.All, _state);
    }
    public void DestroyObject(int objId)
    {
        view.RPC("EndObj", RpcTarget.All, objId);
    }

    [PunRPC]
    void EndObj(GameObject _obj)
    {
        Destroy(_obj);
    }
    [PunRPC]
    void TestRPC(string str)
    {
        print(str);
    }
    [PunRPC]
    void SetDay(bool _state)
    {
        levelCont.SetCycle(_state);
    }
    #endregion
}
