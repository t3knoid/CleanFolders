using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logger = Utilities.Logger;

namespace CleanFolders
{
    class Settings
    {
        public 
            Logger Logger { get; set; }
        List<Folder> _folders;
        public List<Folder> Folders
        {
            get
            {
                return _folders;
            }
            set
            {
                _folders = value;
            }
        }
        /// <summary>
        /// Save folder entries
        /// </summary>
        public void SaveFolderEntries()
        {
            DeleteFolderEntries();
            try
            {
                Microsoft.Win32.RegistryKey key;
                key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\\CleanFolders\\Files");
                for (var i = 0; i < _folders.Count; i++)
                {
                    key.SetValue(String.Format("file{0}", i.ToString()), _folders[i].Path);
                }
                key.Close();
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to save folder entries into the registry.");
                Logger.Error(ex);
            }
        }

        public void DeleteFolderEntries()
        {
            try
            {
                Microsoft.Win32.RegistryKey key;
                key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\CleanFolders\\Files", true);  // Open registry key for write
                string[] subkeys = key.GetValueNames();
                foreach (string sk in subkeys)
                {
                    key.DeleteValue(sk);
                }
                key.Close();
            }
            catch (Exception ex)
            {
                Logger.Error("Error deleting folder entries from registry.");
                Logger.Error(ex);
            }
        }
        public void LoadFolderEntries()
        {
            try
            {
                Microsoft.Win32.RegistryKey key;
                key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\CleanFolders\\Files");
                string[] subkeys = key.GetValueNames();
                foreach (string sk in subkeys)
                {
                    Folders.Add(new Folder { Path = (string)key.GetValue(sk) });
                }
                key.Close();
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to load folder entries from the registry.");
                Logger.Error(ex);
               
            }
        }
    }
}
