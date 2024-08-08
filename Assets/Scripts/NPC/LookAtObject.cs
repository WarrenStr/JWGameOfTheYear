using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtObject : MonoBehaviour
{
    [SerializeField] GameObject target;

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - target.transform.position); 
    }
}
