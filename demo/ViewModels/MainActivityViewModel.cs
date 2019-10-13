using System;
using System.IO;
using System.Threading.Tasks;
using demo.Models;

namespace demo.ViewModels
{
    public class MainActivityViewModel : Android.Arch.Lifecycle.ViewModel
    {
        public string Text { get; set; }
        public string InternalFileLocation { get; set; }

        public MainActivityViewModel()
        {
            InternalFileLocation = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "demo.txt");
        }

        public async Task SaveTextAsync(string text)
        {
            var textInfo = new TextInfo
            {
                //DateTime = DateTime.Now,
                Text = text
            };
            using (var writer = File.CreateText(InternalFileLocation))
            {
                await writer.WriteLineAsync(string.Format("{0}", textInfo.Text));
            }
        }

        public async Task<string> ReadTextAsync()
        {
            if (InternalFileLocation == null || !File.Exists(InternalFileLocation))
            {
                return "";
            }

            var fileText = "";
            using (var reader = new StreamReader(InternalFileLocation, true))
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