namespace ImageProcess
{
    public class SettingsTemplate
    {
        public byte MinFaceSharpness { get; set; }
        public byte MinBadgeSharpness { get; set; }
        public byte MinFaceCount { get; set; }
        public byte MinBadgeCount { get; set; }
        public int DetectionMode { get; set; }
        public int ReduceCoeff { get; set; }
        public string TempFolder { get; set; }
        public string TemplateBagdeImage { get; set; }

        public int RoiWidthPercent { get; set; }
        public int RoiHeightPercent { get; set; }

        public string FaceApiSubscriptionKey { get; set; }
        public string OcpApiSubscriptionKey { get; set; }
        public string WebApiUrl { get; set; }
        public string InteractionWebApiUrl { get; set; }
    }
}
