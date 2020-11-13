using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using System.Drawing;
using System.Diagnostics;

namespace GameSaveBackup
{
    class GoogleDrive
    {
        public static DriveService gDriveSetup()
        {
            Debug.WriteLine("gDriveSetup launching...");
            String ci = "316702680224-2u2gemtlvm60hhqcjldf4na2dq88jri1.apps.googleusercontent.com";
            String cs = "c8b6m3DNcNA410DJMZ7CRX2O";
            DriveService service = Authentication.AuthenticateOauth(ci, cs, Environment.UserName);
            
            if (service == null)
            {
                Debug.WriteLine("ERROR: Google Drive service is null");
                return null;
            }

            try
            {
                string Q = "title = 'GameBackup' and mimeType = 'application/vnd.google-apps.folder'";
                IList<Google.Apis.Drive.v2.Data.File> _Files = GetFiles(service, Q);
                Debug.WriteLine("gdrive Files Found: " + _Files.Count);
                if (_Files.Count == 0)
                {
                    //Form1.AppendOutputText(rm.GetString("googlesetup"), Color.Yellow);
                    _Files.Add(createDirectory(service, "GameBackup", "GameBackup", "root"));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Authentication: " + ex.Message);
            }

            return service;
        }

        public static Boolean downloadFile(DriveService _service, Google.Apis.Drive.v2.Data.File _fileResource, string _saveTo)
        {

            if (!String.IsNullOrEmpty(_fileResource.DownloadUrl))
            {
                try
                {
                    var x = _service.HttpClient.GetByteArrayAsync(_fileResource.DownloadUrl);
                    byte[] arrBytes = x.Result;
                    System.IO.File.WriteAllBytes(_saveTo, arrBytes);
                    return true;
                }
                catch //(Exception e)
                {
                    //AppendOutputText("ERROR (downloadFile): " + e.Message, Color.Red);
                    return false;
                }
            }
            else
            {
                // The file doesn't have any content stored on Drive.
                return false;
            }
        }

        public static Google.Apis.Drive.v2.Data.File createDirectory(DriveService _service, string _title, string _description, string _parent)
        {
            Google.Apis.Drive.v2.Data.File NewDirectory = null;

            // Create metaData for a new Directory
            Google.Apis.Drive.v2.Data.File body = new Google.Apis.Drive.v2.Data.File();
            body.Title = _title;
            body.Description = _description;
            body.MimeType = "application/vnd.google-apps.folder";
            body.Parents = new List<ParentReference>() { new ParentReference() { Id = _parent } };

            try
            {
                if (!_service.Files.Equals("_title"))
                {
                    FilesResource.InsertRequest request = _service.Files.Insert(body);
                    NewDirectory = request.Execute();
                }
            }
            catch //(Exception e)
            {
                //Form1.AppendOutputText("ERROR (createDirectory): " + e.Message, Color.Red);
            }

            return NewDirectory;
        }

        public static IList<Google.Apis.Drive.v2.Data.File> GetFiles(DriveService service, string search)
        {

            IList<Google.Apis.Drive.v2.Data.File> Files = new List<Google.Apis.Drive.v2.Data.File>();

            try
            {
                //List all of the files and directories for the current user.  
                // Documentation: https://developers.google.com/drive/v2/reference/files/list
                FilesResource.ListRequest list = service.Files.List();
                list.MaxResults = 1000;
                if (search != null)
                {
                    list.Q = search;
                }
                FileList filesFeed = list.Execute();

                //// Loop through until we arrive at an empty page
                while (filesFeed.Items != null)
                {
                    // Adding each item  to the list.
                    foreach (Google.Apis.Drive.v2.Data.File item in filesFeed.Items)
                    {
                        Files.Add(item);
                    }

                    // We will know we are on the last page when the next page token is
                    // null.
                    // If this is the case, break.
                    if (filesFeed.NextPageToken == null)
                    {
                        break;
                    }

                    // Prepare the next page of results
                    list.PageToken = filesFeed.NextPageToken;

                    // Execute and process the next page request
                    filesFeed = list.Execute();
                }
            }
            catch (Exception ex)
            {
                // In the event there is an error with the request.
                //Form1.AppendOutputText(ex.Message, Color.Red);
            }
            return Files;
        }

        public static Google.Apis.Drive.v2.Data.File uploadFile(DriveService _service, string _uploadFile, string _parent)
        {

            if (System.IO.File.Exists(_uploadFile))
            {
                Google.Apis.Drive.v2.Data.File body = new Google.Apis.Drive.v2.Data.File();
                body.Title = System.IO.Path.GetFileName(_uploadFile);
                body.Description = "File uploaded by Steam Backup";
                body.MimeType = GetMimeType(_uploadFile);
                body.Parents = new List<ParentReference>() { new ParentReference() { Id = _parent } };

                // File's content.
                byte[] byteArray = System.IO.File.ReadAllBytes(_uploadFile);
                System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);
                try
                {
                    FilesResource.InsertMediaUpload request = _service.Files.Insert(body, stream, GetMimeType(_uploadFile));
                    request.Upload();
                    return request.ResponseBody;
                }
                catch (Exception e)
                {
                    //Form1.AppendOutputText("ERROR (uploadFile)1: " + e.Message, Color.Red);
                    return null;
                }
            }
            else
            {
                //Form1.AppendOutputText("ERROR (uploadFile)2: " + rm.GetString("fileDoesntExist") + ": " + _uploadFile, Color.Red);
                return null;
            }
        }
        private static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }
    }
}
