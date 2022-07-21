using Microsoft.EntityFrameworkCore;

using MyModel;

namespace MyDbContext
{
    public class SqliteDbContext : DbContext
    {
        public SqliteDbContext(DbContextOptions<SqliteDbContext> options) : base(options)
        {
        }

        public DbSet<RulesInfoModel> RulesInfo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RulesInfoModel>()
                .Property(e => e.KeyType)
                // 枚举转int
                .HasConversion<int>();

            //modelBuilder.Entity<RulesInfoModel>()
            //    .HasData(new RulesInfoModel
            //    {
            //        Id = 1,
            //        KeyName = "test",
            //        Remark = "remark",
            //        KeyType = KeyTypeEnum.Section
            //    },
            //    new RulesInfoModel
            //    {
            //        Id = 2,
            //        KeyName = "test2",
            //        Remark = "remark2",
            //        KeyType = KeyTypeEnum.Key
            //    });
        }
    }
}