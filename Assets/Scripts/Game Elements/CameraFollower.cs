using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public GameObject ObjectToFollow;
    public float FollowSpeed;

    private void LateUpdate()
    {
        if (!ObjectToFollow) return;
        Vector3 distance = ObjectToFollow.transform.position - transform.position;
        Vector3 direction = distance.normalized;
        Vector3 offset = Vector3.ClampMagnitude(direction * FollowSpeed, distance.magnitude);
        transform.Translate(new Vector3(offset.x, offset.y, 0f));
        //transform.position = transform.position + new Vector3(offset.x, offset.y, 0f);
    }
}
