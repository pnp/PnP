namespace OfficeDevPnP.Core.Entities
{
    public class ThemeEntity
    {
        public string Name { get; set; }

        public bool IsCustomComposedLook { get; set; }

        public string MasterPage { get; set; }

        public string CustomMasterPage { get; set; }

        public string Theme { get; set; }

        public string BackgroundImage { get; set; }

        public string Font { get; set; }
    }
}
