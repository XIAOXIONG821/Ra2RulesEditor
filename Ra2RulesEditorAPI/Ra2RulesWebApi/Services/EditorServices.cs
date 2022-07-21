using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using MyDbContext;

using MyModel;

using MyUtils;

namespace Ra2RulesWebApi.Services
{
    public class EditorServices
    {

        private SqliteDbContext _dbContext;
        private IniFileHelper _iniHelper;

        public EditorServices(SqliteDbContext dbContext, IniFileHelper iniHelper)
        {
            _dbContext = dbContext;
            _iniHelper = iniHelper;
        }


        /// <summary>
        /// 游戏相关的设置列表
        /// </summary>
        public List<RulesInfoModel> SettingList { get; set; } = new(){
            new()
            {
                KeyName="General",
                Remark="基本",
            },
            new()
            {
                KeyName="CrateRules",
                Remark="升级工具箱",
            },
             new()
            {
                KeyName="MultiplayerDialogSettings",
                Remark="多人游戏设置",
            }, new()
            {
                KeyName="AI",
                Remark="电脑AI设置",
            },
        };

        /// <summary>
        /// 对象种类列表
        /// </summary>
        public List<RulesInfoModel> TypesList { get; set; } = new()
        {
            new()
        {
            KeyName="BuildingTypes",
            Remark="建筑类型",
        },
            new()
            {
                KeyName="InfantryTypes",
                Remark="步兵类型",
                },
            new()
            {
                KeyName="VehicleTypes",
                Remark="车船类型",
             },
            new()
            {
                KeyName="AircraftTypes",
                Remark="飞机类型",
            },
            new()
            {
                KeyName="Countries",
                Remark="国家",
            }, new()
            {
                KeyName="SuperWeaponTypes",
                Remark="超级武器类型",
            },
        };


        public delegate string ReadRemarkDelegate(string keyName, string value);


        public List<RulesInfoDtoModel> GetRulesListBySectionNameAndDelegate(string sectionName, ReadRemarkDelegate readRemarkDelegate)
        {

            var keyList = _iniHelper.ReadKeys(sectionName);

            var rulesList = new List<RulesInfoDtoModel>();

            var index = 0;
            foreach (var keyName in keyList)
            {
                var value = _iniHelper.ReadString(sectionName, keyName);

                var remark = readRemarkDelegate(keyName, value);

                //var remark = QueryRemark(value, KeyTypeEnum.Section);
                //var remark2 = QueryRemark(keyName, KeyTypeEnum.Key);

                index++;
                rulesList.Add(new()
                {
                    Id = index,
                    KeyName = keyName,
                    CurrentValue = value,
                    Remark = remark,
                    SectionName = sectionName,
                });
            }

            return rulesList;
        }
        public string QueryRemark(string keyName, KeyTypeEnum keyType)
        {
            return _dbContext.RulesInfo.AsNoTracking().Where(a => a.KeyName == keyName && a.KeyType == keyType)
            .Select(a => a.Remark).FirstOrDefault() ?? "";
        }
        public void AddOrUpdateKeyRemark(string keyName, string remark)
        {
            var data = _dbContext.RulesInfo.FirstOrDefault(a => a.KeyName == keyName && a.KeyType == KeyTypeEnum.Key);
            if (data == null)
            {
                _dbContext.RulesInfo.Add(new() { KeyName = keyName, Remark = remark, KeyType = KeyTypeEnum.Key });
            }
            else
            {
                data.Remark = remark;
            }
            _dbContext.SaveChanges();

        }

        public void AddOrUpdateSectionRemark(string sectionName, string remark)
        {
            var data = _dbContext.RulesInfo.FirstOrDefault(a => a.KeyName == sectionName && a.KeyType == KeyTypeEnum.Section);
            if (data == null)
            {
                _dbContext.RulesInfo.Add(new() { KeyName = sectionName, Remark = remark, KeyType = KeyTypeEnum.Section });
            }
            else
            {
                data.Remark = remark;
            }
            _dbContext.SaveChanges();

        }

        public List<RulesInfoModel> SearchRulesInfo(string keyName)
        {
            var list = _dbContext.RulesInfo
                // 此处不可以使用vs的Contains忽略大小写的传参建议.会报错..
                .Where(a => a.KeyName.ToLower().Contains(keyName.ToLower()) && a.KeyType == KeyTypeEnum.Key)
                .OrderBy(a => a.KeyName.Length) // 名称长度较短的排在上面                
                .Take(50) // 只查询前 50 的数据
                .ToList();

            return list ?? new List<RulesInfoModel>();
        }
    }
}
