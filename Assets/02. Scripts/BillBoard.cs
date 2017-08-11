using UnityEngine;
using System.Collections;

public class BillBoard : MonoBehaviour {
	Transform cameraTransform;
	public Axis axis = new Axis(true, true, true);

	[System.Serializable]
	public struct Axis {
		public bool X;
		public bool Y;
		public bool Z;

		public Axis(bool x, bool y, bool z) {
			this.X = x;
			this.Y = y;
			this.Z = z;
		}
	}

	void Start() {
        if (Camera.main)
        {
            cameraTransform = Camera.main.transform;
        }
	}

	void Update () {
        if (!cameraTransform )
        {
            if (Camera.main)
            {
                cameraTransform = Camera.main.transform;
            }
        }else{
            Vector3 rotation = transform.eulerAngles;

            if (axis.X)
            {
                rotation.x = cameraTransform.eulerAngles.x;
            }
            if (axis.Y)
            {
                rotation.y = cameraTransform.eulerAngles.y;
            }
            if (axis.Z)
            {
                rotation.z = cameraTransform.eulerAngles.z;
            }
            transform.eulerAngles = rotation;
            //transform.eulerAngles = cameraTransform.eulerAngles;
        }
	}

	public void SetBillboardAxis(bool x, bool y, bool z) {
		axis.X = x;
		axis.Y = y;
		axis.Z = z;
	}
}
