Console.WriteLine("123");
//var path = @"E:\Desktop\keyvalue.txt";

//IServiceCollection services = new ServiceCollection();

//services.AddDbContext<SqliteDbContext>(options =>
//{
//    options.UseSqlite(@"Data Source=E:\Desktop\RulesDB.db",
//        (optionsBuilder) =>
//        {
//            optionsBuilder.MigrationsAssembly("DbMigrations");
//        });
//});

//var service = services.BuildServiceProvider();

//var db = service.GetService<SqliteDbContext>();

//var fileDataList = File.ReadAllLines(path);

//foreach (var item in fileDataList)
//{
//    if (item.Length > 0)
//    {
//        var data = item.Replace(" ", "");
//        var dataArr = data.Split("=");
//        db.RulesInfo.Add(new()
//        {
//            KeyName = dataArr[0],
//            Remark = dataArr[1],
//            KeyType = KeyTypeEnum.Key
//        });
//    }

//}
//db.SaveChanges();