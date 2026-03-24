using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BlurOnImageActive : MonoBehaviour
    {
        [Header("Blur References")]
        [SerializeField] private ScreenBlurRendererFeature _rendererFeatureScreenBlur;
        [SerializeField] private RenderObjects _rendererFeatureSecondaryUIRender;
        private CanvasGroup _canvasGroup;

        [Header("Blur Settings")]
        [SerializeField] private float _blurPower = 0.1f;


        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            _canvasGroup.alpha = 0;
            ApplyBlur(false);
        }
    
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                bool isActive = _canvasGroup.alpha <= 0;

                _canvasGroup.alpha = isActive ? 1f : 0f;
                ApplyBlur(isActive);
            }
        }

        private void ApplyBlur(bool active)
        {
            Debug.Log("apply blur effect");
            if (_rendererFeatureScreenBlur != null)
            {
                _rendererFeatureScreenBlur.SetActive(active);
                //_rendererFeatureScreenBlur.Params.BlurValue = _blurPower;
                //_rendererFeatureScreenBlur.Params.IsApplyScreen = false;
            }

            if (_rendererFeatureSecondaryUIRender != null)
            {
                _rendererFeatureSecondaryUIRender.SetActive(active);
            }
        }
    }
    