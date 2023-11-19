using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace MiHoYoStarter
{
    [Serializable]
    public class MiHoYoAccount
    {
        public string Name { get; set; }

        /// <summary>
        /// 每個遊戲保存的資料存在不同的目錄
        /// </summary>
        public string SaveFolderName { get; set; }

        /// <summary>
        /// 註冊表記住帳戶資訊的鍵值位置
        /// </summary>
        public string AccountRegKeyName { get; set; }

        /// <summary>
        /// 註冊表記住帳戶資訊的鍵值名
        /// </summary>
        public string AccountRegValueName { get; set; }

        /// <summary>
        /// 註冊表記住帳戶資訊的鍵值數據
        /// </summary>
        public string AccountRegDataValue { get; set; }

        public MiHoYoAccount()
        {
        }


        public MiHoYoAccount(string saveFolderName, string accountRegKeyName, string accountRegValueName)
        {
            SaveFolderName = saveFolderName;
            AccountRegKeyName = accountRegKeyName;
            AccountRegValueName = accountRegValueName;
        }

        public void WriteToDisk()
        {
            File.WriteAllText(Path.Combine(Application.StartupPath, "UserData", SaveFolderName, Name),
                new JavaScriptSerializer().Serialize(this));
        }

        public static void DeleteFromDisk(string userDataPath, string name)
        {
            File.Delete(Path.Combine(userDataPath, name));
        }

        public static MiHoYoAccount CreateFromDisk(string userDataPath, string name)
        {
            string p = Path.Combine(userDataPath, name);
            string json = File.ReadAllText(p);
            return new JavaScriptSerializer().Deserialize<MiHoYoAccount>(json);
        }


        public void ReadFromRegistry()
        {
            AccountRegDataValue = GetStringFromRegistry(AccountRegValueName);
        }

        public void WriteToRegistry()
        {
            SetStringToRegistry(AccountRegValueName, AccountRegDataValue);
        }

        protected string GetStringFromRegistry(string key)
        {
            object value = Registry.GetValue(AccountRegKeyName, key, "");
            if (value == null)
            {
                throw new Exception($@"註冊表{AccountRegKeyName}\{key}中沒有找到帳戶資訊");
            }
            return Encoding.UTF8.GetString((byte[])value);
        }

        protected void SetStringToRegistry(string key, string value)
        {
            Registry.SetValue(AccountRegKeyName, key, Encoding.UTF8.GetBytes(value));
        }
    }
}