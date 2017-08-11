using UnityEngine;
using System.Collections;

public interface IPullable {
	void Pull(float aSpeed, float aTime, Vector3 aSource, float aStopDistance);
}
