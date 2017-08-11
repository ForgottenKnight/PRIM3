using UnityEngine;
using System.Collections;

public class DebugDrawer
{
	public static void AddOval(float x, float z, float xRadius, float zRadius, Color c, GameObject p)
	{
		int segments = 30;
		GameObject go = BasicInit (x, z, segments+1, c, p);
		go.name = "DebugDrawer: Oval";
		LineRenderer lr = go.GetComponent<LineRenderer> ();
		
		float angle = 20.0f;
		float auxX = 0.0f;
		float auxZ = 0.0f;
		
		for (int i = 0; i<segments+1; ++i)
		{
			auxX = Mathf.Sin(Mathf.Deg2Rad * angle) * xRadius;
			auxZ = Mathf.Cos(Mathf.Deg2Rad * angle) * zRadius;
			
			lr.SetPosition(i, new Vector3(auxX, 0.0f, auxZ));
			angle += (360.0f/segments);
		}
	}

	public static void AddAngle(float x, float z, float angle, float radius, Color c, GameObject p, Quaternion initRot = default(Quaternion), int segments = 30)
	{
		GameObject go = BasicInit (x, z, segments + 1, c, p);
		go.name = "DebugDrawer::Angle";
		LineRenderer lr = go.GetComponent<LineRenderer> ();

		float auxAngle = -angle/2.0f;
		float auxX = 0.0f;
		float auxZ = 0.0f;

		for (int i = 0; i<segments+1; ++i)
		{
			auxX = Mathf.Sin(Mathf.Deg2Rad * auxAngle) * radius;
			auxZ = Mathf.Cos(Mathf.Deg2Rad * auxAngle) * radius;
			
			lr.SetPosition(i, new Vector3(auxX, 0.0f, auxZ));
			auxAngle += (angle/segments);
		}

		lr.transform.rotation = initRot;

		float rad = Mathf.Deg2Rad * (angle/2.0f);
		DebugDrawer.AddLine (0.0f, 0.0f, Mathf.Sin (rad) * radius, Mathf.Cos (rad)*radius, Color.red, p);
		DebugDrawer.AddLine (0.0f, 0.0f, -Mathf.Sin (rad) * radius, Mathf.Cos (rad)*radius, Color.red, p);
	}
	
	public static void AddLine(float x, float z, float xEnd, float zEnd, Color c, GameObject p, Quaternion initRot = default(Quaternion))
	{
		int segments = 2;
		GameObject go = BasicInit (0, 0, segments, c, p);
		go.name = "DebugDrawer: Line";
		LineRenderer lr = go.GetComponent<LineRenderer> ();
		
		lr.SetPosition (0, new Vector3 (x, 0.0f, z));
		lr.SetPosition (1, new Vector3 (xEnd, 0.0f, zEnd));
		lr.transform.rotation = initRot;
	}
	
	
	static GameObject BasicInit(float x, float z, int segments, Color c, GameObject p)
	{
		GameObject go = new GameObject ();
		go.layer = LayerMask.NameToLayer ("DebugLayer");
		go.transform.parent = p.transform;
		go.transform.localPosition = new Vector3 (x, 0.0f, z);
		
		LineRenderer lr = go.AddComponent<LineRenderer>();
		lr.material = new Material (Shader.Find("Particles/Additive"));
		
        lr.positionCount = segments;
        lr.startColor = c;
        lr.endColor = c;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
		lr.useWorldSpace = false;
		
		return go;
		
	}
	
}
