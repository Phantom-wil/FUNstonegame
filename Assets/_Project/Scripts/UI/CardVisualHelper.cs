using PasserCard.Cards;
using UnityEngine;
using UnityEngine.UI;

namespace PasserCard.UI
{
    /// <summary>
    /// Layered card: parchment face + keyed rank/suit art on top.
    /// </summary>
    public static class CardVisualHelper
    {
        public const float HandCardWidth = 118f;
        public const float HandCardHeight = 165f;
        public const float PlaySlotWidth = 132f;
        public const float PlaySlotHeight = 185f;

        private const float ArtInsetX = 0.05f;
        private const float ArtInsetY = 0.04f;
        /// <summary>Compensates deck art sitting ~20% card-width left of parchment center.</summary>
        private const float ArtShiftX = -0.3f;
        private const float SelectBorderThickness = 3f;
        private const float SelectBorderOutset = 2f;

        private static readonly Color SelectBorderColor = new(1f, 0.82f, 0.28f, 1f);

        private static Material? _keyedArtMaterial;
        private static Sprite? _whitePixelSprite;

        public static Button CreateCardButton(Transform parent, string name, float width, float height)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);

            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(width, height);

            var rootImage = go.GetComponent<Image>();
            rootImage.color = new Color(1f, 1f, 1f, 0.01f);
            rootImage.raycastTarget = true;

            CreateLayerImage(go.transform, "Face", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, false);
            CreateArtLayer(go.transform, width, height);
            CreateSelectBorder(go.transform);

            var textGo = new GameObject("Badge", typeof(RectTransform), typeof(Text));
            textGo.transform.SetParent(go.transform, false);
            var textRect = textGo.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(1f, 0f);
            textRect.anchorMax = new Vector2(1f, 0f);
            textRect.pivot = new Vector2(1f, 0f);
            textRect.anchoredPosition = new Vector2(-6f, 6f);
            textRect.sizeDelta = new Vector2(28f, 22f);
            var text = textGo.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 14;
            text.alignment = TextAnchor.LowerRight;
            text.color = new Color(0.15f, 0.12f, 0.1f, 0.95f);
            text.raycastTarget = false;

            var button = go.GetComponent<Button>();
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.white;
            colors.pressedColor = new Color(0.95f, 0.95f, 0.95f, 1f);
            colors.selectedColor = Color.white;
            button.colors = colors;
            button.targetGraphic = rootImage;
            return button;
        }

        public static void ApplyPlayingCard(Button button, PlayingCardInstance? card, bool selected, bool fogBadge)
        {
            BalatroSpriteLibrary.EnsureLoaded();
            EnsureMaterial();

            var face = button.transform.Find("Face")?.GetComponent<Image>();
            var art = button.transform.Find("Art")?.GetComponent<Image>();
            var badge = button.transform.Find("Badge")?.GetComponent<Text>();

            if (face == null || art == null || badge == null)
            {
                return;
            }

            face.sprite = BalatroSpriteLibrary.CardFace;
            face.type = Image.Type.Simple;
            face.preserveAspect = true;
            face.color = Color.white;
            face.enabled = BalatroSpriteLibrary.CardFace != null;

            SetSelectedBorder(button, selected);

            if (card != null)
            {
                var sprite = BalatroSpriteLibrary.GetCardSprite(card);
                art.sprite = sprite;
                art.type = Image.Type.Simple;
                art.preserveAspect = true;
                art.color = Color.white;
                art.material = sprite != null ? _keyedArtMaterial : null;
                art.enabled = sprite != null;

                if (card.CornerA != card.CornerB)
                {
                    badge.text = card.IsFlipped ? "B" : "A";
                    badge.enabled = true;
                }
                else
                {
                    badge.text = fogBadge ? "雾" : string.Empty;
                    badge.enabled = fogBadge;
                }

                return;
            }

            art.sprite = null;
            art.enabled = false;
            badge.text = string.Empty;
            badge.enabled = false;
        }

        public static void ApplyFogSlot(Button button)
        {
            BalatroSpriteLibrary.EnsureLoaded();
            EnsureMaterial();

            var face = button.transform.Find("Face")?.GetComponent<Image>();
            var art = button.transform.Find("Art")?.GetComponent<Image>();
            var badge = button.transform.Find("Badge")?.GetComponent<Text>();

            if (face == null || art == null || badge == null)
            {
                return;
            }

            face.enabled = false;
            SetSelectedBorder(button, false);

            art.sprite = BalatroSpriteLibrary.FogSlotBack;
            art.type = Image.Type.Simple;
            art.preserveAspect = true;
            art.material = null;
            art.color = new Color(0.72f, 0.74f, 0.78f, 1f);
            art.enabled = BalatroSpriteLibrary.FogSlotBack != null;

            badge.text = "雾槽";
            badge.fontSize = 13;
            badge.alignment = TextAnchor.LowerCenter;
            badge.color = new Color(0.92f, 0.94f, 0.98f, 0.95f);
            badge.enabled = true;
        }

        public static void ApplyEmptySlot(Button button, bool fog)
        {
            BalatroSpriteLibrary.EnsureLoaded();
            EnsureMaterial();

            var face = button.transform.Find("Face")?.GetComponent<Image>();
            var art = button.transform.Find("Art")?.GetComponent<Image>();
            var badge = button.transform.Find("Badge")?.GetComponent<Text>();

            if (face == null || art == null || badge == null)
            {
                return;
            }

            face.sprite = BalatroSpriteLibrary.CardFace;
            face.type = Image.Type.Simple;
            face.preserveAspect = true;
            face.color = fog ? new Color(0.55f, 0.58f, 0.62f, 0.35f) : new Color(0.45f, 0.48f, 0.52f, 0.22f);
            face.enabled = BalatroSpriteLibrary.CardFace != null;

            art.enabled = false;
            SetSelectedBorder(button, false);

            badge.text = fog ? "雾" : string.Empty;
            badge.fontSize = 16;
            badge.alignment = TextAnchor.MiddleCenter;
            badge.color = new Color(0.85f, 0.88f, 0.92f, 0.75f);
            badge.enabled = true;
        }

        private static void SetSelectedBorder(Button button, bool selected)
        {
            var borderRoot = button.transform.Find("SelectBorder");
            if (borderRoot == null)
            {
                return;
            }

            borderRoot.gameObject.SetActive(selected);
            if (!selected)
            {
                return;
            }

            var color = SelectBorderColor;
            for (var i = 0; i < borderRoot.childCount; i++)
            {
                var edge = borderRoot.GetChild(i).GetComponent<Image>();
                if (edge != null)
                {
                    edge.color = color;
                }
            }
        }

        private static void CreateArtLayer(Transform parent, float cardWidth, float cardHeight)
        {
            var go = new GameObject("Art", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(
                cardWidth * (1f - (2f * ArtInsetX)),
                cardHeight * (1f - (2f * ArtInsetY)));
            rect.anchoredPosition = new Vector2(cardWidth * ArtShiftX, 0f);

            var image = go.GetComponent<Image>();
            image.raycastTarget = false;
        }

        private static void CreateSelectBorder(Transform parent)
        {
            var borderRoot = new GameObject("SelectBorder", typeof(RectTransform));
            borderRoot.transform.SetParent(parent, false);
            var rootRect = borderRoot.GetComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            var outset = SelectBorderOutset;
            rootRect.offsetMin = new Vector2(-outset, -outset);
            rootRect.offsetMax = new Vector2(outset, outset);
            borderRoot.gameObject.SetActive(false);

            CreateBorderEdge(borderRoot.transform, "Top",
                new Vector2(0f, 1f), new Vector2(1f, 1f),
                new Vector2(0f, -SelectBorderThickness), Vector2.zero);
            CreateBorderEdge(borderRoot.transform, "Bottom",
                new Vector2(0f, 0f), new Vector2(1f, 0f),
                Vector2.zero, new Vector2(0f, SelectBorderThickness));
            CreateBorderEdge(borderRoot.transform, "Left",
                new Vector2(0f, 0f), new Vector2(0f, 1f),
                Vector2.zero, new Vector2(SelectBorderThickness, 0f));
            CreateBorderEdge(borderRoot.transform, "Right",
                new Vector2(1f, 0f), new Vector2(1f, 1f),
                new Vector2(-SelectBorderThickness, 0f), Vector2.zero);
        }

        private static void CreateBorderEdge(Transform parent, string name,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var edge = CreateLayerImage(parent, name, anchorMin, anchorMax, offsetMin, offsetMax, false);
            edge.sprite = GetWhitePixelSprite();
            edge.type = Image.Type.Simple;
            edge.color = SelectBorderColor;
        }

        private static void EnsureMaterial()
        {
            if (_keyedArtMaterial != null)
            {
                return;
            }

            var shader = Shader.Find("PasserCard/UI/BlackTransparent");
            if (shader != null)
            {
                _keyedArtMaterial = new Material(shader);
            }
        }

        private static Sprite GetWhitePixelSprite()
        {
            if (_whitePixelSprite != null)
            {
                return _whitePixelSprite;
            }

            var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            _whitePixelSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 100f);
            return _whitePixelSprite;
        }

        private static Image CreateLayerImage(Transform parent, string name,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax, bool raycast)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
            var image = go.GetComponent<Image>();
            image.raycastTarget = raycast;
            return image;
        }
    }
}
