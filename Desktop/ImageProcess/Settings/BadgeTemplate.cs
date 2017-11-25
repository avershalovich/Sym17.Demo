using System.Collections.Generic;

namespace ImageProcess
{
    public class BadgeTemplate
    {
        public List<BageRegion> Regions { get; set; }
    }

    public class BageRegion
    {
        public List<BadgeLine> Lines { get; set; }
    }
    public class BadgeLine
    {
        public List<BadgeWord> Words { get; set; }
    }

    public class BadgeWord
    {
        public string FieldName { get; set; }
    }
}
