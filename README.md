# Ra2RulesEditor

红色警戒2 Rules 编辑器



## 支持的 ini 文件

- Rules.ini	(原版)
- Rulesmd.ini  (尤里复仇)
- rulesmo.ini  (心灵终结)



## 技术栈

- Next.js (React)
- .Net 6 + EF Core



## 如何运行项目

需要将`appsettings.json` 中的 `Sqlite`  改为自己本机的地址, 

`IniFilePath` 需要改为游戏目录中对应的 ini 地址

(文件都在 当前仓库的 `Files` 目录中)

```
"ConnectionStrings": {
  "Sqlite": "Data Source=E:\\Desktop\\RulesDB.db"
},
"IniFilePath": "E:\\Desktop\\rulesmo.ini"
```



## 打包

1. 执行 `package.json` 中的 `push` 命令. 会自动执行 PowerShell 脚本, 编译项目,同步到 C# 的 wwwroot 目录.

2. 接下来正常发布 C# Release 程序即可 (建议勾选生成单个文件,目录更清爽).

3. 浏览器访问 `http://localhost:5000/index.html`



## 题外话

```
本来是准备重构为 Next.js 全栈, 
使用 nodejs 操作 ini 文件, Prisma 访问数据库.
全部写完后, 
突然发现ini的库 非常的简陋(使用的人数倒是挺多).  
用正则实现解析ini, 还需要全部加载到内存中, 需要自己手动再保存到文件,
ini注释也不支持,
甚至保存到文件, 还会出现数据错乱,游戏加载不到正确的配置,导致弹窗.
找了一圈,也没找到其他的库,
nodejs调用系统api非常的麻烦,需要配置Python和C++环境,
环境都有, 还是报一些错误. 就果断放弃了.
最后只能采用把HTML文件复制到C#目录这种方案.
要不然每次使用都启动两个项目, 太痛苦了.
```



