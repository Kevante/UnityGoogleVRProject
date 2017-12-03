using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode()]
public class scale_controller : MonoBehaviour 
{


	#if UNITY_EDITOR

	[SerializeField()]
	Transform target_trans;
	[SerializeField()]
	float coeff = 1f;


	// Update is called once per frame
	void Update () 
	{
		if (Application.isPlaying)
			return;

		if (!target_trans)
			return;

		transform.localScale = new Vector3 (target_trans.localScale.x , target_trans.localScale.z , target_trans.localScale.y) * coeff;
	}


	#endif
}
