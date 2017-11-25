//using Sitecore.Marketing.Automation.Models;
//using Sitecore.XConnect;
//using Sitecore.XConnect.Collection.Model;
//using Sitecore.Xdb.MarketingAutomation.Core.Processing.Plan;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace Sym17.Web.Demo.Controllers
//{
//    public class TestController : Controller
//    {
//        // GET: Test
//        public ActionResult Index()
//        {
//            string contactID;

//            List<BehaviorProfileValue> newProfile = new List<BehaviorProfileValue>();
//            //newProfile.Add(new BehaviorProfileValue())

//            if (newProfile != null)
//            {
//                if (Enumerable.Any<BehaviorProfileValue>((IEnumerable<BehaviorProfileValue>)newProfile))
//                {

//                    CollectionModel.SetProfileScores(
//                        this.Services.Collection, 
//                        this.CreateInteraction(context), 
//                        this.GetProfileScores(context));

//                }
//            }
//        }

//        private Interaction CreateInteraction(IContactProcessingContext context)
//        {
//            Interaction interaction = new Interaction((IEntityReference<Contact>)context.Contact, InteractionInitiator.Brand, Constants.SystemChannelId, Constants.UserAgent);
//            Event @event = new Event(Constants.ProfileScoreChangeEventDefinitionId, DateTime.UtcNow);
//            interaction.Events.Add(@event);
//            XdbContextOperationExtensions.AddInteraction(this.Services.Collection, interaction);
//            return interaction;
//        }

//        private ProfileScores GetProfileScores(IContactProcessingContext context)
//        {
//            ContactBehaviorProfile contactFacet = this.GetContactFacet<ContactBehaviorProfile>(context.Contact);
//            TestController.UpdateContactBehaviorProfile(contactFacet.Scores, (IEnumerable<BehaviorProfileValue>)this.BehaviorProfileValues);
//            ProfileScores profileScores = new ProfileScores();
//            foreach (KeyValuePair<Guid, ProfileScore> keyValuePair in contactFacet.Scores)
//                profileScores.Scores.Add(keyValuePair.Key, keyValuePair.Value);
//            return profileScores;
//        }

//        protected virtual T GetContactFacet<T>(Contact contact) where T : Facet, new()
//        {
//            T facet = contact.GetFacet<T>();
//            if ((object)facet != null)
//                return facet;
//            return Activator.CreateInstance<T>();
//        }

//        private static void UpdateContactBehaviorProfile(Dictionary<Guid, ProfileScore> profileScores, IEnumerable<BehaviorProfileValue> behaviorProfileValues)
//        {
//            foreach (BehaviorProfileValue behaviorProfileValue in behaviorProfileValues)
//            {
//                if ((behaviorProfileValue != null ? behaviorProfileValue.KeyValues : (IDictionary<Guid, double>)null) != null)
//                {
//                    ProfileScore profileScore1;
//                    if (!profileScores.TryGetValue(behaviorProfileValue.ProfileId, out profileScore1))
//                    {
//                        Dictionary<Guid, ProfileScore> dictionary1 = profileScores;
//                        Guid profileId = behaviorProfileValue.ProfileId;
//                        ProfileScore profileScore2 = new ProfileScore();
//                        profileScore2.ProfileDefinitionId = behaviorProfileValue.ProfileId;
//                        IDictionary<Guid, double> keyValues = behaviorProfileValue.KeyValues;
//                        Func<KeyValuePair<Guid, double>, Guid> func = (Func<KeyValuePair<Guid, double>, Guid>)(kv => kv.Key);
//                        Func<KeyValuePair<Guid, double>, Guid> keySelector;
//                        Dictionary<Guid, double> dictionary2 = Enumerable.ToDictionary<KeyValuePair<Guid, double>, Guid, double>((IEnumerable<KeyValuePair<Guid, double>>)keyValues, keySelector, (Func<KeyValuePair<Guid, double>, double>)(kv => kv.Value));
//                        profileScore2.Values = dictionary2;
//                        dictionary1[profileId] = profileScore2;
//                    }
//                    else if (profileScore1.Values == null)
//                    {
//                        ProfileScore profileScore2 = profileScore1;
//                        IDictionary<Guid, double> keyValues = behaviorProfileValue.KeyValues;
//                        Func<KeyValuePair<Guid, double>, Guid> func = (Func<KeyValuePair<Guid, double>, Guid>)(kv => kv.Key);
//                        Func<KeyValuePair<Guid, double>, Guid> keySelector;
//                        Dictionary<Guid, double> dictionary = Enumerable.ToDictionary<KeyValuePair<Guid, double>, Guid, double>((IEnumerable<KeyValuePair<Guid, double>>)keyValues, keySelector, (Func<KeyValuePair<Guid, double>, double>)(kv => kv.Value));
//                        profileScore2.Values = dictionary;
//                    }
//                    else
//                    {
//                        foreach (KeyValuePair<Guid, double> keyValuePair in (IEnumerable<KeyValuePair<Guid, double>>)behaviorProfileValue.KeyValues)
//                        {
//                            double num;
//                            if (profileScore1.Values.TryGetValue(keyValuePair.Key, out num))
//                            {
//                                Dictionary<Guid, double> values = profileScore1.Values;
//                                Guid key = keyValuePair.Key;
//                                values[key] = values[key] + keyValuePair.Value;
//                            }
//                            else
//                                profileScore1.Values.Add(keyValuePair.Key, keyValuePair.Value);
//                        }
//                    }
//                }
//            }
//        }
//    }
//}