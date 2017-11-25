using System;
using System.Collections.Generic;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace Sym17.Web.Demo.Models
{
    [SitecoreType(TemplateId = "{2209A37D-C526-4A3C-8F4C-ED5834697E6F}")]
    public interface ITimeSlot 
    {
        [SitecoreId]
        Guid Id { get; set; }

        [SitecoreInfo(SitecoreInfoType.Url)]
        string Url { get; set; }

        [SitecoreInfo(SitecoreInfoType.Name)]
        string Name { get; set; }

        [SitecoreInfo(SitecoreInfoType.DisplayName)]
        string DisplayName { get; set; }

        [SitecoreChildren]
        IEnumerable<Agenda> Agendas { get; set; }
    }
}