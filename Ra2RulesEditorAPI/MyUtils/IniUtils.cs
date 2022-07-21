using System.Runtime.InteropServices;
using System.Text;

namespace MyUtils
{
    public class IniApi
    {
        /// <summary>
        /// 读取 (int 类型)
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="noText"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        protected static extern int GetPrivateProfileInt(string section, string key, int noText, string filePath);

        /// <summary>
        /// 读取 (string 类型)
        /// </summary>
        /// <param name="section">欲在其中查找条目的小节名称。这个字串不区分大小写。如设为null，就在returnString缓冲区内装载这个ini文件所有小节的列表</param>
        /// <param name="key">欲获取的项名或条目名。这个字串不区分大小写。如设为null，就在returnString缓冲区内装载指定小节所有项的列表</param>
        /// <param name="noText">指定的条目没有找到时返回的默认值。可设为空（""）</param>
        /// <param name="returnString">指定一个字串缓冲区，长度至少为Size</param>
        /// <param name="size">指定装载到returnString缓冲区的最大字符数量</param>
        /// <param name="filePath">初始化文件的名字。如没有指定一个完整路径名，windows就在Windows目录中查找文件</param>
        /// 注意：如lpKeyName参数为null，那么returnString缓冲区会载入指定小节所有设置项的一个列表。
        /// 每个项都用一个NULL字符分隔，最后一个项用两个NULL字符中止。也请参考GetPrivateProfileInt函数的注解
        /// <returns></returns>
        [DllImport("kernel32")]
        protected static extern long GetPrivateProfileString(string section, string key, string noText, StringBuilder returnString, int size, string filePath);

        /// <summary>
        /// 读取 (byte[] 类型)
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="vDefault"></param>
        /// <param name="returnBytes"></param>
        /// <param name="size"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [DllImport("kernel32")]
        protected static extern uint GetPrivateProfileString(string section, string key, string noText, byte[] returnBytes, int size, string filePath);

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="section">要在其中写入新字串的小节名称。这个字串不区分大小写</param>
        /// <param name="key">要设置的项名或条目名。这个字串不区分大小写。用null可删除这个小节的所有设置项</param>
        /// <param name="lpString">指定为这个项写入的字串值。用null表示删除这个项现有的字串</param>
        /// <param name="filePath">初始化文件的名字。如果没有指定完整路径名，则windows会在windows目录查找文件。如果文件没有找到，则函数会创建它</param>
        /// <returns></returns>
        [DllImport("kernel32")]
        protected static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
    }

    /// <summary>
    /// ini文件类
    /// </summary>
    public class IniFileHelper : IniApi
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filePath">Ini文件路径</param>
        public IniFileHelper(string filePath)
        {
            this.FilePath = filePath;
        }

        #region 读

        /// <summary>
        /// 读Int数值
        /// </summary>
        /// <param name="sectionName">节</param>
        /// <param name="keyName">键</param>
        /// <param name="noVal">默认值</param>
        /// <returns></returns>
        public int ReadInt(string sectionName, string keyName, int noVal = 0)
        {
            return GetPrivateProfileInt(sectionName, keyName, noVal, this.FilePath);
        }

        /// <summary>
        /// 读取String值
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="keyName"></param>
        /// <param name="noText">如果没有查到则返回该值</param>
        /// <returns></returns>
        public string ReadString(string sectionName, string keyName, string noText = "")
        {
            return ReadString(sectionName, keyName, FilePath, noText);
        }

        /// <summary>
        /// 读取String值
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="keyName"></param>
        /// <param name="filePath">文件路径</param>
        /// <param name="noText">如果没有查到则返回该值</param>
        /// <returns></returns>
        public static string ReadString(string sectionName, string keyName, string filePath, string noText = "")
        {
            if (File.Exists(filePath))
            {
                StringBuilder sBuilder = new StringBuilder(1024);
                GetPrivateProfileString(sectionName, keyName, noText, sBuilder, 1024, filePath);
                return sBuilder.ToString();
            }
            else
            {
                throw new Exception("没有找到文件:[" + filePath + "]");
            }
        }

        /// <summary>
        /// 读取所有Section
        /// </summary>
        /// <returns></returns>
        public List<string> ReadAllSectionNames()
        {
            return ReadAllSectionNames(this.FilePath);
        }

        /// <summary>
        /// 读取 文件的所有 SectionName
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>字符串集合</returns>
        public static List<string> ReadAllSectionNames(string filePath)
        {
            List<string> result = new List<string>();
            Byte[] bytes = new Byte[65536];
            uint len = GetPrivateProfileString(null!, null!, null!, bytes, bytes.Length, filePath);
            int j = 0;
            for (int i = 0; i < len; i++)
                if (bytes[i] == 0)
                {
                    result.Add(Encoding.Default.GetString(bytes, j, i - j));
                    j = i + 1;
                }

            return result;
        }

        /// <summary>
        /// 读取 SectionName 下 所有的keys
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public List<string> ReadKeys(String sectionName)
        {
            return ReadKeys(sectionName, FilePath);
        }

        /// <summary>
        /// 读取 SectionName 下 所有的keys
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<string> ReadKeys(string sectionName, string filePath)
        {
            List<string> result = new List<string>();
            Byte[] bytes = new Byte[65536];
            uint len = GetPrivateProfileString(sectionName, null!, null!, bytes, bytes.Length, filePath);
            int j = 0;
            for (int i = 0; i < len; i++)
                if (bytes[i] == 0)
                {
                    result.Add(Encoding.Default.GetString(bytes, j, i - j));
                    j = i + 1;
                }

            return result;
        }

        /// <summary>
        /// 获取所有的 section和 key
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, Dictionary<string, string>> ReadAllSectionAndKeys()
        {
            return ReadAllSectionAndKeys(FilePath);
        }

        /// <summary>
        /// 获取所有的 section和 key
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Dictionary<string, Dictionary<string, string>> ReadAllSectionAndKeys(string filePath)
        {
            var resDictionary = new Dictionary<string, Dictionary<string, string>>();

            List<string> allSectionList = ReadAllSectionNames(filePath);

            foreach (var item in allSectionList)
            {
                List<string> allKeyList = ReadKeys(item, filePath);
                Dictionary<string, string> dataDictionary = new Dictionary<string, string>();
                foreach (var child in allKeyList)
                {
                    string sVal = ReadString(item, child, filePath);
                    dataDictionary.Add(child, sVal);
                }

                resDictionary.Add(item, dataDictionary);
            }

            return resDictionary;
        }

        #endregion 读

        #region 写

        /// <summary>
        /// 新增/修改  (如果存在则修改,否则添加,没有文件会创建)
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="keyName"></param>
        /// <param name="sVal"></param>
        /// <returns></returns>
        public bool WriteString(string sectionName, string keyName, string sVal)
        {
            return WriteString(sectionName, keyName, sVal, FilePath);
        }

        /// <summary>
        ///  新增/修改  (如果存在则修改,否则添加,没有文件会创建)
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="keyName"></param>
        /// <param name="sVal"></param>
        /// <param name="filePath">文件地址</param>
        /// <returns></returns>
        public static bool WriteString(string sectionName, string keyName, string sVal, string filePath)
        {
            long res = WritePrivateProfileString(sectionName, keyName, sVal, filePath);
            if (res == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion 写

        #region 删除

        /// <summary>
        /// 删除指定的 Section
        /// </summary>
        /// <param name="sectionName"></param>
        public void DeleteSection(string sectionName)
        {
            WritePrivateProfileString(sectionName, null!, null!, this.FilePath);
        }

        /// <summary>
        /// 删除全部 Section
        /// </summary>
        public void DeleteAllSection()
        {
            WritePrivateProfileString(null!, null!, null!, this.FilePath);
        }

        /// <summary>
        /// 删除指定的 key
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="keyName"></param>
        public void DeleteKey(string sectionName, string keyName)
        {
            WritePrivateProfileString(sectionName, keyName, null!, this.FilePath);
        }

        #endregion 删除
    }

    public class IniDictionaryHelper
    {
        #region 自定义读取

        /// <summary>
        /// 容器
        /// </summary>
        private Dictionary<string, Dictionary<string, string>> _dataDictionary = new Dictionary<string, Dictionary<string, string>>();

        public IniDictionaryHelper(string filePath)
        {
            // 获取所有的 section和 key
            this._dataDictionary = IniFileHelper.ReadAllSectionAndKeys(filePath);
        }

        /// <summary>
        /// 获取key的值
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public string Get(string sectionName, string keyName)
        {
            if (_dataDictionary.ContainsKey(sectionName))
            {
                var item = _dataDictionary[sectionName];
                if (item.ContainsKey(keyName))
                {
                    return item[keyName];
                }
            }

            return string.Empty;
        }

        #endregion 自定义读取
    }
}
