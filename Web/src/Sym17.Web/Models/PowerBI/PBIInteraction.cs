namespace Sym17.Web.Models.PowerBI
{
    using System;
    using System.Collections.Generic;

    public class PBIInteraction
    {
        public string ContactId { get; set; }

        public string Id { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Channel { get; set; }

        public string VenueName { get; set; }

        public List<string> Goals { get; set; }

        public int EngagmentValue { get; set; }

    }
}