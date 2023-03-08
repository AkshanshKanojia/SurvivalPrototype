using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RPCManager : MonoBehaviour
{
    //private 
    PhotonView view;
    
    #region Private Methods
    private void Start()
    {
        view = GetComponent<PhotonView>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            view.RPC("TestRPC",RpcTarget.All );
        }
    }

    [PunRPC]
    void TestRPC()
    {
    }
    #endregion
}
