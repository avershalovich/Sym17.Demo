using System;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace ImageProcess
{
    public static class SettingsReader
    {
        private static string _badgeTemplateFile = @"..\..\badge.json";
        private static string _settingsFile = @"..\..\settings.json";
        public static BadgeTemplate ReadBadgeSettings()
        {
            using (var r = File.OpenText(_badgeTemplateFile))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<BadgeTemplate>(json);
            }
        }
        public static void WriteBadgeSettings(string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    //var template = JsonConvert.DeserializeObject<BadgeTemplate>(json);
                    File.WriteAllText(_badgeTemplateFile, json);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show(_badgeTemplateFile+" is empty!");
            }

            //open file stream
            //using (StreamWriter file = File.CreateText(_badgeTemplateFile))
            //{
            //    JsonSerializer serializer = new JsonSerializer();
            //    serialize object directly into file stream
            //    serializer.Serialize(file, template);
            //}
        }

        public static SettingsTemplate ReadSettings()
        {
            using (var r = File.OpenText(_settingsFile))
            {
                string json = r.ReadToEnd();
                return JsonConvert.DeserializeObject<SettingsTemplate>(json);
            }
        }

        public static void WriteSettings(string json)
        {
            if (string.IsNullOrEmpty(json))
                MessageBox.Show(_settingsFile + " is empty!");
            else
                try
                {
                    File.WriteAllText(_settingsFile, json);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
        }
    }
}
