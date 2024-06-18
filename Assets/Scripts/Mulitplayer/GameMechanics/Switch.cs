using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Switch : NetworkBehaviour
{
    private NetworkVariable<bool> _isActive = new NetworkVariable<bool>();


    public delegate void SwitchChanged(Switch doorSwitch, bool isActive);
    public event SwitchChanged OnSwitchChanged;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _isActive.OnValueChanged += OnValueChanged;
    }

    private void OnValueChanged(bool wasActive, bool isActive)
    {
        if (isActive)
        {
            Debug.Log("is active");
        }
        else
        {
            Debug.Log("is not active");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        OnSwitchChangedServerRpc(isActive: true);
    }

    private void OnTriggerExit(Collider other)
    {
        OnSwitchChangedServerRpc(isActive: false);
    }

    [ServerRpc] // Server Rpc's method name must end in "ServerRpc"
    private void OnSwitchChangedServerRpc(bool isActive)
    {
        _isActive.Value = isActive;
        OnSwitchChanged?.Invoke(this, isActive);
    }
}
