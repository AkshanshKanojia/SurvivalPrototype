using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] string roomJoinScene = "PlayerTestProto";
    UIManager uiMang;
    SceneHandler sceneMang;
    private void Start()
    {
        uiMang = FindObjectOfType<UIManager>();
        sceneMang = FindObjectOfType<SceneHandler>();

        //initialize connection
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
        print("connecting");
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        //when connected to server
        uiMang.DisableAllUI();
        uiMang.SetUI(1, true);
        print("connected");
    }

    public void CreateRoom(string _value, int _maxPlayers)
    {
        RoomOptions _op = new RoomOptions()
        {
            MaxPlayers = (byte)_maxPlayers
        };
        PhotonNetwork.CreateRoom(_value, _op);
    }

    public void JoinRoom(string _value)
    {
        PhotonNetwork.JoinRoom(_value);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        sceneMang.SetScene(roomJoinScene);
    }
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        sceneMang.SetScene(roomJoinScene);
    }
}
