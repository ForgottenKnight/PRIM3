using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEditor;

public class PointScatterWindow : EditorWindow {
	// Parametros generales
	enum NoiseType {NoNoise, Noise}
	NoiseType m_NoiseType = NoiseType.NoNoise;
	uint m_SecurityMaximum = 50000;
	float m_ProximityX = 1f;
	float m_ProximityZ = 1f;
	float m_MaxNormalDot = 0.1f;
	float m_MinMeshSize = 0.5f;
	float m_MaxMeshSize = 1.0f;
	float m_MinSizeLimit = 0.0f;
	float m_MaxSizeLimit = 10.0f;

	string m_Folder = "ScatterPoints";

	// Parametros NoiseFromPack
	float m_VarianceRatio = 0.5f;

	// Parametros splat
	bool m_UseSplat = false;
	Texture2D m_SplatImage;
	string m_SplatName;
	string m_LastSplatName;
	Color m_SplatColor;
	float m_ColorThreshold;

	// Parametros del brush
	bool m_UseBrush = false;
	float m_BrushSize = 5f;

	private LayerMask scatterMask;


	List<Vector4> m_PointsList;

	[MenuItem ("PRIM3/Point Scatter")]
	public static void  ShowWindow () {
		EditorWindow.GetWindow(typeof(PointScatterWindow));
	}

	void OnGUI () {
		// EditorGUI
		// GUILayout
		// EditorGUILayout
		// General
		EditorGUILayout.Space ();
		GUILayout.Label("Opciones generales", EditorStyles.boldLabel);
		m_Folder = EditorGUILayout.TextField ("Folder name", m_Folder);
		m_ProximityX = EditorGUILayout.FloatField("Proximidad en X", m_ProximityX);
		m_ProximityZ = EditorGUILayout.FloatField("Proximidad en Z", m_ProximityZ);
		m_MaxNormalDot = EditorGUILayout.Slider ("Diferencia angular máxima", m_MaxNormalDot, 0.0f, 1.0f);
		EditorGUILayout.LabelField ("Mínimo: ", m_MinMeshSize.ToString());
		EditorGUILayout.LabelField ("Máximo: ", m_MaxMeshSize.ToString());
		EditorGUILayout.MinMaxSlider ("Tamaño límite", ref m_MinMeshSize, ref m_MaxMeshSize, m_MinSizeLimit, m_MaxSizeLimit);
		// Noise
		m_NoiseType = (NoiseType) EditorGUILayout.EnumPopup ("Tipo de ruido", m_NoiseType);
		switch (m_NoiseType) {
		case NoiseType.Noise:
			EditorGUILayout.Space ();
			GUILayout.Label("Opciones del noise", EditorStyles.boldLabel);
			m_VarianceRatio = EditorGUILayout.Slider("Ratio de variación", m_VarianceRatio, 0.0f, 1.0f);
			break;
		default:
			break;
		}
		// Splat
		m_UseSplat = EditorGUILayout.Toggle ("Usar splat", m_UseSplat);
		if (m_UseSplat == true) {
			EditorGUILayout.Space ();
			GUILayout.Label("Opciones del splat", EditorStyles.boldLabel);
			m_SplatImage = (Texture2D)EditorGUILayout.ObjectField ("Image", m_SplatImage, typeof(Texture2D), false);
			m_SplatName = EditorGUILayout.TextField ("Nombre del splat", m_SplatName);
			m_SplatColor = EditorGUILayout.ColorField ("Color del splat", m_SplatColor);
			m_ColorThreshold = EditorGUILayout.Slider ("Threshold de color", m_ColorThreshold, 0.0f, 1.0f);
		}
		// Brush
		EditorGUILayout.Space ();
		EditorGUILayout.LabelField("Pincel");
		m_UseBrush = EditorGUILayout.Toggle ("Usar pincel", m_UseBrush);
		if (m_UseBrush == true) {
			m_BrushSize = EditorGUILayout.Slider ("Tamaño del pincel", m_BrushSize, 1.0f, 20.0f);
		} else {
			ScatterBrush.instance.DisableBrush ();
		}
		// Botones
		EditorGUILayout.Space ();
		if (GUILayout.Button ("Obtener puntos")) {
			scatterMask = GameObject.FindObjectOfType<ScatterGizmosDebug> ().scatterMask;
			if (Selection.gameObjects.Length > 1) {
				GetAllPoints ();
				Debug.Log ("GetAllPoints");
			} else {
				GetPoints ();
				SceneView.RepaintAll ();
				Debug.Log ("GetPoints");
			}
			// TODO Blue noise
		}
		if (GUILayout.Button ("Limpiar puntos")) {
			ClearPoints ();
		}
		if (GUILayout.Button ("Generar archivo de puntos")) {
			SavePoints ();
		}


	}

	void OnEnable()
	{
		SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
		SceneView.onSceneGUIDelegate += this.OnSceneGUI;
	}

	void OnDisable()
	{
		SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
	}

	void OnDestroy()
	{
		SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
	}

	private void OnSceneGUI(SceneView aView) {
		if (m_UseBrush == true) {
			ScatterBrush l_Brush = ScatterBrush.instance;
			Event l_CurrentEvent = Event.current;
			if (l_CurrentEvent != null) {
				EventType l_EventType = l_CurrentEvent.type;
				Ray l_Ray = HandleUtility.GUIPointToWorldRay (l_CurrentEvent.mousePosition);
				RaycastHit l_HitPoint;
				bool l_Hit = Physics.Raycast (l_Ray, out l_HitPoint, Mathf.Infinity, scatterMask);
				if (l_Hit) {
                    Debug.Log(l_HitPoint.collider.name);
					Vector3 l_Point = l_HitPoint.point;
					l_Brush.DrawBrush (l_Point + Vector3.up * 100f, m_BrushSize);
					//if ((l_EventType == EventType.MouseDrag || l_EventType == EventType.MouseDown) && l_CurrentEvent.button == 0) {
					if (l_EventType == EventType.MouseDown && l_CurrentEvent.button == 0 && l_CurrentEvent.alt == false) {
						float l_HalfBrush = m_BrushSize * 0.5f;
						Vector2 l_BoundsX = new Vector2 (l_Point.x - m_BrushSize, l_Point.x + m_BrushSize);
						Vector2 l_BoundsZ = new Vector2 (l_Point.z - m_BrushSize, l_Point.z + m_BrushSize);
						Collider l_Collider = l_HitPoint.collider;
						float l_RaycastY = l_Point.y + 1f;
						float l_RaycastMaxY = 10f;
						m_PointsList.Clear ();
						List<Vector4> aux = GetPointsNoNoise (l_BoundsX, l_BoundsZ, l_Collider, l_RaycastY, l_RaycastMaxY);
						aux = CircleShape (l_Point, m_BrushSize, aux);
						ApplyNoiseToPoints (l_RaycastY, l_RaycastMaxY, l_Collider, aux);
						if (m_PointsList.Count > 0) {
							Debug.Log (m_PointsList.Count);
							ScatterGizmosDebug l_SGD = GameObject.FindObjectOfType<ScatterGizmosDebug> ();
							l_SGD.m_PointsList.AddRange (m_PointsList);
							/*for (int i = 0; i < m_PointsList.Count; ++i) { // Por que narices se crea un bucle infinito con esto?
								l_SGD.m_PointsList.Add (m_PointsList [i]);
							}*/
							m_PointsList.Clear ();
							SceneView.RepaintAll ();
						}
					} else if (l_EventType == EventType.MouseDown && l_CurrentEvent.button == 0 && l_CurrentEvent.alt == true) {
						ScatterGizmosDebug l_SGD = GameObject.FindObjectOfType<ScatterGizmosDebug> ();
						l_SGD.RemoveCircle (l_Point, m_BrushSize);
					}
				}
			}
		}
	}

	List<Vector4> CircleShape(Vector3 aCenter, float aRadius, List<Vector4> other) {
		List<Vector4> l_AuxList = new List<Vector4> ();
		for (int i = 0; i < other.Count; ++i) {
			if (Vector3.Distance (other [i], aCenter) > aRadius) {
				l_AuxList.Add (other [i]);
			}
		}
		for (int i = 0; i < l_AuxList.Count; ++i) {
			other.Remove (l_AuxList [i]);
		}
		return other;
	}

	private void GetAllPoints() {
		m_PointsList = new List<Vector4> ();
		float l_Time = Time.time;
		for (int objectIndex = 0; objectIndex < Selection.gameObjects.Length; ++objectIndex) {
			GameObject l_Target = Selection.gameObjects [objectIndex];
			Renderer l_Renderer = l_Target.GetComponentInChildren<Renderer> ();
			if (l_Renderer == null) {
				Debug.LogWarning ("El objeto seleccionado debe tener un renderer.");
				return;
			}
			MeshCollider l_Collider = l_Renderer.GetComponent<MeshCollider> ();
			bool l_AddedMeshCollider = false;
			if (l_Collider == null) {
				l_AddedMeshCollider = true;
				l_Collider = l_Renderer.gameObject.AddComponent<MeshCollider> ();
			}
			Bounds l_Bounds = l_Renderer.bounds;
			Vector2 l_BoundsX = new Vector2 (l_Bounds.min.x, l_Bounds.max.x);
			Vector2 l_BoundsZ = new Vector2 (l_Bounds.min.z, l_Bounds.max.z);
			float l_BoundsY = l_Bounds.max.y;
			float l_RaycastY = l_BoundsY + 1f;
			float l_RaycastMaxY = l_RaycastY - l_Bounds.min.y;
			if (m_UseSplat == true) {
				m_LastSplatName = m_SplatName;
			} else {
				m_LastSplatName = "";
			}
			Debug.Log ("Bounds X: " + l_BoundsX + ", Bounds Z: " + l_BoundsZ + ", Altura de raycast: " + l_RaycastY);
			switch (m_NoiseType) {
			case NoiseType.NoNoise:
				m_PointsList = GetPointsNoNoise (l_BoundsX, l_BoundsZ, l_Collider, l_RaycastY, l_RaycastMaxY);
				break;
			case NoiseType.Noise:
				List<Vector4> aux = GetPointsNoNoise (l_BoundsX, l_BoundsZ, l_Collider, l_RaycastY, l_RaycastMaxY);
				ApplyNoiseToPoints (l_RaycastY, l_RaycastMaxY, l_Collider, aux);
				break;
			}
			if (l_AddedMeshCollider == true) {
				Destroy (l_Collider);
			}
			SavePoints (objectIndex);
		}
	}

	private void GetPoints() {
		m_PointsList = new List<Vector4> ();
		float l_Time = Time.time;
		for (int objectIndex = 0; objectIndex < Selection.gameObjects.Length; ++objectIndex) {
			GameObject l_Target = Selection.gameObjects [objectIndex];
			Renderer l_Renderer = l_Target.GetComponentInChildren<Renderer> ();
			if (l_Renderer == null) {
				Debug.LogWarning ("El objeto seleccionado debe tener un renderer.");
				return;
			}
			MeshCollider l_Collider = l_Renderer.GetComponent<MeshCollider> ();
			bool l_AddedMeshCollider = false;
			if (l_Collider == null) {
				l_AddedMeshCollider = true;
				l_Renderer.gameObject.AddComponent<MeshCollider> ();
			}
			Bounds l_Bounds = l_Renderer.bounds;
			Vector2 l_BoundsX = new Vector2 (l_Bounds.min.x, l_Bounds.max.x);
			Vector2 l_BoundsZ = new Vector2 (l_Bounds.min.z, l_Bounds.max.z);
			float l_BoundsY = l_Bounds.max.y;
			float l_RaycastY = l_BoundsY + 100f;
			float l_RaycastMaxY = l_RaycastY - l_Bounds.min.y;

			Debug.Log ("Bounds X: " + l_BoundsX + ", Bounds Z: " + l_BoundsZ + ", Altura de raycast: " + l_RaycastY);
			switch (m_NoiseType) {
			case NoiseType.NoNoise:
				m_PointsList = GetPointsNoNoise (l_BoundsX, l_BoundsZ, l_Collider, l_RaycastY, l_RaycastMaxY);
				break;
			case NoiseType.Noise:
				List<Vector4> aux = GetPointsNoNoise (l_BoundsX, l_BoundsZ, l_Collider, l_RaycastY, l_RaycastMaxY);
				ApplyNoiseToPoints (l_RaycastY, l_RaycastMaxY, l_Collider, aux);
				break;
			}
			if (l_AddedMeshCollider == true) {
				Destroy (l_Collider);
			}
		}
		float l_Duration = Time.time - l_Time;
		Debug.Log ("Duración del scatter de puntos: " + l_Duration + " segundos");
		Debug.Log ("Número de puntos resultantes: " + m_PointsList.Count);
		if (m_PointsList.Count > 0) {
			ScatterGizmosDebug l_SGD = GameObject.FindObjectOfType<ScatterGizmosDebug> ();
			//Debug.Log
			l_SGD.m_PointsList.AddRange(m_PointsList);
			//l_SGD.m_PointsList = m_PointsList;
			SceneView.RepaintAll ();
			Debug.Log ("Puntos debug: " + l_SGD.m_PointsList.Count);
			m_PointsList.Clear ();
			if (m_UseSplat == true) {
				m_LastSplatName = m_SplatName;
			} else {
				m_LastSplatName = "";
			}
		}
	}

	List<Vector4> GetPointsNoNoise(Vector2 l_BoundsX, Vector2 l_BoundsZ, Collider l_Collider, float l_RaycastY, float l_RaycastMaxY) {
		List<Vector4> l_Aux = new List<Vector4> ();
		float fColumns = (l_BoundsX.y - l_BoundsX.x) / m_ProximityX;
		int l_Columns = (int)fColumns + 1;
		float fRows = (l_BoundsZ.y - l_BoundsZ.x) / m_ProximityZ;
		int l_Rows = (int)fRows + 1;

		if (l_Rows * l_Columns > m_SecurityMaximum) {
			Debug.LogWarning ("La cantidad de puntos excede la cantidad de seguridad de " + m_SecurityMaximum + ". Considera aumentar la distancia entre puntos.");
			return l_Aux;
		}

		//Debug.Log ("Columnas: " + l_Columns + ", Filas: " + l_Rows);

		RaycastHit l_Hit;

		float l_MinimumDot = 1 - m_MaxNormalDot;
		for (int i = 0; i < l_Columns; ++i) {
			for (int j = 0; j < l_Rows; ++j) {
				Vector3 l_Origin = new Vector3 ((i) * m_ProximityX + l_BoundsX.x, l_RaycastY, (j) * m_ProximityZ + l_BoundsZ.x);
				Ray l_Ray = new Ray (l_Origin, -Vector3.up);
				if (Physics.Raycast (l_Ray, out l_Hit, l_RaycastMaxY, scatterMask)) {
					if (l_Hit.collider.gameObject.name == l_Collider.gameObject.name) {
						Vector3 l_Normal = l_Hit.normal.normalized;
						float l_Dot = Vector3.Dot (l_Normal, Vector3.up);
						if (l_Dot >= l_MinimumDot) {
							if (m_UseSplat == true) {
								if (CheckSplat (l_Hit.textureCoord.x, l_Hit.textureCoord.y)) {
									AddFromPoint (l_Aux, l_Hit.point, Random.Range(m_MinMeshSize, m_MaxMeshSize));
								}
							} else {
								AddFromPoint (l_Aux, l_Hit.point, Random.Range(m_MinMeshSize, m_MaxMeshSize));
							}
						}
					}
				}
			}
		}
		return l_Aux;
	}

	private void AddFromPoint(Vector3 aPoint) { // Llamada desde la función de crear los puntos
		Vector4 l_Aux = new Vector4 (aPoint.x, aPoint.y, aPoint.z);
		l_Aux.w = Random.Range (m_MinMeshSize, m_MaxMeshSize);
		m_PointsList.Add (l_Aux);
	}

	private void AddFromPoint(List<Vector4> aOtherList, Vector3 aPoint, float aSize) { // Llamada desde la función de noise
		Vector4 l_Aux = new Vector4 (aPoint.x, aPoint.y, aPoint.z, aSize);
		aOtherList.Add (l_Aux);
	}

	void ApplyNoiseToPoints(float l_RaycastY, float l_RaycastMaxY, Collider l_Collider, List<Vector4> aPoints) {
		List<Vector4> l_RandomizedPointsList = new List<Vector4> ();
		for (int i = 0; i < aPoints.Count; ++i) {
			Vector4 l_Pos = aPoints [i];
			float l_RandomAngle = Random.Range (0f, 360f);
			// sin X, cos Z
			float l_RandomVarianceX = Random.Range (0f, m_VarianceRatio * m_ProximityX / 2f); // Obtenemos un número entre 0 y la mitad de la distancia entre los puntos (contando nuestro ratio de variación)
			float l_RandomVarianceZ = Random.Range (0f, m_VarianceRatio * m_ProximityZ / 2f);
			l_Pos.x = l_Pos.x + (Mathf.Sin (l_RandomAngle) * l_RandomVarianceX);
			l_Pos.z = l_Pos.z + (Mathf.Cos (l_RandomAngle) * l_RandomVarianceZ);


			Vector3 l_Origin = new Vector3 (l_Pos.x, l_RaycastY, l_Pos.z);
			Ray l_Ray = new Ray (l_Origin, -Vector3.up);
			RaycastHit l_Hit;
			float l_MinimumDot = 1 - m_MaxNormalDot;
			if (Physics.Raycast (l_Ray, out l_Hit, l_RaycastMaxY, scatterMask)) {
				if (l_Hit.collider.gameObject.name == l_Collider.gameObject.name) {
					Vector3 l_Normal = l_Hit.normal.normalized;
					float l_Dot = Vector3.Dot (l_Normal, Vector3.up);
					if (l_Dot >= l_MinimumDot) {
						if (m_UseSplat == true) {
							if (CheckSplat (l_Hit.textureCoord.x, l_Hit.textureCoord.y)) {
								AddFromPoint (l_RandomizedPointsList, l_Hit.point, l_Pos.w);
							}
						} else {
							AddFromPoint (l_RandomizedPointsList, l_Hit.point, l_Pos.w);
						}
					}
				}
			}
		}
		m_PointsList = l_RandomizedPointsList;
	}

	private bool CheckSplat(float u, float v) {
		Color l_Color = m_SplatImage.GetPixel ((int) (u * m_SplatImage.width), (int)(v * m_SplatImage.height));
		Vector3 l_Resta = Vector3.zero;
		l_Resta.x = l_Color.r - m_SplatColor.r;
		l_Resta.y = l_Color.g - m_SplatColor.g;
		l_Resta.z = l_Color.b - m_SplatColor.b;
		if (l_Resta.magnitude <= m_ColorThreshold) {
			return true;
		}
		return false;
	}

	private void ClearPoints() {
		m_PointsList = new List<Vector4> ();
		ScatterGizmosDebug l_SGD = GameObject.FindObjectOfType<ScatterGizmosDebug> ();
		l_SGD.m_PointsList = new List<Vector4>();
		SceneView.RepaintAll ();
	}

	private void SavePoints(int i = 0) {
		ScatterPointsData l_DataFile = ScriptableObject.CreateInstance<ScatterPointsData> ();
		ScatterGizmosDebug l_SGD = GameObject.FindObjectOfType<ScatterGizmosDebug> ();
		l_DataFile.pointsList = l_SGD.m_PointsList;
		Scene scene = SceneManager.GetActiveScene ();
		if (!AssetDatabase.IsValidFolder ("Assets/" + m_Folder)) {
			AssetDatabase.CreateFolder ("Assets", m_Folder);
		}
		if (!AssetDatabase.IsValidFolder ("Assets/" + m_Folder + "/" + scene.name)) {
			AssetDatabase.CreateFolder ("Assets/" + m_Folder, scene.name);
		}
		AssetDatabase.CreateAsset (l_DataFile, "Assets/" + m_Folder + "/" + scene.name + "/" + Selection.gameObjects [i].name + "_" + m_LastSplatName + "Points.asset");
		AssetDatabase.SaveAssets ();
		ScatterInstance si = Selection.gameObjects [i].GetComponent<ScatterInstance> ();
		if (si != null) {
			si.m_ScatterPointsData = l_DataFile;
		}
		ClearPoints ();
	}
}