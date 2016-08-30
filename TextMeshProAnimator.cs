using UnityEngine;
using System.Collections;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class TextMeshProAnimator : MonoBehaviour {

	TextMeshPro m_TextMeshPro;
	[SerializeField] float interval;
	[SerializeField] Gradient m_Gradient;
	[SerializeField] AnimationCurve m_Curve;


	float curveTimeStart;
	float curveTimeEnd;
	float curveTimeDuration;

	void Awake()
	{
		m_TextMeshPro = GetComponent<TextMeshPro>();
		m_TextMeshPro.ForceMeshUpdate();

		if(m_Curve.length > 0)
		{
			curveTimeStart = m_Curve.keys[0].time;
			curveTimeEnd = m_Curve.keys[ m_Curve.length - 1].time;

			curveTimeDuration = curveTimeEnd - curveTimeStart;
		}
	}

	// Use this for initialization
	void Start () {
	
		StartCoroutine(WaitJitter());
	}


	IEnumerator WaitJitter()
	{
		

		Vector3[] vertices_Origin = m_TextMeshPro.mesh.vertices;
		#if !UNITY_EDITOR
		Vector3[] vertices = new Vector3[m_TextMeshPro.mesh.vertexCount];
		Color32[] colors = new Color32[m_TextMeshPro.mesh.vertexCount];
		#endif

		float curveT = 0;

		while(true)
		{

			#if UNITY_EDITOR
			m_TextMeshPro.ForceMeshUpdate();


			Vector3[] vertices = new Vector3[m_TextMeshPro.mesh.vertexCount];
			Color32[] colors = new Color32[m_TextMeshPro.mesh.vertexCount];
			#endif


			int characterCount = m_TextMeshPro.textInfo.characterCount;
			int charIndex = 0;


			for(int i = 0; i < characterCount; ++i)
			{
				TMP_CharacterInfo charInfo = m_TextMeshPro.textInfo.characterInfo[i];

				if(!charInfo.isVisible)
					continue;
				
				int vertexIndex = charInfo.vertexIndex;


				if(m_Curve.length > 0)
				{
					Vector3 offset = Vector3.zero;

					offset.y = m_Curve.Evaluate(curveT + curveTimeStart + ( curveTimeDuration * ( i + 1 ) ) / characterCount );

					vertices[vertexIndex + 0] = vertices_Origin[vertexIndex + 0] + offset;
					vertices[vertexIndex + 1] = vertices_Origin[vertexIndex + 1] + offset;
					vertices[vertexIndex + 2] = vertices_Origin[vertexIndex + 2] + offset;
					vertices[vertexIndex + 3] = vertices_Origin[vertexIndex + 3] + offset;
				}


				Color32 charColor = m_Gradient.Evaluate(Mathf.Repeat(  curveT + ((float) i / characterCount) , 1));

			
				colors[vertexIndex + 0] = charColor;
				colors[vertexIndex + 1] = charColor;
				colors[vertexIndex + 2] = charColor;
				colors[vertexIndex + 3] = charColor;

				++charIndex;
			}

			m_TextMeshPro.mesh.colors32 = colors;
			m_TextMeshPro.mesh.vertices = vertices;
			m_TextMeshPro.ForceMeshUpdate();


			yield return new WaitForSeconds(interval);

			curveT += interval;
		}



	}

	// Update is called once per frame
	void Update () {
	
	}
}
