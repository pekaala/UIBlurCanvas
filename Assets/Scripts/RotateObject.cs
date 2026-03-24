using UnityEngine;

	public class RotateObject : MonoBehaviour
	{
		void Update()
		{
			transform.Rotate(Vector3.up * 60 * Time.deltaTime);
		}
	}

