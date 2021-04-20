using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace Dairy
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string ServerUrl { get; set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            ServerUrl = JsonSerializer.Deserialize<ServerUrlObject>(File.ReadAllText("ServerUrl.json")).ServerUrl;
            base.OnStartup(e);
        }

        private class ServerUrlObject
        {
            public string ServerUrl { get; set; }
        }
    }
}
