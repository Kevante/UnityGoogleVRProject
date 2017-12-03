using UnityEngine;
using System.Collections;
using UnityEngine.Events;



[System.Serializable]
public class Trigger_Event : UnityEvent<Collider>{}


public class Dynamic_Laser : MonoBehaviour 
{
	[Space(15)]

	[SerializeField()]
	bool enable_at_start = true;

	[Space(5)]

	[SerializeField()]
	[Range(12 , 30)]
	float laser_fade_speed = 18;

	[Space(15)]

	[Header("Slicing (Experimental)")]
	[SerializeField()]
	bool slicing = false;
	[SerializeField()]
	LayerMask slice_layer;
	[SerializeField()]
	Material sliced_material;

	[Space(20)]

	[Header("Flickering")]
	[SerializeField()]
	bool flickering;
	[SerializeField()][Range(0.01f , 0.05f)]
	float flick_rate = 0.025f;
	[SerializeField()][Range(0.1f , 0.9f)]
	float flick_min = 0.5f;
	[SerializeField()]
	Light laser_light;
	[SerializeField()][Range(1,8)]
	float light_multiplier = 2f;

    [Space(20)]

    [Header("Beam Glow")]
    [SerializeField()]
    bool glowing;
    [SerializeField()]
    Light beamGlow;
    [SerializeField()] [Range(0, 10)]
    float brightness;

    [Space(20)]

	[Header("Lens Flare")]
	[SerializeField()]
	LensFlare hit_lens;
	[SerializeField()][Range(0,50)]
	float lens_min = 50f;
	[SerializeField()][Range(50,100)]
	float lens_max = 70f;

	[Space(20)]

	[Header("Particles")]
	[SerializeField()]
	ParticleSystem lightning_particle;
	[SerializeField()]
	ParticleSystem smoke_particle;


	[Space(15)]

	[Header("Sounds")]
	[SerializeField()]
	AudioClip hit_sound;
	[SerializeField()]
	AudioClip idle_sound;
	[SerializeField()]
	AudioClip move_sound;
	[SerializeField()]
	AudioClip show_sound;

	[Space(15)]
	[Header("Trigger Events")]
	[SerializeField()]
	Trigger_Event On_Trigger_Enter;
	[SerializeField()]
	Trigger_Event On_Trigger_Stay;
	[SerializeField()]
	Trigger_Event On_Trigger_Exit;


    public bool active;

    Transform bone_target;
	float lerp_time;
	float current_time;
	bool reset;
	Vector3 last_pos;
	float flick_timer;
	float fast_move_timer;
	LayerMask laser_layer_mask;
	RaycastHit rhit_laser;

	Vector3 coll_start_point;
	Vector3 coll_end_point_1;
	Vector3 coll_end_point_2;
	bool slice_started;

	Transform laser_trans;
	Transform laser_bone;
	Transform rig_trans;
	Transform hand_trans;
	Transform origin;
	Transform laser_point;
	Animation anim;
	Laser_Collider laser_coll;
	AudioSource audio_source;






#region MONO Behaviour

	// Use this for initialization
	void Start ()
	{
		// Chaching References
		laser_trans = transform.Find ("laser");
		rig_trans = laser_trans.Find ("rig");
		laser_bone = rig_trans.Find ("Bone03");
		laser_coll = laser_trans.GetComponent<Laser_Collider>();
		origin = rig_trans.Find ("origin");
		anim = laser_trans.GetComponent<Animation>();
		audio_source = laser_trans.GetComponent<AudioSource>();

		// disable
		active = false;
		rig_trans.localScale = Vector3.zero;
		laser_coll.Enable_Collider (false);

		// get layer masks
		laser_layer_mask = 1 << LayerMask.NameToLayer("LaserSword");

		// registering for trigger events
		laser_coll.OnCollEnter += TriggerEnter;
		laser_coll.OnCollStay += TriggerStay;
		laser_coll.OnCollExit += TriggerExit;

		Create_Target ();

		if(enable_at_start)
			_Enable ();

        glowing = active;
	}



	void Update () 
	{
        if (glowing) {
            beamGlow.enabled = true;
            while (beamGlow.intensity < brightness)
                beamGlow.intensity += Time.deltaTime / .4f;
        } else {
            beamGlow.intensity -= Time.deltaTime / .4f;
            if (beamGlow.intensity.Equals(0))
                beamGlow.enabled = false;
        }

        if (GvrControllerInput.AppButtonDown) {
            if (active) {
                _Disable();
            } else {
                _Enable();
            }
        }
        
        Update_Laser();
    }


	void FixedUpdate()
	{
		if (!flickering || !active)
			return;

		flick_timer += Time.deltaTime;
		if (flick_timer > flick_rate)
		{
			flick_timer = 0f;
			float r = Random.Range (flick_min, 1f);

			rig_trans.localScale = new Vector3 (r*1f, 1f, r*1f);

			if (laser_light != null)
				laser_light.intensity = r*light_multiplier;
		}
	}


	// TRIGGER ENTER
	void TriggerEnter(Collider coll)
	{
		if (coll.GetComponent<Laser_Collider> () != null)
		{
			if(hit_lens != null)
				hit_lens.enabled = true;
			audio_source.PlayOneShot (hit_sound);
		}

		if(slicing)
		if(slice_layer == (slice_layer | (1 << coll.gameObject.layer)))
		{
			slice_started = true;
		}

		if (On_Trigger_Enter != null)
			On_Trigger_Enter.Invoke (coll);
	}


	// TRIGGER STAY
	void TriggerStay(Collider coll)
	{
		if (coll.GetComponent<Laser_Collider> () != null)
		{
			if (Physics.Linecast (laser_trans.position, origin.position, out rhit_laser, laser_layer_mask))
			{
				if (hit_lens != null) 
				{
					hit_lens.brightness = Random.Range (lens_min, lens_max);
					hit_lens.transform.position = rhit_laser.point;
				}
			}
		}

		if(slicing && slice_started)
		if(slice_layer == (slice_layer | (1 << coll.gameObject.layer)))
		{
			if (Physics.Linecast (laser_trans.position, origin.position, out rhit_laser, slice_layer)) 
			{
				coll_start_point = rhit_laser.point;
				audio_source.PlayOneShot (hit_sound);
				slice_started = false;
			}
		}

		if (On_Trigger_Stay != null)
			On_Trigger_Stay.Invoke (coll);
	}


	// TRIGGER EXIT
	void TriggerExit(Collider coll)
	{
		if (On_Trigger_Exit != null)
			On_Trigger_Exit.Invoke (coll);
		
		if (coll.GetComponent<Laser_Collider> () != null)
		{
			if(hit_lens != null)
				hit_lens.enabled = false;
		}

		if(slicing)
		if(slice_layer == (slice_layer | (1 << coll.gameObject.layer)))
		{
			coll_end_point_1 = coll.ClosestPointOnBounds (laser_trans.position);
			coll_end_point_2 = coll.ClosestPointOnBounds (origin.position);
			Plane plane = new Plane (coll_start_point, coll_end_point_1, coll_end_point_2);
			Vector3 center = (coll_start_point + coll_end_point_1 + coll_end_point_2) / 3;

			Material mat = sliced_material;
			if (mat == null)
				mat = coll.GetComponent<Renderer> ().material;
			
			GameObject[] gos = MeshSlicer.Cut (coll.gameObject , center , plane.normal , mat);

			Destroy(coll);

			gos [1].gameObject.layer = coll.gameObject.layer;
			if(!gos [1].GetComponent<Rigidbody> ())
				gos [1].AddComponent<Rigidbody> ();
			MeshCollider mc1 = gos [1].AddComponent<MeshCollider> ();
			mc1.convex = true;

			gos [0].gameObject.layer = coll.gameObject.layer;
			if(!gos [0].GetComponent<Rigidbody> ())
				gos [0].AddComponent<Rigidbody> ();
			MeshCollider mc2 = gos [0].AddComponent<MeshCollider> ();
			mc2.convex = true;
		}
	}

#endregion






#region PUBLIC
	/// <summary>
	/// Enable_s the laser effect.
	/// </summary>
	public void _Enable()
	{
		if (active)
			return;
		
		active = true;

		anim.Play ("open");

		audio_source.clip = idle_sound;
		audio_source.loop = true;
		audio_source.Play ();
		audio_source.PlayOneShot (show_sound);

		laser_coll.Enable_Collider (true);
		laser_point.position = origin.position;
		fast_move_timer = 0;

		if (laser_light != null) {
            laser_light.enabled = true;
            glowing = true;
        }
		if (lightning_particle != null && lightning_particle.gameObject.activeSelf == true)
			lightning_particle.Play ();
		if (smoke_particle != null && smoke_particle.gameObject.activeSelf == true)
			smoke_particle.Play ();
	}

	/// <summary>
	/// Disable_s the laser effect.
	/// </summary>
	public void _Disable()
	{
		if (!active)
			return;

		active = false;

		anim.Play ("close");

		laser_coll.Enable_Collider (false);
		audio_source.loop = false;
		audio_source.PlayOneShot (show_sound);

		if (laser_light != null) {
            laser_light.enabled = false;
            glowing = false;
        }
		if (lightning_particle != null && lightning_particle.gameObject.activeSelf == true)
			lightning_particle.Stop ();
		if (smoke_particle != null && smoke_particle.gameObject.activeSelf == true)
			smoke_particle.Stop ();
		if(hit_lens != null)
			hit_lens.enabled = false;
	}

#endregion








#region Private
	void Update_Laser()
	{
		if (!active)
			return;
		
		if (!bone_target)
			return;

		bone_target.position = Vector3.Lerp(bone_target.position , origin.position , lerp_time);
		if (Vector3.Distance (bone_target.position, origin.position) > 0.1f)
		{
			reset = false;
			current_time = 0;
		}

		if(!reset)
		{
			Vector3 temp = laser_bone.position - bone_target.position;
			laser_bone.right = temp;
			lerp_time = Time.deltaTime * laser_fade_speed;
		}
		else
		{
			laser_bone.localEulerAngles = new Vector3 (0, 0, 269.7f);
		}

		current_time += Time.deltaTime;
		if(current_time > lerp_time-0.1f)
		{
			reset = true;
		}


		// check for fast move (also play audio)
		fast_move_timer += Time.deltaTime;
		if(fast_move_timer > 0.3f)
		if (Vector3.Distance (bone_target.position, origin.position) > 0.5f) 
		{
			fast_move_timer = 0;
			audio_source.PlayOneShot (move_sound);
		}
	}
		



	void Create_Target()
	{
		GameObject go = new GameObject ("_laser_point");
		laser_point = go.transform;
		bone_target = go.transform;
		bone_target.position = origin.position;

		laser_bone.localEulerAngles = origin.localEulerAngles;
		bone_target.eulerAngles = origin.eulerAngles;
	}

#endregion




//	void OnDrawGizmos()
//	{
//		Gizmos.color = Color.blue;
//		Gizmos.DrawSphere (coll_start_point , 0.1f);
//		Gizmos.color = Color.red;
//		Gizmos.DrawSphere (coll_end_point_1 , 0.1f);
//		Gizmos.color = Color.red;
//		Gizmos.DrawSphere (coll_end_point_2 , 0.1f);
//	}

}
