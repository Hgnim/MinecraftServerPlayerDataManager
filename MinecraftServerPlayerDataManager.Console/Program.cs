using MinecraftServerPlayerDataManager.Core;

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
                    case 1: {
                            ///<summary>
                            /// 检查文件块的彩色输出日志
                            /// </summary>
                            /// <param name="ynStr">顾名思义，按照原来的往里塞就行</param>
                            /// <param name="ucData">原名usercacheData，数组含义与原变量相同</param>
                            /// <param name="verifyDir">顾名思义，按照原来的往里塞就行</param>
                            /// <param name="dataSuffix">顾名思义，按照原来的往里塞就行</param>
                            static void CheckFileOutputLog(byte[] ynId, string[] ucData, string verifyDir, string[] dataSuffix, string RunPath) {
                                string[] ynStr=new string[ynId.Length];
                                for(int i=0; i < ynId.Length; i++) {
                                    switch(ynId[i]) {
                                        case 0: ynStr[i] = "成功"; break;
                                        case 1: ynStr[i] = "失败";break;
                                        case 2: ynStr[i] = "半成功"; break;
									}
                                }
                                if (ynId[2] == 0) Console.ForegroundColor = ConsoleColor.Green;
                                else if (ynId[2] == 2) Console.ForegroundColor = ConsoleColor.DarkYellow;
                                else Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("验证" + ynStr[2]);
                                Console.ForegroundColor = defFore;
                                Console.Write(": ");
                                Console.ForegroundColor = ConsoleColor.Blue;
                                Console.ForegroundColor = ConsoleColor.Gray;
                                Console.Write("[");
                                Console.ForegroundColor = ConsoleColor.Blue;
                                Console.Write(ucData[0]);
                                Console.ForegroundColor = ConsoleColor.Gray;
                                Console.Write(", ");
                                Console.ForegroundColor = ConsoleColor.DarkBlue;
                                Console.Write(ucData[1]);
                                Console.ForegroundColor = ConsoleColor.Gray;
                                Console.Write("]");
                                //Console.Write(ucData[0] + "[" + ucData[1] + "]");
                                Console.ForegroundColor = defFore;
                                Console.Write(" >>>> " + "\'" + RunPath + "/world/");
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write(verifyDir);
                                Console.ForegroundColor = defFore;
                                Console.Write("/" + ucData[1] + dataSuffix[0] + "\'[");
                                if (ynId[0] == 0) Console.ForegroundColor = ConsoleColor.Green;
                                else Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(ynStr[0]);
                                Console.ForegroundColor = defFore;
                                if (verifyDir == "playerdata") {
                                    Console.Write("] & " + "\'" + RunPath + "/world/");
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.Write(verifyDir);
                                    Console.ForegroundColor = defFore;
                                    Console.Write("/" + ucData[1] + dataSuffix[1] + "\'[");
                                    if (ynId[1] == 0) Console.ForegroundColor = ConsoleColor.Green;
                                    else Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write(ynStr[1]);
                                }
                                Console.ForegroundColor = defFore;
                                Console.Write("]");
                                Console.WriteLine();
                            }
							/// <summary>
							/// 检查json文件块的彩色输出
							/// </summary>
							/// <param name="tempStr">顾名思义，按照原来的往里塞就行</param>
							/// <param name="verifyDir">顾名思义，按照原来的往里塞就行</param>
							/// <param name="file">顾名思义，按照原来的往里塞就行</param>
							static void CheckJsonOutputLog(string[] tempStr, string verifyDir, string file, string RunPath) {
								if (tempStr[1] == "成功")
									Console.ForegroundColor = ConsoleColor.Green;
								else
									Console.ForegroundColor = ConsoleColor.Red;
								Console.Write("验证" + tempStr[1]);
								Console.ForegroundColor = defFore;
								Console.Write(": " + RunPath + "/world/");
								Console.ForegroundColor = ConsoleColor.Yellow;
								Console.Write(verifyDir);
								Console.ForegroundColor = defFore;
								Console.Write("/" + Path.GetFileName(file) + " >>>> ");

								if (tempStr[3] == null) {
									Console.ForegroundColor = ConsoleColor.Blue;
									Console.Write(tempStr[2]);
								}
								else {
									Console.ForegroundColor = ConsoleColor.Gray;
									Console.Write("[");
									Console.ForegroundColor = ConsoleColor.Blue;
									Console.Write(tempStr[2]);
									Console.ForegroundColor = ConsoleColor.Gray;
									Console.Write(", ");
									Console.ForegroundColor = ConsoleColor.DarkBlue;
									Console.Write(tempStr[3]);
									Console.ForegroundColor = ConsoleColor.Gray;
									Console.Write("]");
								}
								Console.ForegroundColor = defFore;
								Console.WriteLine();
							}
							void loFunc(uint id, params object[] logArg) {
                                switch (id) {
                                    case 0:
                                        Console.WriteLine("正在读取文件:\'" + logArg[0] + "/usercache.json" + "\'");
                                        break;
                                    case 1:
                                        Console.WriteLine("正在读取: \"name\":\"" + logArg[0] + "\",\"uuid\":\"" + logArg[1] + "\"");
                                        break;
                                    case 2:
                                        Console.WriteLine("一共读取到" + logArg[0] + "个数据");
                                        break;
                                    case 3:
                                        Console.WriteLine("开始将\'" + logArg[0] + "/usercache.json" + "\'与\'" + logArg[0] + "/world/" + logArg[1] + "\'进行验证");
                                        break;
                                    case 4:
                                    case 8:
                                        CheckFileOutputLog((logArg[0] as byte[])!, (logArg[1] as string[])!, (logArg[2] as string)!, (logArg[3] as string[])!, (logArg[4] as string)!);
                                        break;
                                    case 5:
										Console.WriteLine("开始将\'" + logArg[0] + "/world/" + logArg[1] + "\'与\'" + logArg[0] + "/usercache.json" + "\'进行验证");
                                        break;
                                    case 6:
                                    case 9:
                                        CheckJsonOutputLog((logArg[0] as string[])!, (logArg[1] as string)!, (logArg[2] as string)!, (logArg[3] as string)!);
                                        break;
                                    case 7:
										Console.WriteLine();
										Console.WriteLine();
										Console.WriteLine("--------*验证结果*--------");
										Console.Write("一共扫描了" + ((ulong)logArg[0]).ToString() + "个对象");
										if ((ulong)logArg[1] != 0)
											Console.Write("；" + ((ulong)logArg[1]).ToString() + "个成功");
										if ((ulong)logArg[2] != 0)
											Console.Write("；" + ((ulong)logArg[2]).ToString() + "个失败");
										if ((ulong)logArg[3] != 0)
											Console.Write("；" + ((ulong)logArg[3]).ToString() + "个半成功");
										Console.WriteLine("。");
										break;
                                    case 10:
										Console.WriteLine("--------*未发现潜在问题*--------");
                                        break;
								}
                            }
                            MspdmAction.IPlayerDataFile.logOutput lo = loFunc;
                            maction.PlayerDataFile.CheckPlayerDataFile(lo);
                        }
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