using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using UnityEngine;
using UnityEngine.UI;

public class RelayTest : MonoBehaviour
{
    [SerializeField] int maxConnectionsAllowed = 2;//host is on one of the players ie 2= hoset + 2  = 3 total players.
    [SerializeField] string regionName,joinCode;
    [SerializeField] Button clientButt, JoinButt;
    private async void Start()
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () => { print("signed in "+ AuthenticationService.Instance.PlayerId
            ); };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        clientButt.onClick.AddListener(StartRelay);
        JoinButt.onClick.AddListener(JoinRelay);
    }

    async void StartRelay()
    {
        //should be written in try to avoid breaking game!
        try
        {
            Allocation _allocation = await RelayService.Instance.CreateAllocationAsync(maxConnectionsAllowed);

           joinCode = await RelayService.Instance.GetJoinCodeAsync(_allocation.AllocationId);

            //for connecting through netccode
            RelayServerData _data = new RelayServerData(_allocation,"dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(_data);

            NetworkManager.Singleton.StartHost();
            print("Code: "+ joinCode);
        }
        catch(RelayServiceException e)
        {
            print(e);
        }
    }

    public async void JoinRelay()
    {
        try
        {
            JoinAllocation _joinAloc = await RelayService.Instance.JoinAllocationAsync(joinCode);
            RelayServerData _data = new RelayServerData(_joinAloc, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(_data);

            NetworkManager.Singleton.StartClient();
        }
        catch(RelayServiceException e)
        {
            print(e);
        }
    }    
}
