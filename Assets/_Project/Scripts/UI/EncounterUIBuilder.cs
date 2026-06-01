using System;
using PasserCard.Cards;
using UnityEngine;
using UnityEngine.UI;

namespace PasserCard.UI
{
    public static class EncounterUIBuilder
    {
        public sealed class BuiltUI
        {
            public Text StatusText = null!;
            public Text MessageText = null!;
            public Transform HandContainer = null!;
            public Button[] SlotButtons = null!;
            public Text[] SlotLabels = null!;
            public Button DiscardButton = null!;
            public Button PassButton = null!;
            public Button PrePinButton = null!;
            public Button ToPassPhaseButton = null!;
            public Button ToPlayPhaseButton = null!;
            public Button SubmitButton = null!;
            public Button NextQuotaButton = null!;
        }

        private static readonly Color TableColor = new(0.07f, 0.11f, 0.09f, 0.98f);
        private static readonly Color PrimaryButtonColor = new(0.18f, 0.2f, 0.22f, 0.92f);
        private static readonly Color PhaseButtonColor = new(0.14f, 0.16f, 0.18f, 0.88f);

        public static BuiltUI Create(Transform parent)
        {
            var canvasGo = new GameObject("EncounterCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGo.transform.SetParent(parent, false);

            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            if (UnityEngine.Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                _ = new GameObject("EventSystem",
                    typeof(UnityEngine.EventSystems.EventSystem),
                    typeof(UnityEngine.EventSystems.StandaloneInputModule));
            }

            var root = CreatePanel(canvasGo.transform, "Root", TableColor, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            var status = CreateText(root.transform, "StatusText", 20, TextAnchor.UpperLeft,
                new Vector2(0, 1), new Vector2(0.55f, 1), new Vector2(28, -28), new Vector2(-12, -118));
            var message = CreateText(root.transform, "MessageText", 16, TextAnchor.UpperLeft,
                new Vector2(0, 1), new Vector2(0.55f, 1), new Vector2(28, -122), new Vector2(-12, -168));

            var slotRow = CreatePanel(root.transform, "PlaySlots", new Color(0, 0, 0, 0),
                new Vector2(0.5f, 0.64f), new Vector2(0.5f, 0.64f), new Vector2(-360, -98), new Vector2(360, 98));

            var slotButtons = new Button[5];
            var slotLabels = new Text[5];
            const float slotStep = CardVisualHelper.PlaySlotWidth + 14f;
            var slotStartX = -((5 - 1) * slotStep) * 0.5f;
            for (var i = 0; i < 5; i++)
            {
                var arcLift = -Mathf.Abs(i - 2) * 6f;
                var slot = CreatePlaySlotButton(slotRow.transform, $"Slot{i + 1}",
                    new Vector2(slotStartX + i * slotStep, arcLift));
                slotButtons[i] = slot;
                slotLabels[i] = slot.transform.Find("Badge")?.GetComponent<Text>()!;
            }

            var handRow = CreatePanel(root.transform, "HandRow", new Color(0, 0, 0, 0),
                new Vector2(0.5f, 0.24f), new Vector2(0.5f, 0.24f), new Vector2(-520, -90), new Vector2(520, 90));

            var rightPanel = CreatePanel(root.transform, "RightActions", new Color(0, 0, 0, 0),
                new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-196, -180), new Vector2(-24, 180));

            var phaseRow = CreatePanel(root.transform, "PhaseActions", new Color(0, 0, 0, 0),
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-420, 24), new Vector2(420, 84));

            return new BuiltUI
            {
                StatusText = status,
                MessageText = message,
                HandContainer = handRow.transform,
                SlotButtons = slotButtons,
                SlotLabels = slotLabels,
                DiscardButton = CreateSideButton(rightPanel.transform, "Discard", 0, "弃牌"),
                PassButton = CreateSideButton(rightPanel.transform, "Pass", 1, "Pass"),
                SubmitButton = CreateSideButton(rightPanel.transform, "Submit", 2, "计分"),
                PrePinButton = CreatePhaseButton(phaseRow.transform, "PrePin", 0, "预钉雾槽"),
                ToPassPhaseButton = CreatePhaseButton(phaseRow.transform, "ToPass", 1, "→ Pass"),
                ToPlayPhaseButton = CreatePhaseButton(phaseRow.transform, "ToPlay", 2, "→ 出牌"),
                NextQuotaButton = CreatePhaseButton(phaseRow.transform, "Next", 3, "下一配额")
            };
        }

        public static Button CreateHandCardButton(Transform parent, PlayingCardInstance card, bool selected, Action onClick)
        {
            var button = CardVisualHelper.CreateCardButton(parent, "Card",
                CardVisualHelper.HandCardWidth, CardVisualHelper.HandCardHeight);
            var layout = button.gameObject.AddComponent<LayoutElement>();
            layout.preferredWidth = CardVisualHelper.HandCardWidth;
            layout.preferredHeight = CardVisualHelper.HandCardHeight;

            CardVisualHelper.ApplyPlayingCard(button, card, selected, false);
            button.onClick.AddListener(() => onClick());
            return button;
        }

        public static void StyleSlot(Button slot, bool fog, PlayingCardInstance? card)
        {
            if (card != null)
            {
                CardVisualHelper.ApplyPlayingCard(slot, card, false, fog);
                return;
            }

            if (fog)
            {
                CardVisualHelper.ApplyFogSlot(slot);
                return;
            }

            CardVisualHelper.ApplyEmptySlot(slot, false);
        }

        private static Button CreatePlaySlotButton(Transform parent, string name, Vector2 anchoredPosition)
        {
            var button = CardVisualHelper.CreateCardButton(parent, name,
                CardVisualHelper.PlaySlotWidth, CardVisualHelper.PlaySlotHeight);
            var rect = button.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            CardVisualHelper.ApplyEmptySlot(button, false);
            return button;
        }

        private static Button CreateSideButton(Transform parent, string name, int index, string label)
        {
            const float height = 72f;
            const float gap = 16f;
            var y = -index * (height + gap);
            return CreateAnchoredButton(parent, name, PrimaryButtonColor, label, 152f, height,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, y), 22);
        }

        private static Button CreatePhaseButton(Transform parent, string name, int index, string label)
        {
            const float width = 196f;
            const float gap = 10f;
            var x = index * (width + gap);
            return CreateAnchoredButton(parent, name, PhaseButtonColor, label, width, 52f,
                new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(x, 0f), 18);
        }

        private static Button CreateAnchoredButton(Transform parent, string name, Color color, string label,
            float width, float height, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, int fontSize)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = anchorMin;
            rect.sizeDelta = new Vector2(width, height);
            rect.anchoredPosition = anchoredPosition;

            var image = go.GetComponent<Image>();
            image.color = color;

            var text = CreateButtonLabel(go.transform, label, fontSize);
            text.color = new Color(0.92f, 0.94f, 0.96f, 1f);

            var button = go.GetComponent<Button>();
            var colors = button.colors;
            colors.highlightedColor = new Color(0.32f, 0.36f, 0.4f, 1f);
            colors.pressedColor = new Color(0.22f, 0.25f, 0.28f, 1f);
            colors.disabledColor = new Color(0.14f, 0.16f, 0.18f, 0.55f);
            button.colors = colors;
            return button;
        }

        private static Text CreateButtonLabel(Transform parent, string label, int fontSize)
        {
            var textGo = new GameObject("Label", typeof(RectTransform), typeof(Text));
            textGo.transform.SetParent(parent, false);
            var textRect = textGo.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(8, 6);
            textRect.offsetMax = new Vector2(-8, -6);
            var text = textGo.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = fontSize;
            text.alignment = TextAnchor.MiddleCenter;
            text.text = label;
            return text;
        }

        private static GameObject CreatePanel(Transform parent, string name, Color color,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var image = go.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
            return go;
        }

        private static Text CreateText(Transform parent, string name, int fontSize, TextAnchor anchor,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var text = go.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = fontSize;
            text.color = new Color(0.9f, 0.92f, 0.88f, 1f);
            text.alignment = anchor;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
            return text;
        }
    }
}
