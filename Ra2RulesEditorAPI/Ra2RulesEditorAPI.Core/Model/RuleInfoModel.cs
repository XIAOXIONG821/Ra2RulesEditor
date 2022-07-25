using System.ComponentModel.DataAnnotations.Schema;

using Furion.DatabaseAccessor;
using Furion.DependencyInjection;

namespace Ra2RulesEditorAPI.Core.Model
{
    [Table("RuleInfo")]
    public class RuleInfoModel : EntityBase
    {
        public string? KeyName { get; set; }
        public string? Remark { get; set; }

        /// <summary>
        /// key的类型 (1:section , 2:属性)
        /// </summary>
        public KeyTypeEnum? KeyType { get; set; }
    }

    public enum KeyTypeEnum
    {
        Section = 1,
        Key = 2
    }

    [SuppressSniffer]
    public class RuleInfoDtoModel : RuleInfoModel
    {
        public string? CurrentValue { get; set; }
        public string? SectionName { get; set; }
    }
}