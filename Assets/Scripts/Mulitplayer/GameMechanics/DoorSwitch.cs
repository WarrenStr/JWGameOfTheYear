using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSwitch : Door
{
    [SerializeField] private List<Switch> _switches;

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
