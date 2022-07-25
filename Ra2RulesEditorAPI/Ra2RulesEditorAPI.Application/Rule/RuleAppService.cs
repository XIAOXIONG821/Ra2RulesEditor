using Ra2RulesEditorAPI.Application.Rule.Services;
using Ra2RulesEditorAPI.Core.Model;
using Ra2RulesEditorAPI.Core.Utils;

namespace Ra2RulesEditorAPI.Application.Rule
{
    public class RuleAppService : IDynamicApiController
    {
        private readonly RuleService _ruleService;
        private readonly IniFileHelper _iniHelper;

        public RuleAppService(RuleService ruleService, IniFileHelper iniHelper)
        {
            _ruleService = ruleService;
            _iniHelper = iniHelper;
        }

        // TODO: 查询多条 需要和前端虚拟表格 做分页.

        /// <summary>
        ///  查询 单位 下所有的 rulesInfo
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        [HttpGet]
        public List<RuleInfoDtoModel> GetRuleListBySectionName(string sectionName)
        {
            return _ruleService.GetRuleListBySectionNameAndDelegate(sectionName, KeyTypeEnum.Key);
        }

        /// <summary>
        /// 查询typesName 下所有的 rulesInfo
        /// </summary>
        /// <param name="typesName">游戏对象类型名称 (比如 "步兵类型")</param>
        /// <returns></returns>
        [HttpGet]
        public List<RuleInfoDtoModel> GetRuleListByTypeName(string typesName)
        {
            return _ruleService.GetRuleListBySectionNameAndDelegate(
                typesName, KeyTypeEnum.Section)
                .OrderByDescending(a => a.Remark)// 排序 ,按照汉字首字母 倒序. (空值会排到最后)
                .ToList();
        }

        /// <summary>
        /// 获取 设置列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<RuleInfoModel> GetSettingList()
        {
            return _ruleService.SettingList;
        }

        /// <summary>
        /// 获取 类型列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<RuleInfoModel> GetTypeList()
        {
            return _ruleService.TypeList;
        }

        /// <summary>
        /// 添加或更新RuleInfo
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public bool AddOrUpdateRuleInfo(RuleInfoDtoModel data)
        {
            _iniHelper.WriteString(data.SectionName, data.KeyName, data.CurrentValue);

            _ruleService.AddOrUpdateKeyRemark(data.KeyName, data.Remark);

            return true;
        }

        /// <summary>
        /// 添加或更新Remark
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public bool AddOrUpdateSectionRemark(RuleInfoDtoModel data)
        {
            _ruleService.AddOrUpdateSectionRemark(data.KeyName, data.Remark);

            return true;
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        [HttpGet]
        public List<RuleInfoModel> SearchRuleInfo(string keyName)
        {
            var list = _ruleService.SearchRuleInfo(keyName);

            return list;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        [HttpDelete]
        public bool DeleteRule(string sectionName, string keyName)
        {
            _iniHelper.DeleteKey(sectionName, keyName);
            return true;
        }
    }
}