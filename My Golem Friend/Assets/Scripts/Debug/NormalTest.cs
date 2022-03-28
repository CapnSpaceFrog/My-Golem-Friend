using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class NormalTest : MonoBehaviour
{
    public LayerMask ground;

    void Update()
    {
        Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 2.5f, ground);

        Debug.Log(hit.normal);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.forward * 3);
    }
}


