using System.Collections.Generic;

namespace JsonToClassInit
{
    public class Sample
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public decimal Weight { get; set; }

        public bool IsAdult { get; set; }

        public List<string> NickNames { get; set; }

        public List<SampleDetail> Detail { get; set; }
    }
}