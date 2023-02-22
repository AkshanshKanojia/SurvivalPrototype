using AkshanshKanojia.Inputs.Button;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonEventManager : MonoBehaviour
{
    public enum AvailableButtonActions { JoinRoom,CreateRoom,StartGame}

    [System.Serializable]
    public class ButtonDataHolder
    {
        public ButtonInputManager TargetButton;
        public List<AvailableButtonActions> CurtActions;
        public int ActionIndex;
        public string ActionValue;
    }

    public List<ButtonDataHolder> CurtButtons;

    [SerializeField] TMP_InputField inp;

    LobbyManager lobbyMang;

    private void Start()
    {
        lobbyMang = FindObjectOfType<LobbyManager>();
        foreach(var v in CurtButtons)
        {
            v.TargetButton.OnLeft += OnTapLeft;
        }
    }

    void OnTapLeft(GameObject _obj)
    {
        foreach(var v in CurtButtons)
        {
            if(v.TargetButton.gameObject == _obj)
            {
                foreach(var _action in v.CurtActions)
                {
                    switch(_action)
                    {
                        case AvailableButtonActions.CreateRoom:
                            lobbyMang.CreateLobby(inp.text,2);
                            break;
                        case AvailableButtonActions.JoinRoom:
                            lobbyMang.JoinLobby(inp.text);
                            break;
                        case AvailableButtonActions.StartGame:
                            FindObjectOfType<SceneHandler>().SetScene(v.ActionValue);
                            break;
                    }
                }
            }
        }
    }
}
