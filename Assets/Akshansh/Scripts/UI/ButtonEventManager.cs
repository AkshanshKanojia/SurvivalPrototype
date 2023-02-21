using AkshanshKanojia.Inputs.Button;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEventManager : MonoBehaviour
{
    [System.Serializable]
    class ButtonDataHolder
    {
        public ButtonInputManager TargetButton;
        public int ActionIndex;
    }
}
