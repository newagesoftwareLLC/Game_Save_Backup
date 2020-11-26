using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using System.Runtime.InteropServices;
using System.Xml;
//using System.Xml.Linq;
using System.IO;
using System.Net;
using Ionic.Zip;
using Microsoft.Win32;
using System.Diagnostics;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using Dropbox.Api;
using Dropbox.Api.Files;
using Microsoft.OneDrive.Sdk;
//using Microsoft.OneDrive.Sdk.WindowsForms;
using IWshRuntimeLibrary;
using System.Deployment.Application;
using Microsoft.OneDrive.Sdk.WindowsForms;
using System.Collections.Specialized;

namespace GameSaveBackup
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr w, IntPtr l);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        //regedit
        [DllImport("advapi32.dll", EntryPoint = "RegOpenKey")]
        public static extern
        int RegOpenKeyA(int hKey, string lpSubKey, ref int phkResult);

        [DllImport("advapi32.dll")]
        public static extern
           int RegCloseKey(int hKey);

        [DllImport("advapi32.dll", EntryPoint = "RegQueryInfoKey")]
        public static extern
           int RegQueryInfoKeyA(int hKey, string lpClass,
           ref int lpcbClass, int lpReserved,
           ref int lpcSubKeys, ref int lpcbMaxSubKeyLen,
           ref int lpcbMaxClassLen, ref int lpcValues,
           ref int lpcbMaxValueNameLen, ref int lpcbMaxValueLen,
           ref int lpcbSecurityDescriptor,
           ref FILETIME lpftLastWriteTime);

        [DllImport("advapi32.dll", EntryPoint = "RegEnumValue")]
        public static extern
           int RegEnumValueA(int hKey, int dwIndex,
           ref byte lpValueName, ref int lpcbValueName,
           int lpReserved, ref int lpType, ref byte lpData, ref int lpcbData);

        [DllImport("advapi32.dll", EntryPoint = "RegEnumKeyEx")]
        public static extern
           int RegEnumKeyExA(int hKey, int dwIndex,
           ref byte lpName, ref int lpcbName, int lpReserved,
           string lpClass, ref int lpcbClass, ref FILETIME lpftLastWriteTime);

        [DllImport("advapi32.dll", EntryPoint = "RegSetValueEx")]
        public static extern
           int RegSetValueExA(int hKey, string lpSubKey,
           int reserved, int dwType, ref byte lpData, int cbData);

        string gDriveDirID;
        string xmlURL = "https://www.dropbox.com/s/cnjd2exggnvd2kb/steam_saves.xml?dl=1";//"http://newagesoldier.com/myfiles/steam_saves.xml";
        string gogXMLurl = "https://www.dropbox.com/s/4rtz5o2byosikjt/gog_saves.xml?dl=1";
        Dictionary <string,string> installedGames = new Dictionary<string,string>();
        List <string> steamPaths = new List<string>();
        //string gogPath = ""; //GOG only has 1 path (before v12)
        List<string> gogPaths = new List<string>();
        string steamID = "";
        Dictionary<string, string> cloudSaves = new Dictionary<string, string>(); //KEY:CloudFileID VALUE:CloudFileName | OneDrive, GoogleDrive and DropBox all share this
        Dictionary<string, string> steamcloudSaveProcs = new Dictionary<string, string>(); //KEY:GameDir VALUE:ExeName (steam only)
        Dictionary<string, string> gogCloudSaveProcs = new Dictionary<string, string>(); //same as above, for GOG only
        string detectedGame;
        string detectedPlatform;
        bool setupCompleted = false;

        public static string appDir = Environment.ExpandEnvironmentVariables(@"%LOCALAPPDATA%\GameSaveBackup\");
        public static string xmlFile = appDir + @"\steam_saves.xml";
        public static string GOGxmlFile = appDir + @"\gog_saves.xml";
        private const int WM_VSCROLL = 277;
        private const int SB_PAGEBOTTOM = 7;

        int globalCount = 0;

        DriveService bgService;
        private string oauth2State;
        private const string RedirectUri = "http://localhost/authorize"; //for GoogleDrive and DropBox

        static Assembly a = Assembly.Load("GameSaveBackup");
        ResourceManager rm = new ResourceManager("GameSaveBackup.Lang." + Properties.Settings.Default.language, a);

        private void googleDriveLogin(string appKey)
        {
            webBrowser1.Invoke(new MethodInvoker(delegate
            {
                webBrowser1.Visible = true;
                string[] scopes = new string[] { DriveService.Scope.Drive,  // view and manage your files and documents
                                             DriveService.Scope.DriveAppdata,  // view and manage its own configuration data
                                             DriveService.Scope.DriveAppsReadonly,   // view your drive apps
                                             DriveService.Scope.DriveFile,   // view and manage files created by this app
                                             DriveService.Scope.DriveMetadataReadonly,   // view metadata for files
                                             DriveService.Scope.DriveReadonly,   // view files and documents on your drive
                                             DriveService.Scope.DriveScripts };  // modify your app scripts 
                string theScope = String.Join("+", scopes);
                webBrowser1.Navigate(String.Format("https://accounts.google.com/o/oauth2/auth?response_type=code&scope=" + theScope + "&redirect_uri={0}&client_id={1}&en=us&from_login=1&as=d832bdaf61552d&pli=1&authuser=0", RedirectUri, appKey));
            }));
        }

        private void dropBoxLogin(string appKey)
        {
            this.oauth2State = Guid.NewGuid().ToString("N");
            var authorizeUri = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Token, appKey, new Uri(RedirectUri), state: oauth2State);
            //System.Diagnostics.Process.Start(authorizeUri.AbsoluteUri);
            webBrowser1.Invoke(new MethodInvoker(delegate
            {
                webBrowser1.Visible = true;
                webBrowser1.Navigate(authorizeUri);
            }));
        }

        public static void SetAutoStart()
        {
            //RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
            //key.SetValue("SteamCloudSave", "\"" + Application.ExecutablePath.ToString() + "\"");
            try
            {
                string shortcutLocation = System.IO.Path.Combine(Environment.ExpandEnvironmentVariables(@"%APPDATA%\Microsoft\Windows\Start Menu\Programs\Startup"), "GameSaveBackup" + ".lnk");
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);
                shortcut.Description = "GameSaveBackup startup";               // The description of the shortcut
                shortcut.IconLocation = Application.StartupPath + @"\cloud3.ico";                       // The icon of the shortcut
                shortcut.TargetPath = Application.ExecutablePath;    // The path of the file that will launch when the shortcut is run
                shortcut.Save();
            }
            catch
            {
                MessageBox.Show(@"ERROR: Can't find %appdata%\Microsoft\Windows\Start Menu\Programs\Startup");
            }
        }

        private IOneDriveClient oneDriveClient { get; set; }
        private static readonly string[] oneDriveScopes = { "wl.signin", "wl.offline_access", "onedrive.readwrite" };
        private static readonly string[] Scopes = { "onedrive.readwrite", "wl.offline_access", "wl.signin" };

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.winStartup == true)
                SetAutoStart(); //make sure this is set and up to date
            localGameList.LargeImageList = imageList1;
            //This is for ClickOnce to put the correct uninstall icon
            /*string Install_Reg_Loc = @"Software\Microsoft\Windows\CurrentVersion\Uninstall";
            string displayIcon = Application.StartupPath + @"\cloud3.ico";
            RegistryKey hKey = (Registry.LocalMachine).OpenSubKey(Install_Reg_Loc, true);
            RegistryKey appKey = hKey.OpenSubKey("Game Save Backup");
            appKey.SetValue("DisplayIcon", (object)displayIcon, RegistryValueKind.String);*/
        }

        private static string FormatResourceName(Assembly assembly, string resourceName)
        {
            return assembly.GetName().Name + "." + resourceName.Replace(" ", "_")
                                                               .Replace("\\", ".")
                                                               .Replace("/", ".");
        }

        private Bitmap LoadPicture(string url)
        {
            HttpWebRequest wreq;
            HttpWebResponse wresp;
            Stream mystream;
            Bitmap bmp;

            bmp = null;
            mystream = null;
            wresp = null;
            try
            {
                wreq = (HttpWebRequest)WebRequest.Create(url);
                wreq.AllowWriteStreamBuffering = true;
                wresp = (HttpWebResponse)wreq.GetResponse();
                if ((mystream = wresp.GetResponseStream()) != null)
                    bmp = new Bitmap(mystream);
            }
            catch
            {
                bmp = new Bitmap(Properties.Resources.header);
            }
            finally
            {
                if (mystream != null)
                    mystream.Close();

                if (wresp != null)
                    wresp.Close();
            }
            return (bmp);
        }

        private string getVal(string s)
        {
            s = Regex.Replace(s, @"\s+", "");
            string[] words = s.Split('"');
            return words[3].Replace(@"\\", @"\");
        }

        private string getVal2(string s)
        {
            //s = Regex.Replace(s, @"\s+", "");
            string[] words = s.Split(':');
            return words[4].Replace(@"] )", @"");
            //return words[0];
        }

        public string[] WriteSafeReadAllLines(String path)
        {
            using (var csv = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(csv))
            {
                List<string> file = new List<string>();
                while (!sr.EndOfStream)
                {
                    file.Add(sr.ReadLine());
                }

                return file.ToArray();
            }
        }

        public string getSteamInstallPath()
        {
            RegistryKey regKey = Registry.CurrentUser;
            regKey = regKey.OpenSubKey(@"Software\Valve\Steam");

            if (regKey != null)
                return regKey.GetValue("SteamPath").ToString();
            else
                return null;
        }

        /*public string getGOGInstallPath()
        {
            RegistryKey regKey = Registry.LocalMachine;
            regKey = regKey.OpenSubKey(@"SOFTWARE\WOW6432NODE\GOG.com");

            if (regKey != null)
                return regKey.GetValue("DefaultPackPath").ToString();
            else
                return null;
        }

        public string getGOGGalaxyPath()
        {
            RegistryKey regKey = Registry.LocalMachine;
            regKey = regKey.OpenSubKey(@"SOFTWARE\WOW6432NODE\GOG.com\GalaxyClient\paths");

            if (regKey != null)
            {
                if (Directory.Exists(regKey.GetValue("client").ToString()))
                    return regKey.GetValue("client").ToString();
                else
                    return null;
            }
            else
                return null;
        }*/

        public Bitmap findBannerArt(string gameID)
        {
            string findURL = Environment.ExpandEnvironmentVariables(@"%ALLUSERSPROFILE%\GOG.com\Galaxy\webcache\") + gameID;
            if (!Directory.Exists(findURL))
                return null;

            DirectoryInfo d = new DirectoryInfo(findURL);

            int i = 1;
            foreach (var file in d.GetFiles("*.png").Union(d.GetFiles("*.jpg")).ToArray())
            {
                if (i == 5) {
                    string myPath = Path.Combine(findURL, file.FullName);
                    using (Stream BitmapStream = System.IO.File.Open(myPath, System.IO.FileMode.Open))
                    {
                        System.Drawing.Image img = System.Drawing.Image.FromStream(BitmapStream);
                        return new Bitmap(img);
                    }
                }
                i++;
            }
            return null;
        }

        /*public string getGOGGamePaths()
        {
            RegistryKey regKey = Registry.LocalMachine;
            regKey = regKey.OpenSubKey(@"SOFTWARE\WOW6432NODE\GOG.com\Games");

            if (regKey != null)
            {
                foreach (string subKeyName in regKey.GetSubKeyNames())
                {
                    // Open the key.
                    RegistryKey subKey = regKey.OpenSubKey(subKeyName);

                    if (gogCloudSaveProcs.ContainsKey(Path.GetFileName(subKey.GetValue("PATH").ToString())))
                        continue;

                    //I only want this for DOSBOX games. The save paths are bad for every other game.
                    if (!subKey.GetValue("EXEFILE").ToString().Replace(".exe", "").Contains("dosbox"))
                        continue;

                    //MessageBox.Show("[DEBUG] getting game... " + Path.GetFileName(subKey.GetValue("PATH").ToString()));
                    string myGameFolder = Path.GetFileName(subKey.GetValue("PATH").ToString());
                    string myProcName = subKey.GetValue("EXEFILE").ToString().Replace(".exe", "");
                    AppendOutputText(myGameFolder + " " + rm.GetString("installed"));
                    installedGames.Add(myGameFolder, "gog");
                    gogCloudSaveProcs.Add(myGameFolder, myProcName);
                    ListViewItem lst = new ListViewItem();
                    lst.Tag = Path.GetFileName(subKey.GetValue("PATH").ToString());
                    lst.Text = subKey.GetValue("GAMENAME").ToString();
                    lst.Name = "gog";
                    lst.ImageIndex = globalCount;

                    localGameList.Invoke(new MethodInvoker(delegate
                    {
                        Bitmap test = findBannerArt(subKey.GetValue("gameID").ToString());
                        if (test == null)
                            test = LoadPicture(null);
                        imageList1.Images.Add(test);
                        localGameList.Items.Add(lst);
                    }));
                    globalCount++;

                }
            }
            return null;
        }*/

        public string getGamePath(string gameDir)
        { //find our dynamic %STEAM% path
            foreach (string sp in steamPaths)
            {
                if (Directory.Exists(sp + @"\" + gameDir))
                    return sp;
            }
            return null; //found nothing.
        }

        public string getGOGGamePath(string gameDir)
        { //find our dynamic %GOG% path
            foreach (string gog in gogPaths)
            {
                if (Directory.Exists(gog + gameDir))
                    return gog;
            }
            return null; //found nothing.
        }

        string tag;
        string gameDir2;
        string sp2;
        string gog2;
        string procName;
        public void checkLocalGames()
        {
            if (xmlFile.Length <= 0)
            {
                MessageBox.Show("ERROR: XML file is 0 bytes!");
                return;
            }
            //parse the Steam XML file
            XmlTextReader reader = new XmlTextReader(xmlFile);
            if (reader.IsStartElement())
            {
                while (reader.Read())
                {
                    switch (reader.Name)
                    {
                        case "game":
                            tag = reader.GetAttribute("name");
                            break;

                        case "dirname":
                            string gameDir = reader.ReadString();
                            procName = gameDir; //for our proc list below
                            foreach (string sp in steamPaths)
                            {
                                if (Directory.Exists(sp + gameDir))
                                {
                                    AppendOutputText("[Steam] " + gameDir + " " + rm.GetString("installed"));
                                    try
                                    {
                                        installedGames.Add(gameDir, "steam");
                                    } catch { }
                                    gameDir2 = gameDir; //pass this down to appid
                                    sp2 = sp;
                                }
                            }
                            break;

                        case "exe":
                            if (procName != null)
                            {
                                try
                                {
                                    //AppendOutputText("[DEBUG] Adding cloudSaveProcs from dir:" + procName + " proc:" + reader.ReadString());
                                    steamcloudSaveProcs.Add(procName, reader.ReadString());
                                }
                                catch
                                {
                                    AppendOutputText("ERROR: Steam XML element reader crashed on exe parse. (dir:" + procName + " exe:" + reader.ReadString());
                                }
                            }
                            break;

                        case "appid":
                            string appID = reader.ReadString();

                            if (appID == "")
                                break;

                            if (gameDir2 != "" && Directory.Exists(sp2 + gameDir2))
                            {
                                ListViewItem lst = new ListViewItem();
                                //AppendOutputText("[DEBUG] Banner path = " + "http://cdn.akamai.steamstatic.com/steam/apps/" + appID + "/header.jpg");
                                //AppendOutputText("[DEBUG] gamedir2:" + gameDir2 + " tag:" + tag + " count:" + count);
                                lst.Tag = gameDir2;
                                lst.Text = gameDir2;
                                lst.Name = "steam";
                                lst.ImageIndex = globalCount;
                                localGameList.Invoke(new MethodInvoker(delegate {
                                    string url = "http://cdn.akamai.steamstatic.com/steam/apps/" + appID + "/header.jpg";
                                    Bitmap bmp = LoadPicture(url);
                                    imageList1.Images.Add(bmp);
                                    localGameList.Items.Add(lst); 
                                }));
                                globalCount++;
                            }
                            sp2 = "";
                            gameDir2 = "";
                            break;
                    }
                }
            }

            //parse the GOG XML file
            XmlTextReader reader2 = new XmlTextReader(GOGxmlFile);
            if (reader2.IsStartElement())
            {
                while (reader2.Read())
                {
                    switch (reader2.Name)
                    {
                        case "game":
                            tag = reader2.GetAttribute("name");
                            break;

                        case "dirname":
                            string gameDir = reader2.ReadString();
                            //AppendOutputText("[DEBUG] gameDir=" + gameDir + " gogpath=" + gogPath + @"\" + gameDir);
                            procName = gameDir; //for our proc list below
                            foreach (string gog in gogPaths)
                            {
                                if (Directory.Exists(gog + gameDir))
                                {
                                    AppendOutputText("[GOG] " + gameDir + " " + rm.GetString("installed"));
                                    installedGames.Add(gameDir, "gog");
                                    gameDir2 = gameDir; //pass this down to appid
                                    gog2 = gog;
                                } //else
                                    //Debug.WriteLine("GOG Directory Doesnt Exist: " + gog + gameDir);
                            }
                            break;

                        case "exe":
                            if (procName != null)
                            {
                                try
                                {
                                    //AppendOutputText("[DEBUG] Adding cloudSaveProcs from dir:" + procName + " proc:" + reader2.ReadString());
                                    //MessageBox.Show("[DEBUG] key:" + procName + " value:" + reader2.ReadString());
                                    gogCloudSaveProcs.Add(procName, reader2.ReadString());
                                }
                                catch
                                {
                                    AppendOutputText("ERROR: GOG XML element reader crashed on exe parse. (dir:" + procName + " exe:" + reader2.ReadString());
                                }
                            }
                            break;

                        case "appid":
                            string appID = reader2.ReadString();

                            if (appID == "")
                                break;

                            if (gameDir2 != "" && Directory.Exists(gog2 + @"\" + gameDir2))
                            {
                                ListViewItem lst = new ListViewItem();
                                //AppendOutputText("[DEBUG] Banner path = " + "https://images-2.gog.com/" + appID + ".jpg");
                                //AppendOutputText("[DEBUG] gamedir2:" + gameDir2 + " tag:" + tag + " count:" + count);
                                lst.Tag = gameDir2;
                                //AppendOutputText("[DEBUG] TAG=" + gameDir2);
                                lst.Text = tag;
                                lst.Name = "gog";
                                lst.ImageIndex = globalCount;
                                localGameList.Invoke(new MethodInvoker(delegate { imageList1.Images.Add(LoadPicture("https://images-2.gog.com/" + appID + "_196.jpg")); localGameList.Items.Add(lst); }));
                                globalCount++;
                            }
                            gog2 = "";
                            gameDir2 = "";
                            break;
                    }
                }
            }

            // done getting local games
        }

        public List<string> getSaveDirs(string gameDir, string platform, string proc) //get a list of our save directories
        {
            List<string> listRange = new List<string>();
            XmlDocument doc = new XmlDocument();

            //parse steam saves
            if (platform == "steam")
            {
                doc.LoadXml(System.IO.File.ReadAllText(xmlFile));
                XmlNodeList nodes = doc.SelectNodes("/backups/game[dirname=\"" + gameDir + "\"]/saves/save");
                foreach (XmlNode node in nodes)
                {
                    listRange.Add(node.InnerText);
                }
            }

            //parse gog saves
            if (platform == "gog")
            {
                if (proc == "dosbox") //special dosbox save
                    listRange.Add(@"%GOG%\*.SAV"); 
                else
                {
                    doc.LoadXml(System.IO.File.ReadAllText(GOGxmlFile));
                    XmlNodeList nodes2 = doc.SelectNodes("/backups/game[dirname=\"" + gameDir + "\"]/saves/save");
                    foreach (XmlNode node in nodes2)
                    {
                        listRange.Add(node.InnerText);
                    }
                }
            }

            return listRange;
        }

        public void GoogleDriveSyncFiles(DriveService service)
        {
            string Q = "title = 'GameBackup' and mimeType = 'application/vnd.google-apps.folder'";
            IList<Google.Apis.Drive.v2.Data.File> _Files = GoogleDrive.GetFiles(service, Q);
            if (_Files.Any())
            {
                Debug.WriteLine("GoogleDriveSyncFiles starting...");
                gDriveDirID = _Files[0].Id;
                setGoogleDriveSaves(service, _Files[0].Id); //build our CloudSaves dictionary
                foreach (var item in installedGames)
                {
                    //Debug.WriteLine("DEBUG: Checking CloudSave array " + string.Join(",", cloudSaves.Values) + " for " + item.Key);
                    if (!cloudSaves.Values.Contains(item.Key))
                    {
                        AppendOutputText(rm.GetString("creatingSaveFolder") + " " + item.Key + " " + rm.GetString("onGoogleDrive"));
                        Google.Apis.Drive.v2.Data.File newID = GoogleDrive.createDirectory(service, item.Key, item.Key, gDriveDirID);
                        cloudSaves.Add(newID.Id, item.Key); // add this to our dictionary of cloud save tokens after making a new directory
                    }
                }
            } else
                AppendOutputText("Google Drive Error: no files found", Color.Red);

            AppendOutputText(rm.GetString("gDataLoadComplete"), Color.DarkGreen);
            refreshGames_btn.Invoke(new MethodInvoker(delegate
            {
                refreshGames_btn.Enabled = true;
            }));
            localGameList.Invoke(new MethodInvoker(delegate
            {
                localGameList.Cursor = Cursors.Default;
            }));
        }

        public async Task FTPSyncFiles()
        {
            ftp ftpClient = new ftp(Properties.Settings.Default.ftpIP, Properties.Settings.Default.ftpUser, Properties.Settings.Default.ftpPass);
            bool reCheck = false;

            await getFTPgames(); //get a list of games currently backed up first

            foreach (var item in installedGames)
            {
                if (cloudSaves.Values.Contains(item.Key))
                {
                    //AppendOutputText("[DEBUG] " + ig + " " + rm.GetString("currentlyBackedUp") + " on OneDrive", Color.Gold);
                    continue;
                }
                else
                {
                    AppendOutputText(rm.GetString("creatingSaveFolder") + " " + item.Key + " on FTP");
                    ftpClient.createDirectory(Properties.Settings.Default.ftpDir + item.Key);
                    reCheck = true;
                }
            }

            //if (reCheck) //update cloud saves if we created new directories
            //    await checkOneDriveSaves();

            AppendOutputText(rm.GetString("gDataLoadComplete"), Color.LimeGreen);
            refreshGames_btn.Invoke(new MethodInvoker(delegate
            {
                refreshGames_btn.Enabled = true;
            }));
            localGameList.Invoke(new MethodInvoker(delegate
            {
                localGameList.Cursor = Cursors.Default;
            }));
        }

        public async Task OneDriveSyncFiles()
        {
            await checkOneDriveSaves();
            bool reCheck = false;
            AppendOutputText("OneDrive GameBackup ID: " + oneDriveSteamBackupFolderID);

            foreach (var item in installedGames)
            {
                if (cloudSaves.Values.Contains(item.Key))
                {
                    //AppendOutputText("[DEBUG] " + ig + " " + rm.GetString("currentlyBackedUp") + " on OneDrive", Color.Gold);
                    continue;
                }
                else
                {
                    AppendOutputText(rm.GetString("creatingSaveFolder") + " " + item.Key + " on OneDrive");
                    await createDir(item.Key, oneDriveSteamBackupFolderID);
                    reCheck = true;
                }
            }

            if (reCheck) //update cloud saves if we created new directories
                await checkOneDriveSaves();

            AppendOutputText(rm.GetString("gDataLoadComplete"), Color.LimeGreen);
            refreshGames_btn.Invoke(new MethodInvoker(delegate
            {
                refreshGames_btn.Enabled = true;
            }));
            localGameList.Invoke(new MethodInvoker(delegate
            {
                localGameList.Cursor = Cursors.Default;
            }));
        }

        public async void DropBoxSyncFiles(DropboxClient service)
        {
            var list = await service.Files.ListFolderAsync(string.Empty);
            await checkDropBoxSaves(service);
            bool reCheck = false;

            foreach (var item in installedGames)
            {
                if (cloudSaves.Values.Contains(item.Key))
                {
                    //AppendOutputText("[DEBUG] " + ig + " " + rm.GetString("currentlyBackedUp") + " on DropBox", Color.Gold);
                    continue;
                }
                else
                {
                    AppendOutputText(rm.GetString("creatingSaveFolder") + " " + item.Key + " on DropBox");
                    await service.Files.CreateFolderAsync(@"/" + item.Key);
                    reCheck = true;
                }
            }

            if (reCheck) //update cloud saves if we created new directories
                setDropBoxSaves(service);

            AppendOutputText(rm.GetString("gDataLoadComplete"), Color.LimeGreen);
            refreshGames_btn.Invoke(new MethodInvoker(delegate
            {
                refreshGames_btn.Enabled = true;
            }));
            localGameList.Invoke(new MethodInvoker(delegate
            {
                localGameList.Cursor = Cursors.Default;
            }));
        }

        public void GoogleDriveFileList(DriveService service, String folderId)
        { //this is slow. We need to speed it up.
            if (service == null)
                AppendOutputText(rm.GetString("errServiceNull"), Color.Red);
            ChildrenResource.ListRequest request = service.Children.List(folderId);
            foreach (var item in installedGames)
            {
                Debug.WriteLine("adding " + item.Key);
                request.Q = "mimeType='application/vnd.google-apps.folder' and title='" + item.Key + "' ";
                do
                {
                    //try
                    //{
                        ChildList children = request.Execute();

                        //foreach (ChildReference child in children.Items)
                        //{
                        Google.Apis.Drive.v2.Data.File file = service.Files.Get(children.Items[0].Id).Execute();
                        AppendOutputText("[" + file.Id + "] " + file.Title + " " + rm.GetString("currentlyBackedUp") + " " + rm.GetString("onGoogleDrive"), Color.Gold);
                        cloudSaves.Add(file.Id, file.Title);
                        request.PageToken = children.NextPageToken;
                        //}
                    /*}
                    catch (Exception e)
                    {
                        //AppendOutputText("ERROR (checkCloudSaves): " + e.Message, Color.Red); //this error is usually index out of range. Let it go silenced because we need to make the index ID later.
                        request.PageToken = null;
                    }*/
                } while (!String.IsNullOrEmpty(request.PageToken));
            }
        }


        public void checkGoogleDriveSaves(DriveService service, String folderId)
        {
            GoogleDriveFileList(service, folderId);
        }

        public async Task checkOneDriveSaves()
        {
            cloudSaves.Clear(); //if we recheck, let's clear that list first.

            string path = "GameBackup";
            if (null == this.oneDriveClient) return;
            LoadChildren(new Item[0]);

            try
            {
                Item folder;
                var expandValue = oneDriveClient.ClientType == ClientType.Consumer ? "children" : "children";
                folder = await this.oneDriveClient.Drive.Root.ItemWithPath("/" + path).Request().Expand(expandValue).GetAsync();
                ProcessFolder(folder);
            }
            catch (Exception e)
            {
                PresentOneDriveException(e);
            }
        }

        private void ProcessFolder3(Item folder)
        {
            if (folder != null)
            {
                if (folder.Folder != null && folder.Children != null && folder.Children.CurrentPage != null)
                    LoadChildren3(folder.Children.CurrentPage);
            }
        }

        //List<string> list = new List<string>();
        private void LoadChildren3(IList<Item> items)
        {
            int t = 1;
            foreach (var obj in items)
            {
                if (t > Properties.Settings.Default.maxBackups && Properties.Settings.Default.maxBackups != 0) //delete items when we hit max
                {
                    if (checkPatreon() == false && Properties.Settings.Default.maxBackups > 5)
                    {
                        AppendOutputText("ERROR: Maximum backups is greater than 5 and your not a Patron!", Color.Red);
                        return;
                    }
                    else if (checkPatreon() == true)
                    {
                        AppendOutputText("Patreon status is good!", Color.Green);
                    }
                    AppendOutputText(rm.GetString("hitMax") + " " + obj.Name, Color.Gold);
                    this.oneDriveClient.Drive.Items[obj.Id].Request().DeleteAsync();
                }
                else
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = obj.Name.Replace(".zip", "");
                    item.Tag = obj.Id;
                    cloudFiles.Invoke(new MethodInvoker(delegate
                    {
                        cloudFiles.Items.Add(item);
                    }));
                }
                t++;
            }
        }
        public async Task getOneDriveZips(string dir)
        {
            if (null == this.oneDriveClient) return;
            LoadChildren3(new Item[0]);

            try
            {
                Item folder;
                var expandValue = this.oneDriveClient.ClientType == ClientType.Consumer ? "children" : "children";
                folder = await this.oneDriveClient.Drive.Root.ItemWithPath("/GameBackup/" + dir).Request().Expand(expandValue).GetAsync();
                ProcessFolder3(folder);
            }
            catch (Exception e)
            {
                PresentOneDriveException(e);
            }
        }

        public async Task checkDropBoxSaves(DropboxClient service)
        {
            if (service == null)
                AppendOutputText(rm.GetString("errServiceNull"), Color.Red);

            try
            {
                var list = await service.Files.ListFolderAsync(string.Empty);
                foreach (var item in list.Entries.Where(i => i.IsFolder)) //IsFile
                {
                    //AppendOutputText("[DEBUG] " + item.Name + " " + rm.GetString("currentlyBackedUp") + " on DropBox", Color.Gold);
                    cloudSaves.Add(item.Name, item.Name);
                }
            }
            catch (Exception e)
            {
                AppendOutputText("ERROR (checkCloudSaves): " + e.Message, Color.Red);
            }
        }

        public void setGoogleDriveSaves(DriveService service, String folderId)
        {
            cloudSaves.Clear();

            if (service == null)
                AppendOutputText(rm.GetString("errServiceNull"), Color.Red);
            //AppendOutputText("[DEBUG] checkCloudSaves is triggered with folderID:" + folderId, Color.Yellow);

            ChildrenResource.ListRequest request = service.Children.List(folderId);

            do
            {
                try
                {
                    ChildList children = request.Execute();

                    foreach (ChildReference child in children.Items)
                    {
                        Google.Apis.Drive.v2.Data.File file = service.Files.Get(child.Id).Execute();
                        cloudSaves.Add(file.Id, file.Title);
                        AppendOutputText("Google Drive Getting Token For " + file.Title, Color.Black);
                    }
                    request.PageToken = children.NextPageToken;
                }
                catch (Exception e)
                {
                    AppendOutputText("ERROR (setCloudSaves): " + e.Message, Color.Red);
                    request.PageToken = null;
                }
            } while (!String.IsNullOrEmpty(request.PageToken));
            Debug.WriteLine("setGoogleDriveSaves finished...");
        }

        public async void setDropBoxSaves(DropboxClient service)
        {
            cloudSaves.Clear();

            if (service == null)
                AppendOutputText(rm.GetString("errServiceNull"), Color.Red);

            var list = await service.Files.ListFolderAsync(string.Empty);
            foreach (var item in list.Entries.Where(i => i.IsFolder)) //IsFile
            {
                cloudSaves.Add(item.Name, item.Name); //TEST
            }
        }

        void debugLog(string text)
        {
            string debugFile = appDir + @"\DEBUG.txt";
            if (Properties.Settings.Default.debugExport)
            {
                if (!System.IO.File.Exists(debugFile))
                {
                    var myFile = System.IO.File.Create(debugFile);
                    myFile.Close();
                }
                System.IO.File.AppendAllText(debugFile, DateTime.Now + " " + text + Environment.NewLine);
            }
        }

        public void AppendOutputText(string text, Color color = default(Color))
        {
            try
            {
                outBox.Invoke(new MethodInvoker(delegate
                {
                    outBox.SelectionStart = outBox.TextLength;
                    outBox.SelectionLength = 0;

                    outBox.SelectionColor = color;
                    outBox.AppendText(DateTime.Now + " " + text + Environment.NewLine);
                    string debugFile = System.Environment.CurrentDirectory + @"\DEBUG.txt";
                    debugLog(text);
                    ScrollToBottom(outBox);
                }));
            }
            catch
            {
                MessageBox.Show("ERROR: AppendOutputText crashed!");
                debugLog("ERROR: AppendOutputText crashed!");
            }
        }
        public static void ScrollToBottom(RichTextBox MyRichTextBox)
        {
            SendMessage(MyRichTextBox.Handle, WM_VSCROLL, (IntPtr)SB_PAGEBOTTOM, IntPtr.Zero);
        }

        private async Task Download(DropboxClient client, string folder, string file)
        {
            byte[] arrBytes;
            AppendOutputText("Download file... " + folder + "/" + file);
            using (var response = await client.Files.DownloadAsync(folder + "/" + file))
            {
                AppendOutputText("Downloading (" + response.Response.Name + ")");

                arrBytes = await response.GetContentAsByteArrayAsync();
            }
            System.IO.File.WriteAllBytes(appDir + "UNZIP.zip", arrBytes);
        }

        private string host = null;
        private string user = null;
        private string pass = null;
        private FtpWebRequest ftpRequest = null;
        private FtpWebResponse ftpResponse = null;
        private Stream ftpStream = null;
        private int bufferSize = 2048;

        private async Task downloadFromCloud(string id, string game)
        {
            string zipFile = appDir + "UNZIP.zip";

            if (Properties.Settings.Default.cloudSource == "googledrive")
            {
                DriveService service = GoogleDrive.gDriveSetup();
                Google.Apis.Drive.v2.Data.File file = service.Files.Get(id).Execute();
                AppendOutputText("Downloading GoogleDrive file " + file.Title);
                GoogleDrive.downloadFile(service, file, zipFile);
            }
            if (Properties.Settings.Default.cloudSource == "dropbox")
            {
                string dlFile = @"/" + localGameList.SelectedItems[0].Tag.ToString() + @"/" + id;
                AppendOutputText("dropbox download ID: " + dlFile);
                DropboxClient dbxClient = new DropboxClient(AccessToken);
                await Download(dbxClient, @"/" + localGameList.SelectedItems[0].Tag.ToString(), id);
            }
            if (Properties.Settings.Default.cloudSource == "onedrive")
            {
                AppendOutputText("onedrive download ID: " + id);
                using (var stream = await oneDriveClient.Drive.Items[id].Content.Request().GetAsync())
                using (var outputStream = new System.IO.FileStream(zipFile, System.IO.FileMode.Create))
                {
                    await stream.CopyToAsync(outputStream);
                }
            }
            if (Properties.Settings.Default.cloudSource == "ftp")
            {
                AppendOutputText("Downloading Remote File: " + Properties.Settings.Default.ftpDir + localGameList.SelectedItems[0].Tag.ToString() + "/" + id);
                ftp ftpClient = new ftp(Properties.Settings.Default.ftpIP, Properties.Settings.Default.ftpUser, Properties.Settings.Default.ftpPass);
                await ftpClient.download(Properties.Settings.Default.ftpDir + localGameList.SelectedItems[0].Tag.ToString() + "/" + id, zipFile);
            }
            if (Properties.Settings.Default.cloudSource == "local")
            {
                AppendOutputText("Copying File: " + Properties.Settings.Default.ftpDir + localGameList.SelectedItems[0].Tag.ToString() + "/" + id);
                System.IO.File.Copy(Properties.Settings.Default.moveDir + localGameList.SelectedItems[0].Tag.ToString() + "/" + id, zipFile);
            }
            //unzip folders/files and translate dynamic paths to static paths.
            using (var zip = ZipFile.Read(zipFile))
            {
                zip.ToList().ForEach(filez =>
                {
                    if (getGamePath(game) == null)
                    {
                        AppendOutputText("ERROR: Steam game not installed, cannot restore files.", Color.Red);
                        return;
                    }

                    try
                    {
                        if (filez.FileName.Equals("EXPORT.reg"))
                        {
                            AppendOutputText(rm.GetString("restoreRegKeys"));
                            filez.Extract(appDir, ExtractExistingFileAction.OverwriteSilently);

                            xmlReg.loadAsXml(xmlReader, appDir + "EXPORT.reg"); //restore registry

                            AppendOutputText(rm.GetString("regKeysRestored"), Color.LimeGreen);
                            return;
                        }
                    }
                    catch
                    {
                        AppendOutputText("Error restoring registry file.", Color.Red);
                    }

                    try
                    {
                        if (filez.FileName.Contains("%USERPROFILE%") && Directory.Exists(Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\OneDrive\Documents"))) //windows 10 fix?
                            filez.FileName = filez.FileName.Replace("%USERPROFILE%", @"%USERPROFILE%\OneDrive");

                        filez.FileName = filez.FileName.Replace("%STEAM%", getGamePath(game));
                        filez.FileName = filez.FileName.Replace("%GOG%", getGOGGamePath(game));
                        filez.FileName = filez.FileName.Replace("%STEAMINST%", getSteamInstallPath());
                        //filez.FileName = filez.FileName.Replace("%GOGINST%", getGOGInstallPath()); //no galaxy support right now
                        filez.FileName = filez.FileName.Replace("%USERID%", steamID);

                        filez.FileName = Environment.ExpandEnvironmentVariables(filez.FileName);

                        filez.Extract(Path.GetPathRoot(Environment.SystemDirectory), ExtractExistingFileAction.OverwriteSilently);
                        AppendOutputText(rm.GetString("restoring") + " " + filez.FileName);
                    }
                    catch
                    {
                        AppendOutputText("Error restoring file " + filez.FileName, Color.Red);
                    }
                });
            }
            try
            {
                System.IO.File.Delete(zipFile);
            }
            catch
            {
                AppendOutputText("Error deleting zip file.", Color.Red);
            }
            AppendOutputText("Restoration process completed!", Color.Green);
        }

        private async void downloadBtn_Click(object sender, EventArgs e)
        {
            if (cloudFiles.SelectedItems.Count < 0)
            {
                AppendOutputText("Select item to download");
                return;
            }

            if (cloudFiles.SelectedItems[0].Tag == null)
            {
                AppendOutputText("Tag is null?");
                return;
            }

            await downloadFromCloud(cloudFiles.SelectedItems[0].Tag.ToString(), localGameList.SelectedItems[0].Tag.ToString());
        }

        private void uploadBtn_Click(object sender, EventArgs e)
        {
            if (localGameList.SelectedItems == null || localGameList.SelectedItems.Count <= 0)
                return;

            if (localGameList.SelectedItems[0].Tag == null)
                return;

            uploadFiles((String)localGameList.SelectedItems[0].Tag, (String)localGameList.SelectedItems[0].Name);
        }

        public string uploadCheck(string savedir, bool oneMoreTime = false)
        {
            if (!System.IO.File.Exists(Environment.ExpandEnvironmentVariables(savedir)) && !Directory.Exists(Environment.ExpandEnvironmentVariables(savedir)))
            {
                if (!savedir.Contains("OneDrive") && savedir.Contains("%USERPROFILE%") && oneMoreTime != true)
                {
                    savedir.Replace("%USERPROFILE%", @"%USERPROFILE%\OneDrive"); //try checking onedrive folder
                    uploadCheck(savedir, true);
                }
                AppendOutputText("ERROR (uploadFiles): " + rm.GetString("missingFiles"), Color.DarkRed);
                return "N/A";
            }
            return savedir;
        }

        public string[] uploadCheck2(string path, string file)
        {
            string[] findFiles;
            path = Environment.ExpandEnvironmentVariables(path);
            findFiles = Directory.GetFiles(path, file, SearchOption.TopDirectoryOnly);
            if (findFiles == null || findFiles.Length == 0)
            {
                if (!path.Contains("OneDrive") && path.Contains("%USERPROFILE%"))
                {
                    path.Replace("%USERPROFILE%", @"%USERPROFILE%\OneDrive"); //try checking onedrive folder
                    uploadCheck2(path, file);
                }
            }
            return findFiles;
        }

        public async void uploadFiles(string gameDir, string platform)
        {
            if (/*localGameList.SelectedItems == null || localGameList.SelectedItems.Count <= 0 ||*/ String.IsNullOrEmpty(gameDir) || String.IsNullOrEmpty(platform))
            {
                Debug.WriteLine("ERROR: uploadFiles gameDir or platform was null.");
                return;
            }

            Debug.WriteLine("uploadFiles function triggered with gameDir:" + gameDir + " platform:" + platform);

            string zipFile = "[" + platform + "] " + DateTime.Now.ToString("MM-dd-yyyy HH-mm-ss") + " " + Environment.MachineName + ".zip";
            string gDir = "";
            if (Properties.Settings.Default.cloudSource == "googledrive")
            {
                gDir = cloudSaves.FirstOrDefault(x => x.Value == gameDir).Key; //our google drive backed up game folder
            }
            //dropbox and onedrive go by name, not ID
            if (Properties.Settings.Default.cloudSource == "dropbox" || Properties.Settings.Default.cloudSource == "onedrive" || Properties.Settings.Default.cloudSource == "ftp" || Properties.Settings.Default.cloudSource == "local")
                gDir = gameDir;

            AppendOutputText(rm.GetString("locatingDir") + " " + gDir);

            // zip it up. Must preserve file hierarchy. Auto detects if file or directory.
            AppendOutputText(rm.GetString("creatingZip") + " " + zipFile);
            using (ZipFile zip = new ZipFile())
            {
                zip.StatusMessageTextWriter = System.Console.Out;
                foreach (string savDir in getSaveDirs(gameDir, /*(String)localGameList.SelectedItems[0].Name*/platform, /*(String)localGameList.SelectedItems[0].Tag*/gameDir))
                {
                    if (savDir.Contains("HKEY"))
                    {
                        ExportRegistry(appDir + "EXPORT.reg", savDir);
                        if (!System.IO.File.Exists(appDir + "EXPORT.reg"))
                        {
                            AppendOutputText("ERROR (uploadFiles): " + rm.GetString("missingFiles"), Color.DarkRed);
                            return;
                        }
                        zip.AddFile(appDir + "EXPORT.reg", ".");
                        break;
                    }
                    //newSaveDir = exact directory, saveDir = dynamic directory
                    string newSaveDir = savDir;
                    newSaveDir = newSaveDir.Replace("%STEAM%", getGamePath(gameDir));
                    newSaveDir = newSaveDir.Replace("%GOG%", getGOGGamePath(gameDir));
                    newSaveDir = newSaveDir.Replace("%USERID%", steamID);
                    newSaveDir = newSaveDir.Replace("%STEAMINST%", getSteamInstallPath());
                    //newSaveDir = newSaveDir.Replace("%GOGINST%", getGOGInstallPath()); //no galaxy support atm

                    AppendOutputText(newSaveDir);

                    string[] findFiles;
                    if (newSaveDir.Contains("*"))
                    {
                        string getPath = Path.GetDirectoryName(newSaveDir);
                        string getFile = Path.GetFileName(newSaveDir);

                        findFiles = uploadCheck2(getPath, getFile); //NEEDS TESTING

                        //findFiles = Directory.GetFiles(getPath, getFile, SearchOption.TopDirectoryOnly);

                        if (findFiles == null || findFiles.Length == 0)
                        {
                            AppendOutputText("ERROR (uploadFiles): " + rm.GetString("missingFiles"), Color.DarkRed);
                            return;
                        }

                        foreach (string sf in findFiles)
                        {
                            AppendOutputText("Found save file " + sf);
                            zip.AddFile(Environment.ExpandEnvironmentVariables(sf), Path.GetDirectoryName(savDir));
                        }
                    }
                    else
                    {
                        newSaveDir = uploadCheck(newSaveDir); //NEEDS TESTING

                        if (newSaveDir.Equals("N/A"))
                            return;

                        if (System.IO.File.GetAttributes(Environment.ExpandEnvironmentVariables(newSaveDir)).HasFlag(FileAttributes.Directory))
                            zip.AddDirectory(Environment.ExpandEnvironmentVariables(newSaveDir), savDir);
                        else
                            zip.AddFile(Environment.ExpandEnvironmentVariables(newSaveDir), Path.GetDirectoryName(savDir));
                    }
                }
                zip.Save(appDir + zipFile);
            }

            await Task.Delay(5000);

            AppendOutputText(rm.GetString("uploadingZip") + " " + appDir + zipFile);
            // upload and delete. Check if upload is successful.
            if (Properties.Settings.Default.cloudSource == "googledrive")
            {
                DriveService service = GoogleDrive.gDriveSetup();
                if (service == null)
                {
                    AppendOutputText(rm.GetString("errServiceNull"), Color.Red);
                    return;
                }
                Google.Apis.Drive.v2.Data.File newFile = GoogleDrive.uploadFile(service, appDir + zipFile, gDir);

                if (newFile == null)
                {
                    //if (newFile.Id == null)
                    //{
                        AppendOutputText(rm.GetString("errorUploading"), Color.Red);
                        return;
                    //}
                }
                AppendOutputText(rm.GetString("upoadSuccess") + " " + newFile.Id);
            }

            if (Properties.Settings.Default.cloudSource == "dropbox")
            {
                using (var mem = new MemoryStream(System.IO.File.ReadAllBytes(appDir + zipFile)))
                {
                    DropboxClient dbxClient = new DropboxClient(AccessToken);
                    //AppendOutputText("[DEBUG] gameDir = " + "/" + gDir);
                    await dbxClient.Files.UploadAsync("/" + gDir + "/" + zipFile, WriteMode.Overwrite.Instance, body: mem);
                    AppendOutputText(rm.GetString("upoadSuccess") + " " + zipFile);
                }
            }

            if (Properties.Settings.Default.cloudSource == "onedrive")
            {
                //AppendOutputText("[DEBUG] OneDrive File Stream = " + appDir + zipFile);
                using (var stream = new System.IO.FileStream(appDir + zipFile, System.IO.FileMode.Open))
                {
                    if (stream != null)
                    {
                        var uploadPath = "/drive/items/root:/GameBackup/" + gDir + "/" + Uri.EscapeUriString(System.IO.Path.GetFileName(zipFile));
                        //AppendOutputText("[DEBUG] OneDrive uploadPath = " + uploadPath);

                        try
                        {
                            var uploadedItem = await this.oneDriveClient.ItemWithPath(uploadPath).Content.Request().PutAsync<Item>(stream);
                            //AddItemToFolderContents(uploadedItem);
                            AppendOutputText("OneDrive File Uploaded with ID: " + uploadedItem.Id, Color.LimeGreen);
                        }
                        catch (Exception exception)
                        {
                            PresentOneDriveException(exception);
                        }
                    }
                }
            }

            if (Properties.Settings.Default.cloudSource == "ftp")
            {
                ftp ftpClient = new ftp(Properties.Settings.Default.ftpIP, Properties.Settings.Default.ftpUser, Properties.Settings.Default.ftpPass);
                //AppendOutputText("[DEBUG] local=" + appDir + zipFile + " remote=" + Properties.Settings.Default.ftpDir + gDir + "/" + zipFile);
                await ftpClient.upload(appDir + zipFile, Properties.Settings.Default.ftpDir + gDir + "/" + zipFile);
                //MessageBox.Show("ftp://" + Properties.Settings.Default.ftpIP + Properties.Settings.Default.ftpDir + gDir + "/" + zipFile);
                
                AppendOutputText(rm.GetString("upoadSuccess") + " " + zipFile);
            }

            if (Properties.Settings.Default.cloudSource == "local")
            {
                AppendOutputText("moving file " + appDir + zipFile + " to " + Properties.Settings.Default.moveDir + zipFile);
                if (!System.IO.Directory.Exists(Properties.Settings.Default.moveDir + gDir))
                    System.IO.Directory.CreateDirectory(Properties.Settings.Default.moveDir + gDir);
                System.IO.File.Move(appDir + zipFile, Properties.Settings.Default.moveDir + gDir + @"\" + zipFile);
                AppendOutputText(rm.GetString("upoadSuccess") + " " + zipFile);
            }

                System.IO.File.Delete(appDir + zipFile);
            if (System.IO.File.Exists(appDir + "EXPORT.reg"))
                System.IO.File.Delete(appDir + "EXPORT.reg");

            //update file list and delete max items
            var item = localGameList.FindItemWithText(gameDir);
            if (localGameList.Items.IndexOf(item) >= 0)
            {
                localGameList.Items[localGameList.Items.IndexOf(item)].Selected = false;
                localGameList.Items[localGameList.Items.IndexOf(item)].Selected = true;
            } else
                downloadReload.PerformClick();

            AppendOutputText("Process completed.", Color.DarkGreen);
        }

        public async Task<string> getLatestBackup(string folderId) //give me the lastest backup so we can restore it upon game launch
        {
            if (Properties.Settings.Default.cloudSource == "googledrive")
            {
                DriveService service = GoogleDrive.gDriveSetup();
                ChildrenResource.ListRequest request = service.Children.List(folderId);
                do
                {
                    try
                    {
                        ChildList children = request.Execute();
                        request.PageToken = children.NextPageToken;
                        return children.Items[0].Id;
                    }
                    catch (Exception e)
                    {
                        //AppendOutputText("ERROR (getLatestBackup): " + e.Message); //this msg will print if there are no files in gdrive
                        request.PageToken = null;
                    }
                } while (!String.IsNullOrEmpty(request.PageToken));
            }
            else if (Properties.Settings.Default.cloudSource == "dropbox")
            {
                DropboxClient dbxClient = new DropboxClient(AccessToken);
                try
                {
                    var list = await dbxClient.Files.ListFolderAsync("/" + folderId);
                    foreach (var dpItem in list.Entries.Where(i => i.IsFile)) //make sure it's a file first
                    {
                        return list.Entries[0].Name;
                    }
                }
                catch
                {

                }
            }
            else if (Properties.Settings.Default.cloudSource == "onedrive")
            {
                Item folder;
                var expandValue = this.oneDriveClient.ClientType == ClientType.Consumer ? "children" : "children";
                folder = await this.oneDriveClient.Drive.Root.ItemWithPath("/GameBackup/" + folderId).Request().Expand(expandValue).GetAsync();
                if (folder != null)
                {
                    if (folder.Folder != null && folder.Children != null && folder.Children.CurrentPage != null)
                    {
                        IList<Item> items = folder.Children.CurrentPage;
                        foreach (var obj in items)
                        {
                            //oneDriveDirs.Add(obj.Id,obj.Name);
                            return obj.Id;
                        }
                    }
                }
            }
            else if (Properties.Settings.Default.cloudSource == "ftp")
            {
                ftp ftpClient = new ftp(Properties.Settings.Default.ftpIP, Properties.Settings.Default.ftpUser, Properties.Settings.Default.ftpPass);
                string[] simpleDirectoryListing = ftpClient.directoryListSimple(Properties.Settings.Default.ftpDir + folderId);
                for (int i = 0; i < simpleDirectoryListing.Count(); i++)
                {
                    if (simpleDirectoryListing[i] == "." || simpleDirectoryListing[i] == ".." || simpleDirectoryListing[i] == "")
                        continue;
                    ListViewItem item = new ListViewItem();
                    return simpleDirectoryListing[i];
                }
                ftpClient = null;
            }
            else if (Properties.Settings.Default.cloudSource == "local")
            {
                if (Directory.Exists(Properties.Settings.Default.moveDir + folderId))
                {
                    var directory = new DirectoryInfo(Properties.Settings.Default.moveDir + folderId);
                    return directory.GetFiles().OrderByDescending(f => f.LastWriteTime).First().Name;
                }
            }
            return "";
        }

        private void localGameList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (localGameList.SelectedItems.Count > 0)
            {
                if (SetupCloudDrives_bgWorker.IsBusy)
                {
                    //this.localGameList.SelectedItems.Clear(); //causes a second trigger
                    MessageBox.Show(rm.GetString("loadingGamesWait"), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                    return;
                }

                try
                {
                    cloudFiles.Clear();
                    uploadBtn.Enabled = true;
                    string gDir = "";

                    //AppendOutputText("[DEBUG] Text:" + (String)localGameList.SelectedItems[0].Text);

                    if (Properties.Settings.Default.cloudSource == "googledrive")
                    {
                        DriveService service = GoogleDrive.gDriveSetup();
                        if (service == null)
                            AppendOutputText("ERROR: GoogleDrive service is null!", Color.Red);
                        else
                            AppendOutputText("GoogleDrive service is good!", Color.LimeGreen);

                        gDir = cloudSaves.FirstOrDefault(x => x.Value == (String)localGameList.SelectedItems[0].Tag).Key;

                        if (String.IsNullOrEmpty(gDir))
                        {
                            AppendOutputText("ERROR: GoogleDrive ID missing!", Color.Red);
                            return;
                        }

                        //AppendOutputText("[DEBUG] TAG=" + (String)localGameList.SelectedItems[0].Tag);
                        AppendOutputText(rm.GetString("getFolderID") + " " + gDir);
                    }
                    if (Properties.Settings.Default.cloudSource == "dropbox" || Properties.Settings.Default.cloudSource == "onedrive" || Properties.Settings.Default.cloudSource == "ftp" || Properties.Settings.Default.cloudSource == "local")
                        gDir = (String)localGameList.SelectedItems[0].Tag;

                    //AppendOutputText("Getting game process name... " + cloudSaveProcs[(String)localGameList.SelectedItems[0].Tag]);
                    if (backgroundWorkerList.IsBusy != true)
                        backgroundWorkerList.RunWorkerAsync(gDir);
                    downloadReload.Enabled = true;

                    uploadBtn.Text = "UPLOAD / BACKUP " + localGameList.SelectedItems[0].Text.ToUpper();
                }
                catch
                {
                    //if the game we're trying to restore doesn't exist, this can crash.
                }
            }
            else {
                uploadBtn.Enabled = false;
                uploadBtn.Text = "UPLOAD / BACKUP";
                downloadReload.Enabled = false;
                downloadBtn.Enabled = false;
                cloudFiles.Clear();
                cloudFiles.SelectedIndices.Clear();
                cloudFiles.SelectedItems.Clear();
            }
        }

        private void cloudFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cloudFiles.SelectedItems.Count > 0)
            {
                downloadBtn.Enabled = true;
                if (Properties.Settings.Default.cloudSource == "googledrive")
                {
                    DriveService service = GoogleDrive.gDriveSetup();
                    string id = cloudFiles.SelectedItems[0].Tag.ToString();
                    Google.Apis.Drive.v2.Data.File file = service.Files.Get(id).Execute();
                }
                //AppendOutputText("Download URL " + file.DownloadUrl); //DEBUG
            } else
            {
                downloadBtn.Enabled = false;
            }
        }

        public bool checkPatreon ()
        {
            var url = "https://update.newagesoftware.net/patreon.php?my_token=" + Properties.Settings.Default.patreonAccessToken;
            var client = new WebClient();

            using (var stream = client.OpenRead(url))
            {
                using (var reader = new StreamReader(stream))
                {
                    string patreon_status_label = reader.ReadToEnd();
                    if (patreon_status_label.Contains("Thank you for being a Patron"))
                        return true;
                }
            }
            return false;
        }

        //google drive stuff
        public async Task PrintGoogleDriveFiles(String folderId) //files in a folder
        {
            DriveService service = GoogleDrive.gDriveSetup();
            ChildrenResource.ListRequest request = service.Children.List(folderId);
            int i = 1;

            do
            {
                try
                {
                    ChildList children = request.Execute();

                    foreach (ChildReference child in children.Items)
                    {
                        string itemID = child.Id;
                        Debug.WriteLine("GDrive File Id: " + child.Id);

                        Google.Apis.Drive.v2.Data.File file = service.Files.Get(itemID).Execute();

                        FilesResource.DeleteRequest DeleteRequest;
                        if (i > Properties.Settings.Default.maxBackups && Properties.Settings.Default.maxBackups != 0) //delete items when we hit max
                        {
                            if (checkPatreon() == false && Properties.Settings.Default.maxBackups > 5)
                            {
                                AppendOutputText("ERROR: Maximum backups is greater than 5 and your not a Patron!", Color.Red);
                                return;
                            }
                            else if (checkPatreon() == true)
                            {
                                AppendOutputText("Patreon status is good!", Color.Green);
                            }
                            AppendOutputText(rm.GetString("hitMax") + " " + file.Title, Color.Gold);
                            DeleteRequest = service.Files.Delete(itemID);
                            DeleteRequest.Execute();
                            continue;
                        }
                        //AppendOutputText("[DEBUG] i=" + i.ToString() + "[" + Properties.Settings.Default.maxBackups + "]");

                        ListViewItem item = new ListViewItem();
                        item.Text = file.Title.Replace(".zip", "");
                        item.Tag = itemID;
                        cloudFiles.Invoke(new MethodInvoker(delegate
                        {
                            cloudFiles.Items.Add(item);
                        }));
                        i++;
                    }
                    request.PageToken = children.NextPageToken;
                }
                catch (Exception e)
                {
                    //AppendOutputText("ERROR (PrintFilesInFolder): " + e.Message); //this msg will print if there are no files in gdrive
                    request.PageToken = null;
                }
            } while (!String.IsNullOrEmpty(request.PageToken));
        }

        public async Task PrintDropBoxFiles(String folderId) //files in a folder
        {
            //AppendOutputText("[DEBUG] checking DropBox folder... " + folderId);
            DropboxClient dbxClient = new DropboxClient(AccessToken);
            try
            {
                var list = await dbxClient.Files.ListFolderAsync("/" + folderId); //no good?
                int t = 1;
                foreach (var dpItem in list.Entries.Where(i => i.IsFile))
                {
                    if (t > Properties.Settings.Default.maxBackups && Properties.Settings.Default.maxBackups != 0) //delete items when we hit max
                    {
                        if (checkPatreon() == false && Properties.Settings.Default.maxBackups > 5)
                        {
                            AppendOutputText("ERROR: Maximum backups is greater than 5 and your not a Patron!", Color.Red);
                            return;
                        }
                        else if (checkPatreon() == true)
                        {
                            AppendOutputText("Patreon status is good!", Color.Green);
                        }
                        AppendOutputText(rm.GetString("hitMax") + " " + dpItem.Name, Color.Gold);
                        await dbxClient.Files.DeleteAsync(@"/" + folderId + @"/" + dpItem.Name);
                    }
                    else
                    {
                        ListViewItem item = new ListViewItem();
                        item.Text = dpItem.Name.Replace(".zip", "");
                        item.Tag = dpItem.Name;
                        cloudFiles.Invoke(new MethodInvoker(delegate
                        {
                            cloudFiles.Items.Add(item);
                        }));
                    }
                    t++;
                }
            } catch
            {

            }
        }

        public async Task PrintOneDriveFiles(String folderId)
        {
            //AppendOutputText("[DEBUG] checking OneDrive folder... " + folderId);
            await getOneDriveZips(folderId);
        }

        public async Task PrintFTPFiles(String folderId)
        {
            //AppendOutputText("[DEBUG] checking FTP folder... ");
            ftp ftpClient = new ftp(Properties.Settings.Default.ftpIP, Properties.Settings.Default.ftpUser, Properties.Settings.Default.ftpPass);
            //AppendOutputText("[DEBUG] FTP Dir=" + Properties.Settings.Default.ftpDir + folderId);
            string[] simpleDirectoryListing = ftpClient.directoryListSimple(Properties.Settings.Default.ftpDir +  folderId);
            for (int i = 0; i < simpleDirectoryListing.Count(); i++) {
                if (simpleDirectoryListing[i] == "." || simpleDirectoryListing[i] == ".." || simpleDirectoryListing[i] == "")
                    continue;
                //AppendOutputText("[DEBUG] item#=" + i + " data=" + simpleDirectoryListing[i]);
                ListViewItem item = new ListViewItem();
                item.Text = simpleDirectoryListing[i].Replace(".zip", "");
                item.Tag = simpleDirectoryListing[i];
                cloudFiles.Invoke(new MethodInvoker(delegate
                {
                    cloudFiles.Items.Add(item);
                }));
            }
            /* Release Resources */
            ftpClient = null;
        }

        public async Task getFTPgames()
        {
            ftp ftpClient = new ftp(Properties.Settings.Default.ftpIP, Properties.Settings.Default.ftpUser, Properties.Settings.Default.ftpPass);
            string[] simpleDirectoryListing = ftpClient.directoryListSimple(Properties.Settings.Default.ftpDir);
            for(int i = 0; i < simpleDirectoryListing.Count(); i++)
            {
                if (simpleDirectoryListing[i] == "." || simpleDirectoryListing[i] == ".." || simpleDirectoryListing[i] == "")
                    continue;
                cloudSaves.Add(simpleDirectoryListing[i], simpleDirectoryListing[i]);
            }
            /* Release Resources */
            ftpClient = null;
        }

        Cll.xmlReader xmlReader = new Cll.xmlReader();
        Cll.xmlRegistry xmlReg = new Cll.xmlRegistry();
        //https://www.codeproject.com/Articles/9526/Import-Export-registry-sections-as-XML
        private static void ExportRegistry(string exportPath, string registryPath)
        {
            if (Cll.xmlRegistry.keyExists(registryPath))
            {
                Cll.xmlWriter w = new Cll.xmlWriter();
                Cll.xmlRegistry xmlReg = new Cll.xmlRegistry();
                w.open(exportPath);
                Cll.xmlElement wroot = new Cll.xmlElement(Cll.xmlRegistry.XML_ROOT);
                wroot.write(w, 1, false, true);
                xmlReg.saveAsXml(w, false, exportPath, "");
                wroot.writeClosingTag(w, -1, false, true);
                w.close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            getNewInstallList();
        }

        async Task PutTaskDelay()
        {
            await Task.Delay(5000);
        }

        private async void GameDetection_timer_Tick(object sender, EventArgs e)
        { //try to find a game we support running in the process
            //This is for BOTH upload and download
            bool breakit = false;
            if (Properties.Settings.Default.autoDL == false || Properties.Settings.Default.cloudSource == "")
            {
                GameDetection_timer.Stop();
                return;
            }

            Process[] processlist = Process.GetProcesses();
            foreach (Process theprocess in processlist)
            {
                //AppendOutputText("[DEBUG] " + theprocess.ProcessName);
                //System.Threading.Thread.Sleep(500);
                //continue;

                foreach (var item in installedGames)
                {
                    //AppendOutputText("[DEBUG] Checking if \"" + theprocess.ProcessName + "\" == \"" + cloudSaveProcs[game] + "\"", Color.Gold);
                    //System.Threading.Thread.Sleep(500);
                    //continue;

                    string myGame = item.Key;
                    if (steamcloudSaveProcs.ContainsKey(myGame))
                    {
                        if (String.Compare(theprocess.ProcessName, steamcloudSaveProcs[myGame], StringComparison.OrdinalIgnoreCase) == 0) //we found A process (could be multiple, we dont know yet)
                        {
                            string fullPath = theprocess.MainModule.FileName; //uncheck Prefer 32-bit in the Build tab or else this will crash!
                            //pull our XML data that matches the process name and put them in a list

                            //steam specific
                            var xml2 = new XmlDocument();
                            xml2.LoadXml(System.IO.File.ReadAllText(xmlFile));
                            var gameDirLoc = xml2.DocumentElement.SelectNodes("/backups/game[exe='" + steamcloudSaveProcs[myGame] + "']/dirname");
                            foreach (XmlNode gameDir in gameDirLoc)
                            {
                                if (fullPath.Contains(@"\" + gameDir.InnerText + @"\")) //if our process directory matches our XML node directory, that's our game.
                                {
                                    //AppendOutputText("[DEBUG] theProc:" + fullPath + " gameDir:" + gameDir.InnerText);
                                    myGame = gameDir.InnerText; //switch it over to the one we detected
                                    detectedGame = item.Key;
                                    detectedPlatform = item.Value;
                                    GameDetection_timer.Stop();
                                    breakit = true;
                                    break;
                                }
                                else
                                    continue;
                            }
                        }
                    }
                    else if (gogCloudSaveProcs.ContainsKey(myGame))
                    {
                        if (String.Compare(theprocess.ProcessName, gogCloudSaveProcs[myGame], StringComparison.OrdinalIgnoreCase) == 0) //we found A process (could be multiple, we dont know yet)
                        {
                            string fullPath = theprocess.MainModule.FileName;

                            //GOG Galaxy (XML) specific
                            var xml3 = new XmlDocument();
                            xml3.LoadXml(System.IO.File.ReadAllText(GOGxmlFile));
                            var gameDirLoc2 = xml3.DocumentElement.SelectNodes("/backups/game[exe='" + gogCloudSaveProcs[myGame] + "']/dirname");
                            foreach (XmlNode gameDir in gameDirLoc2)
                            {
                                if (fullPath.Contains(@"\" + gameDir.InnerText + @"\")) //if our process directory matches our XML node directory, that's our game.
                                {
                                    //AppendOutputText("[DEBUG] theProc:" + fullPath + " gameDir:" + gameDir.InnerText);
                                    myGame = gameDir.InnerText; //switch it over to the one we detected
                                    detectedGame = item.Key;
                                    detectedPlatform = item.Value;
                                    GameDetection_timer.Stop();
                                    breakit = true;
                                    break;
                                }
                                else
                                    continue;
                            }

                            //GOG Galaxy (REG) specific
                            /*RegistryKey OurKey = Registry.LocalMachine;
                            OurKey = OurKey.OpenSubKey(@"SOFTWARE\WOW6432Node\GOG.com\Games", true);

                            foreach (string Keyname in OurKey.GetSubKeyNames())
                            {
                                RegistryKey key = OurKey.OpenSubKey(Keyname);
                                if (key.GetValue("GAMENAME").ToString() == myGame)
                                {
                                    myGame = key.GetValue("GAMENAME").ToString(); //switch it over to the one we detected
                                    detectedGame = item.Key;
                                    detectedPlatform = item.Value;
                                    timer1.Stop();
                                    break;
                                }
                                else
                                    continue;
                            }*/
                        }
                    }
                    else
                        continue;

                    if (!String.IsNullOrEmpty(detectedGame) && detectedGame == myGame)
                    {
                        AppendOutputText(rm.GetString("detectedGame") + " " + myGame + " " + rm.GetString("currentlyRunning"), Color.Gold);
                        AppendOutputText(rm.GetString("whenClosing"), Color.Gold);
                        AppendOutputText(rm.GetString("gettingProc") + " " + theprocess.Id.ToString());

                        // RESTORE CLOUD SAVE
                        if (Properties.Settings.Default.autoRestore)
                        {
                            ListViewItem findGame = localGameList.FindItemWithText(detectedGame);
                            //this isn't necessary, I just want to select the game for the hell of it.
                            /*localGameList.Focus();
                            if (findGame != null)
                                localGameList.Items[localGameList.Items.IndexOf(findGame)].Selected = true;*/

                            if (findGame == null)
                            {
                                AppendOutputText("ERROR: localGameList.FindItemWithText returned null - " + detectedGame, Color.Red);
                                return;
                            }
                            //find our game from the localGameList by the index of findGame. The tag holds our cloud service folder ID.
                            string findID = "";
                            if (Properties.Settings.Default.cloudSource == "googledrive")
                                findID = cloudSaves.FirstOrDefault(x => x.Value == (String)localGameList.Items[localGameList.Items.IndexOf(findGame)].Tag).Key;
                            else
                                findID = (String)localGameList.Items[localGameList.Items.IndexOf(findGame)].Tag;

                            string DownloadID = await getLatestBackup(findID); //now we find our lastest download ID

                            if (DownloadID != "")
                            {
                                SuspendProcess(theprocess.Id);
                                AppendOutputText(rm.GetString("suspending"));

                                if (Properties.Settings.Default.hideNotifications == false)
                                {
                                    notifyIcon1.BalloonTipText = rm.GetString("suspending");
                                    notifyIcon1.ShowBalloonTip(500);
                                }

                                //unzip folders/files and translate dynamic paths to static paths.
                                await getZipFiles(myGame);
                                await downloadFromCloud(DownloadID, myGame);

                                ResumeProcess(theprocess.Id);
                                breakit = true;
                                GameDetection_timer.Stop(); timer2.Start();
                                break;
                            }
                            else
                            {
                                AppendOutputText("Supported Game Detected, but no cloud saves to restore...");
                                breakit = true;
                                GameDetection_timer.Stop(); timer2.Start();
                                break;
                            }
                        }
                        timer2.Start(); //stop timer1, start timer2. watch this game till it closes.
                        if (theprocess.Responding == false)
                        {
                            AppendOutputText("Process not responding");
                            return;
                        }
                        if (breakit) //parent foreach stop.
                            break;
                    }
                    if (breakit) //parent foreach stop.
                        break;
                }

                if (breakit) //parent foreach stop.
                    break;
            }
        }

        private async void timer2_Tick(object sender, EventArgs e)
        { //set to 5 seconds for games like Bejeweled that takes several seconds to restart after settings change in game.
            if (steamcloudSaveProcs.ContainsKey(detectedGame))
            {
                Process[] pname = Process.GetProcessesByName(steamcloudSaveProcs[detectedGame]);
                if (pname.Length == 0)
                {
                    timer2.Stop(); //stop and wait for this process to finish.
                    AppendOutputText(detectedGame + " has closed. Automatic back up system starts in 5 seconds..." );
                    await PutTaskDelay();
                    AppendOutputText(detectedGame + " " + rm.GetString("needToBackup"), Color.Gold);
                    uploadFiles(detectedGame, detectedPlatform); //attempt a backup
                    if (Properties.Settings.Default.hideNotifications == false)
                    {
                        notifyIcon1.BalloonTipText = rm.GetString("autoUpload") + " " + detectedGame + " " + rm.GetString("gameSave");
                        notifyIcon1.ShowBalloonTip(500);
                    }
                    detectedGame = ""; //erase our detected game
                    GameDetection_timer.Start(); //start auto detection back up
                }
            }
            if (gogCloudSaveProcs.ContainsKey(detectedGame))
            {
                Process[] pname2 = Process.GetProcessesByName(gogCloudSaveProcs[detectedGame]);
                if (pname2.Length == 0)
                {
                    timer2.Stop(); //stop and wait for this process to finish.
                    AppendOutputText(detectedGame + " has closed. Automatic back up system starts in 5 seconds...");
                    await PutTaskDelay();
                    AppendOutputText(detectedGame + " " + rm.GetString("needToBackup"), Color.Gold);
                    uploadFiles(detectedGame, detectedPlatform); //attempt a backup
                    if (Properties.Settings.Default.hideNotifications == false)
                    {
                        notifyIcon1.BalloonTipText = rm.GetString("autoUpload") + " " + detectedGame + " " + rm.GetString("gameSave");
                        notifyIcon1.ShowBalloonTip(500);
                    }
                    detectedGame = ""; //erase our detected game
                    GameDetection_timer.Start(); //start auto detection back up
                }
            }
        }

        private void mouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (cloudFiles.FocusedItem.Bounds.Contains(e.Location) == true)
                    contextMenuStrip1.Show(Cursor.Position);
            }
        }

        private void processIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show(rm.GetString("areYouSure") + " " + cloudFiles.SelectedItems[0].Text + " " + rm.GetString("fromCloud"), rm.GetString("deleteTitle"), MessageBoxButtons.YesNo);
            debugLog(rm.GetString("areYouSure") + " " + cloudFiles.SelectedItems[0].Text + " " + rm.GetString("fromCloud"));
            if (dialogResult == DialogResult.Yes)
            {
                if (Properties.Settings.Default.cloudSource == "googledrive")
                {
                    DriveService service = GoogleDrive.gDriveSetup();
                    FilesResource.DeleteRequest DeleteRequest = service.Files.Delete((string)cloudFiles.SelectedItems[0].Tag);
                    DeleteRequest.Execute();
                }
                if (Properties.Settings.Default.cloudSource == "dropbox")
                {
                    DropboxClient dbxClient = new DropboxClient(AccessToken);
                    dbxClient.Files.DeleteAsync(@"/" + (string)localGameList.SelectedItems[0].Tag + @"/" + (string)cloudFiles.SelectedItems[0].Tag);
                }
                if (Properties.Settings.Default.cloudSource == "onedrive")
                {
                    this.oneDriveClient.Drive.Items[(string)cloudFiles.SelectedItems[0].Tag].Request().DeleteAsync();
                }
                if (Properties.Settings.Default.cloudSource == "ftp")
                {
                    ftp ftpClient = new ftp(Properties.Settings.Default.ftpIP, Properties.Settings.Default.ftpUser, Properties.Settings.Default.ftpPass);
                    ftpClient.delete(Properties.Settings.Default.ftpDir + (string)cloudFiles.SelectedItems[0].Tag);
                }
                if (Properties.Settings.Default.cloudSource == "local")
                {
                    System.IO.File.Delete(Properties.Settings.Default.moveDir + (string)cloudFiles.SelectedItems[0].Tag);
                }

                 AppendOutputText(rm.GetString("deletedFile") + " " + cloudFiles.SelectedItems[0].Text, Color.LimeGreen);
                //MessageBox.Show("Deleted Google Cloud file " + cloudFiles.SelectedItems[0].Text);
                downloadReload.PerformClick();
            }
        }

        //Reload our cloud files
        private void downloadReload_Click(object sender, EventArgs e)
        {
            if (localGameList.SelectedItems.Count > 0)
            {
                cloudFiles.Clear();
                uploadBtn.Enabled = true;

                string gDir = "";
                if (Properties.Settings.Default.cloudSource == "googledrive")
                    gDir = cloudSaves.FirstOrDefault(x => x.Value == (String)localGameList.SelectedItems[0].Tag).Key;
                if (Properties.Settings.Default.cloudSource == "dropbox" || Properties.Settings.Default.cloudSource == "onedrive" || Properties.Settings.Default.cloudSource == "ftp" || Properties.Settings.Default.cloudSource == "local")
                    gDir = (String)localGameList.SelectedItems[0].Tag;

                AppendOutputText(rm.GetString("getFolderID") + " " + gDir);
                //AppendOutputText("Getting game process name... " + cloudSaveProcs[(String)localGameList.SelectedItems[0].Tag]);
                if (backgroundWorkerList.IsBusy != true)
                    backgroundWorkerList.RunWorkerAsync(gDir);
            }
        }

        private void programmerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(rm.GetString("createdBy") + " New Age Software LLC. https://nas.llc", "Application Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
        }

        private void applicationInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutbox = new AboutBox1();
            aboutbox.Show();
        }

        private void visitNewagesoldiercomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://nas.llc");
        }

        private void settingsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            settings settingsForm = new settings();
            settingsForm.Show();
        }

        private void googleDriveFolderListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            /*if (FormWindowState.Minimized == this.WindowState)
            {
                //notifyIcon1.BalloonTipText = rm.GetString("miniToTray");
                //notifyIcon1.ShowBalloonTip(500);
                this.Hide();
            }*/
        }

        private void notifyIcon1_DoubleClick(object sender, MouseEventArgs e)
        {
            
        }

        // ONE DRIVE FUNCTIONS
        private void ProcessFolder(Item folder)
        {
            if (folder != null)
            {
                if (folder.Folder != null && folder.Children != null && folder.Children.CurrentPage != null)
                    LoadChildren(folder.Children.CurrentPage);
            }
        }

        //List<string> list = new List<string>();
        private void LoadChildren(IList<Item> items)
        {
            foreach (var obj in items)
            {
                //oneDriveDirs.Add(obj.Id,obj.Name);
                cloudSaves.Add(obj.Id, obj.Name);
            }
        }

        private async Task createDir(string folder, string parentID)
        {
            var folderToCreate = new Item { Name = folder, Folder = new Microsoft.OneDrive.Sdk.Folder() };
            await this.oneDriveClient.Drive.Items[parentID].Children.Request().AddAsync(folderToCreate);
        }

        string oneDriveSteamBackupFolderID = "";
        private void LoadChildren2(IList<Item> items)
        {
            foreach (var obj in items)
            {
                if (obj.Name.Contains("GameBackup"))
                    oneDriveSteamBackupFolderID = obj.Id;
            }
        }
        private void ProcessFolder2(Item folder)
        {
            if (folder != null)
            {
                if (folder.Folder != null && folder.Children != null && folder.Children.CurrentPage != null)
                    LoadChildren2(folder.Children.CurrentPage);
            }
        }
        private async Task checkOneDriveFolders()
        {
            if (oneDriveSteamBackupFolderID != "")
                return;

            if (null == this.oneDriveClient) return;
            LoadChildren2(new Item[0]);

            try
            {
                Item folder;
                var expandValue = this.oneDriveClient.ClientType == ClientType.Consumer ? "children" : "children";
                folder = await this.oneDriveClient.Drive.Root.Request().Expand(expandValue).GetAsync();
                ProcessFolder2(folder);
            }
            catch (Exception exception)
            {
                PresentOneDriveException(exception);
            }
        }

        private async Task LoadFolderFromPath(string path = null)
        {
            if (null == this.oneDriveClient) return;
            LoadChildren(new Item[0]);

            try
            {
                Item folder;
                var expandValue = this.oneDriveClient.ClientType == ClientType.Consumer ? "children" : "children";

                if (path == null)
                    folder = await this.oneDriveClient.Drive.Root.Request().Expand(expandValue).GetAsync();
                else
                    folder = await this.oneDriveClient.Drive.Root.ItemWithPath("/" + path).Request().Expand(expandValue).GetAsync();

                ProcessFolder(folder);
            }
            catch (Exception exception)
            {
                PresentOneDriveException(exception);
            }
        }

        private static void PresentOneDriveException(Exception exception)
        {
            string message = null;
            var oneDriveException = exception as OneDriveException;
            if (oneDriveException == null)
            {
                message = exception.Message;
            }
            else
            {
                message = string.Format("{0}{1}", Environment.NewLine, oneDriveException.ToString());
            }

            MessageBox.Show(string.Format("OneDrive reported the following error: {0}", message));
        }
        // END

        private async void Form1_Shown(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.miniStartup)
            {
                Hide();
                WindowState = FormWindowState.Minimized;
            }

            ToolTip toolTip1 = new ToolTip();
            toolTip1.SetToolTip(settings_button, "Settings Menu");
            toolTip1.SetToolTip(cloudsources_button, "Change Cloud Storage Providers");
            toolTip1.SetToolTip(supportedgames_button, "Supported Games");
            toolTip1.SetToolTip(refreshGames_btn, "Refresh Games List");
            toolTip1.SetToolTip(uploadBtn, "Upload Game Saves to Server");
            toolTip1.SetToolTip(downloadBtn, "Download and Restore Selected Game Save");
            toolTip1.SetToolTip(downloadReload, "Refresh Game Saves List From Server");

            //try
            //{
            Process[] p = Process.GetProcessesByName("GameSaveBackup");
            if (p.Count() > 1)
            {
                Close();
                return;
            }

            groupBox1.ForeColor = Color.White;
            groupBox2.ForeColor = Color.White;

            if (!Directory.Exists(appDir))
                Directory.CreateDirectory(appDir);

            if (Properties.Settings.Default.language == "")
                Properties.Settings.Default.language = "English"; //debug has an issue with loading this.
            if (Properties.Settings.Default.maxBackups < 0)
                Properties.Settings.Default.maxBackups = 10; //debug has an issue with loading this.

            AppendOutputText(rm.GetString("welcomeMsg"), Color.DarkBlue);
            AppendOutputText(rm.GetString("gameDetectMsg"), Color.AliceBlue);

            AppendOutputText("Current selected cloud service: " + Properties.Settings.Default.cloudSource, Color.Blue);

            // download the XML file for offline use. This is a precaution for the site being down.
            if (!xmlDownloader.IsBusy)
                xmlDownloader.RunWorkerAsync();

            string steamPath = getSteamInstallPath();
            if (String.IsNullOrEmpty(steamPath))
                AppendOutputText("WARNING: Steam not installed.", Color.Yellow);
            else
            {
                AppendOutputText("Steam is installed.", Color.LimeGreen);
                steamPaths.Add(steamPath.Replace(@"/", @"\") + @"\steamapps\common\");
                try
                {
                    foreach (string line in System.IO.File.ReadLines(getSteamInstallPath() + @"\config\config.vdf"))
                    {
                        if (line.Contains("BaseInstallFolder_"))
                            steamPaths.Add(getVal(line) + @"\steamapps\common\");
                    }
                } catch
                {
                    Debug.WriteLine("Steam config.vdf was not readable at this moment.");
                }

                try
                {
                    foreach (string line2 in WriteSafeReadAllLines(getSteamInstallPath() + @"\logs\connection_log.txt"))
                    {
                        if (line2.Contains("SetSteamID") && !line2.Contains("SetSteamID( [U:1:0] )"))
                            steamID = getVal2(line2);
                    }
                } catch
                {
                    Debug.WriteLine(getSteamInstallPath() + @"\logs\connection_log.txt" + " file is not readable or does not exist.");
                }
                //AppendOutputText("Your Steam ID is " + steamID);
            }

            if (System.IO.Directory.Exists(@"C:\GOG Games\"))
            {
                if (Properties.Settings.Default.gogDirs == null)
                    Properties.Settings.Default.gogDirs = new StringCollection();
                if (!Properties.Settings.Default.gogDirs.Contains(@"C:\GOG Games\"))
                    Properties.Settings.Default.gogDirs.Add(@"C:\GOG Games\");
            }


            if (Properties.Settings.Default.gogDirs != null)
            {
                if (Properties.Settings.Default.gogDirs.Count == 0)
                    AppendOutputText(rm.GetString("noGOGGames"), Color.Yellow);
                else
                {
                    AppendOutputText("GOG game directories found.", Color.LimeGreen);
                    foreach (string gogPath in Properties.Settings.Default.gogDirs)
                    {
                        if (System.IO.Directory.Exists(gogPath))
                            gogPaths.Add(gogPath);
                        else
                            AppendOutputText("ERROR: GOG directory \"" + gogPath + "\" does not exist! Please remove it in the settings menu.", Color.Red);
                    }
                }
            }
            else
                AppendOutputText(rm.GetString("noGOGGames"), Color.Yellow);

            if (Properties.Settings.Default.autoDL == false)
                AppendOutputText(rm.GetString("gameBackupMsg") + " " + rm.GetString("disabled") + ".");
            else
                AppendOutputText(rm.GetString("gameBackupMsg") + " " + rm.GetString("enabled") + ".");

            //language string loading
            Text = rm.GetString("Form1");
            applicationInformationToolStripMenuItem.Text = rm.GetString("applicationInformationToolStripMenuItem");
            visitNewagesoldiercomToolStripMenuItem.Text = rm.GetString("visitNewagesoldiercomToolStripMenuItem");
            uploadBtn.Text = rm.GetString("uploadBtn");
            downloadBtn.Text = rm.GetString("downloadBtn");

            if (Properties.Settings.Default.cloudSource == "onedrive")
                await oneDriveLogin();

            if (Properties.Settings.Default.cloudSource == "ftp")
            {
                ftp ftpClient = new ftp(Properties.Settings.Default.ftpIP, Properties.Settings.Default.ftpUser, Properties.Settings.Default.ftpPass);
                AppendOutputText(ftpClient.testConnection());
            }

            if (Properties.Settings.Default.firstRun == false || Properties.Settings.Default.cloudSource == "none")
            {
                storageSource storageSource = new storageSource();
                storageSource.Show();
                Properties.Settings.Default.firstRun = true;
                Properties.Settings.Default.Save();
            }
        }

        [Flags]
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);
        
        private static void SuspendProcess(int pid)
        {
            var process = Process.GetProcessById(pid);

            if (process.ProcessName == string.Empty)
                return;

            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);
                if (pOpenThread == IntPtr.Zero)
                    continue;

                SuspendThread(pOpenThread);
                CloseHandle(pOpenThread);
            }
        }

        public static void ResumeProcess(int pid)
        {
            var process = Process.GetProcessById(pid);
            if (process.ProcessName == string.Empty)
                return;

            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);
                if (pOpenThread == IntPtr.Zero)
                    continue;

                var suspendCount = 0;
                do
                {
                    suspendCount = ResumeThread(pOpenThread);
                } while (suspendCount > 0);
                CloseHandle(pOpenThread);
            }
        }

        private void visitDrivegooglecomToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://drive.google.com");
        }

        private void localGame_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (localGameList.FocusedItem.Bounds.Contains(e.Location))
                    localGamesContextMenuStrip.Show(Cursor.Position);
                if (localGameList.SelectedItems.Count > 0)
                {
                    if (localGameList.SelectedItems[0].Name == "gog")
                        playGameToolStripMenuItem.Enabled = false;
                }
            }
        }

        private void playGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (localGameList.SelectedItems.Count > 0)
            {
                if (localGameList.SelectedItems[0].Name == "steam")
                {
                    var xml2 = new XmlDocument();
                    xml2.LoadXml(System.IO.File.ReadAllText(xmlFile));
                    var gameAppID = xml2.DocumentElement.SelectSingleNode("/backups/game[dirname=\"" + localGameList.SelectedItems[0].Tag + "\"]/appid").InnerText;
                    Process.Start("steam://run/" + gameAppID);
                }
            }
        }

        private async Task oneDriveLogin()
        {
            if (oneDriveClient == null)
            {
                string refreshToken = Properties.Settings.Default.oneDriveToken;

                if (String.IsNullOrEmpty(refreshToken))
                    oneDriveClient = OneDriveClient.GetMicrosoftAccountClient("000000004017CBBC", @"https://login.live.com/oauth20_desktop.srf", Scopes, webAuthenticationUi: new FormsWebAuthenticationUi());
                else
                    oneDriveClient = await OneDriveClient.GetSilentlyAuthenticatedMicrosoftAccountClient("000000004017CBBC", @"https://login.live.com/oauth20_desktop.srf", Scopes, refreshToken);
                try
                {
                    if (!oneDriveClient.IsAuthenticated)
                    {
                        await oneDriveClient.AuthenticateAsync();
                        AccountSession accountSession = await this.oneDriveClient.AuthenticateAsync();
                        if (String.IsNullOrEmpty(refreshToken))
                        {
                            Properties.Settings.Default.oneDriveToken = accountSession.RefreshToken;
                            Properties.Settings.Default.Save();
                        }
                    }
                    
                    await createDir("GameBackup", "root");
                    await checkOneDriveFolders();
                    //MessageBox.Show(string.Join(Environment.NewLine, oneDriveDirs)); //DEBUG
                }
                catch (OneDriveException exception)
                {
                    // Swallow authentication cancelled exceptions
                    if (!exception.IsMatch(OneDriveErrorCode.AuthenticationCancelled.ToString()))
                    {
                        if (exception.IsMatch(OneDriveErrorCode.AuthenticationFailure.ToString()))
                        {
                            MessageBox.Show(
                                "Authentication failed",
                                "Authentication failed",
                                MessageBoxButtons.OK);

                            var httpProvider = this.oneDriveClient.HttpProvider as HttpProvider;
                            httpProvider.Dispose();
                            this.oneDriveClient = null;
                        }
                    }
                }
            }
        }

        private async void SetupCloudDrives_bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Debug.WriteLine("SetupCloudDrives_bgWorker started");
            //try
            //{
            checkLocalGames(); //something we always need to do.
            //}
            try
            {
                if (Properties.Settings.Default.cloudSource == "googledrive")
                {
                    DriveService service = GoogleDrive.gDriveSetup();
                    GoogleDriveSyncFiles(service);
                }
                if (Properties.Settings.Default.cloudSource == "dropbox")
                {
                    if (!String.IsNullOrEmpty(Properties.Settings.Default.dropBoxToken))
                        AccessToken = Properties.Settings.Default.dropBoxToken;

                    if (String.IsNullOrEmpty(AccessToken))
                    {
                        dropBoxLogin("540c6xa0wasn1sy");
                        AppendOutputText("No DropBox AccessToken set yet.");
                        return;
                    }
                    else
                        AppendOutputText("DropBox AccessToken: " + AccessToken);
                    DropboxClient dbxClient = new DropboxClient(AccessToken);
                    DropBoxSyncFiles(dbxClient);
                }
                if (Properties.Settings.Default.cloudSource == "onedrive")
                    await OneDriveSyncFiles();
                if (Properties.Settings.Default.cloudSource == "ftp")
                    await FTPSyncFiles();
                if (Properties.Settings.Default.cloudSource == "local")
                {
                    if (!Directory.Exists(Properties.Settings.Default.moveDir))
                    {
                        AppendOutputText("WARNING: Local Directory \"" + Properties.Settings.Default.moveDir + "\" doesn\'t exist! I'll make it for you.", Color.Yellow);
                        Directory.CreateDirectory(Properties.Settings.Default.moveDir);
                    }
                    string[] dirs = Directory.GetFiles(Properties.Settings.Default.moveDir);
                    foreach (string dir in dirs)
                    {
                        cloudSaves.Add(dir, dir);
                    }
                    AppendOutputText(rm.GetString("getMoveDirComplete"), Color.LimeGreen);
                    refreshGames_btn.Invoke(new MethodInvoker(delegate
                    {
                        refreshGames_btn.Enabled = true;
                    }));
                    localGameList.Invoke(new MethodInvoker(delegate
                    {
                        localGameList.Cursor = Cursors.Default;
                    }));
                }
            }
            catch
            {
                AppendOutputText("SetupCloudDrives_bgWorker crashed!");
            }
        }

        async Task getZipFiles(string game)
        {
            cloudFiles.Clear();
            if (Properties.Settings.Default.cloudSource == "googledrive")
            {
                await PrintGoogleDriveFiles(game);
                //break;
            }
            if (Properties.Settings.Default.cloudSource == "dropbox")
            {
                await PrintDropBoxFiles(game);
                //break;
            }
            if (Properties.Settings.Default.cloudSource == "onedrive")
            {
                await PrintOneDriveFiles(game);
                //break;
            }
            if (Properties.Settings.Default.cloudSource == "ftp")
            {
                await PrintFTPFiles(game);
                //break;
            }
            if (Properties.Settings.Default.cloudSource == "local")
            {
                if (!Directory.Exists(Properties.Settings.Default.moveDir + game))
                    Directory.CreateDirectory(Properties.Settings.Default.moveDir + game);
                DirectoryInfo d = new DirectoryInfo(Properties.Settings.Default.moveDir + game);
                FileInfo[] Files = d.GetFiles("*.zip");
                foreach (FileInfo file in Files)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = file.Name.Replace(".zip", "");
                    item.Tag = file.Name;
                    cloudFiles.Invoke(new MethodInvoker(delegate
                    {
                        cloudFiles.Items.Add(item);
                    }));
                }
            }
        }

        private async void backgroundWorkerList_DoWork(object sender, DoWorkEventArgs e)
        {
            //while (true)
            //{
            //try
            //{
                string value = (string)e.Argument;
                await getZipFiles(value);
            //} catch { }
            //}
        }

        private void localGameList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (SetupCloudDrives_bgWorker.IsBusy)
            {
                if (e.IsSelected) e.Item.Selected = false;
            }
        }

        private void getNewInstallList()
        {
            localGameList.Items.Clear();
            localGameList.Clear();
            cloudSaves.Clear();
            installedGames.Clear();
            steamcloudSaveProcs.Clear();
            gogCloudSaveProcs.Clear();
            AppendOutputText("Installed games list updating...");
            if (Properties.Settings.Default.hideNotifications == false)
            {
                notifyIcon1.BalloonTipText = "Installed games list updating...";
                notifyIcon1.ShowBalloonTip(500);
            }
            newGameDetection.Enabled = false;
            newGameDetection.Stop();
            Debug.WriteLine("stopping newGameDetection timer for a moment.");
            if (SetupCloudDrives_bgWorker.IsBusy != true)
                SetupCloudDrives_bgWorker.RunWorkerAsync();
        }

        private void newGameDetection_Tick_1(object sender, EventArgs e)
        {
            if (!setupCompleted)
            {
                Debug.WriteLine("[DEBUG:newGameDetection_Tick_1] CloudDriveSetup not completed yet");
                return;
            }

            if (Properties.Settings.Default.cloudSource == "none")
            {
                AppendOutputText("WARNING: Must select cloud source before continuing...", Color.Red);
                Debug.WriteLine("No cloud source selected, newGameDetection timer not starting.");
                newGameDetection.Enabled = false;
                newGameDetection.Stop();
                return;
            }

            // BUILD A TEMP STEAM PATH LIST TO COMPARE TO OUR INITIAL STEAM PATH LIST

            List<string> tempSteamPaths = new List<string>();
            string myTempPath = getSteamInstallPath();
            if (myTempPath == null)
            {
                Debug.WriteLine("myTempPath is null, returning newGameDetection code here");
                return;
            }

            tempSteamPaths.Add(myTempPath.Replace(@"/", @"\") + @"\steamapps\common\");
            try {
                foreach (string line in System.IO.File.ReadLines(getSteamInstallPath() + @"\config\config.vdf"))
                {
                    if (line.Contains("BaseInstallFolder_"))
                        tempSteamPaths.Add(getVal(line) + @"\steamapps\common\");
                }
            }
            catch
            {
                Debug.WriteLine("Steam config.vdf was not readable at this moment.");
            }
            //Debug.WriteLine("temp = " + String.Join(",", tempSteamPaths));
            //Debug.WriteLine("curr = " + String.Join(",", steamPaths));
            if (tempSteamPaths.SequenceEqual(steamPaths) == false)
            {
                //BUILD A NEW STEAM PATHS LIST
                steamPaths.Clear();
                steamPaths.Add(getSteamInstallPath().Replace(@"/", @"\") + @"\steamapps\common\");
                try
                {
                    foreach (string line in System.IO.File.ReadLines(getSteamInstallPath() + @"\config\config.vdf"))
                    {
                        if (line.Contains("BaseInstallFolder_"))
                            steamPaths.Add(getVal(line) + @"\steamapps\common\");
                    }
                }
                catch
                {
                    Debug.WriteLine("Steam config.vdf was not readable at this moment.");
                }
            }

            // DETECT IF A GAME IS INSTALLED OR UNINSTALLED
            Dictionary<string, string> tempGameCheck = new Dictionary<string, string>();

            //make list of supported steam games
            XmlTextReader reader = new XmlTextReader(xmlFile);
            try
            {
                if (reader.IsStartElement())
                {
                    while (reader.Read())
                    {
                        switch (reader.Name)
                        {
                            case "dirname":
                                string gameDir = reader.ReadString();
                                foreach (string sp in steamPaths)
                                {
                                    if (Directory.Exists(sp + gameDir) && Directory.GetFileSystemEntries(sp + gameDir).Length > 0) //check if not empty and exists
                                        tempGameCheck.Add(gameDir, "steam");
                                }
                                break;
                        }
                    }
                }
            }
            catch
            {
                AppendOutputText("ERROR: The Steam XML file disapeared!", Color.Red);
                return;
            }

            //make list of supported GOG games
            XmlTextReader reader2 = new XmlTextReader(GOGxmlFile);
            try
            {
                if (reader2.IsStartElement())
                {
                    while (reader2.Read())
                    {
                        switch (reader2.Name)
                        {
                            case "dirname":
                                string gameDir = reader2.ReadString();
                                foreach (string gog in gogPaths)
                                {
                                    if (Directory.Exists(gog + gameDir))
                                        tempGameCheck.Add(gameDir, "gog");
                                }
                                break;
                        }
                    }
                }
            }
            catch
            {
                AppendOutputText("ERROR: The GOG XML file disapeared!", Color.Red);
                return;
            }

            //check all GOG paths in system registry
            foreach (string gp in gogPaths)
            {
                //check all installed games in those paths to see if they exist.
                foreach (var item in installedGames)
                {
                    if (Directory.Exists(System.IO.Path.Combine(gp, item.Key))) //Add any paths that just happen to exist
                        if (!tempGameCheck.ContainsKey(item.Key))
                            tempGameCheck.Add(item.Key, "gog");
                }
            }

            //AppendOutputText("[gog] temp = " + String.Join(",", tempGameCheck));
            //AppendOutputText("[gog] inst = " + String.Join(",", installedGames));

            if (installedGames.Count != tempGameCheck.Count)
            {
                Debug.WriteLine("getNewInstallList triggered");
                getNewInstallList();
            }

            reader.Close();
            tempSteamPaths.Clear();
            tempGameCheck.Clear();
            //Debug.WriteLine("newGameDetection tick...");
        }

        private void SetupCloudDrives_bgWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            GameDetection_timer.Start();
            this.newGameDetection.Enabled = true;
            this.newGameDetection.Start();
            setupCompleted = true;
            Debug.WriteLine("newGameDetection timer starting...");
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            foreach (Form frm in Application.OpenForms)
            {
                if (frm.Name.Equals("closeQuestion"))
                    return;
            }
            closeQuestion closebox = new closeQuestion();
            closebox.Show();
        }

        private void minimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void aboutButton_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutbox = new AboutBox1();
            aboutbox.Show();
        }

        private void panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void notify_DoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Show();
                WindowState = FormWindowState.Normal;
                this.TopMost = true;
                this.TopMost = false;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            settings settingsForm = new settings();
            settingsForm.Show();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in cloudSaves)
            {
                sb.AppendFormat("{0} {1}", item.Key, Environment.NewLine);
            }

            string result = sb.ToString().TrimEnd();//when converting to string we also want to trim the redundant new line at the very end
            MessageBox.Show(result, rm.GetString("myDriveBackupsTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            supported_games supported_games = new supported_games();
            supported_games.Show();
        }

        private void supportedSteamGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            supported_games supported_games = new supported_games();
            supported_games.Show();
        }

        private void showHideApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Visible)
            {
                Show();
                this.WindowState = FormWindowState.Normal;
                this.TopMost = true;
                this.TopMost = false;
            }
            else
                Hide();
        }

        private void googleDriveBackupCopiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            storageSource storageSource = new storageSource();
            storageSource.Show();
        }

        public string AccessToken { get; private set; }

        public string Uid { get; private set; }

        public bool Result { get; private set; }

        private void completed(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.ToString().StartsWith(RedirectUri, StringComparison.OrdinalIgnoreCase)) {
                //MessageBox.Show("We're at localhost : " + e.Url.ToString());

                try
                {
                    OAuth2Response result = DropboxOAuth2Helper.ParseTokenFragment(e.Url);
                    if (result.State != oauth2State)
                    {
                        return;
                    }

                    AccessToken = result.AccessToken;
                    Uid = result.Uid;
                    Result = true;
                    //MessageBox.Show(AccessToken);
                }
                catch (ArgumentException)
                {
                    // There was an error in the URI passed to ParseTokenFragment
                }
                finally
                {
                    webBrowser1.Visible = false;
                    //MessageBox.Show(AccessToken);
                    Properties.Settings.Default.dropBoxToken = AccessToken;
                    Properties.Settings.Default.Save();
                    System.Diagnostics.Process.Start(Application.ExecutablePath);
                    Application.Exit();
                }
            }
        }

        private void localGame_DoubleClick(object sender, EventArgs e)
        {
            if (localGameList.SelectedItems.Count > 0 && Properties.Settings.Default.doubleClick)
            {
                var xml2 = new XmlDocument();
                xml2.LoadXml(System.IO.File.ReadAllText(xmlFile));
                var gameAppID = xml2.DocumentElement.SelectSingleNode("/backups/game[dirname=\"" + localGameList.SelectedItems[0].Tag + "\"]/appid").InnerText;
                System.Diagnostics.Process.Start("steam://run/" + gameAppID);
            }
        }

        private void completed(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (e.Url.ToString().StartsWith(RedirectUri, StringComparison.OrdinalIgnoreCase))
            {
                //MessageBox.Show("[DEBUG] URL:" + e.Url.ToString());

                try
                {
                    OAuth2Response result = DropboxOAuth2Helper.ParseTokenFragment(e.Url);
                    if (result.State != oauth2State)
                    {
                        return;
                    }

                    AccessToken = result.AccessToken;
                    Uid = result.Uid;
                    Result = true;
                    //MessageBox.Show(AccessToken);
                }
                catch (ArgumentException)
                {
                    // There was an error in the URI passed to ParseTokenFragment
                }
                finally
                {
                    webBrowser1.Visible = false;
                    //MessageBox.Show(AccessToken);
                    Properties.Settings.Default.dropBoxToken = AccessToken;
                    Properties.Settings.Default.Save();
                    System.Diagnostics.Process.Start(Application.ExecutablePath);
                    Application.Exit();
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            settings settingsForm = new settings();
            settingsForm.Show();
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            storageSource storageSource = new storageSource();
            storageSource.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            supported_games supported_games = new supported_games();
            supported_games.Show();
        }

        private void xmlDownloader_DoWork(object sender, DoWorkEventArgs e)
        {
            AppendOutputText("Checking remote XML files, will populate games in a second...", Color.LightBlue);
            if (!System.IO.File.Exists(GOGxmlFile))
            {
                WebClient Client = new WebClient();
                Client.DownloadFile(gogXMLurl, GOGxmlFile);
                AppendOutputText(rm.GetString("dlGOGXML"), Color.LimeGreen);
            }
            else
            {
                var xml = new XmlDocument();
                var textFromFile = (new WebClient()).DownloadString(gogXMLurl);
                xml.LoadXml(textFromFile);
                var xmlURLVer = xml.DocumentElement.SelectSingleNode("/backups/date").InnerText;

                var xml2 = new XmlDocument();
                if (new FileInfo(GOGxmlFile).Length <= 0)
                {
                    MessageBox.Show("XML file " + GOGxmlFile + " is 0 bytes! Delete the file and re-open GameSaveBackup.", "XML Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                try
                {
                    xml2.LoadXml(System.IO.File.ReadAllText(GOGxmlFile));
                } catch
                {
                    MessageBox.Show("XML file " + GOGxmlFile + " is formatted incorrectly! Delete the file and re-open GameSaveBackup.", "XML Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var xmlFileVer = xml2.DocumentElement.SelectSingleNode("/backups/date").InnerText;

                if (!xmlURLVer.Equals(xmlFileVer))
                {
                    WebClient Client = new WebClient();
                    Client.DownloadFile(gogXMLurl, GOGxmlFile);
                    AppendOutputText(rm.GetString("dlGOGXML"), Color.LimeGreen);
                }
                else
                    AppendOutputText(rm.GetString("goodGOGXML"), Color.LimeGreen);
            }

            if (!System.IO.File.Exists(xmlFile))
            {
                WebClient Client = new WebClient();
                Client.DownloadFile(xmlURL, xmlFile);
                AppendOutputText(rm.GetString("dlSteamXML"), Color.LimeGreen);
            }
            else
            {
                var xml = new XmlDocument();
                var textFromFile = (new WebClient()).DownloadString(xmlURL);
                xml.LoadXml(textFromFile);
                var xmlURLVer = xml.DocumentElement.SelectSingleNode("/backups/date").InnerText;

                var xml2 = new XmlDocument();
                if (new FileInfo(xmlFile).Length <= 0)
                {
                    MessageBox.Show("XML file " + xmlFile + " is 0 bytes! Delete the file and re-open GameSaveBackup.", "XML Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                try {
                    xml2.LoadXml(System.IO.File.ReadAllText(xmlFile));
                }
                catch
                {
                    MessageBox.Show("XML file " + xmlFile + " is formatted incorrectly! Delete the file and re-open GameSaveBackup.", "XML Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var xmlFileVer = xml2.DocumentElement.SelectSingleNode("/backups/date").InnerText;

                if (!xmlURLVer.Equals(xmlFileVer))
                {
                    WebClient Client = new WebClient();
                    Client.DownloadFile(xmlURL, xmlFile);
                    AppendOutputText(rm.GetString("dlSteamXML"), Color.LimeGreen);
                }
                else
                    AppendOutputText(rm.GetString("goodSteamXML"), Color.LimeGreen);
            }

            AppendOutputText("Done checking remote XML files.", Color.LightBlue);

            if (SetupCloudDrives_bgWorker.IsBusy != true)
                SetupCloudDrives_bgWorker.RunWorkerAsync();
        }

        private void openGameDirectoryMenuItem_Click(object sender, EventArgs e)
        {
            if (localGameList.SelectedItems.Count > 0)
            {
                if (localGameList.SelectedItems[0].Name == "steam")
                {
                    foreach (string paths in steamPaths)
                    {
                        //Debug.WriteLine("checking: " + paths + localGameList.SelectedItems[0].Tag);
                        if (Directory.Exists(paths + localGameList.SelectedItems[0].Tag))
                            Process.Start(paths + localGameList.SelectedItems[0].Tag);
                    }
                }
                //MessageBox.Show(localGameList.SelectedItems[0].Tag.ToString());
            }
            else if (localGameList.SelectedItems[0].Name == "gog")
            {
                foreach (string paths in gogPaths)
                {
                    if (Directory.Exists(paths + localGameList.SelectedItems[0].Tag))
                        Process.Start(paths + localGameList.SelectedItems[0].Tag);
                }
            }
        }

        private void launchGameexeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (localGameList.SelectedItems.Count > 0)
            {
                if (localGameList.SelectedItems[0].Name == "steam")
                {
                    var xml2 = new XmlDocument();
                    xml2.LoadXml(System.IO.File.ReadAllText(xmlFile));
                    var launchExe = xml2.DocumentElement.SelectSingleNode("/backups/game[dirname=\"" + localGameList.SelectedItems[0].Tag + "\"]/exe").InnerText;
                    foreach (string sp in steamPaths)
                    {
                        if (Directory.Exists(sp + @"/" + localGameList.SelectedItems[0].Tag))
                        {
                            Process.Start(sp + @"/" + localGameList.SelectedItems[0].Tag + @"/" + launchExe + ".exe");
                            break;
                        }
                    }
                }
                else if (localGameList.SelectedItems[0].Name == "gog")
                {
                    var xml2 = new XmlDocument();
                    xml2.LoadXml(System.IO.File.ReadAllText(GOGxmlFile));

                    var launchExe = xml2.DocumentElement.SelectSingleNode("/backups/game[dirname=\"" + localGameList.SelectedItems[0].Tag + "\"]/exe").InnerText;
                    foreach (string gog in gogPaths)
                    {
                        if (Directory.Exists(gog + @"/" + localGameList.SelectedItems[0].Tag))
                        {
                            Process.Start(gog + @"/" + localGameList.SelectedItems[0].Tag + @"/" + launchExe + ".exe");
                            return;
                        }
                    }
                }
            }
        }
    }
}
