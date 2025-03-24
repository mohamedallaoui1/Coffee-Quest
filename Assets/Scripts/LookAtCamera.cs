using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    void Update()
    {
        if (GlobalVariables.instance.mainCamera)
        {
            transform.LookAt(GlobalVariables.instance.mainCamera.transform);
        }
    }
}