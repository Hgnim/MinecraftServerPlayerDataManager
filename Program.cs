using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;

namespace MinecraftServerPlayerDataManager
{
    class Program
    {
        static readonly string version = "1.1.0.20241009";
        public static System.ConsoleColor defBack;
        public static System.ConsoleColor defFore;

       static string filePath = ".";
        static void Main(string[] args)
        {
            if (args.Length == 0)
                args = ["--version"];
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
                if (!(File.Exists(filePath + "/usercache.json")))//检查路径是否正确
                {
                    Console.WriteLine("错误：提供的路径有误！请在MC服务端根目录执行或指定MC服务端根目录路径。");
                    goto exitProgram;
                }
                else
                {
                    if(filePath.Substring(filePath.Length-1,1)=="/" || filePath.Substring(filePath.Length - 1, 1) == "\\")
                        filePath=filePath[..^1];
                }

                List<CheckData.ResultsData> dataCheck_ResultsOutput = [];
                switch (mode)
                {
                    case 1:
                    case 4:                       
                        {
                            Console.WriteLine("正在读取文件:\'" + filePath + "/usercache.json" + "\'");
                            DelayUs();
                            ///用户数据临时存储变量
                            ///0: name
                            ///1: uuid
                             List<string[]> usercacheData=[];
                            ///最后的结果输出                            
                            CheckData.ResultsStats stats = new();                            
                            // StreamReader reader = File.OpenText(filePath + "/usercache.json");
                            // //usercacheFileData = reader.ReadToEnd();
                            // JsonTextReader jsonReader = new JsonTextReader(reader);
                            // JObject jsonObject = (JObject)JToken.ReadFrom(jsonReader);
                            //string str= jsonObject[""].ToList()[0]["name"].ToString();
                            // reader.Close();
#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
                            JArray jArry = (JArray)JsonConvert.DeserializeObject(File.ReadAllText(filePath + "/usercache.json"));
#pragma warning restore CS8600
                            //int jaCount = jArry.Count;
#pragma warning disable CS8602 // 解引用可能出现空引用。
                            for (int i = 0; i < jArry.Count; i++)
                            {
                                usercacheData.Add([jArry[i]["name"].ToString(), jArry[i]["uuid"].ToString()]);
                                Console.WriteLine("正在读取: \"name\":\"" + usercacheData[i][0] + "\",\"uuid\":\"" + usercacheData[i][1] + "\"");
                                DelayUs();
                            }
                            Console.WriteLine("一共读取到" + jArry.Count.ToString() + "个数据");
#pragma warning restore CS8602
                            DelayUs();
                            {
                                bool[] pass = new bool[2];
                                string verifyDir="";
                                string[] dataSuffix = new string[2];
                                for (int dirId = 1; dirId <= 3; dirId++)
                                {
                                    switch (dirId)
                                    {
                                        case 1:
                                            verifyDir = "playerdata";
                                            dataSuffix[0] = ".dat";
                                            dataSuffix[1] = ".dat_old";
                                            
                                            break;
                                        case 2:
                                            verifyDir = "advancements";
                                            dataSuffix[0] = ".json";
                                            break;
                                        case 3:
                                            verifyDir = "stats";
                                            dataSuffix[0] = ".json";
                                            break;
                                    }
                                    Console.WriteLine("开始将\'" + filePath + "/usercache.json" + "\'与\'" + filePath + "/world/"+ verifyDir + "\'进行验证");
                                    DelayUs();
                                    for (int i = 0; i < jArry.Count; i++)//使用usercache.json文件中的数据和别的文件夹进行对比验证
                                    {
                                        if (File.Exists(filePath + "/world/"+ verifyDir +"/" + usercacheData[i][1] + dataSuffix[0])) pass[0] = true;
                                        else pass[0] = false;
                                        if (verifyDir == "playerdata")//因为playerdata有.dat和.dat_old,其他文件夹只有.json，所以要添加判断语句
                                        {
                                            if (File.Exists(filePath + "/world/" + verifyDir + "/" + usercacheData[i][1] + dataSuffix[1])) pass[1] = true;
                                            else pass[1] = false;
                                        }

                                        string[] ynStr = new string[4];//用于判断的文本
                                        //ps:以前写的什么狗屎代码，看得犯恶心
                                        if (pass[0]) 
                                            ynStr[0] = "存在"; 
                                        else ynStr[0] = "丢失";
                                        if (verifyDir == "playerdata") 
                                        { 
                                            if (pass[1]) 
                                                ynStr[1] = "存在"; 
                                            else 
                                                ynStr[1] = "丢失";
                                        }
                                        if (verifyDir == "playerdata")
                                        {
                                            if (pass[0] && pass[1]) 
                                                ynStr[2] = "成功"; 
                                            else if (pass[0] && pass[1] == false) 
                                                ynStr[2] = "半成功"; 
                                            else 
                                                ynStr[2] = "失败";
                                        }
                                        else 
                                        { 
                                            if (pass[0]) 
                                                ynStr[2] = "成功"; 
                                            else 
                                                ynStr[2] = "失败"; 
                                        }
                                        /*if (verifyDir == "playerdata")
                                        {
                                            ynStr[3] = "验证" + ynStr[2] + ":" + usercacheData[i][0] + "[" + usercacheData[i][1] + "] >>>> " +
                                                    "\'" + filePath + "/world/" + verifyDir + "/" + usercacheData[i][1] + dataSuffix[0] + "\'[" + ynStr[0] + "] & " +
                                                    "\'" + filePath + "/world/" + verifyDir + "/" + usercacheData[i][1] + dataSuffix[1] + "\'[" + ynStr[1] + "]";                                                                                       
                                        }
                                        else
                                        {
                                            ynStr[3] = "验证" + ynStr[2] + ":" + usercacheData[i][0] + "[" + usercacheData[i][1] + "] >>>> " +
                                                    "\'" + filePath + "/world/" + verifyDir + "/" + usercacheData[i][1] + dataSuffix[0] + "\'[" + ynStr[0] + "]";
                                        }*/
                                        //将输出变为彩色输出
                                        if (verifyDir == "playerdata")
                                        {
                                            if (pass[0] && pass[1])
                                                stats.successNum++;
                                            else if (pass[0] && pass[1] == false)
                                                stats.halfSuccessNum++;
                                            else
                                                stats.failNum++;
                                        }
                                        else
                                        {
                                            if (pass[0])
                                                stats.successNum++;
                                            else
                                                stats.failNum++;
                                        }
                                        stats.allNum++;
                                        CheckData.CheckFileOutputLog(ynStr, usercacheData[i],verifyDir,dataSuffix);
                                        if (!(pass[0]))
                                        {
                                            dataCheck_ResultsOutput.Add(
                                            new()
                                            {
                                                type = CheckData.ResultsData.Type.CheckFile,
                                                ynStr = ynStr,
                                                ucData = usercacheData[i],
                                                verifyDir = verifyDir,
                                                dataSuffix = dataSuffix
                                            });
                                            //dataCheck_ResultsOutput.Add(ynStr[3]);
                                        }                                            
                                        DelayUs();
                                    }                                    
                                }
                                for(int dirId = 1; dirId <= 3; dirId++)
                                {
                                    switch (dirId)
                                    {
                                        case 1:
                                            verifyDir = "playerdata";
                                            dataSuffix[0] = ".dat";
                                            dataSuffix[1] = ".dat_old";

                                            break;
                                        case 2:
                                            verifyDir = "advancements";
                                            dataSuffix[0] = ".json";
                                            break;
                                        case 3:
                                            verifyDir = "stats";
                                            dataSuffix[0] = ".json";
                                            break;
                                    }
                                    Console.WriteLine("开始将\'" + filePath + "/world/" + verifyDir + "\'与\'" + filePath + "/usercache.json" + "\'进行验证");
                                    DelayUs();
                                    string[] files = Directory.GetFiles(filePath + "/world/" + verifyDir, "*.*");
                                    foreach (string file in files)
                                    {
                                        string[] tempStr = new string[5];
                                        pass[0] = false;
                                        for (int i = 0; i < usercacheData.Count; i++)
                                        {
                                            if (usercacheData[i][1]== Path.GetFileNameWithoutExtension(file))
                                            {
                                                pass[0] = true;
                                                tempStr[2] =usercacheData[i][0];
                                                tempStr[3] = usercacheData[i][1];
                                                break;
                                            }                                            
                                        }
                                        if (pass[0])
                                        {
                                            tempStr[1] = "成功";
                                        }
                                        else
                                        {
                                            tempStr[1] = "失败";
                                            tempStr[2] = "[未找到相应数据]";
                                        }
                                        //tempStr[0] = "验证" + tempStr[1] + ":" + filePath + "/world/" + verifyDir+"/"+ Path.GetFileName(file) + " >>>> " + tempStr[2];
                                        if (pass[0])
                                            stats.successNum++;
                                        else
                                            stats.failNum++;
                                        stats.allNum++;
                                        CheckData.CheckJsonOutputLog(tempStr, verifyDir, file);
                                        if (!(pass[0]))
                                        {
                                            dataCheck_ResultsOutput.Add(
                                            new()
                                            {
                                                type = CheckData.ResultsData.Type.CheckJson,
                                                tempStr = tempStr,
                                                verifyDir = verifyDir,
                                                file = file
                                            });
                                            //dataCheck_ResultsOutput.Add(tempStr[0]);
                                        }
                                        DelayUs();
                                    }
                                }
                               
                            }


                            Console.WriteLine();
                            Console.WriteLine();
                            Console.WriteLine("--------*验证结果*--------");
                            Console.Write("一共扫描了" + stats.allNum.ToString() + "个对象");
                            if (stats.successNum != 0)
                                Console.Write("；"+ stats.successNum.ToString() + "个成功");
                            if (stats.failNum != 0)
                                Console.Write("；"+stats.failNum.ToString() + "个失败");
                            if (stats.halfSuccessNum != 0)
                                Console.Write("；" + stats.halfSuccessNum.ToString() + "个半成功");
                            Console.WriteLine("。");
                            if (dataCheck_ResultsOutput.Count != 0)
                            {                                
                                for (int i = 0; i < dataCheck_ResultsOutput.Count; i++)
                                {
                                    //Console.WriteLine(dataCheck_ResultsOutput[i]); 
                                    switch (dataCheck_ResultsOutput[i].type)
                                    {
                                        case CheckData.ResultsData.Type.CheckFile:
                                            CheckData.CheckFileOutputLog(dataCheck_ResultsOutput[i].ynStr!, dataCheck_ResultsOutput[i].ucData!, dataCheck_ResultsOutput[i].verifyDir!, dataCheck_ResultsOutput[i].dataSuffix!);
                                            break;
                                        case CheckData.ResultsData.Type.CheckJson:
                                            CheckData.CheckJsonOutputLog(dataCheck_ResultsOutput[i].tempStr!, dataCheck_ResultsOutput[i].verifyDir!, dataCheck_ResultsOutput[i].file!);
                                            break;
                                    }
                                    DelayUs(); }
                                if (mode == 4)
                                    goto gotoDataAction;
                            }
                            else
                            {
                                Console.WriteLine("--------*未发现潜在问题*--------");
                            }
                            break;
                        }
                    case 2:
                    case 3:                       
gotoDataAction:;
                        {
                            string delFilePath = filePath + "/mspdm_DeleteFiles/";
                            void DelFile(string sourceFilePath, string subDir)//将源文件删除至备份文件夹
                            {
                                try
                                {
                                    Directory.CreateDirectory(delFilePath + subDir);//创建子文件夹，因为部分数据的名字是完全一样的，需要在此区分
                                    File.Move(sourceFilePath, delFilePath + subDir + "/" + Path.GetFileName(sourceFilePath));
                                    Console.WriteLine("\'" + sourceFilePath + "\'删除成功");
                                }
                                catch
                                {
                                    Console.WriteLine("\'" + sourceFilePath + "\'删除失败");
                                }
                            }
                            switch (mode)
                            {
                                case 4:
                                    {
                                        bool deleteConfirm = false;
                                        bool isNothing = true;
                                        Console.WriteLine("下列文件将被删除:");
confirm:;
                                        for (int i = 0; i < dataCheck_ResultsOutput.Count; i++)
                                        {
                                            switch (dataCheck_ResultsOutput[i].type)
                                            {
                                                case CheckData.ResultsData.Type.CheckJson:
                                                    if (!deleteConfirm)
                                                    {
                                                        Console.WriteLine(dataCheck_ResultsOutput[i].file!);
                                                        if(isNothing)isNothing = false;
                                                    }
                                                    else
                                                        DelFile(dataCheck_ResultsOutput[i].file!, dataCheck_ResultsOutput[i].verifyDir!);
                                                    break;
                                            }
                                            DelayUs();
                                        }                                       
                                        if (!deleteConfirm)
                                        {
                                            if (isNothing)
                                            {
                                                Console.WriteLine("没有找到需要被删除的垃圾文件。");
                                                goto exitProgram;
                                            }
                                            else
                                            {
                                                Console.Write("是否删除？(Y/N) ");
                                                string input = Console.ReadLine()!;
                                                switch (input.ToLower())
                                                {
                                                    case "y":
                                                        Directory.CreateDirectory(delFilePath);
                                                        deleteConfirm = true;
                                                        goto confirm;
                                                    case "n":
                                                    default:
                                                        Console.WriteLine("操作已取消！");
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    {
                                        string[] GetPlayerData = FindData.FindPlayerData(inputPlayerData);
                                        switch (GetPlayerData[0])
                                        {
                                            case "true":
                                                switch (mode)
                                                {
                                                    case 2:
                                                        Console.WriteLine("玩家ID: " + GetPlayerData[1]);
                                                        Console.WriteLine("玩家UUID: " + GetPlayerData[2]);
                                                        break;
                                                    case 3:
                                                        {
                                                            Console.WriteLine("已找到目标玩家的数据文件:\r\n" +
                                                                "玩家ID: " + GetPlayerData[1] + "\r\n" +
                                                                "玩家UUID: " + GetPlayerData[2]);
                                                            Console.Write("是否删除？(Y/N) ");
                                                            string input = Console.ReadLine()!;
                                                            switch (input.ToLower())
                                                            {
                                                                case "y":
                                                                    {
                                                                        Directory.CreateDirectory(delFilePath);

                                                                        DelFile(filePath + "/world/playerdata/" + GetPlayerData[2] + ".dat", "playerdata");
                                                                        DelFile(filePath + "/world/playerdata/" + GetPlayerData[2] + ".dat_old", "playerdata");
                                                                        DelFile(filePath + "/world/advancements/" + GetPlayerData[2] + ".json", "advancements");
                                                                        DelFile(filePath + "/world/stats/" + GetPlayerData[2] + ".json", "stats");
                                                                        Console.WriteLine("完成。");
                                                                    }
                                                                    break;
                                                                case "n":
                                                                default:
                                                                    Console.WriteLine("操作已取消！");
                                                                    break;
                                                            }
                                                        }
                                                        break;
                                                }
                                                break;
                                            case "error1":
                                                Console.WriteLine("错误：请提供玩家UUID或玩家ID！");
                                                goto exitProgram;
                                            case "error2":
                                                Console.WriteLine("错误：只能提供玩家UUID或玩家ID其中一项！");
                                                goto exitProgram;
                                            case "nothing":
                                                Console.WriteLine("没有找到该玩家的数据！");
                                                goto exitProgram;
                                        }
                                    }
                                    break;
                            }
                            
                            break;
                        }
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
        /// <summary>
        ///  微秒延时
        /// </summary>
        /// <param name="time">延时时间,单位:ms</param>
        /// <returns></returns>
        static double DelayUs(double time=0.8)
        {
            System.Diagnostics.Stopwatch stopTime = new();

            stopTime.Start();
            while (stopTime.Elapsed.TotalMilliseconds < time) { }
            stopTime.Stop();

            return stopTime.Elapsed.TotalMilliseconds;
        }

        /// <summary>
        /// 检查文件功能类
        /// </summary>
        static class CheckData
        {
            ///<summary>
            /// 检查文件块的彩色输出日志
            /// </summary>
            /// <param name="ynStr">顾名思义，按照原来的往里塞就行</param>
            /// <param name="ucData">原名usercacheData，数组含义与原变量相同</param>
            /// <param name="verifyDir">顾名思义，按照原来的往里塞就行</param>
            /// <param name="dataSuffix">顾名思义，按照原来的往里塞就行</param>
           internal static void CheckFileOutputLog(string[] ynStr, string[] ucData, string verifyDir,  string[] dataSuffix)
            {                
                if (ynStr[2] == "成功") Console.ForegroundColor = ConsoleColor.Green;
                else if (ynStr[2] == "半成功") Console.ForegroundColor = ConsoleColor.DarkYellow;
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
                Console.Write(" >>>> " + "\'" + filePath + "/world/");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(verifyDir);
                Console.ForegroundColor = defFore;
                Console.Write("/" + ucData[1] + dataSuffix[0] + "\'[");
                if (ynStr[0]=="存在") Console.ForegroundColor = ConsoleColor.Green;
                else Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(ynStr[0]);
                Console.ForegroundColor = defFore;
                if (verifyDir == "playerdata")
                {
                    Console.Write("] & " + "\'" + filePath + "/world/");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(verifyDir);
                    Console.ForegroundColor = defFore;
                    Console.Write("/" + ucData[1] + dataSuffix[1] + "\'[");
                    if (ynStr[1] == "存在") Console.ForegroundColor = ConsoleColor.Green;
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
            internal static void CheckJsonOutputLog(string[] tempStr,string verifyDir,string file)
            {
                if (tempStr[1]=="成功")
                    Console.ForegroundColor = ConsoleColor.Green;
                else
                    Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("验证" + tempStr[1]);
                Console.ForegroundColor = defFore;
                Console.Write(": " + filePath + "/world/");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(verifyDir);
                Console.ForegroundColor = defFore;
                Console.Write("/" + Path.GetFileName(file) + " >>>> ");

                if (tempStr[3] == null)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(tempStr[2]); }
                else
                {
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
            /// <summary>
            /// 最后数据结果存储变量
            /// </summary>
            internal class ResultsData
            {
                internal enum Type
                {
                    CheckFile,CheckJson
                }
                /// <summary>
                /// 数据类型
                /// </summary>
                internal Type type;
                //批注：将数据全部往里塞就行了，要怪就怪之前写的狗屎代码
                internal string[]? ynStr, ucData, dataSuffix, tempStr;
                internal string? verifyDir,file; 
                //internal bool[]? pass;
            }
            /// <summary>
            /// 结果统计存储类
            /// </summary>
            internal class ResultsStats
            {
                internal long allNum=0;
                internal long successNum=0;
                internal long halfSuccessNum=0;
                internal long failNum=0;
            }
        }
        /// <summary>
        /// 查询功能类
        /// </summary>
        static class FindData
        {
            /// <summary>
            /// 查询玩家数据方法函数
            /// </summary>
            /// <param name="inputPD">全名inputPlayerData<br/>
            /// 0: 玩家ID数据<br/>
            /// 1: 玩家UUID数据<br/>
            /// 注意，只能提供其中一项，另一项必须为null
            /// </param>
            /// <returns>返回玩家数据<br/>
            /// 0: 输出返回值状态<br/>
            /// 1: 玩家ID<br/>
            /// 2: 玩家UUID<br/><br/>
            /// 0:<br/>
            /// true: 正常<br/>
            /// error1: 错误，未提供UUID或PID<br/>
            /// error2: 错误，提供了两个玩家数据值<br/>
            /// nothing: 没有找到数据
            /// </returns>
            static internal string[] FindPlayerData(string[] inputPD)
            {
                if (inputPD[0] == null && inputPD[1] == null)
                {                    
                    return ["error1"];//goto exitProgram;
                }
                else if (inputPD[0] != null && inputPD[1] != null &&
                    inputPD[0] != "" && inputPD[1] != "")
                {                    
                    return ["error2"];
                }
                string[] outputStr = new string[2];
                List<string[]> usercacheData = [];
                {
                    JArray jArry = (JArray)JsonConvert.DeserializeObject(File.ReadAllText(filePath + "/usercache.json"))!;

                    for (int i = 0; i < jArry.Count; i++)
                        usercacheData.Add([jArry[i]["name"]!.ToString(), jArry[i]["uuid"]!.ToString()]);
                }
                if (inputPD[0] != "" && inputPD[0] != null)
                {
                    for (int i = 0; i < usercacheData.Count; i++)
                    {
                        if (inputPD[0] == usercacheData[i][0])
                        {
                            outputStr[0] = usercacheData[i][0];
                            outputStr[1] = usercacheData[i][1];
                            break;
                        }
                    }
                }
                else if (inputPD[1] != "" && inputPD[1] != null)
                {
                    for (int i = 0; i < usercacheData.Count; i++)
                    {
                        if (inputPD[1] == usercacheData[i][1])
                        {
                            outputStr[0] = usercacheData[i][0];
                            outputStr[1] = usercacheData[i][1];
                            break;
                        }
                    }
                }
                if (outputStr[0] != null && outputStr[1] != null &&
                    outputStr[0] != "" && outputStr[1] != "")
                {
                    return ["true", outputStr[0],outputStr[1]];
                }
                else
                {                    
                    return ["nothing"];
                }
            }
        }
    }
}