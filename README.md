# 我的世界服务器玩家数据文件管理器
对软件有更好的建议或发现了bug，欢迎提出议题。<br/>
欢迎提交拉取请求。但如果需要提交大幅更改的拉取请求，请联系作者。
## 简介
- 该软件可以帮助更好的管理我的世界服务端中玩家的数据文件。
  - 检查是否有孤立的玩家数据(通过usercache.json文件判断)
  - 通过玩家的UUID寻找匹配的玩家ID或通过玩家ID寻找匹配的玩家UUID
  - 删除指定玩家的数据文件
  - 清除孤立的玩家数据(该功能处于测试版，请谨慎使用)(如果最近清除了usercache.json文件，请不要使用此功能。)
  - 删除数据文件时自动备份被删除的文件，防止误操作。
  - 注：该软件无法更改玩家数据文件的内容。
- 该软件必需要安装.net 8.0 runtime框架才能运行。
- 支持windows和linux操作系统，操作更偏向于linux操作系统

## 开始使用:
- 检查是否安装了.net 8.0 runtime框架。如果没有，请前往[此处下载和安装](https://dotnet.microsoft.com/zh-cn/download/dotnet/8.0)
- [下载软件](https://github.com/Hgnim/MinecraftServerPlayerDataManager/releases/latest)
- 在MC服务端根目录运行软件或指定MC服务端根目录，或者直接将软件放入MC服务端的根目录
- 需要保证这些关键文件夹的位置对应
  ```shell
  mc_server_root
  ├── world
  │   ├── advancements
  │   ├── playerdata
  │   └── stats
  ├── usercache.json
  └── mspdm (Windows系统则是: mspdm.exe)
  ```
  如果无法保证这些文件夹和文件位置在对应的位置，则说明该软件可能不支持你的MC服务端。
- Linux用户所需要的步骤(Windows用户可跳过这一步): ```chmod +x mspdm``` #给予软件的执行权限
- Linux用户输入./mspdm使用该软件，Windows用户启动终端后启动该程序以使用该软件

enjoy!
## 注意事项
- 该软件只在Paper1.21.x和Paper1.20.x版本中测试过。其它版本不保证能100%完美使用。
- 请查阅相关资料和阅读该软件的资料以自行判断该软件是否适配自己的MC服务端。
- 开发者已经尽可能将软件会造成致命操作的可能性降到最低。如果是因不当使用或在不了解软件是否兼容自己的服务端时使用或进行敏感操作时不备份文件而导致的致命后果，开发者概不负责。

## 命令帮助
```
命令帮助:
使用:
mspdm [options] [file-path]
mspdm <{-fd|-D}> <{-pid|-uuid}> <{pid|uuid}> [file-path]
options:
"--checkData" or "-cd": 检查玩家数据是否对应
"--findData" or "-fd": 通过UUID或玩家ID搜索玩家数据，需要配合-uuid或--pid的其中一项使用。
"--deleteData" or "-D": 删除指定的玩家数据，需要配合-uuid或--pid的其中一项使用。(删除的文件会备份到"mspdm_DeleteFiles"文件夹，防止误操作而导致的致命问题)
"--clearJunkData" or "-C": 寻找不在usercache.json中的无头数据文件后询问并删除。(此功能处于测试状态，请谨慎使用。删除的文件会备份到"mspdm_DeleteFiles"文件夹)
"-uuid [PlayerUUID]": 玩家UUID输入
"-pid [PlayerID]": 玩家ID输入
command:
"'--version' or '--help'": 帮助信息与版本信息
file-path: 提供服务端文件根目录
提示: 目前关于删除数据相关的功能只包含删除文件，不会更改usercache.json文件的内容。后续的更新会弥补这一点。
```
