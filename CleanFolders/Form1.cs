using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Logger = Utilities.Logger;

namespace CleanFolders
{
    public partial class Form1 : Form
    {
        BindingList<FolderViewModel> foldersViewModel = new BindingList<FolderViewModel>();  // Bind to datagridview
        List<Folder> folders = new List<Folder>();
        public Logger Logger { get; set; }
        public Form1()
        {
            InitializeComponent();
            this.rtDescription.Text = Properties.Resources.String_rtDescription_Text;
            this.btAdd.Text = Properties.Resources.String_btAdd_Text;
            this.btRemove.Text = Properties.Resources.String_btRemove_Text;
            this.btOK.Text = Properties.Resources.String_btOK_Text;
            this.btCancel.Text = Properties.Resources.String_btCancel_Text;
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            SaveFoldersEntered();
            this.Close();
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            // Don't save settings
            this.Close();
        }

        private void btAdd_Click(object sender, EventArgs e)
        {
            // Browse for folder and add in list
            this.folderBrowserDialog1.Description = Properties.Resources.String_folderBrowserDialog1_Description;
            this.folderBrowserDialog1.ShowNewFolderButton = false;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                // Need to check if selected item is already in the list
                // Reject if it is
                this.foldersViewModel.Add(new FolderViewModel { Path = folderBrowserDialog1.SelectedPath });
            }
        }
        
        private void btRemove_Click(object sender, EventArgs e)
        {
            // Delete selected file from list
            string selectedFile = (string)dgvFolders.CurrentCell.Value;
            FolderViewModel itemToRemove = foldersViewModel.SingleOrDefault(r => r.Path == selectedFile);
            this.foldersViewModel.Remove(itemToRemove);
        }

        #region Event handlers
        private void Form1_Load(object sender, EventArgs e)
        {
            // Configure datagrid
            this.dgvFolders.RowHeadersVisible = false;
            this.dgvFolders.ColumnHeadersVisible = false;
            this.dgvFolders.Dock = DockStyle.None;
            this.dgvFolders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            LoadSavedFolders();
        }

        /// <summary>
        /// Event handler when a cell changes. This will also get triggered when a new path is added.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvFolders_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Check to make sure that the Path column is getting edited
            if (dgvFolders.Columns[e.ColumnIndex].Name == "Path")
            {
                string newPath = dgvFolders[e.ColumnIndex, e.RowIndex].Value.ToString();  // Get the new entered or edited path entry
                if (!Directory.Exists(newPath)) // Check make sure the path exists
                {
                    MessageBox.Show(String.Format("The file {0} does not exist. Removing entry.", newPath),"Invalid Entry",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                    dgvFolders.Rows.RemoveAt(e.RowIndex);  // Remove entry if it does not exist
                }
            }
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Saved folders entred in the datagrid into the registry.
        /// </summary>
        private void SaveFoldersEntered()
        {
            // Save settings here
            // Loop through datagridview items
            List<Folder> folders = new List<Folder>();
            foreach (DataGridViewRow item in dgvFolders.Rows)
            {
                // Make sure to check that the cell is not null before trying to access it
                if (item.Cells[0].Value != null)
                {
                    string _path = item.Cells["Path"].Value.ToString();
                    folders.Add(new Folder { Path = _path });
                }
            }
            // Save folders into the registry
            Settings settings = new Settings
            {
                Folders = folders,
            };
            settings.SaveFolderEntries();
        }

        /// <summary>
        /// Load saved folder saved in the registry.
        /// </summary>
        private void LoadSavedFolders()
        {
            // Load folders from registry
            Settings settings = new Settings
            {
                Folders = folders, // Pass folders object
                Logger = Logger,
            };
            settings.LoadFolderEntries();

            // Initialize binding list with read folders list
            foreach (Folder folder in folders)
            {
                foldersViewModel.Add(new FolderViewModel { Path = folder.Path });
            }
            this.dgvFolders.DataSource = foldersViewModel;
        }

        #endregion
    }
}
