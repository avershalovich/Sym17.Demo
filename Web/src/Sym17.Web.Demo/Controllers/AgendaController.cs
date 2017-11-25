using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Glass.Mapper.Sc;
using Newtonsoft.Json;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sym17.Web.Demo.Models;
using Sym17.Web.Demo.Segmentation;
using Sitecore.XConnect.Client;
using Sitecore.XConnect;

namespace Sym17.Web.Demo.Controllers
{
    public class AgendaController : Controller
    {
        private static string _json = @"agenda.json";
        private readonly SitecoreService _service = new SitecoreService("master"); 
        private readonly ID _agendaFolder = new ID("{7355E59B-E6CD-49EF-9BC6-F96B3DC7A2D7}");

        public ActionResult Index()
        {
            using (var r = System.IO.File.OpenText(Server.MapPath("~/" + _json)))
            {
                string json = r.ReadToEnd();
                var items =  JsonConvert.DeserializeObject<List<Agenda>>(json);
                var dates = items.GroupBy(x => x.Date).OrderBy(x => x.Key).ToList();

                var folder = _service.GetItem<AgendaFolder>(_agendaFolder.Guid);

                foreach (var date in dates)
                {
                    var dateName = date.Key.ToString("dd MMMM");
                    var dateFolder = _service.Create(folder, new AgendaFolder{Name = dateName });
                    var times = date.GroupBy(x => x.Time).OrderBy(x => x.Key).ToList();
                    foreach (var time in times)
                    {
                       var timeName = ItemUtil.ProposeValidItemName(Regex.Replace(time.Key, @"[^a-zA-Z0-9\x7f-\xff\s\-]+", ""));
                       var timeFolder = _service.Create(dateFolder, new AgendaFolder { Name = timeName, DisplayName = time.Key });
                        foreach (var agenda in time)
                        {
                            agenda.Name = ItemUtil.ProposeValidItemName(Regex.Replace(agenda.Title, @"[^a-zA-Z0-9\x7f-\xff\s\-]+", ""));
                            _service.Create(timeFolder, agenda);
                        }
                    }
                }
                return null;
            }
        }
    }
}