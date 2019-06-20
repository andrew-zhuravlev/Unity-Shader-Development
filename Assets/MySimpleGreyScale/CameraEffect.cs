using UnityEngine;

public class CameraEffect : MonoBehaviour
{
    [SerializeField] Shader imageEffectShader;

    Material imageEffect;

    private void Awake() {
        imageEffect = new Material(imageEffectShader);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        Graphics.Blit(source, destination, imageEffect);
    }
}
