using Godot;

namespace GodotSharp.BuildingBlocks.Terrain2
{
    public partial class TerrainData
    {
        private Image heightImg;
        private Vector2I heightImgSize;
        private Vector2I heightImgOffset;

        private void OnHeightMapChanged()
        {
            heightImg = HeightMap?.GetImage();
            if (heightImg is null) return;

            heightImg.Decompress();
            heightImgSize = heightImg.GetSize();
            heightImgOffset = heightImgSize / 2;
        }

        public float GetHeight(Vector2 xz)
        {
            xz += Offset;
            var height = RawHeight();
            var gradient = Gradient();
            return height * gradient * Amplitude * ChunkScale;

            float RawHeight()
            {
                return
                    Noise is not null ? RawNoiseHeight() :
                    heightImg is not null ? RawImageHeight() :
                    (float)default;

                float RawNoiseHeight()
                {
                    var noise = Noise.GetNoise(xz.X, xz.Y);
                    return (noise + 1) / 2;
                }

                float RawImageHeight()
                {
                    return heightImg.GetPixel(
                        Mathf.PosMod(Utils.TrueRound(xz.X + heightImgOffset.X), heightImgSize.X),
                        Mathf.PosMod(Utils.TrueRound(xz.Y + heightImgOffset.Y), heightImgSize.Y)).R;
                }
            }

            float Gradient()
            {
                return UseHeightCurve ? HeightCurveGradient() : 1;

                float HeightCurveGradient()
                    => HeightCurve?.Sample(height) ?? 1;
            }
        }
    }
}
