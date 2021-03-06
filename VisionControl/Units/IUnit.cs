namespace VisionControl.Units
{
    using Ensage;

    using SharpDX;

    internal interface IUnit
    {
        #region Public Properties

        float Duration { get; }

        float EndTime { get; }

        uint Handle { get; }

        bool IsValid { get; }

        ParticleEffect ParticleEffect { get; }

        Vector3 Position { get; }

        Vector2 PositionCorrection { get; }

        float Radius { get; }

        bool ShowTexture { get; }

        bool ShowTimer { get; }

        DotaTexture Texture { get; }

        Vector2 TextureSize { get; set; }

        #endregion
    }
}