using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static MinecraftServerPlayerDataManager.Core.GlobalFunction;

namespace MinecraftServerPlayerDataManager.Core {
	public class MspdmAction {
		public string RunPath => runPath;
		readonly string runPath;

		readonly System.ConsoleColor defBack;
		readonly System.ConsoleColor defFore;
		public MspdmAction(string path, System.ConsoleColor defBack_,System.ConsoleColor defFore_) {
			defBack = defBack_;
			defFore = defFore_;

			runPath = path;
			if (!(File.Exists(RunPath + "/usercache.json"))) {//检查路径是否正确
				throw new Exceptions.InitError("错误：提供的路径有误！请在MC服务端根目录执行或指定MC服务端根目录路径。");
			}
			else {
				while (RunPath[^1] is '/' or '\\')
					runPath = RunPath[..^1];
			}

			PlayerDataFile = new PlayerDataFile_Class(RunPath,defBack,defFore);
		}
		public interface IPlayerDataFile {
			/// <summary>
			/// 最后数据结果存储变量
			/// </summary>
			public class ResultsData {
				internal enum Type {
					CheckFile, CheckJson
				}
				/// <summary>
				/// 数据类型
				/// </summary>
				internal Type type;
				internal byte[]? ynId;
				internal string[]? ucData, dataSuffix, tempStr;
				internal string? verifyDir, file;
			}
			delegate void logOutput(uint id, params object[] logArg);
			List<ResultsData> CheckPlayerDataFile(logOutput? logOutput=null);
			void FindPlayerDataFile(string[] inputPlayerData);
			void DeletePlayerDataFile(string[] inputPlayerData);
			void ClearJunkPlayerDataFile();
		}
		public IPlayerDataFile PlayerDataFile { get; private set; }
		private class PlayerDataFile_Class: IPlayerDataFile {
			readonly string RunPath;
			readonly System.ConsoleColor defBack;
			readonly System.ConsoleColor defFore;
			internal PlayerDataFile_Class(string runPath, System.ConsoleColor defBack_, System.ConsoleColor defFore_) {
				defBack = defBack_;
				defFore = defFore_;

				RunPath = runPath;
				backupDelFiles = new(RunPath);
			}


			public List<IPlayerDataFile.ResultsData> CheckPlayerDataFile(IPlayerDataFile.logOutput? logOutput=null) {
				List<IPlayerDataFile.ResultsData> dataCheck_ResultsOutput = [];

				logOutput?.Invoke(0, RunPath);
				DelayUs();
				///用户数据临时存储变量
				///0: name
				///1: uuid
				List<string[]> usercacheData = [];
				///最后的结果输出                            
				dynamic stats = new System.Dynamic.ExpandoObject();{
					stats.allNum = (ulong)0;
					stats.successNum = (ulong)0;
					stats.halfSuccessNum = (ulong)0;
					stats.failNum = (ulong)0;
				}
#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
				JArray jArry = (JArray)JsonConvert.DeserializeObject(File.ReadAllText(RunPath + "/usercache.json"));
#pragma warning restore CS8600
#pragma warning disable CS8602 // 解引用可能出现空引用。
				for (int i = 0; i < jArry.Count; i++) {
					usercacheData.Add([jArry[i]["name"].ToString(), jArry[i]["uuid"].ToString()]);
					logOutput?.Invoke(1, usercacheData[i][0], usercacheData[i][1]);
					DelayUs();
				}
				logOutput?.Invoke(2, jArry.Count.ToString());
#pragma warning restore CS8602
				DelayUs();
				{
					bool[] pass = new bool[2];
					string verifyDir = "";
					string[] dataSuffix = new string[2];
					for (int dirId = 1; dirId <= 3; dirId++) {
						switch (dirId) {
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
						logOutput?.Invoke(3, RunPath, verifyDir);
						DelayUs();
						for (int i = 0; i < jArry.Count; i++)//使用usercache.json文件中的数据和别的文件夹进行对比验证
						{
							if (File.Exists(RunPath + "/world/" + verifyDir + "/" + usercacheData[i][1] + dataSuffix[0])) pass[0] = true;
							else pass[0] = false;
							if (verifyDir == "playerdata")//因为playerdata有.dat和.dat_old,其他文件夹只有.json，所以要添加判断语句
							{
								if (File.Exists(RunPath + "/world/" + verifyDir + "/" + usercacheData[i][1] + dataSuffix[1])) pass[1] = true;
								else pass[1] = false;
							}

							byte[] ynid = new byte[4];//用于判断
							if (pass[0])
								ynid[0] = 0;
							else ynid[0] = 1;
							if (verifyDir == "playerdata") {
								if (pass[1])
									ynid[1] = 0;
								else
									ynid[1] = 1;
							}
							if (verifyDir == "playerdata") {
								if (pass[0] && pass[1])
									ynid[2] = 0;//"成功";
								else if (pass[0] && pass[1] == false)
									ynid[2] = 2;//"半成功";
								else
									ynid[2] = 1;//"失败";
							}
							else {
								if (pass[0])
									ynid[2] = 0;
								else
									ynid[2] = 1;
							}
							//将输出变为彩色输出
							if (verifyDir == "playerdata") {
								if (pass[0] && pass[1])
									stats.successNum++;
								else if (pass[0] && pass[1] == false)
									stats.halfSuccessNum++;
								else
									stats.failNum++;
							}
							else {
								if (pass[0])
									stats.successNum++;
								else
									stats.failNum++;
							}
							stats.allNum++;
							logOutput?.Invoke(4, ynid, usercacheData[i], verifyDir, dataSuffix, RunPath);
							if (!(pass[0])) {
								dataCheck_ResultsOutput.Add(
								new() {
									type = IPlayerDataFile.ResultsData.Type.CheckFile,
									ynId = ynid,
									ucData = usercacheData[i],
									verifyDir = verifyDir,
									dataSuffix = dataSuffix
								});
								//dataCheck_ResultsOutput.Add(ynid[3]);
							}
							DelayUs();
						}
					}
					for (int dirId = 1; dirId <= 3; dirId++) {
						switch (dirId) {
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
						logOutput?.Invoke(5, RunPath, verifyDir);
						DelayUs();
						string[] files = Directory.GetFiles(RunPath + "/world/" + verifyDir, "*.*");
						foreach (string file in files) {
							string[] tempStr = new string[5];
							pass[0] = false;
							for (int i = 0; i < usercacheData.Count; i++) {
								if (usercacheData[i][1] == Path.GetFileNameWithoutExtension(file)) {
									pass[0] = true;
									tempStr[2] = usercacheData[i][0];
									tempStr[3] = usercacheData[i][1];
									break;
								}
							}
							if (pass[0]) {
								tempStr[1] = "成功";
							}
							else {
								tempStr[1] = "失败";
								tempStr[2] = "[未找到相应数据]";
							}
							if (pass[0])
								stats.successNum++;
							else
								stats.failNum++;
							stats.allNum++;
							logOutput?.Invoke(6, tempStr, verifyDir, file, RunPath);
							//CheckData.CheckJsonOutputLog(tempStr, verifyDir, file,RunPath,defFore);
							if (!(pass[0])) {
								dataCheck_ResultsOutput.Add(
								new() {
									type = IPlayerDataFile.ResultsData.Type.CheckJson,
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


				
				logOutput?.Invoke(7, 
					stats.allNum, 
					stats.successNum, 
					stats.failNum, 
					stats.halfSuccessNum);
				if (dataCheck_ResultsOutput.Count != 0) {
					for (int i = 0; i < dataCheck_ResultsOutput.Count; i++) {
						//Console.WriteLine(dataCheck_ResultsOutput[i]); 
						switch (dataCheck_ResultsOutput[i].type) {
							case IPlayerDataFile.ResultsData.Type.CheckFile:
								logOutput?.Invoke(8,
									dataCheck_ResultsOutput[i].ynId!,
									dataCheck_ResultsOutput[i].ucData!,
									dataCheck_ResultsOutput[i].verifyDir!,
									dataCheck_ResultsOutput[i].dataSuffix!,
									RunPath);
								break;
							case IPlayerDataFile.ResultsData.Type.CheckJson:
								logOutput?.Invoke(9, 
									dataCheck_ResultsOutput[i].tempStr!, 
									dataCheck_ResultsOutput[i].verifyDir!, 
									dataCheck_ResultsOutput[i].file!, 
									RunPath);
								break;
						}
						DelayUs();
					}
				}
				else {
					logOutput?.Invoke(10);
				}

				return dataCheck_ResultsOutput;
			}

			readonly BackupDelFiles backupDelFiles;
			class BackupDelFiles {
				internal BackupDelFiles(string runPath) {
					delFilePath = runPath + "/mspdm_DeleteFiles/";
				}
				internal string delFilePath;
				internal void DelFile(string sourceFilePath, string subDir)//将源文件删除至备份文件夹
				{
					try {
						Directory.CreateDirectory(delFilePath + subDir);//创建子文件夹，因为部分数据的名字是完全一样的，需要在此区分
						File.Move(sourceFilePath, delFilePath + subDir + "/" + Path.GetFileName(sourceFilePath));
						Console.WriteLine("\'" + sourceFilePath + "\'删除成功");
					} catch {
						Console.WriteLine("\'" + sourceFilePath + "\'删除失败");
					}
				}
			}

			string[]? GetPlayerDataFunc(string[] inputPlayerData) {
				string[] GetPlayerData = McFindData.FindPlayerData(inputPlayerData,RunPath);
				switch (GetPlayerData[0]) {
					case "true":
						return GetPlayerData;
					case "error1":
						Console.WriteLine("错误：请提供玩家UUID或玩家ID！");
						return null;
					case "error2":
						Console.WriteLine("错误：只能提供玩家UUID或玩家ID其中一项！");
						return null;
					case "nothing":
						Console.WriteLine("没有找到该玩家的数据！");
						return null;
				}
				return null;
			}
			public void FindPlayerDataFile(string[] inputPlayerData) {
				string[]? GetPlayerData = GetPlayerDataFunc(inputPlayerData);
				if (GetPlayerData != null) {
					Console.WriteLine("玩家ID: " + GetPlayerData[1]);
					Console.WriteLine("玩家UUID: " + GetPlayerData[2]);
				}

			}

			public void DeletePlayerDataFile(string[] inputPlayerData) {
				string[]? GetPlayerData = GetPlayerDataFunc(inputPlayerData);
				if (GetPlayerData != null) {
					Console.WriteLine("已找到目标玩家的数据文件:\r\n" +
											"玩家ID: " + GetPlayerData[1] + "\r\n" +
											"玩家UUID: " + GetPlayerData[2]);
					Console.Write("是否删除？(Y/N) ");
					string input = Console.ReadLine()!;
					switch (input.ToLower()) {
						case "y": {
								Directory.CreateDirectory(backupDelFiles.delFilePath);

								backupDelFiles.DelFile(RunPath + "/world/playerdata/" + GetPlayerData[2] + ".dat", "playerdata");
								backupDelFiles.DelFile(RunPath + "/world/playerdata/" + GetPlayerData[2] + ".dat_old", "playerdata");
								backupDelFiles.DelFile(RunPath + "/world/advancements/" + GetPlayerData[2] + ".json", "advancements");
								backupDelFiles.DelFile(RunPath + "/world/stats/" + GetPlayerData[2] + ".json", "stats");
								Console.WriteLine("完成。");
							}
							break;
						case "n":
						default:
							Console.WriteLine("操作已取消！");
							break;
					}
				}
			}

			public void ClearJunkPlayerDataFile() {
				List<IPlayerDataFile.ResultsData> dataCheck_ResultsOutput = CheckPlayerDataFile();

				bool deleteConfirm = false;
				bool isNothing = true;
				Console.WriteLine("下列文件将被删除:");
			confirm:;
				for (int i = 0; i < dataCheck_ResultsOutput.Count; i++) {
					switch (dataCheck_ResultsOutput[i].type) {
						case IPlayerDataFile.ResultsData.Type.CheckJson:
							if (!deleteConfirm) {
								Console.WriteLine(dataCheck_ResultsOutput[i].file!);
								if (isNothing) isNothing = false;
							}
							else
								backupDelFiles.DelFile(dataCheck_ResultsOutput[i].file!, dataCheck_ResultsOutput[i].verifyDir!);
							break;
					}
					DelayUs();
				}
				if (!deleteConfirm) {
					if (isNothing) {
						Console.WriteLine("没有找到需要被删除的垃圾文件。");
					}
					else {
						Console.Write("是否删除？(Y/N) ");
						string input = Console.ReadLine()!;
						switch (input.ToLower()) {
							case "y":
								Directory.CreateDirectory(backupDelFiles.delFilePath);
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
		}
		
	}

	internal static class Exceptions {
		internal class InitError : Exception {
			internal InitError() { }
			internal InitError(string message) : base(message) { }
		}
	}
	internal static class GlobalFunction {
		/// <summary>
		///  微秒延时
		/// </summary>
		/// <param name="time">延时时间,单位:ms</param>
		/// <returns></returns>
		internal static double DelayUs(double time = 0.8) {
			System.Diagnostics.Stopwatch stopTime = new();

			stopTime.Start();
			while (stopTime.Elapsed.TotalMilliseconds < time) { }
			stopTime.Stop();

			return stopTime.Elapsed.TotalMilliseconds;
		}

		/// <summary>
		/// 查询功能类
		/// </summary>
		internal static class McFindData {
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
			static internal string[] FindPlayerData(string[] inputPD, string RunPath) {
				if (inputPD[0] == null && inputPD[1] == null) {
					return ["error1"];//goto exitProgram;
				}
				else if (inputPD[0] != null && inputPD[1] != null &&
					inputPD[0] != "" && inputPD[1] != "") {
					return ["error2"];
				}
				string[] outputStr = new string[2];
				List<string[]> usercacheData = [];
				{
					JArray jArry = (JArray)JsonConvert.DeserializeObject(File.ReadAllText(RunPath + "/usercache.json"))!;

					for (int i = 0; i < jArry.Count; i++)
						usercacheData.Add([jArry[i]["name"]!.ToString(), jArry[i]["uuid"]!.ToString()]);
				}
				if (inputPD[0] != "" && inputPD[0] != null) {
					for (int i = 0; i < usercacheData.Count; i++) {
						if (inputPD[0] == usercacheData[i][0]) {
							outputStr[0] = usercacheData[i][0];
							outputStr[1] = usercacheData[i][1];
							break;
						}
					}
				}
				else if (inputPD[1] != "" && inputPD[1] != null) {
					for (int i = 0; i < usercacheData.Count; i++) {
						if (inputPD[1] == usercacheData[i][1]) {
							outputStr[0] = usercacheData[i][0];
							outputStr[1] = usercacheData[i][1];
							break;
						}
					}
				}
				if (outputStr[0] != null && outputStr[1] != null &&
					outputStr[0] != "" && outputStr[1] != "") {
					return ["true", outputStr[0], outputStr[1]];
				}
				else {
					return ["nothing"];
				}
			}
		}
	}
}