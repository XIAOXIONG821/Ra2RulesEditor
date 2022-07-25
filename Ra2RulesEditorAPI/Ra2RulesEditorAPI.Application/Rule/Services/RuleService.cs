using Ra2RulesEditorAPI.Core.Model;
using Ra2RulesEditorAPI.Core.Utils;

namespace Ra2RulesEditorAPI.Application.Rule.Services
{
    public class RuleService : ITransient
    {
        private readonly IRepository<RuleInfoModel> _ruleInfoRepository;
        private readonly IniFileHelper _iniHelper;

        public RuleService(IRepository<RuleInfoModel> ruleInfoRepository, IniFileHelper iniHelper)
        {
            _ruleInfoRepository = ruleInfoRepository;
            _iniHelper = iniHelper;
        }

        /// <summary>
        /// 游戏相关的设置列表
        /// </summary>
        public List<RuleInfoModel> SettingList { get; set; } = new(){
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
        public List<RuleInfoModel> TypeList { get; set; } = new()
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

        public List<RuleInfoDtoModel> GetRuleListBySectionNameAndDelegate(string sectionName, KeyTypeEnum keyType)
        {
            var keyList = _iniHelper.ReadKeys(sectionName);

            var ruleList = new List<RuleInfoDtoModel>();

            // 获取所有待查询的name.
            var queryNameList = new List<string>();

            var index = 0;
            foreach (var keyName in keyList)
            {
                var value = _iniHelper.ReadString(sectionName, keyName);

                if (keyType == KeyTypeEnum.Key)
                {
                    queryNameList.Add(keyName);
                }
                else if (keyType == KeyTypeEnum.Section)
                {
                    queryNameList.Add(value);
                }

                index++;
                // 拼装一些数据, remark 最后再拼装
                ruleList.Add(new()
                {
                    Id = index,
                    KeyName = keyName,
                    CurrentValue = value,
                    Remark = "",
                    SectionName = sectionName,
                    KeyType = keyType
                });
            }

            // 使用 name 去 in 查询
            var query = _ruleInfoRepository.AsQueryable(false);

            var dbDataList = query.Where(a => queryNameList.Contains(a.KeyName) && a.KeyType == keyType)
                .Select(a => new RuleInfoModel()
                {
                    KeyName = a.KeyName,
                    Remark = a.Remark,
                }).ToList();
            if (keyType == KeyTypeEnum.Key)
            {
                foreach (var item in dbDataList)
                {
                    // 将备注修改到 集合的对象中
                    ruleList.Where(a => a.KeyName == item.KeyName).First().Remark = item.Remark;
                }
            }
            else if (keyType == KeyTypeEnum.Section)
            {
                foreach (var item in dbDataList)
                {
                    // 将备注修改到 集合的对象中
                    ruleList.Where(a => a.CurrentValue == item.KeyName).First().Remark = item.Remark;
                }
            }

            return ruleList;
        }

        public void AddOrUpdateKeyRemark(string keyName, string remark)
        {
            var data = _ruleInfoRepository.FirstOrDefault(a => a.KeyName == keyName && a.KeyType == KeyTypeEnum.Key);
            if (data == null)
            {
                _ruleInfoRepository.InsertAsync(new RuleInfoModel() { KeyName = keyName, Remark = remark, KeyType = KeyTypeEnum.Key });
            }
            else
            {
                data.Remark = remark;
            }
        }

        public void AddOrUpdateSectionRemark(string sectionName, string remark)
        {
            var data = _ruleInfoRepository.FirstOrDefault(a => a.KeyName == sectionName && a.KeyType == KeyTypeEnum.Section);
            if (data == null)
            {
                _ruleInfoRepository.InsertAsync(new RuleInfoModel() { KeyName = sectionName, Remark = remark, KeyType = KeyTypeEnum.Section });
            }
            else
            {
                data.Remark = remark;
            }
        }

        public List<RuleInfoModel> SearchRuleInfo(string keyName)
        {
            var list = _ruleInfoRepository.AsQueryable(false)
                // 此处不可以使用vs的Contains忽略大小写的传参建议.会报错..
                .Where(a => a.KeyName.ToLower().Contains(keyName.ToLower()) && a.KeyType == KeyTypeEnum.Key)
                .OrderBy(a => a.KeyName.Length) // 名称长度较短的排在上面
                .Take(50) // 只查询前 50 的数据
                .ToList();

            return list ?? new List<RuleInfoModel>();
        }
    }
}