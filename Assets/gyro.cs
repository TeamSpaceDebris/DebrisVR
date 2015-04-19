using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class gyro : MonoBehaviour {
#if UNITY_EDITOR
	private Vector3 rot;
#endif
	
	// Use this for initialization
	void Start () {
#if UNITY_EDITOR
		rot = transform.rotation.eulerAngles;
#else
		Input.gyro.enabled = true;
#endif

		debris[] allDebris = debris.AllDebris();
		int len = debris.Length ();
		Debug.Log(debris.AllDebris());
		for (int i=0; i < len; i++) {
			debris deb = allDebris[i];

			GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);

			go.AddComponent("SphereCollider");
			DebriData data = go.AddComponent<DebriData>();
			data.carbon(deb);

			go.transform.position = Random.onUnitSphere*10f;
			go.transform.rotation = Random.rotation;
			go.transform.position = SphericalToCartesian(10 + deb.altitude / 500, deb.longitude, deb.latitude);
		}

	}
	
	public static Vector3 SphericalToCartesian(float radius, float polar, float elevation){
		float a = radius * Mathf.Cos(elevation);
		float x = a * Mathf.Cos(polar);
		float y = radius * Mathf.Sin(elevation);
		float z = a * Mathf.Sin(polar);
		return new Vector3 (x, y, z);
	}
	
	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
		float spd = Time.deltaTime*100.0f;
		if(Input.GetKey(KeyCode.LeftArrow)){
			rot.y -= spd;
		}
		if(Input.GetKey(KeyCode.RightArrow)){
			rot.y += spd;
		}
		if(Input.GetKey(KeyCode.UpArrow)){
			rot.x -= spd;
		}
		if(Input.GetKey(KeyCode.DownArrow)){
			rot.x += spd;
		}
		transform.rotation = Quaternion.Euler(rot);
#else

		// (*) :: Quaternion -> Quaternion -> Quaternion
		transform.rotation = Quaternion.AngleAxis (90.0f, Vector3.right)
						* Input.gyro.attitude
						* Quaternion.AngleAxis (180.0f, Vector3.forward);
#endif
		// Touch detection
		if (Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit = new RaycastHit();

			// When hit
			if (Physics.Raycast(ray, out hit)) {
				GameObject obj = hit.collider.gameObject;
				DebriData data = obj.GetComponent<DebriData>();

				// get sliders GUI
				sliders[] sl_array = FindObjectsOfType<sliders>();
				sliders sl = sl_array[0];
				sl.debri = data;
			}
		}
	}
}
