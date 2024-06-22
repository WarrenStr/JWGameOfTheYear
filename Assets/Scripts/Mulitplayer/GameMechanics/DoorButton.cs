using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

public class DoorButton : Door
{
    [SerializeField] private List<ButtonDoor> _buttons;
    [SerializeField] private float _timeBetweenButtonPressed; 

    private Dictionary<ButtonDoor, bool> _activeButtons = new Dictionary<ButtonDoor, bool>();

    //private int _numberOfButtonPressed = 0;
    private float _lastButtonPressed;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        foreach (ButtonDoor buttonDoor in _buttons)
        {
            buttonDoor.OnButtonPressed += OnButtonPressed;
            _activeButtons.Add(buttonDoor, false);
        }
    }

    private void OnButtonPressed(ButtonDoor buttonDoor)
    {
        _activeButtons[buttonDoor] = true;
        int _numberOfButtonPressed = CountActiveButtons();

        if (_numberOfButtonPressed == 1)
        {
            Debug.Log("First button pressed");
            _lastButtonPressed = Time.time;

            if (_numberOfButtonPressed == _buttons.Count)
            {
                _animCtrl.SetTrigger("OpenDorr");
            }
        }
        else
        {
            if (_lastButtonPressed + _timeBetweenButtonPressed >= Time.time)
            {
                if (_numberOfButtonPressed == _buttons.Count)
                {
                    _animCtrl.SetTrigger("OpenDoor");
                }
            }
            else
            {
                ResetButtons();
                _activeButtons[buttonDoor] = true;
                Debug.Log("Reset button");
            }
        }
        Debug.Log("Number of button pressed: " + _numberOfButtonPressed);
    }

    private int CountActiveButtons()
    {
        int _numberOfActiveButtons = 0;
        foreach (KeyValuePair<ButtonDoor, bool> button in _activeButtons)
        {
            _numberOfActiveButtons = button.Value ? _numberOfActiveButtons + 1 : _numberOfActiveButtons;
        }

        return _numberOfActiveButtons;
    }

    private void ResetButtons()
    {
        foreach (ButtonDoor button in _buttons)
        {
            _activeButtons[button] = false;
        }
    }
}
