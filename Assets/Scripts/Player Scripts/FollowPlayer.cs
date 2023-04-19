using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform playerPos;
    public Vector3 offset;
    private Vector3 velocity = Vector3.zero;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, playerPos.position + offset, ref velocity, 0.2f);
    }
}
