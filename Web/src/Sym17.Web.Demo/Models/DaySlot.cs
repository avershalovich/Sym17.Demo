using System;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace Sym17.Web.Demo.Models
{
    [SitecoreType(TemplateId = "{0EAF8F79-AB91-4EFC-BEF3-E5E285A174C0}")]
    public interface IDaySlot
    {
        [SitecoreId]
        Guid Id { get; set; }

        [SitecoreInfo(SitecoreInfoType.Url)]
        string Url { get; set; }

        [SitecoreInfo(SitecoreInfoType.Name)]
        string Name { get; set; }

        [SitecoreInfo(SitecoreInfoType.DisplayName)]
        string DisplayName { get; set; }

        [SitecoreField("Day of Week")]
        string DayOfWeek { get; set; }
        [SitecoreField("Day Number")]
        int DayNumber { get; set; }
    }
}