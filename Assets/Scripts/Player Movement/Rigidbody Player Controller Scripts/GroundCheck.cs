using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] RbPlayerController rbPlayerController; 
    // Start is called before the first frame update
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == rbPlayerController.gameObject)
        {
            return;
        }

        rbPlayerController.SetIsGrounded(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == rbPlayerController.gameObject)
        {
            return;
        }

        rbPlayerController.SetIsGrounded(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == rbPlayerController.gameObject)
        {
            return;
        }

        rbPlayerController.SetIsGrounded(true);
    }
}
