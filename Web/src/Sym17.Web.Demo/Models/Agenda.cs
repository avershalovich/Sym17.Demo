using System;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;

namespace Sym17.Web.Demo.Models
{
    // "Date": "10/17/2017",
    //"Category": "Partner Theatre",
    //"Colour": "gray",
    //"Title": "Commerce experiences that teach themselves",
    //"Teaser": "...the role AI will play in ontext marketing",
    //"Speaker": "XCentium",
    //"Location": "Partner Pavilion Theatre",
    //"Time": "10:15am–10:35am",
    //"StartTime": "10:15 AM",
    //"FormattedTime": "10:15am<br/>10:35am",
    //"Description": "With the growth of Machine Learning (ML), Artificial Intelligence (AI), and Natural Language Processing (NLP), we are not only able to draw conclusions and find patterns in large sets of data, but also create self-improving and “thinking” applications! With the introduction of the Internet of Things we can now interact with the Web using the language that we speak - not computers. Come to our Theatre session to see how we have harnessed the power of ML and IoT to provide seamless, highly personalized and most importantly self-improving Sitecore Commerce experiences.",
    //"Tags": "machine learning, IoT, commerce, personalization"


    [SitecoreType(TemplateId = "{2209A37D-C526-4A3C-8F4C-ED5834697E6F}")]
    public class AgendaFolder
    {
        [SitecoreId]
        public virtual Guid Id { get; set; }

        [SitecoreInfo(SitecoreInfoType.Url)]
        public virtual string Url { get; set; }

        [SitecoreInfo(SitecoreInfoType.Name)]
        public virtual string Name { get; set; }

        [SitecoreInfo(SitecoreInfoType.DisplayName)]
        public virtual string DisplayName { get; set; }
    }

    [SitecoreType(TemplateId = "{C26DA533-75B3-4D5A-A5F3-B85053C2C1EB}")]
    public class Agenda
    { 
        [SitecoreId]
        public virtual Guid Id { get; set; }

        [SitecoreInfo(SitecoreInfoType.Url)]
        public virtual string Url { get; set; }

        [SitecoreInfo(SitecoreInfoType.Name)]
        public virtual string Name { get; set; }
        
        [SitecoreField("Date")]
        public virtual DateTime Date { get; set; }

        [SitecoreField("Category")]
        public virtual string Category { get; set; }

        [SitecoreField("Colour")]
        public virtual string Colour { get; set; }

        [SitecoreField("Title")]
        public virtual string Title { get; set; }

        [SitecoreField("Teaser")]
        public virtual string Teaser { get; set; }

        [SitecoreField("Speaker")]
        public virtual string Speaker { get; set; }

        [SitecoreField("Location")]
        public virtual string Location { get; set; }

        [SitecoreField("Time")]
        public virtual string Time { get; set; }

        [SitecoreField("StartTime")]
        public virtual string StartTime { get; set; }

        [SitecoreField("FormattedTime")]
        public virtual string FormattedTime { get; set; }

        [SitecoreField("Description")]
        public virtual string Description { get; set; }

        [SitecoreField("Tags")]
        public virtual string Tags { get; set; }
    }
}