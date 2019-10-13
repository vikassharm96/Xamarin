using System;
using System.IO;
using System.Threading.Tasks;
using demo.Models;

namespace demo.ViewModels
{
    public class MainActivityViewModel : Android.Arch.Lifecycle.ViewModel
    {
        public string Text { get; set; }
        public string FileLocation { get; set; }

        public MainActivityViewModel()
        {
            FileLocation = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "demo.txt");
        }

        public async Task SaveTextAsync(string text)
        {
            var textInfo = new TextInfo
            {
                //DateTime = DateTime.Now,
                Text = text
            };
            using (var writer = File.CreateText(FileLocation))
            {
                await writer.WriteLineAsync(string.Format("{0}", textInfo.Text));
            }
        }

        public async Task<string> ReadTextAsync()
        {
            if (FileLocation == null || !File.Exists(FileLocation))
            {
                return "";
            }

            var fileText = "";
            using (var reader = new StreamReader(FileLocation, true))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    fileText = line;
                }
            }
            return fileText;
        }
    }
}