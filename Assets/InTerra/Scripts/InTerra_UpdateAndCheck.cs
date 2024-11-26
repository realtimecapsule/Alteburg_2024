using System.Collections.Generic;
using UnityEngine;
using System;

namespace InTerra
{
	[Serializable] public class DictionaryMaterialTerrain : SerializableDictionary<Material, Terrain> { }
	[Serializable] public class DictionaryMaterialMeshTerrain : SerializableDictionary<Material, MeshRenderer> { }

	[AddComponentMenu("/")]
	public class InTerra_UpdateAndCheck : MonoBehaviour
	{
		[SerializeField, HideInInspector] public bool FirstInit;
		[SerializeField, HideInInspector] public DictionaryMaterialTerrain MaterialTerrain = new DictionaryMaterialTerrain();
		[SerializeField, HideInInspector] public DictionaryMaterialMeshTerrain MaterialMeshTerrain = new DictionaryMaterialMeshTerrain();

		[SerializeField, HideInInspector] public List<MeshRenderer> MeshTerrainsList = new List<MeshRenderer>();

		[SerializeField, HideInInspector] public bool TracksEnabled;
		[SerializeField, HideInInspector] public bool TracksFading;
		[SerializeField, HideInInspector] public float TracksFadingTime = 30.0f;
		[HideInInspector] public RenderTexture TrackTexture;
		[HideInInspector] public float TrackDepthIndex;

		[SerializeField, HideInInspector] public float GlobalSmoothness = 0;

		[SerializeField, HideInInspector] public InTerra_GlobalData GlobalData;

		void Update()
		{
			if (!InTerra_Setting.DisableAllAutoUpdates) InTerra_Data.CheckAndUpdate();
		}
	}
}
