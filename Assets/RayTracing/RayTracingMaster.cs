using System.Collections.Generic;
using UnityEngine;

struct Sphere {
    public Vector3 position;
    public float radius;
    public Vector3 albedo;
    public Vector3 specular;
};


public class RayTracingMaster : MonoBehaviour
{
    [SerializeField] Texture skyboxTexture;

    private Camera _camera;

    [SerializeField] Light directionalLight;

    private uint currentSample = 0;
    private Material mat;


    public Vector2 SphereRadius = new Vector2(3.0f, 8.0f);
    public uint SpheresMax = 100;
    public float SpherePlacementRadius = 100.0f;
    private ComputeBuffer _sphereBuffer;


    private void OnEnable() {
        currentSample = 0;
        SetUpScene();
    }

    private void OnDisable() {
        if (_sphereBuffer != null)
            _sphereBuffer.Release();
    }

    private void SetUpScene() {
        List<Sphere> spheres = new List<Sphere>();

        // Add a number of random spheres
        for (int i = 0; i < SpheresMax; i++) {
            Sphere sphere = new Sphere();

            // Radius and radius
            sphere.radius = SphereRadius.x + Random.value * (SphereRadius.y - SphereRadius.x);
            Vector2 randomPos = Random.insideUnitCircle * SpherePlacementRadius;
            sphere.position = new Vector3(randomPos.x, sphere.radius, randomPos.y);

            // Reject spheres that are intersecting others
            foreach (Sphere other in spheres) {
                float minDist = sphere.radius + other.radius;
                if (Vector3.SqrMagnitude(sphere.position - other.position) < minDist * minDist)
                    goto SkipSphere;
            }

            // Albedo and specular color
            Color color = Random.ColorHSV();
            bool metal = Random.value < 0.5f;
            sphere.albedo = metal ? Vector3.zero : new Vector3(color.r, color.g, color.b);
            sphere.specular = metal ? new Vector3(color.r, color.g, color.b) : Vector3.one * 0.04f;

            // Add the sphere to the list
            spheres.Add(sphere);

        SkipSphere:
            continue;
        }

        // Assign to compute buffer
        _sphereBuffer = new ComputeBuffer(spheres.Count, 40);
        _sphereBuffer.SetData(spheres);
    }



    private void Awake() {
        _camera = GetComponent<Camera>();
    }

    private void Update() {
        if (transform.hasChanged) {
            currentSample = 0;
            transform.hasChanged = false;
        }
    }

    private void SetShaderParameters() {
        Vector3 l = directionalLight.transform.forward;
        raytracingShader.SetVector("_DirectionalLight", new Vector4(l.x, l.y, l.z, directionalLight.intensity));

        raytracingShader.SetBuffer(0, "_Spheres", _sphereBuffer);

        raytracingShader.SetVector("_PixelOffset", new Vector2(Random.value, Random.value));
        raytracingShader.SetTexture(0, "_SkyboxTexture", skyboxTexture);
        raytracingShader.SetMatrix("_CameraToWorld", _camera.cameraToWorldMatrix);
        raytracingShader.SetMatrix("_CameraInverseProjection", _camera.projectionMatrix.inverse);
    }

    public ComputeShader raytracingShader;

    RenderTexture targetTexture;

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        //Debug.Log("OnRenderImage");

        SetShaderParameters();

        Render(destination);
    }

    void Render(RenderTexture destination) {
        
        // Make sure we have a current render target
        CheckRenderTexture();

        // Set the target and dispatch the compute shader
        raytracingShader.SetTexture(0, "Result", targetTexture);
        int threadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);
        raytracingShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        // Blit the result texture to the screen
        if (mat == null)
            mat = new Material(Shader.Find("Hidden/AddShader"));
        mat.SetFloat("_Sample", currentSample);
        Graphics.Blit(targetTexture, destination, mat);
        currentSample++;
    }

    void CheckRenderTexture() {

        if (targetTexture == null || targetTexture.width != Screen.width || targetTexture.height != Screen.height) {

            currentSample = 0;

            // Release render texture if we already have one
            if (targetTexture != null)
                targetTexture.Release();

            // Get a render target for Ray Tracing
            targetTexture = new RenderTexture(Screen.width, Screen.height, 0,
                RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            targetTexture.enableRandomWrite = true;
            targetTexture.Create();
        }
    }
}
