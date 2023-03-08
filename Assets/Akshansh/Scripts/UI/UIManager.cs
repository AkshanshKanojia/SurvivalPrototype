using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject[] UIObjs;

    public void DisableAllUI()
    {
        foreach (var v in UIObjs)
        {
            v.SetActive(false);
        }    
    }

    public void SetUI(int _index,bool _state)
    {
        UIObjs[_index].SetActive(_state);
    }

    /// <summary>
    /// hide/show mouse cursor
    /// </summary>
    /// <param name="_state"></param>
    public void SetCursorVisibility(bool _state)
    {
        Cursor.visible = _state;
        Cursor.lockState = (_state) ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
