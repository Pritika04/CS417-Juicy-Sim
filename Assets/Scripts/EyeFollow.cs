using UnityEngine;

public class EyeFollow : MonoBehaviour
{
    public Transform target;
    public Transform eyeball;

    public float sensitivity = 0.08f;
    public float maxOffset = 0.08f;
    public float speed = 10f;

    private Vector3 startLocalPos;

    private void Start()
    {
        startLocalPos = transform.localPosition;
    }

    private void Update()
    {
        if (target == null || eyeball == null) return;

        Vector3 dir = (target.position - eyeball.position).normalized;
        Vector3 localDir = eyeball.InverseTransformDirection(dir);

        Vector2 offsetXY = new Vector2(localDir.x, localDir.y) * sensitivity;
        offsetXY = Vector2.ClampMagnitude(offsetXY, maxOffset);

        Vector3 targetPos = new Vector3(
            startLocalPos.x + offsetXY.x,
            startLocalPos.y + offsetXY.y,
            startLocalPos.z
        );

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPos,
            Time.deltaTime * speed
        );
    }
}