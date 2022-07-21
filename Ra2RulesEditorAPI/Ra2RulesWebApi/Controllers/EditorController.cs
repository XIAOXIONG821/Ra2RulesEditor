
using Microsoft.AspNetCore.Mvc;

using MyDbContext;

using MyModel;

using MyUtils;

using Ra2RulesWebApi.Services;

namespace Ra2RulesWebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class EditorController : ControllerBase
    {
        private SqliteDbContext _dbContext;
        private IniFileHelper _iniHelper;
        private EditorServices _editorServices;

        public EditorController(SqliteDbContext dbContext, IniFileHelper iniHelper, EditorServices editorServices)
        {
            _dbContext = dbContext;
            _iniHelper = iniHelper;
            _editorServices = editorServices;
        }

        /// <summary>
        ///  查询 单位 下所有的 rulesInfo
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        [HttpGet]
        public List<RulesInfoDtoModel> GetRulesListBySectionName(string sectionName)
        {
            return _editorServices.GetRulesListBySectionNameAndDelegate(sectionName, (name, _) => _editorServices.QueryRemark(name, KeyTypeEnum.Key));
        }

        /// <summary>
        /// 查询typesName 下所有的 rulesInfo 
        /// </summary>
        /// <param name="typesName">游戏对象类型名称 (比如 "步兵类型")</param>
        /// <returns></returns>
        [HttpGet]
        public List<RulesInfoDtoModel> GetRulesListByTypesName(string typesName)
        {
            return _editorServices.GetRulesListBySectionNameAndDelegate(
                typesName, (_, value) => _editorServices.QueryRemark(value, KeyTypeEnum.Section))
                .OrderByDescending(a => a.Remark)// 排序 ,按照汉字首字母 倒序. (空值会排到最后)                
                .ToList();
        }

        [HttpGet]
        public List<RulesInfoModel> GetSettingList()
        {
            return _editorServices.SettingList;
        }

        [HttpGet]
        public List<RulesInfoModel> GetTypesList()
        {
            return _editorServices.TypesList;
        }

        [HttpPost]
        public bool AddOrUpdateRulesInfo(RulesInfoDtoModel data)
        {

            _iniHelper.WriteString(data.SectionName, data.KeyName, data.CurrentValue);

            _editorServices.AddOrUpdateKeyRemark(data.KeyName, data.Remark);

            return true;
        }

        [HttpPost]
        public bool AddOrUpdateSectionRemark(RulesInfoDtoModel data)
        {
            _editorServices.AddOrUpdateSectionRemark(data.KeyName, data.Remark);

            return true;
        }

        [HttpGet]
        public List<RulesInfoModel> SearchRulesInfo(string keyName)
        {
            var list = _editorServices.SearchRulesInfo(keyName);

            return list;
        }

        [HttpPost]
        public bool DeleteRule(RulesInfoDtoModel data)
        {
            _iniHelper.DeleteKey(data.SectionName, data.KeyName);
            return true;
        }

    }
}
