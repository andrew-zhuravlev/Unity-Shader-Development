using UnityEngine;

namespace SimpleGrayScale {
    public class CameraEffect : MonoBehaviour {
        [SerializeField] Shader imageEffectShader;
        [Range(0f, 1f), SerializeField] float grayScaleAmount;
        bool grayScaleChanged = false;

        Material imageEffect;

        private void Awake() {
            imageEffect = new Material(imageEffectShader);
        }

        private void Update() {
            imageEffect.SetFloat("_GrayScaleAmount", grayScaleAmount);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination) {
            Graphics.Blit(source, destination, imageEffect);
        }
    }
}