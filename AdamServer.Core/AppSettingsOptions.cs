using AdamServer.Interfaces;

namespace AdamServer.Core
{
    public class AppSettingsOptions : IAppSettingsOptions
    {
        public Test1 Test1 { get; set; }
        public Test2 Test2 { get; set; }
        public string Test3 { get; set; }
    }

    public class Test1
    {
        public string TestName1 { get; set; }
        public string TestName2 { get; set; }
    }

    public class Test2
    {
        public string TestName1 { get; set; }
        public string TestName2 { get; set; }
    }
}
