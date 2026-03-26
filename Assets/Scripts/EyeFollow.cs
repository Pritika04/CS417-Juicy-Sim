using UnityEngine;

public class EyeFollow : MonoBehaviour
{
    public Transform target;
    public Transform eyeball;
    public float distance = 0.02f;
    public float exaggeration = 3f;
    public float speed = 10f;

    private Vector3 startLocalPos;

    void Start()
    {
        startLocalPos = transform.localPosition;
    }

    void Update()
    {
        if (target == null || eyeball == null) return;

        Vector3 dir = target.position - eyeball.position;
        Vector3 localDir = eyeball.InverseTransformDirection(dir);

        Vector3 offset = new Vector3(localDir.x, localDir.y, 0f) * distance * exaggeration;
        offset = Vector3.ClampMagnitude(offset, distance);

        Vector3 targetPos = startLocalPos + offset;

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPos,
            Time.deltaTime * speed
        );
    }
}