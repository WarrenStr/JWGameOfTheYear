using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class Door : NetworkBehaviour
{
    [SerializeField] private List<Switch> _switches;
    [SerializeField] private NetworkAnimator _animCtrl;

    private Dictionary<Switch, bool> _activeSwitches = new Dictionary<Switch, bool>();


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            foreach (Switch doorSwitch in _switches)
            {
                doorSwitch.OnSwitchChanged += OnSwitchChnaged;
                _activeSwitches.Add(doorSwitch, false);
            }
        }
    }

    private void OnSwitchChnaged(Switch doorswitch, bool isActive)
    {
        _activeSwitches[doorswitch] = isActive;

        foreach (var doorSwitch in _switches)
        {
            if (!_activeSwitches[doorSwitch])
            {
                return;
            }
        }

        Debug.Log("Open the door.");
        _animCtrl.SetTrigger("OpenDoor");
    }
}
