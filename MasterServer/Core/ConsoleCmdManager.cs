using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using CommandLine.Text;
using HK2Net.Kernel;

namespace MasterServer.Core
{
	// Token: 0x0200079F RID: 1951
	public class ConsoleCmdManager : IConsoleCmdManager
	{
		// Token: 0x06002856 RID: 10326 RVA: 0x000AD9B5 File Offset: 0x000ABDB5
		public ConsoleCmdManager(IContainer container)
		{
			this.m_container = container;
		}

		// Token: 0x06002857 RID: 10327 RVA: 0x000AD9C4 File Offset: 0x000ABDC4
		public void Init()
		{
			Assembly assembly = Assembly.GetAssembly(typeof(ConsoleCmdManager));
			Type[] types = assembly.GetTypes();
			ConsoleCmdManager.m_commands = (from t in types
			where typeof(IConsoleCmd).IsAssignableFrom(t)
			where !t.GetCustomAttributes(typeof(DebugCommandAttribute), false).Any<object>() || Resources.DebugQueriesEnabled
			select t).SelectMany((Type t) => from attr in t.GetCustomAttributes(typeof(ConsoleCmdAttributes), false).OfType<ConsoleCmdAttributes>()
			select new ConsoleCmdManager.CommandEntity(attr, this.m_container.Create(t) as IConsoleCmd)).ToDictionary((ConsoleCmdManager.CommandEntity e) => e.Attributes.CmdName, (ConsoleCmdManager.CommandEntity e) => e);
		}

		// Token: 0x06002858 RID: 10328 RVA: 0x000ADA84 File Offset: 0x000ABE84
		public static void ConsoleInteract(ConsoleCmdManager.ProcessConsoleDelegate keyFunc, params ConsoleKey[] defInputs)
		{
			bool flag = true;
			int num = 0;
			while (flag)
			{
				ConsoleKey consoleKey;
				if (Resources.IsDaemon)
				{
					consoleKey = defInputs[num];
					Log.Info<ConsoleKey>("Running in daemon mode. Key: {0}", consoleKey);
				}
				else
				{
					ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
					if (consoleKeyInfo.Key == ConsoleKey.Enter)
					{
						consoleKey = defInputs[num];
					}
					else
					{
						consoleKey = consoleKeyInfo.Key;
					}
				}
				flag = !keyFunc(consoleKey);
				num++;
			}
		}

		// Token: 0x06002859 RID: 10329 RVA: 0x000ADAF4 File Offset: 0x000ABEF4
		public static void ExecuteCmd(string cmd)
		{
			try
			{
				string[] array = ConsoleCmdManager.ParseCmdLine(cmd);
				if (array.Length != 0)
				{
					ConsoleCmdManager.CommandEntity commandEntity;
					if (ConsoleCmdManager.m_commands.TryGetValue(array[0], out commandEntity))
					{
						if (array.Length == 2 && array[1] == "?")
						{
							string p = ConsoleCmdManager.FormatCommandHelp(commandEntity, true);
							Log.Info<string>("\n{0}", p);
						}
						else
						{
							if (commandEntity.Attributes.ArgsSize > 0)
							{
								if (array.Length > commandEntity.Attributes.ArgsSize + 1)
								{
									Log.Error<int>("Too many arguments, expected {0}", commandEntity.Attributes.ArgsSize);
									goto IL_C5;
								}
							}
							try
							{
								commandEntity.Instance.ExecuteCmd(array);
							}
							catch
							{
								ConsoleCmdManager.ExecuteCmd(string.Format("{0} ?", array[0]));
								throw;
							}
						}
						IL_C5:;
					}
					else
					{
						Log.Error("No command found");
					}
				}
			}
			catch (Exception e)
			{
				Log.Error<string>("Can't execute command '{0}'", cmd);
				Log.Error(e);
			}
		}

		// Token: 0x0600285A RID: 10330 RVA: 0x000ADC10 File Offset: 0x000AC010
		public static string[] ParseCmdLine(string cmd)
		{
			List<string> list = new List<string>();
			bool flag = false;
			char c = '\0';
			string text = string.Empty;
			string text2 = cmd + " ";
			foreach (char c2 in text2.ToCharArray())
			{
				if (c2 == ' ' && !flag)
				{
					text = text.Trim();
					if (!string.IsNullOrEmpty(text))
					{
						list.Add(text);
					}
					text = string.Empty;
				}
				else if ((c2 == '"' || c2 == '\'') && (c == '\0' || c == c2))
				{
					flag = !flag;
					c = ((!flag) ? '\0' : c2);
				}
				else
				{
					text += c2;
				}
			}
			return list.ToArray();
		}

		// Token: 0x0600285B RID: 10331 RVA: 0x000ADCE4 File Offset: 0x000AC0E4
		public static void Dump(string namePart, bool full)
		{
			if (string.IsNullOrEmpty(namePart))
			{
				ConsoleCmdManager.Dump((ConsoleCmdAttributes a) => true, (ConsoleCmdManager.CommandEntity c) => ConsoleCmdManager.FormatCommandHelp(c, full));
			}
			else
			{
				ConsoleCmdManager.Dump((ConsoleCmdAttributes a) => a.CmdName.Contains(namePart), (ConsoleCmdManager.CommandEntity c) => ConsoleCmdManager.FormatCommandHelp(c, full));
			}
		}

		// Token: 0x0600285C RID: 10332 RVA: 0x000ADD68 File Offset: 0x000AC168
		internal static void Dump(Predicate<ConsoleCmdAttributes> predicate, Func<ConsoleCmdManager.CommandEntity, string> formatter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			IEnumerable<ConsoleCmdManager.CommandEntity> enumerable = (predicate != null) ? (from c in ConsoleCmdManager.m_commands.Values
			where predicate(c.Attributes)
			select c) : ConsoleCmdManager.m_commands.Values;
			foreach (ConsoleCmdManager.CommandEntity arg in enumerable)
			{
				stringBuilder.Append(formatter(arg)).Append("\n");
			}
			Log.Info(stringBuilder.ToString());
		}

		// Token: 0x0600285D RID: 10333 RVA: 0x000ADE28 File Offset: 0x000AC228
		private static string FormatCommandHelp(ConsoleCmdManager.CommandEntity commandEntity, bool full = false)
		{
			string text;
			if (full)
			{
				text = string.Format("{0} - {1}", commandEntity.Attributes.CmdName, commandEntity.Attributes.Help);
			}
			else
			{
				text = commandEntity.Attributes.CmdName;
			}
			Type type = commandEntity.Instance.GetType();
			Type baseType = type.BaseType;
			if (baseType != null && baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(ConsoleCommand<>))
			{
				Type type2 = baseType.GetGenericArguments()[0];
				object options = Activator.CreateInstance(type2);
				HelpText helpText = new HelpText
				{
					Heading = text,
					AdditionalNewLineAfterOption = false
				};
				if (full)
				{
					helpText.AddOptions(options);
				}
				return helpText.ToString();
			}
			return text;
		}

		// Token: 0x0600285E RID: 10334 RVA: 0x000ADEFC File Offset: 0x000AC2FC
		public static void ExecuteCmdLine(List<string> cmdLine)
		{
			for (int i = 0; i < cmdLine.Count; i += 2)
			{
				if (i + 1 >= cmdLine.Count)
				{
					throw new ApplicationException(string.Format("Wrong command line arguments count: {0}", cmdLine.Count));
				}
				string text = string.Format("{0} {1}", cmdLine[i], cmdLine[i + 1]);
				Log.Info<string>("* {0}", text);
				ConsoleCmdManager.ExecuteCmd(text);
			}
		}

		// Token: 0x04001528 RID: 5416
		private readonly IContainer m_container;

		// Token: 0x04001529 RID: 5417
		private static Dictionary<string, ConsoleCmdManager.CommandEntity> m_commands;

		// Token: 0x020007A0 RID: 1952
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		internal struct CommandEntity
		{
			// Token: 0x06002865 RID: 10341 RVA: 0x000AE00D File Offset: 0x000AC40D
			public CommandEntity(ConsoleCmdAttributes attributes, IConsoleCmd instance)
			{
				this = default(ConsoleCmdManager.CommandEntity);
				this.Attributes = attributes;
				this.Instance = instance;
			}

			// Token: 0x170003BE RID: 958
			// (get) Token: 0x06002866 RID: 10342 RVA: 0x000AE024 File Offset: 0x000AC424
			// (set) Token: 0x06002867 RID: 10343 RVA: 0x000AE02C File Offset: 0x000AC42C
			public ConsoleCmdAttributes Attributes { get; private set; }

			// Token: 0x170003BF RID: 959
			// (get) Token: 0x06002868 RID: 10344 RVA: 0x000AE035 File Offset: 0x000AC435
			// (set) Token: 0x06002869 RID: 10345 RVA: 0x000AE03D File Offset: 0x000AC43D
			public IConsoleCmd Instance { get; private set; }
		}

		// Token: 0x020007A1 RID: 1953
		// (Invoke) Token: 0x0600286B RID: 10347
		public delegate bool ProcessConsoleDelegate(ConsoleKey key);
	}
}
