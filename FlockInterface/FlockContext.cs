using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlockProcess;

namespace FlockInterface
{
    /// <summary>
    /// The Flock application context. Handles all the UI interactions.
    /// </summary>
    public class FlockContext : ApplicationContext
    {
        #region Vars

        /// <summary>
        /// The icon to be displayed in the task bar
        /// </summary>
        private NotifyIcon notifyIcon;

        /// <summary>
        /// The UI components container
        /// </summary>
        private IContainer components;

        /// <summary>
        /// The location of the application icon
        /// </summary>
        private static readonly string iconLocation = "flock.ico";

        /// <summary>
        /// The text to be displayed when hovering over the icon
        /// </summary>
        private static readonly string defaultToolTip = "Wallpaper retriever from Unsplash";

        /// <summary>
        /// The retrieve button's enable text
        /// </summary>
        private static readonly string retrieveButtonEnabledText = "Get New Wallpaper";

        /// <summary>
        /// The retrieve button disable text
        /// </summary>
        private static readonly string retrieveButtonDisabledText = "Retrieving New Wallpaper...";

        private ToolStripMenuItem photoByMenuItem;

        private ToolStripMenuItem authorMenuItem;

        private ToolStripMenuItem unsplashMenuItem;

        private ToolStripMenuItem getNewMenuItem;

        private ToolStripMenuItem exitMenuItem;

        private ToolStripMenuItem settingsMenuItem;

        private ToolStripMenuItem wallpaperOnOpenMenuItem;

        private ToolStripMenuItem startupMenuItem;

        private ToolStripSeparator authorSeperator;

        #endregion

        #region Init Methods

        /// <summary>
        /// Initialises a new instance of the application context
        /// </summary>
        public FlockContext()
        {
            Initialise();
            if (WallpaperProcess.IsDesktopSetByFlock())
            {
                UpdateAuthor();
            }
        }

        /// <summary>
        /// Creates the UI elements
        /// </summary>
        private void Initialise()
        {
            InitialiseMenuItems();

            components = new Container();
            notifyIcon = new NotifyIcon(components)
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = new Icon(iconLocation),
                Text = defaultToolTip,
                Visible = true
            };
            notifyIcon.ContextMenuStrip.BackColor = Color.WhiteSmoke;

            // Add context menu items
            notifyIcon.ContextMenuStrip.Items.Add(getNewMenuItem);
            notifyIcon.ContextMenuStrip.Items.Add(authorSeperator);
            notifyIcon.ContextMenuStrip.Items.Add(photoByMenuItem);
            notifyIcon.ContextMenuStrip.Items.Add(authorMenuItem);
            notifyIcon.ContextMenuStrip.Items.Add(unsplashMenuItem);
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add(string.Format("Flock v{0}", Config.ApplicationVersion)).Enabled = false;
            notifyIcon.ContextMenuStrip.Items.Add(settingsMenuItem);
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add(exitMenuItem);

            // Initialise the settings menu
            settingsMenuItem.DropDownItems.Add(wallpaperOnOpenMenuItem);
            settingsMenuItem.DropDownItems.Add(startupMenuItem);

            getNewMenuItem.Click += GetNewItem_Click;
            unsplashMenuItem.Click += UnsplashItem_Click;
            authorMenuItem.Click += AuthorItem_Click;
            startupMenuItem.Click += StartupItem_Click;
            exitMenuItem.Click += ExitItem_Click;


            notifyIcon.ContextMenuStrip.Opening += ContextMenu_Opened;
        }

        private void InitialiseMenuItems()
        {
            getNewMenuItem = new ToolStripMenuItem()
            {
                Text = retrieveButtonEnabledText,
                Visible = true,
                Image = null
            };
            getNewMenuItem.Font = new Font(getNewMenuItem.Font, getNewMenuItem.Font.Style | FontStyle.Bold);
            authorMenuItem = new ToolStripMenuItem()
            {
                Visible = false,
                Margin = new Padding()
                {
                    Left = 30
                },
                ToolTipText = "Check out more photos from this author on Unsplash.com"
            };
            unsplashMenuItem = new ToolStripMenuItem()
            {
                Text = "Unsplash",
                Visible = false,
                Margin = new Padding()
                {
                    Left = 30
                },
                ToolTipText = "Go to Unsplash.com"
            };
            photoByMenuItem = new ToolStripMenuItem()
            {
                Text = "Photo By:",
                Visible = false,
                Enabled = false
            };
            exitMenuItem = new ToolStripMenuItem()
            {
                Text = "Exit"
            };
            authorSeperator = new ToolStripSeparator()
            {
                Visible = false
            };
            settingsMenuItem = new ToolStripMenuItem()
            {
                Text = "Settings"
            };
            wallpaperOnOpenMenuItem = new ToolStripMenuItem()
            {
                Text = "New Wallpaper on Open"
            };
            startupMenuItem = new ToolStripMenuItem()
            {
                Text = "Open on Startup",
                Checked = Config.AutomaticStartup
            };
        }

        private void UpdateAuthor()
        {
            photoByMenuItem.Visible = true;
            authorSeperator.Visible = true;
            authorMenuItem.Text = WallpaperProcess.GetAuthorFromSavedFile();
            authorMenuItem.Visible = true;
            unsplashMenuItem.Visible = true;
        }

        #endregion

        #region UI Actions

        /// <summary>
        /// Handles the context menu being opened (right click)
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The cancel event</param>
        private void ContextMenu_Opened(object sender, CancelEventArgs e)
        {
            e.Cancel = false;
        }

        /// <summary>
        /// Handles the context menu being opened (left click)
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The mouse event</param>
        private void NotifyIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(notifyIcon, null);
            }
        }


        /// <summary>
        /// Fires when a user has clicked the exit button. Exits the UI thread
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event</param>
        private void ExitItem_Click(object sender, EventArgs e)
        {
            ExitThread();
        }

        /// <summary>
        /// Fires when a user has clicked the retrieve button. Calls a new thread to retrieve a wallpaper
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event</param>
        private void GetNewItem_Click(object sender, EventArgs e)
        {
            getNewMenuItem.Text = retrieveButtonDisabledText;
            getNewMenuItem.Enabled = false;

            var wallpaperTask = Task.Factory.StartNew(() =>
            {
                WallpaperProcess.Run();
            });

            wallpaperTask.ContinueWith(task =>
            {
                getNewMenuItem.Text = retrieveButtonEnabledText;
                getNewMenuItem.Enabled = true;
                getNewMenuItem.Image = null;
                UpdateAuthor();
            }, TaskScheduler.FromCurrentSynchronizationContext());

            wallpaperTask.ContinueWith(task =>
            {
                MessageBox.Show(task.Exception.InnerException.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                getNewMenuItem.Text = retrieveButtonEnabledText;
                getNewMenuItem.Enabled = true;
                getNewMenuItem.Image = null;
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        /// <summary>
        /// Fires when a user has clicked the Unsplash button. Opens Unsplash
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event</param>
        private void UnsplashItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(string.Format("{0}{1}", Config.UnsplashHomeURL, Config.UnsplashReferral));
        }

        /// <summary>
        /// Fires when a user has clicked the author button. Opens Unsplash
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event</param>
        private void AuthorItem_Click(object sender, EventArgs e)
        {
            var currentWallpaper = WallpaperProcess.GetAuthorURLFromSavedFile();
            System.Diagnostics.Process.Start(string.Format("{0}{1}", currentWallpaper, Config.UnsplashReferral));
        }

        /// <summary>
        /// Fires when a user has clicked the open on startup checkbox
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event</param>
        private void StartupItem_Click(object sender, EventArgs e)
        {
            var senderMenuItem = (ToolStripMenuItem)sender;
            senderMenuItem.Checked = !senderMenuItem.Checked;
            Config.AutomaticStartup = senderMenuItem.Checked;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// The override dispose method
        /// </summary>
        /// <param name="disposing">Disposing flag</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) { components.Dispose(); }
        }

        #endregion
    }
}
