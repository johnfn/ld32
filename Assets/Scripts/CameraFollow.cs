using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public GameObject Target;

    public float Height
    {
        get { return 2.0f * GetComponent<Camera>().pixelHeight; }
    }

    public float Width
    {
        get { return 2.0f * GetComponent<Camera>().pixelWidth; }
    }

	void LateUpdate()
	{
	    transform.position = new Vector3 (
            Target.transform.position.x,
            Target.transform.position.y,
            transform.position.z
        );
	}
}
