using Microsoft.Extensions.Logging;
using Serilog;

using Sitecore.Marketing.Automation.Activity;
using Sitecore.Marketing.Automation.Models;
using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Collection.Model;
using Sitecore.XConnect.Operations;
using Sitecore.Xdb.MarketingAutomation.Core.Activity;
using Sitecore.Xdb.MarketingAutomation.Core.Processing.Plan;
using Sym17.Web.Demo.Segmentation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Sym17.Web.Demo.Activity
{
    using Constants = Sitecore.Marketing.Automation.Models.Constants;

    public class ChangeContactBehaviorProfileValue : BaseActivity
    {
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Marketing Automation engine requires it to be a generic List<T>")]
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter is required to be public by Marketing Automation engine")]
        public List<BehaviorProfileValue> BehaviorProfileValues { get; set; }

        public ChangeContactBehaviorProfileValue(ILogger<ChangeContactBehaviorProfileValue> logger)
          : base((ILogger<IActivity>)logger)
        {
        }

        public override ActivityResult Invoke(IContactProcessingContext context)
        {
            if (this.BehaviorProfileValues != null)
            {
                if (Enumerable.Any<BehaviorProfileValue>((IEnumerable<BehaviorProfileValue>)this.BehaviorProfileValues))
                {
                    try
                    {
                        CollectionModel.SetProfileScores(this.Services.Collection, this.CreateInteraction(context), this.GetProfileScores(context));
                        XConnectSynchronousExtensions.Submit(this.Services.Collection);
                        //LoggerExtensions.LogDebug((ILogger)this.Logger, string.Format((IFormatProvider)CultureInfo.InvariantCulture, Resources.TheFacetHasBeenSetAndSubmittedSuccessfullyPattern, (object)"ContactBehaviorProfile"), Array.Empty<object>());
                        return (ActivityResult)new SuccessMove();
                    }
                    catch (Exception ex) when (ex is FacetOperationException || ex is ArgumentNullException || ex is InvalidOperationException)
                    {
                        //LoggerExtensions.LogError((ILogger)this.Logger, (EventId)0, ex, string.Format((IFormatProvider)CultureInfo.InvariantCulture, Resources.TheFacetHasNotBeenSetAndSubmittedSuccessfullyPattern, (object)"ContactBehaviorProfile"), Array.Empty<object>());
                        return (ActivityResult)new Failure(string.Format((IFormatProvider)CultureInfo.CurrentCulture, Resources.TheFacetHasNotBeenSetAndSubmittedSuccessfullyPattern, (object)"ContactBehaviorProfile"));
                    }
                }
            }
            Log.Debug( string.Format((IFormatProvider)CultureInfo.InvariantCulture, Resources.TheActivityPropertyHasNotBeenSetPattern, (object)"BehaviorProfileValues"), Array.Empty<object>());
            return (ActivityResult)new Failure(string.Format((IFormatProvider)CultureInfo.CurrentCulture, Resources.TheActivityPropertyHasNotBeenSetPattern, (object)"BehaviorProfileValues"));
        }

        private Interaction CreateInteraction(IContactProcessingContext context)
        {
            Interaction interaction = new Interaction((IEntityReference<Contact>)context.Contact, InteractionInitiator.Brand, Constants.SystemChannelId, Constants.UserAgent);
            Event @event = new Event(Constants.ProfileScoreChangeEventDefinitionId, DateTime.UtcNow);
            interaction.Events.Add(@event);
            XdbContextOperationExtensions.AddInteraction(this.Services.Collection, interaction);
            return interaction;
        }

        private ProfileScores GetProfileScores(IContactProcessingContext context)
        {
            //ContactBehaviorProfile contactFacet = this.GetContactFacet<ContactBehaviorProfile>(context.Contact);

            XConnectClient client = XConnectClientReference.GetClient();

            Contact existingContact = client.Get<Contact>(
                       (IEntityReference<Contact>)context.Contact,
                       new ContactExpandOptions(ContactBehaviorProfile.DefaultFacetKey));

            ContactBehaviorProfile contactFacet = existingContact.GetFacet<ContactBehaviorProfile>(ContactBehaviorProfile.DefaultFacetKey);

            if (contactFacet == null) contactFacet = new ContactBehaviorProfile();

            ChangeContactBehaviorProfileValue.UpdateContactBehaviorProfile(contactFacet.Scores, (IEnumerable<BehaviorProfileValue>)this.BehaviorProfileValues);
            ProfileScores profileScores = new ProfileScores();
            foreach (KeyValuePair<Guid, ProfileScore> keyValuePair in contactFacet.Scores)
                profileScores.Scores.Add(keyValuePair.Key, keyValuePair.Value);

            foreach (var profileScore in profileScores.Scores)
            {
                    Log.Debug("profileScore kv Key = " + profileScore.Key);
                    Log.Debug("profileScore kv Value = " + profileScore.Value);
            }

            return profileScores;
        }

        private static void UpdateContactBehaviorProfile(Dictionary<Guid, ProfileScore> profileScores, IEnumerable<BehaviorProfileValue> behaviorProfileValues)
        {
            foreach (var profile in profileScores)
            {
                Log.Debug("profileScores.Key = " + profile.Key);
                Log.Debug("profileScores.Value = " + profile.Value);
            }
            foreach (var behaviorProfile in behaviorProfileValues)
            {
                Log.Debug("behavior profile id = " + behaviorProfile.ProfileId);
                foreach (var kv in behaviorProfile.KeyValues)
                {
                    Log.Debug("behavior kv Key = " + kv.Key);
                    Log.Debug("behavior kv Value = " + kv.Value);
                }
            }

            foreach (BehaviorProfileValue behaviorProfileValue in behaviorProfileValues)
            {
                if ((behaviorProfileValue != null ? behaviorProfileValue.KeyValues : (IDictionary<Guid, double>)null) != null)
                {
                    ProfileScore profileScore1;
                    if (!profileScores.TryGetValue(behaviorProfileValue.ProfileId, out profileScore1))
                    {
                        Dictionary<Guid, ProfileScore> dictionary1 = profileScores;
                        Guid profileId = behaviorProfileValue.ProfileId;
                        ProfileScore profileScore2 = new ProfileScore();
                        profileScore2.ProfileDefinitionId = behaviorProfileValue.ProfileId;
                        IDictionary<Guid, double> keyValues = behaviorProfileValue.KeyValues;
                        //Func<KeyValuePair<Guid, double>, Guid> func = (Func<KeyValuePair<Guid, double>, Guid>)(kv => kv.Key);
                        //Func<KeyValuePair<Guid, double>, Guid> keySelector;
                        Dictionary<Guid, double> dictionary2 = keyValues.ToDictionary(kv => kv.Key, kv => kv.Value);
                        profileScore2.Values = dictionary2;
                        dictionary1[profileId] = profileScore2;
                    }
                    else if (profileScore1.Values == null)
                    {
                        ProfileScore profileScore2 = profileScore1;
                        IDictionary<Guid, double> keyValues = behaviorProfileValue.KeyValues;
                        //Func<KeyValuePair<Guid, double>, Guid> func = (Func<KeyValuePair<Guid, double>, Guid>)(kv => kv.Key);
                        //Func<KeyValuePair<Guid, double>, Guid> keySelector;
                        Dictionary<Guid, double> dictionary = keyValues.ToDictionary(kv => kv.Key, kv => kv.Value);
                        profileScore2.Values = dictionary;
                    }
                    else
                    {
                        foreach (KeyValuePair<Guid, double> keyValuePair in (IEnumerable<KeyValuePair<Guid, double>>)behaviorProfileValue.KeyValues)
                        {
                            double num;
                            if (profileScore1.Values.TryGetValue(keyValuePair.Key, out num))
                            {
                                Dictionary<Guid, double> values = profileScore1.Values;
                                Guid key = keyValuePair.Key;
                                values[key] = values[key] + keyValuePair.Value;
                            }
                            else
                                profileScore1.Values.Add(keyValuePair.Key, keyValuePair.Value);
                        }
                    }
                }
            }
        }
    }

}