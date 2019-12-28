using DiscordBot.EscapeFromTarkovAPI;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using static DiscordBot.EscapeFromTarkovAPI.TarkovMaps.Map;

namespace DiscordBot.EscapeFromTarkovDataBrowser
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public List<VisualMap> visualMaps = new List<VisualMap>();

        private void btnAddVisualMap_Click(object sender, RoutedEventArgs e)
        {
            VisualMap visualMap = new VisualMap()
            {
                Name = tboxVisualMapName.Text,
                URL = tboxVisualMapUrl.Text
            };

            visualMaps.Add(visualMap);
            lstVisualMaps.ItemsSource = visualMaps.ToArray();
        }

        private void btnAddMap_Click(object sender, RoutedEventArgs e)
        {
            var map = new TarkovMaps.Map()
            {
                Name = tboxMapName.Text,
                URL = tboxUrl.Text,
                VisualMaps = visualMaps.ToArray()
            };
            TarkovMaps.AddMap(map);

            // Reset fields
            visualMaps = new List<VisualMap>();
            lstVisualMaps.ItemsSource = visualMaps;
        }
    }
}
