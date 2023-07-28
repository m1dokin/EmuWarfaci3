using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using MasterServer.Core;
using MasterServer.Core.Diagnostics.Threading;
using MasterServer.Telemetry.Metrics;
using OLAPHypervisor;
using StatsDataSource.Storage;

namespace MasterServer.Telemetry
{
	// Token: 0x020007C8 RID: 1992
	internal class StatsProcessor : MasterServer.Core.ProcessingQueue<StatsProcessor.ProcessingTask>
	{
		// Token: 0x060028CA RID: 10442 RVA: 0x000B0DBC File Offset: 0x000AF1BC
		public StatsProcessor(ITelemetryService telem, IProcessingQueueMetricsTracker processingQueueMetricsTracker) : base("StatsProcessor", processingQueueMetricsTracker, true)
		{
			this.m_service = telem;
			this.InitDataSource();
			if (Directory.Exists(Resources.TelemStreamDir))
			{
				foreach (string path in Directory.GetFiles(Resources.TelemStreamDir, "*.tlm"))
				{
					File.Delete(path);
				}
			}
			else
			{
				Directory.CreateDirectory(Resources.TelemStreamDir);
			}
		}

		// Token: 0x060028CB RID: 10443 RVA: 0x000B0E30 File Offset: 0x000AF230
		private void InitDataSource()
		{
			using (FileStream fileStream = new FileStream(Resources.StatsFormatFile, FileMode.Open, FileAccess.Read))
			{
				List<string> list;
				this.m_statsRegistry = new StatsRegistry(XmlReader.Create(fileStream), Resources.StatsPreprocDefFile, Resources.StatsDataAccDefFile, ref list);
				if (list != null && list.Count != 0)
				{
					foreach (string format in list)
					{
						MasterServer.Core.Log.Error(format);
					}
					throw new Exception("Failed to parse processing definition files");
				}
			}
		}

		// Token: 0x060028CC RID: 10444 RVA: 0x000B0EEC File Offset: 0x000AF2EC
		internal StatsRegistry GetRegistry()
		{
			return this.m_statsRegistry;
		}

		// Token: 0x060028CD RID: 10445 RVA: 0x000B0EF4 File Offset: 0x000AF2F4
		public void QueueStatsProcessing(string input_file)
		{
			base.Add(new StatsProcessor.ProcessingTask(input_file, null, StatsProcessor.EInputType.Full));
		}

		// Token: 0x060028CE RID: 10446 RVA: 0x000B0F04 File Offset: 0x000AF304
		public void QueueStreamProcessing(string input_file, string output_file)
		{
			base.Add(new StatsProcessor.ProcessingTask(input_file, output_file, StatsProcessor.EInputType.Streamed));
		}

		// Token: 0x060028CF RID: 10447 RVA: 0x000B0F14 File Offset: 0x000AF314
		public override void Process(StatsProcessor.ProcessingTask task)
		{
			try
			{
				using (new ThreadTracker.Tracker("StatsProcessor"))
				{
					using (new FunctionProfiler(Profiler.EModule.TELEMETRY, "StatsProcessor.ProcessData"))
					{
						StatsRepository repo = new StatsRepository(this.m_statsRegistry);
						if (task.type == StatsProcessor.EInputType.Full)
						{
							this.ProcessIncomingFile(repo, task.input_file);
						}
						else
						{
							this.ProcessIncomingStream(repo, task.input_file, task.output_file);
						}
					}
				}
			}
			catch (Exception e)
			{
				MasterServer.Core.Log.Error(e);
			}
		}

		// Token: 0x060028D0 RID: 10448 RVA: 0x000B0FD4 File Offset: 0x000AF3D4
		private void ProcessIncomingFile(StatsRepository repo, string input_file)
		{
			using (FileStream fileStream = new FileStream(input_file, FileMode.Open, FileAccess.Read))
			{
				repo.LoadFromXML(XmlReader.Create(fileStream));
				List<DataUpdate> list = repo.DataProcessor.FinalizeAccumulation();
				MasterServer.Core.Log.Info(string.Format("Incoming stats processed, {0} DB updates created", list.Count));
				Measure[] msrs = TelemetryService.MeasureFromUpdatesSt(list);
				this.m_service.AddMeasure(msrs);
			}
		}

		// Token: 0x060028D1 RID: 10449 RVA: 0x000B1054 File Offset: 0x000AF454
		private void ProcessIncomingStream(StatsRepository repo, string input_file, string output_file)
		{
			if (!this.m_service.CheckMode(TelemetryMode.StatsMerging))
			{
				if (this.m_service.CheckMode(TelemetryMode.RemoveStreams))
				{
					File.Delete(input_file);
				}
				return;
			}
			repo.DataProcessor.ProcessingEnabled = false;
			using (FileStream fileStream = new FileStream(input_file, FileMode.Open, FileAccess.Read))
			{
				using (StreamReader streamReader = new StreamReader(fileStream))
				{
					using (TelemetryStreamParser telemetryStreamParser = new TelemetryStreamParser(repo, streamReader))
					{
						telemetryStreamParser.Parse();
					}
				}
			}
			int num = 0;
			string file = Path.Combine(Resources.StatsDirectory, output_file);
			do
			{
				try
				{
					Directory.CreateDirectory(Resources.StatsDirectory);
					StatsProcessor.SerializeToXMLAtomic(repo, file);
					break;
				}
				catch (Exception ex)
				{
					MasterServer.Core.Log.Warning<string, int, string>("Cannot serialize the stats to XML: {0}. Attempt: {1}. Reason: {2}", output_file, num, ex.Message);
				}
			}
			while (++num < 3);
			if (this.m_service.CheckMode(TelemetryMode.RemoveStreams))
			{
				File.Delete(input_file);
			}
		}

		// Token: 0x060028D2 RID: 10450 RVA: 0x000B1184 File Offset: 0x000AF584
		private static void SerializeToXMLAtomic(StatsRepository repo, string file)
		{
			string text = Path.Combine(Path.GetTempPath(), Path.GetFileName(file));
			bool flag = false;
			try
			{
				repo.SaveToXML(text);
				File.Move(text, file);
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					File.Delete(text);
				}
			}
		}

		// Token: 0x040015AF RID: 5551
		private const byte MaxSerializationRetriesCount = 3;

		// Token: 0x040015B0 RID: 5552
		private StatsRegistry m_statsRegistry;

		// Token: 0x040015B1 RID: 5553
		private ITelemetryService m_service;

		// Token: 0x020007C9 RID: 1993
		public enum EInputType
		{
			// Token: 0x040015B3 RID: 5555
			Streamed,
			// Token: 0x040015B4 RID: 5556
			Full
		}

		// Token: 0x020007CA RID: 1994
		public class ProcessingTask
		{
			// Token: 0x060028D3 RID: 10451 RVA: 0x000B11D8 File Offset: 0x000AF5D8
			public ProcessingTask(string input_file, string output_file, StatsProcessor.EInputType type)
			{
				this.input_file = input_file;
				this.output_file = output_file;
				this.type = type;
			}

			// Token: 0x040015B5 RID: 5557
			public string input_file;

			// Token: 0x040015B6 RID: 5558
			public string output_file;

			// Token: 0x040015B7 RID: 5559
			public StatsProcessor.EInputType type;
		}
	}
}
