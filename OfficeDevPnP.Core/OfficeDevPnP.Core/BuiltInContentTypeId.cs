using System.Collections.Generic;

namespace OfficeDevPnP.Core
{
    /// <summary>
    /// A class that returns strings that represent identifiers (IDs) for built-in content types.
    /// </summary>
    public static class BuiltInContentTypeId
    {
        public const string AdminTask = "0x010802";
        public const string Announcement = "0x0104";
        public const string BasicPage = "0x010109";
        public const string BlogComment = "0x0111";
        public const string BlogPost = "0x0110";
        public const string CallTracking = "0x0100807FBAC5EB8A4653B8D24775195B5463";
        public const string Contact = "0x0106";
        public const string Discussion = "0x012002";
        public const string DisplayTemplateJS = "0x0101002039C03B61C64EC4A04F5361F3851068";
        public const string Document = "0x0101";

        /// <summary>
        /// Contains the content identifier (ID) for the DocumentSet content type. To get content type from a list, use BestMatchContentTypeId().
        /// </summary>
        public const string DocumentSet = "0x0120D520";

        public const string DocumentWorkflowItem = "0x010107";
        public const string DomainGroup = "0x010C";
        public const string DublinCoreName = "0x01010B";
        public const string Event = "0x0102";
        public const string FarEastContact = "0x0116";
        public const string Folder = "0x0120";
        public const string GbwCirculationCTName = "0x01000F389E14C9CE4CE486270B9D4713A5D6";
        public const string GbwOfficeNoticeCTName = "0x01007CE30DD1206047728BAFD1C39A850120";
        public const string HealthReport = "0x0100F95DB3A97E8046B58C6A54FB31F2BD46";
        public const string HealthRuleDefinition = "0x01003A8AA7A4F53046158C5ABD98036A01D5";
        public const string Holiday = "0x01009BE2AB5291BF4C1A986910BD278E4F18";
        public const string IMEDictionaryItem = "0x010018F21907ED4E401CB4F14422ABC65304";
        public const string Issue = "0x0103";

        /// <summary>
        /// Contains the content identifier (ID) for the Item content type.
        /// </summary>
        public const string Item = "0x01";

        public const string Link = "0x0105";
        public const string LinkToDocument = "0x01010A";
        public const string MasterPage = "0x010105";
        public const string Message = "0x0107";
        public const string ODCDocument = "0x010100629D00608F814DD6AC8A86903AEE72AA";
        public const string Person = "0x010A";
        public const string Picture = "0x010102";
        public const string Resource = "0x01004C9F4486FBF54864A7B0A33D02AD19B1";
        public const string ResourceGroup = "0x0100CA13F2F8D61541B180952DFB25E3E8E4";
        public const string ResourceReservation = "0x0102004F51EFDEA49C49668EF9C6744C8CF87D";
        public const string RootOfList = "0x012001";
        public const string Schedule = "0x0102007DBDC1392EAF4EBBBF99E41D8922B264";
        public const string ScheduleAndResourceReservation = "0x01020072BB2A38F0DB49C3A96CF4FA85529956";
        public const string SharePointGroup = "0x010B";
        public const string SummaryTask = "0x012004";
        public const string System = "0x";
        public const string Task = "0x0108";
        public const string Timecard = "0x0100C30DDA8EDB2E434EA22D793D9EE42058";
        public const string UDCDocument = "0x010100B4CBD48E029A4AD8B62CB0E41868F2B0";
        public const string UntypedDocument = "0x010104";
        public const string WebPartPage = "0x01010901";
        public const string WhatsNew = "0x0100A2CA87FF01B442AD93F37CD7DD0943EB";
        public const string Whereabouts = "0x0100FBEEE6F0C500489B99CDA6BB16C398F7";
        public const string WikiDocument = "0x010108";
        public const string WorkflowHistory = "0x0109";
        public const string WorkflowTask = "0x010801";
        public const string XMLDocument = "0x010101";
        public const string XSLStyle = "0x010100734778F2B7DF462491FC91844AE431CF";


        private static Dictionary<string, bool> s_dict = (Dictionary<string, bool>) null;


        public static bool Contains(string id)
        {
            if (s_dict == null)
            {
                s_dict = new Dictionary<string, bool>();
                s_dict.Add(AdminTask, true);
                s_dict.Add(Announcement, true);
                s_dict.Add(BasicPage, true);
                s_dict.Add(BlogComment, true);
                s_dict.Add(CallTracking, true);
                s_dict.Add(Contact, true);
                s_dict.Add(Discussion, true);
                s_dict.Add(DisplayTemplateJS, true);
                s_dict.Add(Document, true);
                s_dict.Add(DocumentSet, true);
                s_dict.Add(DocumentWorkflowItem, true);
                s_dict.Add(DomainGroup, true);
                s_dict.Add(DublinCoreName, true);
                s_dict.Add(Event, true);
                s_dict.Add(FarEastContact, true);
                s_dict.Add(Folder, true);
                s_dict.Add(GbwCirculationCTName, true);
                s_dict.Add(GbwOfficeNoticeCTName, true);
                s_dict.Add(HealthReport, true);
                s_dict.Add(HealthRuleDefinition, true);
                s_dict.Add(Holiday, true);
                s_dict.Add(IMEDictionaryItem, true);
                s_dict.Add(Issue, true);
                s_dict.Add(Item, true);
                s_dict.Add(Link, true);
                s_dict.Add(LinkToDocument, true);
                s_dict.Add(MasterPage, true);
                s_dict.Add(Message, true);
                s_dict.Add(ODCDocument, true);
                s_dict.Add(Person, true);
                s_dict.Add(Picture, true);
                s_dict.Add(Resource, true);
                s_dict.Add(ResourceGroup, true);
                s_dict.Add(ResourceReservation, true);
                s_dict.Add(RootOfList, true);
                s_dict.Add(Schedule, true);
                s_dict.Add(ScheduleAndResourceReservation, true);
                s_dict.Add(SharePointGroup, true);
                s_dict.Add(SummaryTask, true);
                s_dict.Add(System, true);
                s_dict.Add(Task, true);
                s_dict.Add(Timecard, true);
                s_dict.Add(UDCDocument, true);
                s_dict.Add(UntypedDocument, true);
                s_dict.Add(WebPartPage, true);
                s_dict.Add(WhatsNew, true);
                s_dict.Add(Whereabouts, true);
                s_dict.Add(WikiDocument, true);
                s_dict.Add(WorkflowHistory, true);
                s_dict.Add(WorkflowTask, true);
                s_dict.Add(XMLDocument, true);
                s_dict.Add(XSLStyle, true);
            }
            bool flag = false;
            s_dict.TryGetValue(id, out flag);
            return flag;
        }
    }
}
