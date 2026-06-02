using PasserCard.Table;
using UnityEngine;
using UnityEngine.UI;

namespace PasserCard.UI
{
    /// <summary>
    /// Balatro-style layered table: scrolling felt backdrop, play-area glow, optional fog overlay, banner.
    /// </summary>
    public sealed class TableBackdropController : MonoBehaviour
    {
        private Image? _backdrop;
        private Image? _playArea;
        private Image? _fogOverlay;
        private Text? _titleText;
        private Text? _ruleText;

        private Material? _backdropMaterial;
        private Material? _playAreaMaterial;
        private Material? _fogMaterial;

        private TableEnvironmentVisual _visual;
        private float _fogIntensity = 0.5f;
        private bool _initialized;

        public TableEnvironmentVisual CurrentVisual => _visual;

        public static TableBackdropController Create(Transform parent)
        {
            var root = new GameObject("TableBackdrop", typeof(RectTransform), typeof(TableBackdropController));
            root.transform.SetParent(parent, false);
            var rect = root.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var controller = root.GetComponent<TableBackdropController>();
            controller.BuildLayers();
            controller.Apply(TableEnvironmentLibrary.FogMist);
            return controller;
        }

        private void BuildLayers()
        {
            _backdrop = CreateFullScreenLayer("Backdrop", transform, out _backdropMaterial, "PasserCard/UI/TableBackdrop");
            _playArea = CreatePlayAreaLayer("PlayAreaGlow", transform, out _playAreaMaterial, "PasserCard/UI/TablePlayArea");
            _fogOverlay = CreateFullScreenLayer("FogOverlay", transform, out _fogMaterial, "PasserCard/UI/FogOverlay");

            var banner = new GameObject("TableBanner", typeof(RectTransform));
            banner.transform.SetParent(transform, false);
            var bannerRect = banner.GetComponent<RectTransform>();
            bannerRect.anchorMin = new Vector2(0.5f, 1f);
            bannerRect.anchorMax = new Vector2(0.5f, 1f);
            bannerRect.pivot = new Vector2(0.5f, 1f);
            bannerRect.anchoredPosition = new Vector2(0f, -12f);
            bannerRect.sizeDelta = new Vector2(520f, 72f);

            _titleText = CreateBannerText(banner.transform, "Title", 26, TextAnchor.UpperCenter,
                new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0f, 28f), new Vector2(0f, 0f));
            _ruleText = CreateBannerText(banner.transform, "Rule", 15, TextAnchor.LowerCenter,
                new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(0f, 0f), new Vector2(0f, 4f));
            _ruleText.color = new Color(0.78f, 0.82f, 0.86f, 0.9f);

            _initialized = true;
        }

        public void Apply(TableEnvironmentVisual visual)
        {
            _visual = visual;
            if (!_initialized)
            {
                return;
            }

            ApplyBackdropMaterial(visual);
            ApplyPlayAreaMaterial(visual);
            ApplyFogMaterial(visual);

            if (_titleText != null)
            {
                _titleText.text = visual.DisplayName;
                _titleText.color = Color.Lerp(visual.AccentColor, Color.white, 0.35f);
            }

            if (_ruleText != null)
            {
                _ruleText.text = visual.RuleSummary;
            }

            if (_fogOverlay != null)
            {
                _fogOverlay.enabled = visual.UseFogOverlay;
            }
        }

        public void SetFogIntensity(float intensity)
        {
            _fogIntensity = Mathf.Clamp01(intensity);
            if (_fogMaterial != null)
            {
                _fogMaterial.SetFloat("_Intensity", _fogIntensity);
            }
        }

        public void SetPlayAreaPulse(float pulse)
        {
            if (_playAreaMaterial != null)
            {
                _playAreaMaterial.SetFloat("_Pulse", pulse);
            }
        }

        private void Update()
        {
            if (!_initialized || _backdropMaterial == null)
            {
                return;
            }

            var pulse = Mathf.Sin(Time.unscaledTime * _visual.PulseSpeed) * 0.5f + 0.5f;
            SetPlayAreaPulse(pulse);
        }

        private void ApplyBackdropMaterial(TableEnvironmentVisual visual)
        {
            if (_backdropMaterial == null)
            {
                return;
            }

            _backdropMaterial.SetColor("_BaseColor", visual.BaseColor);
            _backdropMaterial.SetColor("_AccentColor", visual.AccentColor);
            _backdropMaterial.SetFloat("_NoiseScale", visual.NoiseScale);
            _backdropMaterial.SetFloat("_ScrollSpeed", visual.ScrollSpeed);
            _backdropMaterial.SetFloat("_VignettePower", visual.VignettePower);
            _backdropMaterial.SetFloat("_PulseSpeed", visual.PulseSpeed);
            _backdropMaterial.SetFloat("_Shimmer", visual.UseShimmer ? 1f : 0f);
            _backdropMaterial.SetFloat("_CrackPulse", visual.UseCrackPulse ? 1f : 0f);
        }

        private void ApplyPlayAreaMaterial(TableEnvironmentVisual visual)
        {
            if (_playAreaMaterial == null)
            {
                return;
            }

            _playAreaMaterial.SetColor("_Color", visual.PlayAreaColor);
            _playAreaMaterial.SetColor("_AccentColor", visual.AccentColor);
        }

        private void ApplyFogMaterial(TableEnvironmentVisual visual)
        {
            if (_fogMaterial == null)
            {
                return;
            }

            _fogMaterial.SetColor("_Color", visual.FogColor);
            _fogMaterial.SetFloat("_Intensity", _fogIntensity);
            _fogMaterial.SetFloat("_ScrollSpeed", visual.ScrollSpeed * 0.75f);
        }

        private static Image CreateFullScreenLayer(string name, Transform parent, out Material material, string shaderName)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var image = go.GetComponent<Image>();
            image.raycastTarget = false;
            material = CreateUiMaterial(shaderName);
            image.material = material;
            image.color = Color.white;
            return image;
        }

        private static Image CreatePlayAreaLayer(string name, Transform parent, out Material material, string shaderName)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.64f);
            rect.anchorMax = new Vector2(0.5f, 0.64f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(820f, 260f);

            var image = go.GetComponent<Image>();
            image.raycastTarget = false;
            material = CreateUiMaterial(shaderName);
            image.material = material;
            image.color = Color.white;
            return image;
        }

        private static Material CreateUiMaterial(string shaderName)
        {
            var shader = Shader.Find(shaderName);
            if (shader == null)
            {
                Debug.LogWarning($"[PasserCard] Shader not found: {shaderName}");
                return new Material(Shader.Find("UI/Default"));
            }

            return new Material(shader);
        }

        private static Text CreateBannerText(Transform parent, string name, int fontSize, TextAnchor anchor,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
            var text = go.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = fontSize;
            text.fontStyle = FontStyle.Bold;
            text.alignment = anchor;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        private void OnDestroy()
        {
            DestroyMaterial(_backdropMaterial);
            DestroyMaterial(_playAreaMaterial);
            DestroyMaterial(_fogMaterial);
        }

        private static void DestroyMaterial(Material? material)
        {
            if (material != null)
            {
                Destroy(material);
            }
        }
    }
}
