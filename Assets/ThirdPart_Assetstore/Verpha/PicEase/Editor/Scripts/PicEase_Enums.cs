#if UNITY_EDITOR
namespace Verpha.PicEase
{
    internal static class PicEase_Enums
    {
        #region Performance
        public enum BehaviourMode { Synced, Unsynced }
        public enum ThreadGroupSize { EightByEight, SixteenBySixteen, ThirtyTwoByThirtyTwo }
        public enum PrecisionMode { Full, Half }
        #endregion
    }
}
#endif