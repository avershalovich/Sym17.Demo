using Sym17.Web.Models.PowerBI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Sym17.Web.Controllers
{
    using Models.ViewModels;

    using Services;

    public class PowerBIController : ApiController
    {
        private Xservice _xservice = new Xservice();

        [HttpGet]
        public List<PBIContact> GetContacts()
        {
            List<PBIContact> contacts = new List<PBIContact>();

            var xc = _xservice.GetContacts(Settings.FaceApiIdentificationSource);

            foreach (ContactViewModel c in xc)
            {
                contacts.Add(new PBIContact()
                {
                    Id = c.Id,
                    Name = c.FirstName + " " + c.LastName,
                    Age = c.Age,
                    Company = c.Company,
                    Gender = c.Gender,
                    PartnerScore = c.PartnerScore,
                    BusinessDevelopmentScore = c.BusinessDevelopmentScore,
                    SitecoreScore = c.SitecoreScore,
                    DevelopmentScore = c.DevelopmentScore,
                    MarketingScore = c.MarketingScore,
                    ClientScore = c.ClientScore
                });
            }

            return contacts;
            
        }

        [HttpGet]
        public IEnumerable<PBIOfflineInteraction> GetOffileInteractions()
        {
            List<PBIOfflineInteraction> interactions = _xservice.GetAllInteractions().ToList();

            return interactions;

        }
    }

}