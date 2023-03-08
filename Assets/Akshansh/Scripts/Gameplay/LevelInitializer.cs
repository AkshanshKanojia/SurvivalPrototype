using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LevelInitializer : MonoBehaviour
{

    private void Start()
    {
        SetPlayer();
    }

    void SetPlayer()
    {
        GameObject _temp = PhotonNetwork.Instantiate("Player",new Vector3(0,2,0),Quaternion.identity);
        if(_temp.GetComponent<PhotonView>().IsMine)
        {
            _temp.transform.GetChild(0).gameObject.SetActive(true);//camera
        }
    }
}
