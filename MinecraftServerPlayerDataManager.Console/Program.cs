using MinecraftServerPlayerDataManager;

namespace MinecraftServerPlayerDataManager.Console {
	using Console = System.Console;
	class Program
    {
        static readonly string version = "1.1.0.20241009";
        public static System.ConsoleColor defBack;
        public static System.ConsoleColor defFore;

       static string filePath = ".";
        static void Main(string[] args)
        {
            if (args.Length == 0) {
#if DEBUG
                Console.Write("debug mode, input arguments: ");
                args = Console.ReadLine()!.Split(' ');
#else
                args = ["--version"];
#endif
			}

            defBack = Console.BackgroundColor;defFore=Console.ForegroundColor;
            int mode=-1;            
            string[] inputPlayerData = new string[2];
            try
            {
                for (int i=0; i < args.Length;i++)
                {
                    switch (args[i])
                    {
                        case "-cd":
                        case "--checkData":
                            mode = 1;//检查玩家数据是否对应模式
                            break;
                        case "-fd":
                        case "--findData":
                            mode = 2;//根据提供的信息查询玩家数据
                            break;
                        case "-D":
                        case "--deleteData":
                            mode = 3;//根据提供的信息查询并删除玩家数据
                            break;
                        case "-C":
                        case "--clearJunkData":
                            mode = 4;
                            break;
                        case "-pid":
                            i++;
                            inputPlayerData[0] = args[i];
                            break;
                        case "-uuid":
                            i++;
                            inputPlayerData[1] = args[i];
                            break;
                        case "-v":
                        case "-h":
                        case "--version":
                        case "--help":
                        case "-help":
                            helpOutput();
                            goto exitProgram;
                        default:
                            if (i == args.Length - 1)
                            {
                                filePath = args[i];
                            }
                            break;
                    }                     
                }
                MspdmAction maction = new(filePath, defBack, defFore);

                
                switch (mode)
                {
                    case 1:
                        maction.PlayerDataFile.CheckPlayerDataFile();
                        break;
                    case 2:
                        maction.PlayerDataFile.FindPlayerDataFile(inputPlayerData);
						break;
                    case 3:
                        maction.PlayerDataFile.DeletePlayerDataFile(inputPlayerData);
						break;
                    case 4:
						maction.PlayerDataFile.ClearJunkPlayerDataFile();
						break;
                    default:{ helpOutput();break;}
                }
            }
            catch { helpOutput(); }

            static void helpOutput()
            {
                Console.WriteLine(
                    "我的世界服务器玩家数据文件管理器(MinecraftServerPlayerDataManager):" + "\r\n" +
                    "版本: V" + version + "; 版权所有者: Copyright (C) 2023-2024 Hgnim, All rights reserved." + "\r\n" +
                    "Github: https://github.com/Hgnim/MinecraftServerPlayerDataManager"+"\r\n"+
                    "命令帮助:" + "\r\n" +
                    "使用:" + "\r\n"+
                    "mspdm [options] [file-path]" +"\r\n"+
                    "mspdm <{-fd|-D}> <{-pid|-uuid}> <{pid|uuid}> [file-path]" +"\r\n"+
                    "options:" +"\r\n"+                    
                    "\"--checkData\" or \"-cd\": 检查玩家数据是否对应"+"\r\n"+
                    "\"--findData\" or \"-fd\": 通过UUID或玩家ID搜索玩家数据，需要配合-uuid或--pid的其中一项使用。" + "\r\n" +
                    "\"--deleteData\" or \"-D\": 删除指定的玩家数据，需要配合-uuid或--pid的其中一项使用。(删除的文件会备份到\"mspdm_DeleteFiles\"文件夹，防止误操作而导致的致命问题)" +"\r\n" +
                    "\"--clearJunkData\" or \"-C\": 寻找不在usercache.json中的无头数据文件后询问并删除。(此功能处于测试状态，请谨慎使用。删除的文件会备份到\"mspdm_DeleteFiles\"文件夹)" +"\r\n"+
                    "\"-uuid [PlayerUUID]\": 玩家UUID输入"+"\r\n"+
                    "\"-pid [PlayerID]\": 玩家ID输入"+"\r\n"+
                    "command:"+"\r\n"+
                    "\"\'--version\' or \'--help\'\": 帮助信息与版本信息" + "\r\n" +
                    "file-path: 提供服务端文件根目录"+"\r\n"+
                    "提示: 目前关于删除数据相关的功能只包含删除文件，不会更改usercache.json文件的内容。后续的更新会弥补这一点。" +"\r\n"+
                    ""
                );
            }
exitProgram:;
        }
        

        
        
    }
}