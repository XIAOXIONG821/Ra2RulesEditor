namespace MyModel
{
    public enum KeyTypeEnum
    {
        Section = 1,
        Key = 2
    }

    public class RulesInfoDtoModel : RulesInfoModel
    {
        public string? CurrentValue { get; set; }
        public string? SectionName { get; set; }
    }

    public class RulesInfoModel
    {
        public int Id { get; set; }
        public string? KeyName { get; set; }
        public string? Remark { get; set; }

        /// <summary>
        /// key的类型 (1:section , 2:属性)
        /// </summary>
        public KeyTypeEnum? KeyType { get; set; }
    }
}