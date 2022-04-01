using UnityEngine;
using UnityEditor;

public class Snap
{
    [MenuItem("Tools/Snap _END")]
    static void SnapNoRotation()
    {
        LayerMask groundMask = LayerMask.GetMask("Ground");

        GameObject[] selectedObjects = Selection.gameObjects;

        for (int i = 0; i < selectedObjects.Length; i++)
        {
            if (Physics.Raycast(selectedObjects[i].transform.position, Vector3.down, out RaycastHit hitInfo, 20, groundMask))
            {
                Collider objectCollider = selectedObjects[i].gameObject.GetComponent<Collider>();
                Vector3 targetPosition = new Vector3(hitInfo.point.x, hitInfo.point.y + objectCollider.bounds.extents.y, hitInfo.point.z);

                selectedObjects[i].transform.position = targetPosition;
            }
        }
    }
}