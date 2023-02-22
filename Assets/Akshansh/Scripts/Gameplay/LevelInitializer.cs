using Unity.Netcode;
using UnityEngine;

public class LevelInitializer : NetworkBehaviour
{
    LobbyManager mang;
    [SerializeField] GameObject playerPref;

    private void Start()
    {
        SetPlayers();
    }

    void SetPlayers()
    {
        mang = FindObjectOfType<LobbyManager>();
        print(mang.activeLobby.Players.Count);
        foreach(var _ in mang.activeLobby.Players)
        {
            Instantiate(playerPref);
        }
    }
}
