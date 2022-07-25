using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Furion.DatabaseAccessor;
using Furion.DependencyInjection;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
