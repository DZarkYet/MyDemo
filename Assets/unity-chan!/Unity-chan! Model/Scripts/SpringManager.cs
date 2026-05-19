//
//SpingManager.cs for unity-chan!
//
//Original Script is here:
//ricopin / SpingManager.cs
//Rocket Jump : http://rocketjump.skr.jp/unity3d/109/
//https://twitter.com/ricopin416
//
//Revised by N.Kobayashi 2014/06/24
//           Y.Ebata
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace UnityChan
{
	public class SpringManager : MonoBehaviour
	{
		//Kobayashi
		// DynamicRatio is paramater for activated level of dynamic animation 
		public float dynamicRatio = 1.0f;

		//Ebata
		public float			stiffnessForce;
		public AnimationCurve	stiffnessCurve;
		public float			dragForce;
		public AnimationCurve	dragCurve;
		public SpringBone[] springBones;

		void Start ()
		{
			UpdateParameters ();
		}

        void Update ()
		{
#if UNITY_EDITOR
		//Kobayashi
		if(dynamicRatio >= 1.0f)
			dynamicRatio = 1.0f;
		else if(dynamicRatio <= 0.0f)
			dynamicRatio = 0.0f;
		//Ebata
		UpdateParameters();
#endif
		}

		[ContextMenu("BindObj")]
		public void GetEveryBones()
		{
			Transform[] transforms = GetComponentsInChildren<Transform>();
			foreach(Transform t in transforms)
			{
				if(t.name.StartsWith("Pd_"))
				{
					if(t.childCount > 0)
					{
						SpringBone springBone = t.gameObject.AddComponent<SpringBone>();
						springBone.boneAxis = new Vector3(0, 1, 0);
						springBone.child = t.GetChild(0);
					}
				}
			}
			springBones = GetComponentsInChildren<SpringBone>();
		}

		[ContextMenu("BindCollider")]
		public void BindCollider()
		{
            Transform[] transforms = GetComponentsInChildren<Transform>();
			List<SpringCollider> colliders = new List<SpringCollider>();
            foreach (Transform t in transforms)
            {
                if (t.name == "LeftKneeCollider" || t.name == "LeftLegCollider" ||
					t.name == "RightKneeCollider" || t.name == "RightLegCollider")
                {
					colliders.Add(t.GetComponent<SpringCollider>());
                }
            }
            foreach (Transform t in transforms)
            {
                if (t.name.StartsWith("Pd_") || t.name.StartsWith("Skirt_") || t.name.StartsWith("Clothes_"))
                {
                    SpringBone sb = t.GetComponent<SpringBone>();
                    if (sb != null)
                    {
                        sb.colliders = colliders.ToArray();
                    }
                }
            }
        }

		private void LateUpdate ()
		{
			//Kobayashi
			if (dynamicRatio != 0.0f) {
				for (int i = 0; i < springBones.Length; i++) {
					if (dynamicRatio > springBones [i].threshold) {
						springBones [i].UpdateSpring ();
					}
				}
			}
		}

		private void UpdateParameters ()
		{
			UpdateParameter ("stiffnessForce", stiffnessForce, stiffnessCurve);
			UpdateParameter ("dragForce", dragForce, dragCurve);
		}
	
		private void UpdateParameter (string fieldName, float baseValue, AnimationCurve curve)
		{
			var start = curve.keys [0].time;
			var end = curve.keys [curve.length - 1].time;
			//var step	= (end - start) / (springBones.Length - 1);
		
			var prop = springBones [0].GetType ().GetField (fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
		
			for (int i = 0; i < springBones.Length; i++) {
				//Kobayashi
				if (!springBones [i].isUseEachBoneForceSettings) {
					var scale = curve.Evaluate (start + (end - start) * i / (springBones.Length - 1));
					prop.SetValue (springBones [i], baseValue * scale);
				}
			}
		}
	}
}