using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ButtonDoor : NetworkBehaviour // Make sure there is a network object component on the game object. 
{
    public delegate void ButtonPressed(ButtonDoor buttonDoor);
    public event ButtonPressed OnButtonPressed;


    public void Activate()
    {
        if (IsServer)
        {
            OnButtonPressed?.Invoke(buttonDoor: this);
        }
    }
}
