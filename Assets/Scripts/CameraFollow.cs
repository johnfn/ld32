using System;
using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public GameObject Target
    {
        get { return CanTakeInput.ActiveInputGuy.gameObject; }
    }

    public float CameraSpeed;

    private bool _trolled = false;

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

        Manager.CustomCamera = this;
    }

    public Vector3 ClampWithinCamera(Vector3 point)
    {
        var resultX = Mathf.Clamp(point.x, transform.position.x - Width, transform.position.x + Width);
        var resultY = Mathf.Clamp(point.y, transform.position.y - Height, transform.position.y + Height);

        return new Vector3(resultX, resultY, point.z);
    }

    public bool IsWithinCamera(Vector3 point)
    {
        return point == ClampWithinCamera(point);
    }

	void LateUpdate()
	{
	    var minX = Mathf.Floor(Target.transform.position.x / Util.MapWidth) * Util.MapWidth;
	    var minY = Mathf.Floor(Target.transform.position.y / Util.MapHeight) * Util.MapHeight;

	    var maxX = minX + Util.MapWidth;
	    var maxY = minY + Util.MapHeight;

	    minX += Width / 2; 
	    maxX -= Width / 2;

	    if (/* !Manager.Instance.Debug && */ !_trolled && Math.Abs(Mathf.Floor(minX) - 10f) < .01f)
	    {
            Manager.Instance.Dialog.ShowDialog(Dialogs.ReallyDumb);
	        _trolled = true;
	    }


	    minY += Height / 2; 
	    maxY -= Height / 2;

	    var camX = Mathf.Clamp(Target.transform.position.x, minX, maxX);
	    var camY = Mathf.Clamp(Target.transform.position.y, minY, maxY);

        var desiredPosition = new Vector3(camX, camY, transform.position.z);

	    transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * CameraSpeed);
	}
}
