using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class ScatterGizmosDebug : MonoBehaviour {
	[HideInInspector]
	public List<Vector4> m_PointsList;

	public float pointSize = 0.01f;
	public bool showGizmos = true;

	public LayerMask scatterMask;

	[Header("Debug mesh")]
	public bool showDebugMesh = false;
	public Material m_Material;
	public Mesh mesh;
	public Vector3 offset;
	public float meshSize = 1f;
	private ComputeBuffer m_PositionBuffer;
	private ComputeBuffer m_ColorBuffer;
	private ComputeBuffer m_ArgsBuffer;
	private uint[] m_Args = new uint[5] { 0, 0, 0, 0, 0 };

	void Start() {
		m_ArgsBuffer = new ComputeBuffer(1, m_Args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
		//UpdateBuffers ();
	}

	void OnDrawGizmos() {
		if (m_PointsList != null && showGizmos == true) {
			Gizmos.color = Color.red;
			for (int i = 0; i < m_PointsList.Count; ++i) {
				Gizmos.DrawSphere(m_PointsList[i], pointSize);
			}
		}
	}

	void Update() {
		if (showDebugMesh == true && m_PointsList != null && Application.isEditor) {
			/*if (cachedInstanceCount != m_PointsList.Count) {
				UpdateBuffers ();
			}*/


		}
	}

	void OnRenderObject() {
		//UpdateBuffers ();
		//Graphics.DrawMeshInstancedIndirect (mesh, 0, m_Material, new Bounds (transform.position, new Vector3 (100.0f, 100.0f, 100.0f)), m_ArgsBuffer);
	}

	void OnDisable() {

		if (m_PositionBuffer != null)
			m_PositionBuffer.Release();
		m_PositionBuffer = null;

		if (m_ArgsBuffer != null)
			m_ArgsBuffer.Release();
		m_ArgsBuffer = null;
	}

	void UpdateBuffers() {
		// positions
		if (m_PositionBuffer != null)
			m_PositionBuffer.Release();
		m_PositionBuffer = new ComputeBuffer(m_PointsList.Count, 16);
		m_ColorBuffer = new ComputeBuffer (m_PointsList.Count, 4);
		float l_MeshHeightHalf = (mesh.bounds.max.y - mesh.bounds.min.y) / 2.0f;
		Vector4[] positions = new Vector4[m_PointsList.Count];
		float[] colors = new float[m_PointsList.Count];
		for (int i = 0; i < m_PointsList.Count; i++) {
			positions [i].w = meshSize * m_PointsList[i].w;
			positions [i].x = m_PointsList [i].x + offset.x;
			positions [i].y = m_PointsList [i].y + offset.y + positions[i].w * l_MeshHeightHalf;
			positions [i].z = m_PointsList [i].z + offset.z;
			colors [i] = Random.Range	(0f, 1f);
		}
		m_PositionBuffer.SetData(positions);
		m_ColorBuffer.SetData (colors);
		m_Material.SetBuffer("positionBuffer", m_PositionBuffer);
		m_Material.SetBuffer ("colorBuffer", m_ColorBuffer);

		// indirect args
		uint numIndices = (mesh != null) ? (uint)mesh.GetIndexCount(0) : 0;
		m_Args[0] = numIndices;
		m_Args[1] = (uint)m_PointsList.Count;
		m_ArgsBuffer = new ComputeBuffer(1, m_Args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
		m_ArgsBuffer.SetData(m_Args);
	}

	public void RemoveCircle(Vector3 aCenter, float aRadius) {
		List<Vector4> l_Aux = new List<Vector4> ();
		for (int i = 0; i < m_PointsList.Count; ++i) {
			if (Vector3.Distance (m_PointsList [i], aCenter) <= aRadius) {
				l_Aux.Add (m_PointsList [i]);
			}
		}

		for (int i = 0; i < l_Aux.Count; ++i) {
			m_PointsList.Remove (l_Aux [i]);
		}
	}
}
