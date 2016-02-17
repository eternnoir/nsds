using System;

namespace nsds.RedisDictionaryService.Test
{
    [Serializable]
    public class TestClass
    {
        public string Id { get; set; }
        public DateTime TDate { get; set; }
        public int Tint { get; set; }
        public bool Tbool { get; set; }
    }
}
