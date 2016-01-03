using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace SimpleHomeAuto.Data
{
    public class Context
    {
        private static Context instance = null;

        public static Context Instance
        {
            get
            {
                if (instance == null) instance = new Context();
                return instance;
            }
        }

        public string Version
        {
            get
            {
                var pv = Package.Current.Id.Version;
                Version version = new Version(Package.Current.Id.Version.Major,
                                              Package.Current.Id.Version.Minor,
                                              Package.Current.Id.Version.Build,
                                              Package.Current.Id.Version.Revision);

                return "v" + version.ToString();
            }
        }

        public Context()
        {
            Scenarios = new ObservableCollection<Scenario>();
            LoadSettings();
        }

        public ObservableCollection<Scenario> Scenarios { get; set; }

        public void LoadSettings()
        {
            Scenarios = new ObservableCollection<Scenario>();
            try
            {
                var data = Windows.Storage.ApplicationData.Current.RoamingSettings.Values["scenarios"] as string;
                if (!string.IsNullOrWhiteSpace(data))
                {
                    Scenarios = JsonConvert.DeserializeObject<ObservableCollection<Scenario>>(data);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        public void SaveSettings()
        {
            try
            {
                var data = JsonConvert.SerializeObject(Scenarios);
                Windows.Storage.ApplicationData.Current.RoamingSettings.Values["scenarios"] = data;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
    }
}
