namespace Sym17.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;

    using Models.Contact;
    using Models.Facets;
    using Models.Facets.Contact;
    using Models.Facets.Interaction;
    using Models.PowerBI;
    using Models.ViewModels;

    using Sitecore.XConnect;
    using Sitecore.XConnect.Client;
    using Sitecore.XConnect.Collection.Model;
    using Sitecore.XConnect.Schema;
    using Sitecore.Xdb.Common.Web;
    using Sitecore.XConnect.Client.WebApi;
    using Sitecore.XConnect.Operations;

    public class Xservice
    {
        public readonly string XconnectUrl = ConfigurationManager.AppSettings["xconnect.url"];
        public readonly string XconnectThumbprint = ConfigurationManager.AppSettings["xconnect.thumbprint"];

        public ContactViewModel TestClient()
        {
            var identifier = "test19";
            var source = "test";

             var testContact = GetContact(source, identifier);

            if (testContact == null)
            {
                var contact = new ImageProcessingContact();

                contact.FirstName = "Test";
                contact.LastName = "Last";
                contact.Id = identifier + "_" + identifier;
                contact.Age = 30;
                contact.Gender = "Male";
                contact.Company = "Brimit";

                this.AddContact(source, identifier, contact);

                this.RegisterOfflineInteraction(source, identifier, new FaceApiFacet() {HappinessValue = 1, SmileValue = 1});

                this.SetEmail(source, identifier, identifier + "@mail.com");
            }

            return GetContact(source, identifier);
        }

        public bool Identify(string knownSource, string knownIdentifier, string newSource, string newIdentifier)
        {
            using (XConnectClient client = GetClient())
            {
                try
                {
                    IdentifiedContactReference reference = new IdentifiedContactReference(knownSource, knownIdentifier);

                    Contact existingContact = client.Get<Contact>(
                        reference,
                        new ContactExpandOptions(new string[] { PersonalInformation.DefaultFacetKey }));

                    if (existingContact == null)
                    {
                        return false;
                    }

                    var identifier = new ContactIdentifier(newSource, newIdentifier, ContactIdentifierType.Known);

                    client.AddContactIdentifier(existingContact, identifier);

                    client.Submit();

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public void SetEmail(string email)
        {
            SetEmail(Settings.FaceApiIdentificationSource, ActiveVisitor.Get().Id, email);
        }

        public void SetEmail(string source, string identifier, string email)
        {
            using (XConnectClient client = GetClient())
            {
                try
                {
                    IdentifiedContactReference reference = new IdentifiedContactReference(source, identifier);

                    Contact contact = client.Get<Contact>(
                        reference,
                        new ContactExpandOptions(new string[] { PersonalInformation.DefaultFacetKey, EmailAddressList.DefaultFacetKey }));

                    if (contact == null)
                    {
                        return;
                    }

                    EmailAddressList emails = contact.GetFacet<EmailAddressList>(EmailAddressList.DefaultFacetKey);

                    var preferredEmail = new EmailAddress(email, true);

                    if (emails == null)
                    {
                        emails = new EmailAddressList(preferredEmail, "Work");
                    }
                    else
                    {
                        emails.PreferredEmail = preferredEmail;
                    }

                    client.SetEmails(contact, emails);

                    client.Submit();

                    var operations = client.LastBatch;

                    // Loop through operations and check status
                    foreach (var operation in operations)
                    {
                        //Sitecore.Diagnostics.Log.Info(operation.OperationType + operation.Target.GetType().ToString() + " Operation: " + operation.Status, this);
                    }
                }
                catch (XdbExecutionException ex)
                {
                    //Sitecore.Diagnostics.Log.Error(ex.Message + ex.StackTrace, this);
                }
            }
        }

        public ContactViewModel GetContact()
        {
            return GetContact(Settings.FaceApiIdentificationSource, ActiveVisitor.Get().Id);
        }

        public ContactViewModel GetContact(string source, string identifier)
        {
            ContactViewModel result = new ContactViewModel();

            using (XConnectClient client = GetClient())
            {
                try
                {
                    IdentifiedContactReference reference = new IdentifiedContactReference(source, identifier);

                    Contact existingContact = client.Get<Contact>(
                        reference,
                        new ContactExpandOptions(PersonalInformation.DefaultFacetKey, 
                        CvPersonFacet.DefaultFacetKey, 
                        Avatar.DefaultFacetKey, 
                        ContactBehaviorProfile.DefaultFacetKey));

                    if (existingContact == null)
                    {
                        return null;
                    }

                    PersonalInformation personalInformation = existingContact.GetFacet<PersonalInformation>(PersonalInformation.DefaultFacetKey);

                    if (personalInformation == null)
                    {
                        return null;
                    }

                    result.FirstName = personalInformation.FirstName;
                    result.LastName = personalInformation.LastName;
                    result.Gender = personalInformation.Gender;
                    result.Id = identifier;

                    Avatar avatar = existingContact.GetFacet<Avatar>(Avatar.DefaultFacetKey);

                    if (avatar?.Picture != null)
                    {
                        result.AvatarImage = Convert.ToBase64String(avatar.Picture);
                    }


                    CvPersonFacet cvPerson = existingContact.GetFacet<CvPersonFacet>(CvPersonFacet.DefaultFacetKey);

                    if (cvPerson != null)
                    {
                        result.LastName = cvPerson.LastName;
                        result.FirstName = cvPerson.FirstName;
                        result.Id = cvPerson.Id;

                        result.Age = cvPerson.Age;
                        result.Gender = cvPerson.Gender;
                    }

                    ContactBehaviorProfile behaviorProfile = existingContact.ContactBehaviorProfile();

                    if (behaviorProfile?.Scores != null)
                    {
                        if (behaviorProfile.Scores.ContainsKey(Settings.RelationshipProfileId))
                        {
                            ProfileScore behaviorProfileScore = behaviorProfile.Scores[Settings.RelationshipProfileId];

                            result.ClientScore = behaviorProfileScore.Values.ContainsKey(Settings.ClientBehaviorProfileId) ?
                                (int)behaviorProfileScore.Values[Settings.ClientBehaviorProfileId] : 0;

                            result.PartnerScore = behaviorProfileScore.Values.ContainsKey(Settings.PartnerBehaviorProfileId) ?
                                (int)behaviorProfileScore.Values[Settings.PartnerBehaviorProfileId] : 0;

                            result.SitecoreScore = behaviorProfileScore.Values.ContainsKey(Settings.SitecoreBehaviorProfileId) ?
                                (int)behaviorProfileScore.Values[Settings.SitecoreBehaviorProfileId] : 0;

                        }

                        if (behaviorProfile.Scores.ContainsKey(Settings.RoleProfileId))
                        {
                            var behaviorProfileScore = behaviorProfile.Scores[Settings.RoleProfileId];

                            result.DevelopmentScore = behaviorProfileScore.Values.ContainsKey(Settings.DevelopmentBehaviorProfileId) ?
                                (int)behaviorProfileScore.Values[Settings.DevelopmentBehaviorProfileId] : 0;

                            result.BusinessDevelopmentScore = behaviorProfileScore.Values.ContainsKey(Settings.BusinessDevelopmentBehaviorProfileId) ?
                                (int)behaviorProfileScore.Values[Settings.BusinessDevelopmentBehaviorProfileId] : 0;

                            result.MarketingScore = behaviorProfileScore.Values.ContainsKey(Settings.MarketingBehaviorProfileId) ?
                                (int)behaviorProfileScore.Values[Settings.MarketingBehaviorProfileId] : 0;
                        }
                    }

                    return result;
                }
                catch (XdbExecutionException ex)
                {
                }
            }

            return null;
        }

        public void UpdateContact(ContactViewModel data)
        {
            UpdateContact(Settings.FaceApiIdentificationSource, ActiveVisitor.Get().Id, data);
        }

        public void UpdateContact(string source, string identifier, ContactViewModel data)
        {
            using (XConnectClient client = GetClient())
            {
                try
                {
                    IdentifiedContactReference reference = new IdentifiedContactReference(source, identifier);

                    Contact existingContact = client.Get<Contact>(
                        reference,
                        new ContactExpandOptions(PersonalInformation.DefaultFacetKey, CvPersonFacet.DefaultFacetKey)
                        
                        );

                    if (existingContact == null)
                    {
                        return;
                    }

                    PersonalInformation personalInformation = existingContact.GetFacet<PersonalInformation>(PersonalInformation.DefaultFacetKey);

                    if (personalInformation == null)
                    {
                        personalInformation = new PersonalInformation();
                    }

                    personalInformation.FirstName = data.FirstName;
                    personalInformation.LastName = data.LastName;
                    personalInformation.Gender = data.Gender;

                    client.SetFacet<PersonalInformation>(existingContact, PersonalInformation.DefaultFacetKey, personalInformation);

                    CvPersonFacet personFacet = existingContact.GetFacet<CvPersonFacet>(CvPersonFacet.DefaultFacetKey);

                    if (personFacet == null)
                    {
                        personFacet = new CvPersonFacet();
                    }

                    personFacet.Company = data.Company;
                    personFacet.Age = data.Age;

                    client.SetFacet<CvPersonFacet>(existingContact, CvPersonFacet.DefaultFacetKey, personFacet);

                    client.Submit();

                    ReadOnlyCollection<IXdbOperation> operations = client.LastBatch;
                }
                catch (XdbExecutionException ex)
                {
                }
            }
        }

        public void AddContact(string source, string identifier, ImageProcessingContact data)
        {
            using (XConnectClient client = GetClient())
            {
                try
                {
                    // Get a known contact
                    IdentifiedContactReference reference = new IdentifiedContactReference(source, identifier);

                    Contact existingContact = client.Get<Contact>(
                        reference,
                        new ContactExpandOptions(PersonalInformation.DefaultFacetKey, CvPersonFacet.DefaultFacetKey, Avatar.DefaultFacetKey));

                    if (existingContact != null)
                    {
                        return;
                    }

                    var contactIdentifier = new[]
                    {
                        new ContactIdentifier(source, identifier, ContactIdentifierType.Known)
                    };

                    Contact contact = new Contact(contactIdentifier);

                    PersonalInformation personalInformation = new PersonalInformation
                    {
                        FirstName = data.FirstName,
                        LastName = data.LastName,
                        Gender = data.Gender
                    };

                    CvPersonFacet cvPersonFacet = new CvPersonFacet
                    {
                        Id = data.Id,
                        Age = data.Age,
                        Company = data.Company,
                        FirstName = data.FirstName,
                        LastName = data.LastName,
                        Photo = data.Photo48,
                        Gender = data.Gender
                    };
                    if (!string.IsNullOrWhiteSpace(data.ParsedText))
                    {
                        cvPersonFacet.BadgeText = data.ParsedText
                            .Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim())
                            .ToList();
                    }

                    Avatar avatarFacet = new Avatar(string.Empty, new byte[] { });
                    if (data.Photo48 != null)
                    {
                        avatarFacet.MimeType = "image/jpg";
                        avatarFacet.Picture = Convert.FromBase64String(data.Photo48);
                    }

                    client.SetFacet<Avatar>(contact, Avatar.DefaultFacetKey, avatarFacet);
                    client.SetFacet<PersonalInformation>(contact, PersonalInformation.DefaultFacetKey, personalInformation);
                    client.SetFacet<CvPersonFacet>(contact, CvPersonFacet.DefaultFacetKey, cvPersonFacet);

                    client.AddContact(contact);

                    client.Submit();
                }
                catch (Sitecore.XConnect.XdbExecutionException ex)
                {
                }
            }
        }

        public void ForgetContact(string source, string identifier)
        {
            using (XConnectClient client = GetClient())
            {
                try
                {
                    IdentifiedContactReference reference = new IdentifiedContactReference(source, identifier);

                    Contact existingContact = client.Get<Contact>(
                        reference,
                        new ContactExpandOptions());

                    if (existingContact == null)
                    {
                        return;
                    }

                    client.ExecuteRightToBeForgotten(existingContact);

                    client.Submit();
                }
                catch (Sitecore.XConnect.XdbExecutionException ex)
                {
                }
            }
        }

        public void RegisterOfflineInteraction(string source, string identifier, FaceApiFacet face)
        {
            using (XConnectClient client = GetClient())
            {
                try
                {
                    IdentifiedContactReference reference = new IdentifiedContactReference(source, identifier);

                    Contact existingContact = client.Get<Contact>(
                        reference,
                        new ContactExpandOptions(PersonalInformation.DefaultFacetKey));

                    if (existingContact == null)
                    {
                        return;
                    }
                    
                    Interaction interaction = new Interaction(existingContact, InteractionInitiator.Contact, channelId: Settings.OfflineChannelId, userAgent: "video camera");
                    
                    //Add Device profile
                    DeviceProfile newDeviceProfile = new DeviceProfile(Guid.NewGuid());
                    newDeviceProfile.LastKnownContact = existingContact;
                    client.AddDeviceProfile(newDeviceProfile);
                    interaction.DeviceProfile = newDeviceProfile;

                    //Add fake Ip info
                    IpInfo fakeIpInfo = new IpInfo("127.0.0.1");
                    fakeIpInfo.BusinessName = "Home";
                    client.SetFacet<IpInfo>(interaction, IpInfo.DefaultFacetKey, fakeIpInfo);

                    //Add fake webvisit
                    WebVisit fakeWebVisit = new WebVisit();
                    fakeWebVisit.SiteName = "Offline";
                    client.SetFacet<WebVisit>(interaction, WebVisit.DefaultFacetKey, fakeWebVisit);

                    //Adding FaceApi facet
                    client.SetFacet<FaceApiFacet>(interaction, FaceApiFacet.DefaultFacetKey, face);

                    Outcome outcome = new Outcome(Settings.OfflineGoalId, DateTime.UtcNow, "USD", 0);
                    outcome.EngagementValue = 10;
                    outcome.Text = "Face recognized";
                    interaction.Events.Add(outcome);

                    client.AddInteraction(interaction);

                    client.Submit();
                }
                catch (XdbExecutionException ex)
                {
                }
            }
        }

        public void RegisterOnlineInteraction(string source, string identifier)
        {
            using (XConnectClient client = GetClient())
            {
                try
                {
                    IdentifiedContactReference reference = new IdentifiedContactReference(source, identifier);

                    Contact existingContact = client.Get<Contact>(
                        reference,
                        new ContactExpandOptions(PersonalInformation.DefaultFacetKey));

                    if (existingContact == null)
                    {
                        return;
                    }

                    Goal goal = new Goal(Settings.OnlineGoalId, DateTime.UtcNow);
                    goal.EngagementValue = 100;
                    goal.Text = "Agenda Signup";

                    Interaction interaction = new Interaction(existingContact, InteractionInitiator.Contact, channelId: Settings.OnlineChannelId, userAgent: "laptop/demo site");
                    interaction.Events.Add(goal);

                    //Add Device profile
                    DeviceProfile newDeviceProfile = new DeviceProfile(Guid.NewGuid());
                    newDeviceProfile.LastKnownContact = existingContact;
                    client.AddDeviceProfile(newDeviceProfile);
                    interaction.DeviceProfile = newDeviceProfile;

                    //Add fake Ip info
                    IpInfo fakeIpInfo = new IpInfo("127.0.0.1");
                    fakeIpInfo.BusinessName = "Home";
                    client.SetFacet<IpInfo>(interaction, IpInfo.DefaultFacetKey, fakeIpInfo);

                    //Add fake webvisit
                    WebVisit fakeWebVisit = new WebVisit();
                    fakeWebVisit.SiteName = "Online";
                    client.SetFacet<WebVisit>(interaction, WebVisit.DefaultFacetKey, fakeWebVisit);
                    
                    client.AddInteraction(interaction);

                    client.Submit();
                }
                catch (XdbExecutionException ex)
                {
                }
            }
        }

        public IEnumerable<ContactViewModel> GetContacts(string identificationSource)
        {
            using (XConnectClient client = GetClient())
            {
                IAsyncQueryable<Sitecore.XConnect.Contact> query = client.Contacts
                    .Where(c => c.Identifiers.Any(s => s.Source == identificationSource));

                SearchResults<Contact> results;
                results = Task.Run(async () => await query.ToSearchResults()).Result;

                List<Contact> contacts = results.Results.Select(x => x.Item).ToList().Result;

                foreach (Contact contact in contacts)
                {
                    yield return GetContact(identificationSource, contact.Identifiers.First(i => i.Source == identificationSource).Identifier);
                }
            }
        }

        public IEnumerable<PBIOfflineInteraction> GetAllInteractions()
        {
            using (XConnectClient client = GetClient())
            {
                IAsyncQueryable<Sitecore.XConnect.Interaction> query = client.Interactions
                    .WithExpandOptions(new InteractionExpandOptions(FaceApiFacet.DefaultFacetKey, WebVisit.DefaultFacetKey)
                    {
                        Contact = new RelatedContactExpandOptions(PersonalInformation.DefaultFacetKey)
                    });

                SearchResults<Interaction> results = Task.Run(async () => await query.ToSearchResults()).Result;

                List<Interaction> interactions = results.Results.Select(x => x.Item).ToList().Result;

                foreach (Interaction interaction in interactions)
                {
                    var contact = interaction.Contact as Contact;
                    var face = interaction.GetFacet<FaceApiFacet>(FaceApiFacet.DefaultFacetKey);

                    var offlineInteraction = new PBIOfflineInteraction
                    {
                        Id = interaction.Id.ToString(),
                        ContactId = contact?.Id.ToString(),
                        StartTime = interaction.StartDateTime,
                        EndTime = interaction.EndDateTime,
                        EngagmentValue = interaction.EngagementValue,
                        Channel = this.GetChannelName(interaction.ChannelId),
                        Goals = interaction.Events.Select(e => e.Text).ToList()
                    };

                    if (face != null)
                    {
                        offlineInteraction.HasFaceInfo = true;
                        offlineInteraction.HappinessValue = face.HappinessValue;
                        offlineInteraction.SmileValue = face.SmileValue;
                        offlineInteraction.AngerValue = face.AngerValue;
                        offlineInteraction.SurpriseValue = face.SurpriseValue;
                        offlineInteraction.SadnessValue = face.SadnessValue;
                        offlineInteraction.FearValue = face.FearValue;
                        offlineInteraction.DisgustValue = face.DisgustValue;
                        offlineInteraction.ContemptValue = face.ContemptValue;
                        offlineInteraction.NeutralValue = face.NeutralValue;
                    }
                    yield return offlineInteraction;
                }
            }
        }

        private string GetChannelName(Guid id)
        {
            if (id == Settings.OfflineChannelId)
            {
                return "Industry event sponsorship";
            }

            if (id == Settings.OnlineChannelId)
            {
                return "Other email";
            }

            return id.ToString();
        }

        private XConnectClient GetClient()
        {
            XConnectClientConfiguration cfg;
            if (string.IsNullOrEmpty(XconnectThumbprint))
            {
                cfg = new XConnectClientConfiguration(
                    new XdbRuntimeModel(FaceApiModel.Model),
                    new Uri(XconnectUrl),
                    new Uri(XconnectUrl));

            }
            else
            {
                CertificateWebRequestHandlerModifierOptions options =
                CertificateWebRequestHandlerModifierOptions.Parse("StoreName=My;StoreLocation=LocalMachine;FindType=FindByThumbprint;FindValue=" + XconnectThumbprint);
                var certificateModifier = new CertificateWebRequestHandlerModifier(options);

                // Step 2 - Client Configuration

                var collectionClient = new CollectionWebApiClient(new Uri(XconnectUrl + "/odata"), null, new[] { certificateModifier });
                var searchClient = new SearchWebApiClient(new Uri(XconnectUrl + "/odata"), null, new[] { certificateModifier });
                var configurationClient = new ConfigurationWebApiClient(new Uri(XconnectUrl + "/configuration"), null, new[] { certificateModifier });


                cfg = new XConnectClientConfiguration(
                new XdbRuntimeModel(FaceApiModel.Model), collectionClient, searchClient, configurationClient);
            }

            cfg.Initialize();

            var client = new XConnectClient(cfg);

            return client;
        }
    }
}