using System.Collections.Generic;
using PasserCard.Cards;
using UnityEngine;

namespace PasserCard.UI
{
    /// <summary>
    /// Runtime slices Balatro temp sprite sheets from Resources/Balatro/.
    /// Prototype-only art; replace before release.
    /// </summary>
    public static class BalatroSpriteLibrary
    {
        private const float PixelsPerUnit = 100f;
        private const int DeckGridColumns = 14;
        private const int DeckRows = 4;
        private const float DeckSliceOffsetColumns = 0.5f;
        private const int DeckSliceVersion = 4;
        private const int EnhancerColumns = 7;
        private const int EnhancerRows = 5;
        private const int UiIconColumns = 4;
        private const int UiIconRows = 2;

        private static Texture2D? _deckTexture;
        private static Texture2D? _enhancersTexture;
        private static Texture2D? _uiTexture;
        private static Sprite? _cardBack;
        private static Sprite? _fogSlotBack;
        private static Sprite? _chipIcon;
        private static Sprite? _cardFace;
        private static readonly Dictionary<(Suit suit, Rank rank), Sprite> CardSprites = new();
        private static int _loadedDeckSliceVersion;

        public static bool IsLoaded { get; private set; }

        public static void EnsureLoaded()
        {
            if (IsLoaded && _loadedDeckSliceVersion == DeckSliceVersion)
            {
                return;
            }

            CardSprites.Clear();
            _loadedDeckSliceVersion = DeckSliceVersion;
            IsLoaded = false;

            _deckTexture = Resources.Load<Texture2D>("Balatro/8BitDeck");
            _enhancersTexture = Resources.Load<Texture2D>("Balatro/Enhancers");
            _uiTexture = Resources.Load<Texture2D>("Balatro/UIAssets");
            var cardFaceTexture = Resources.Load<Texture2D>("Balatro/CardFace");

            if (_deckTexture != null)
            {
                _deckTexture.filterMode = FilterMode.Point;
            }

            if (_enhancersTexture != null)
            {
                _enhancersTexture.filterMode = FilterMode.Point;
                _cardBack = SliceGrid(_enhancersTexture, EnhancerColumns, EnhancerRows, 0, 0);
                _fogSlotBack = SliceGrid(_enhancersTexture, EnhancerColumns, EnhancerRows, 4, 0);
            }

            if (_uiTexture != null)
            {
                _uiTexture.filterMode = FilterMode.Point;
                _chipIcon = SliceGrid(_uiTexture, UiIconColumns, UiIconRows, 0, 0);
            }

            if (cardFaceTexture != null)
            {
                cardFaceTexture.filterMode = FilterMode.Bilinear;
                _cardFace = SliceCardFace(cardFaceTexture);
            }

            IsLoaded = _deckTexture != null;
        }

        private static Sprite SliceCardFace(Texture2D texture)
        {
            if (TryGetCardFaceRect(texture, out var rect))
            {
                return Slice(texture, rect);
            }

            return Slice(texture, new Rect(0f, 0f, texture.width, texture.height));
        }

        private static bool TryGetCardFaceRect(Texture2D texture, out Rect rect)
        {
            if (texture.isReadable && TryGetOpaqueBounds(texture, out rect))
            {
                return true;
            }

            return TryGetProportionalCardFaceRect(texture, out rect);
        }

        private static bool TryGetOpaqueBounds(Texture2D texture, out Rect rect)
        {
            rect = default;
            var width = texture.width;
            var height = texture.height;
            var pixels = texture.GetPixels32();
            var minX = width;
            var minY = height;
            var maxX = -1;
            var maxY = -1;

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var color = pixels[(y * width) + x];
                    if (color.a < 8 && color.r + color.g + color.b < 24)
                    {
                        continue;
                    }

                    if (x < minX)
                    {
                        minX = x;
                    }

                    if (y < minY)
                    {
                        minY = y;
                    }

                    if (x > maxX)
                    {
                        maxX = x;
                    }

                    if (y > maxY)
                    {
                        maxY = y;
                    }
                }
            }

            if (maxX < minX || maxY < minY)
            {
                return false;
            }

            var cropW = maxX - minX + 1;
            var cropH = maxY - minY + 1;
            rect = new Rect(minX, minY, cropW, cropH);
            return cropW > 1 && cropH > 1;
        }

        private static bool TryGetProportionalCardFaceRect(Texture2D texture, out Rect rect)
        {
            const float authoredWidth = 190f;
            const float authoredHeight = 297f;
            const float authoredCropX = 6f;
            const float authoredCropTop = 2f;
            const float authoredCropWidth = 178f;
            const float authoredCropHeight = 293f;

            var scaleX = texture.width / authoredWidth;
            var scaleY = texture.height / authoredHeight;
            var cropX = authoredCropX * scaleX;
            var cropWidth = authoredCropWidth * scaleX;
            var cropHeight = authoredCropHeight * scaleY;
            var cropTop = authoredCropTop * scaleY;
            var cropY = texture.height - cropTop - cropHeight;

            cropX = Mathf.Clamp(cropX, 0f, texture.width - 1f);
            cropY = Mathf.Clamp(cropY, 0f, texture.height - 1f);
            cropWidth = Mathf.Min(cropWidth, texture.width - cropX);
            cropHeight = Mathf.Min(cropHeight, texture.height - cropY);

            if (cropWidth <= 1f || cropHeight <= 1f)
            {
                rect = default;
                return false;
            }

            rect = new Rect(cropX, cropY, cropWidth, cropHeight);
            return true;
        }

        public static Sprite? GetCardSprite(Suit suit, Rank rank)
        {
            EnsureLoaded();
            if (_deckTexture == null)
            {
                return null;
            }

            var key = (suit, rank);
            if (CardSprites.TryGetValue(key, out var cached))
            {
                return cached;
            }

            if (!TryGetGridPosition(suit, rank, out var col, out var row))
            {
                return null;
            }

            var cellW = _deckTexture.width / (float)DeckGridColumns;
            var cellH = _deckTexture.height / (float)DeckRows;
            var offsetX = cellW * DeckSliceOffsetColumns;
            var rect = new Rect(
                offsetX + col * cellW,
                _deckTexture.height - (row + 1) * cellH,
                cellW,
                cellH);

            if (TryTrimRectToContent(_deckTexture, rect, out var trimmed))
            {
                rect = trimmed;
            }

            var sprite = Slice(_deckTexture, rect);
            CardSprites[key] = sprite;
            return sprite;
        }

        private static Sprite SliceGrid(Texture2D texture, int columns, int rows, int col, int row)
        {
            var cellW = texture.width / (float)columns;
            var cellH = texture.height / (float)rows;
            var rect = new Rect(col * cellW, texture.height - (row + 1) * cellH, cellW, cellH);
            return Slice(texture, rect);
        }

        public static Sprite? GetCardSprite(PlayingCardInstance card)
        {
            return GetCardSprite(card.Suit, card.EffectiveRank);
        }

        public static Sprite? CardBack => _cardBack;

        public static Sprite? FogSlotBack => _fogSlotBack ?? _cardBack;

        public static Sprite? ChipIcon => _chipIcon;

        public static Sprite? CardFace => _cardFace;

        private static bool TryGetGridPosition(Suit suit, Rank rank, out int col, out int row)
        {
            row = suit switch
            {
                Suit.Hearts => 0,
                Suit.Clubs => 1,
                Suit.Diamonds => 2,
                Suit.Spades => 3,
                _ => 0
            };

            if (rank < Rank.Two || rank > Rank.Ace)
            {
                col = 0;
                return false;
            }

            col = rank.ToSortValue() - 2;
            return true;
        }

        private static bool TryTrimRectToContent(Texture2D texture, Rect source, out Rect trimmed)
        {
            trimmed = source;
            if (!texture.isReadable)
            {
                return false;
            }

            var textureWidth = texture.width;
            var x0 = Mathf.Clamp(Mathf.FloorToInt(source.x), 0, textureWidth - 1);
            var y0 = Mathf.Clamp(Mathf.FloorToInt(source.y), 0, texture.height - 1);
            var x1 = Mathf.Clamp(Mathf.CeilToInt(source.x + source.width), x0 + 1, textureWidth);
            var y1 = Mathf.Clamp(Mathf.CeilToInt(source.y + source.height), y0 + 1, texture.height);

            var pixels = texture.GetPixels32();
            var minX = x1;
            var minY = y1;
            var maxX = x0;
            var maxY = y0;

            for (var y = y0; y < y1; y++)
            {
                for (var x = x0; x < x1; x++)
                {
                    var color = pixels[(y * textureWidth) + x];
                    if (color.a < 8 && color.r + color.g + color.b < 32)
                    {
                        continue;
                    }

                    if (x < minX)
                    {
                        minX = x;
                    }

                    if (y < minY)
                    {
                        minY = y;
                    }

                    if (x > maxX)
                    {
                        maxX = x;
                    }

                    if (y > maxY)
                    {
                        maxY = y;
                    }
                }
            }

            if (maxX < minX || maxY < minY)
            {
                return false;
            }

            trimmed = new Rect(minX, minY, maxX - minX + 1, maxY - minY + 1);
            return trimmed.width > 1f && trimmed.height > 1f;
        }

        private static Sprite Slice(Texture2D texture, Rect rect)
        {
            return Sprite.Create(
                texture,
                rect,
                new Vector2(0.5f, 0.5f),
                PixelsPerUnit,
                0,
                SpriteMeshType.FullRect);
        }
    }
}
