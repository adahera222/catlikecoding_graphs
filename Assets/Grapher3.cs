using UnityEngine;
using System.Collections;

public class Grapher3 : MonoBehaviour {
	
	public enum FunctionOption {
		Linear,
		Cubic,
		Parabola,
		Sine,
		Ripple
	}
	
	public FunctionOption function;
	public int resolution = 10;
	
	private delegate float FunctionDelegate (Vector3 p, float t);
	private static FunctionDelegate[] functionDelegates = {
		Linear,
		Exponential,
		Parabola,
		Sine,
		Ripple
	};
	
	private int currentResolution;
	private ParticleSystem.Particle[] points;
		
	// Use this for initialization
	void Start () {
		CreatePoints();
	}
	
	private void CreatePoints() {
		// prevent divide by zero
		if (resolution < 2) {
			resolution = 2;
		}
		else if (resolution > 40){
			resolution = 40;
		}
		currentResolution = resolution;
		
		points = new ParticleSystem.Particle[resolution * resolution * resolution]; // 3-dimensions
		float increment = 1f / (resolution - 1);
		int i = 0;
		for(int x = 0; x < resolution; x++){
			for(int z = 0; z < resolution; z++){
				for(int y = 0; y < resolution; y++){
					Vector3 p = new Vector3(x, y, z) * increment;
					points[i].position = p;
					points[i].color = new Color(p.x, p.y, p.z);
					points[i++].size = 0.1f;
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (currentResolution != resolution) {
			CreatePoints();
		}
		
		// set the Y position of the points
		FunctionDelegate f = functionDelegates[(int)function];
		float t = Time.timeSinceLevelLoad;
		for (int i = 0; i < points.Length; i++) {
			Color c = points[i].color;
			c.a = f(points[i].position, t);
			points[i].color = c;
		}
		
		particleSystem.SetParticles(points, points.Length);
	}
	
	private static float Linear (Vector3 p, float t) {
		return 1f - p.x - p.y - p.z + 0.5f * Mathf.Sin(t);
	}
	
	private static float Exponential (Vector3 p, float t) {
		return 1f - p.x * p.x - p.y * p.y - p.z * p.z + 0.5f * Mathf.Sin(t);
	}

	private static float Parabola (Vector3 p, float t) {
		p.x = 2f * p.x - 1f;
		p.z = 2f * p.z - 1f;
		return 1f - p.x * p.x * p.z * p.z;
	}

	private static float Sine (Vector3 p, float t) {
		return 0.50f +
			0.25f * Mathf.Sin(4 * Mathf.PI * p.x + 4 * t) * Mathf.Sin(2 * Mathf.PI * p.z + t) +
			0.10f * Mathf.Cos(3 * Mathf.PI * p.x + 5 * t) * Mathf.Cos(5 * Mathf.PI * p.z + 3 * t) +
			0.15f * Mathf.Sin(Mathf.PI * p.x + 0.6f * t);
	}
	
	private static float Ripple (Vector3 p, float t) {
		float squareRadius = (p.x - 0.5f) * (p.x - 0.5f) + (p.z - 0.5f) * (p.z - 0.5f);
		return 0.5f + Mathf.Sin(15 * Mathf.PI * squareRadius - 2f * t) / (2f + 100f * squareRadius);
	}
}
