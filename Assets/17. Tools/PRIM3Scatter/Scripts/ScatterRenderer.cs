using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatterRenderer : MonoBehaviour {
	[HideInInspector]
	public List<Vector4> m_PointsList;
	static ScatterRenderer _instance;

	public Material baseMaterial;
	public Mesh mesh;
	public string layer = "Grass";
	public UnityEngine.Rendering.ShadowCastingMode shadowType = UnityEngine.Rendering.ShadowCastingMode.On;
	public Vector3 offset;
	[Range(0f, 10f)]
	public float meshSize = 1f;
	public Vector3 boundsSize = new Vector3 (100.0f, 100.0f, 100.0f);
	private Bounds meshBounds;
	private ComputeBuffer m_PositionBuffer;
	private ComputeBuffer m_ArgsBuffer;
	private ComputeBuffer m_ColorBuffer;
	private uint[] m_Args = new uint[5] { 0, 0, 0, 0, 0 };
	private int cachedInstanceCount = -1;
	MaterialPropertyBlock m_MPB;

	//[HideInInspector]
	public List<ScatterPointsData> pointsData;

	public static ScatterRenderer instance {
		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<ScatterRenderer> ();
			}
			return _instance;
		}
	}

	// Use this for initialization
	void Start () {
		if (pointsData.Count > 0) {
			
			meshBounds = new Bounds (transform.position, boundsSize);
			m_ArgsBuffer = new ComputeBuffer (1, m_Args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
			UpdateBuffers();
			m_MPB = new MaterialPropertyBlock ();
			m_MPB.SetFloat ("_ASD", 0f); // Propiedad dummy. Con esto se fuerza a hacer una llamada diferente por cada material, arreglando el problema de las sombras.
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (m_PointsList != null) {
			if (cachedInstanceCount != pointsData.Count) {
				UpdateBuffers ();
			}
			if (layer != "") {
				Graphics.DrawMeshInstancedIndirect (mesh, 0, baseMaterial, meshBounds, m_ArgsBuffer, 0, m_MPB, shadowType, true, LayerMask.NameToLayer (layer));
			} else {
				Graphics.DrawMeshInstancedIndirect (mesh, 0, baseMaterial, meshBounds, m_ArgsBuffer, 0, m_MPB, shadowType);
			}
		}
	}

	void OnDisable() {
		Release ();
	}

	void OnDestroy() {
		Release ();
	}

	void Release() {
		if (m_PositionBuffer != null)
			m_PositionBuffer.Release();
		m_PositionBuffer = null;

		if (m_ArgsBuffer != null)
			m_ArgsBuffer.Release();
		m_ArgsBuffer = null;
	}

	void UpdateBuffers() {
		// positions
		m_PointsList = new List<Vector4> ();
		for (int i = 0; i < pointsData.Count; ++i) {
			m_PointsList.AddRange (pointsData [i].pointsList);
		}
		if (m_PositionBuffer != null)
			m_PositionBuffer.Release();
		if (m_ColorBuffer != null)
			m_ColorBuffer.Release ();
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
		baseMaterial.SetBuffer("positionBuffer", m_PositionBuffer);
		baseMaterial.SetBuffer ("colorBuffer", m_ColorBuffer);

		// indirect args
		uint numIndices = (mesh != null) ? (uint)mesh.GetIndexCount(0) : 0;
		m_Args[0] = numIndices;
		m_Args[1] = (uint)m_PointsList.Count;
		m_ArgsBuffer.SetData(m_Args);

		cachedInstanceCount = pointsData.Count;
	}
}
