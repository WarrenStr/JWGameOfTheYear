using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerDetecter : MonoBehaviour
{
    [Header("Floating Text Strings")]
    [SerializeField] private string enterFloatingT;
    [SerializeField] private string exitFloatingT;

    [Header("Command Line Strings")]
    [SerializeField] private string enterCommandLine;
    [SerializeField] private string exitCommandLine;

    private bool trigActivated = false;

    public TextMeshProUGUI floatingText;
    public TextMeshProUGUI textLine;

    // Start is called before the first frame update
    void Start()
    {
        floatingText = GetComponentInChildren<TextMeshProUGUI>();
        floatingText.text = enterFloatingT;

        textLine = GameObject.Find("playerUpdateText").GetComponent<TextMeshProUGUI>();
        textLine.text = null;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            if (!trigActivated)
            {
                trigActivated = true;
                if (trigActivated)
                {
                    Debug.Log(enterCommandLine);
                    textLine.text = enterCommandLine;
                    floatingText.text = exitFloatingT;
                }
            }

            else if (trigActivated)
            {
                trigActivated = false;
                if (!trigActivated)
                {
                    Debug.Log(exitCommandLine);
                    textLine.text = exitCommandLine;
                    floatingText.text = enterFloatingT;
                }
            }
        }
    }
}
