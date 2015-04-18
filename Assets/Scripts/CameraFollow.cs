using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public GameObject Target;

    public float CameraSpeed;

    public float Height
    {
        get { return 2.0f * _camera.orthographicSize; }
    }

    public float Width
    {
        get { return Height * _camera.aspect; }
    }

    private Camera _camera;

    void Awake()
    {
        _camera = GetComponent<Camera>();
    }

	void LateUpdate()
	{
	    var minX = Mathf.Floor(Target.transform.position.x / Util.MapWidth) * Util.MapWidth;
	    var minY = Mathf.Floor(Target.transform.position.y / Util.MapHeight) * Util.MapHeight;

	    var maxX = minX + Util.MapWidth;
	    var maxY = minY + Util.MapHeight;

	    minX += Width / 2; 
	    maxX -= Width / 2; 

	    minY += Height / 2; 
	    maxY -= Height / 2;

	    var camX = Mathf.Clamp(Target.transform.position.x, minX, maxX);
	    var camY = Mathf.Clamp(Target.transform.position.y, minY, maxY);

        var desiredPosition = new Vector3(camX, camY, transform.position.z);

	    transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * CameraSpeed);
	}
}
