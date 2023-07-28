using System;
using System.Collections.Generic;
using System.Threading;
using MasterServer.Core;
using MasterServer.Core.Timers;
using MasterServer.Database;
using NCrontab;
using OLAPHypervisor;

namespace MasterServer.Telemetry
{
	// Token: 0x020006E8 RID: 1768
	internal class DeferredTelemetryStream : IDisposable
	{
		// Token: 0x06002521 RID: 9505 RVA: 0x0009B07C File Offset: 0x0009947C
		internal DeferredTelemetryStream(TelemetryService service)
		{
			this.m_service = service;
		}

		// Token: 0x06002522 RID: 9506 RVA: 0x0009B0BC File Offset: 0x000994BC
		public void Dispose()
		{
			object deferredMeasures = this.m_deferredMeasures;
			lock (deferredMeasures)
			{
				foreach (DeferredTelemetryStream.DeferredMeasure deferredMeasure in this.m_deferredMeasures.Values)
				{
					if (deferredMeasure.Timer != null)
					{
						deferredMeasure.Timer.Dispose();
					}
				}
				this.m_deferredMeasures.Clear();
			}
		}

		// Token: 0x06002523 RID: 9507 RVA: 0x0009B164 File Offset: 0x00099564
		public void RegisterMeasureCallback(string schedule, MeasureCallback clb)
		{
			this.RegisterMeasureCallback(schedule, TimeSpan.Zero, clb);
		}

		// Token: 0x06002524 RID: 9508 RVA: 0x0009B174 File Offset: 0x00099574
		public void RegisterMeasureCallback(string schedule, TimeSpan jitter, MeasureCallback clb)
		{
			DeferredTelemetryStream.DeferredSchedule schedule2 = new DeferredTelemetryStream.DeferredSchedule(schedule, jitter);
			object deferredMeasures = this.m_deferredMeasures;
			lock (deferredMeasures)
			{
				DeferredTelemetryStream.DeferredMeasure orCreateDeferredMeasure = this.GetOrCreateDeferredMeasure(schedule2);
				orCreateDeferredMeasure.Callbacks += clb;
				if (orCreateDeferredMeasure.Timer == null)
				{
					this.ScheduleDeferredMeasure(orCreateDeferredMeasure);
				}
			}
		}

		// Token: 0x06002525 RID: 9509 RVA: 0x0009B1DC File Offset: 0x000995DC
		public void UnregisterMeasureCallback(MeasureCallback clb)
		{
			object deferredMeasures = this.m_deferredMeasures;
			lock (deferredMeasures)
			{
				foreach (DeferredTelemetryStream.DeferredMeasure deferredMeasure in this.m_deferredMeasures.Values)
				{
					deferredMeasure.Callbacks -= clb;
					if (deferredMeasure.CallbacksCount == 0 && deferredMeasure.Timer != null && deferredMeasure.Measures.Count == 0)
					{
						deferredMeasure.Timer.Dispose();
						deferredMeasure.Timer = null;
					}
				}
			}
		}

		// Token: 0x06002526 RID: 9510 RVA: 0x0009B2A4 File Offset: 0x000996A4
		private DeferredTelemetryStream.DeferredMeasure GetOrCreateDeferredMeasure(DeferredTelemetryStream.DeferredSchedule schedule)
		{
			DeferredTelemetryStream.DeferredMeasure deferredMeasure;
			if (!this.m_deferredMeasures.TryGetValue(schedule, out deferredMeasure))
			{
				deferredMeasure = new DeferredTelemetryStream.DeferredMeasure(schedule);
				this.m_deferredMeasures.Add(schedule, deferredMeasure);
			}
			return deferredMeasure;
		}

		// Token: 0x06002527 RID: 9511 RVA: 0x0009B2D9 File Offset: 0x000996D9
		public void AddMeasure(Measure msr, string schedule)
		{
			this.AddMeasure(new Measure[]
			{
				msr
			}, schedule);
		}

		// Token: 0x06002528 RID: 9512 RVA: 0x0009B2F5 File Offset: 0x000996F5
		public void AddMeasure(IEnumerable<Measure> msrs, string schedule)
		{
			this.AddMeasure(msrs, schedule, TimeSpan.Zero);
		}

		// Token: 0x06002529 RID: 9513 RVA: 0x0009B304 File Offset: 0x00099704
		public void AddMeasure(IEnumerable<Measure> msrs, string schedule, TimeSpan jitter)
		{
			object deferredMeasures = this.m_deferredMeasures;
			lock (deferredMeasures)
			{
				if (this.m_aggOps == null)
				{
					ITelemetryDALService service = ServicesManager.GetService<ITelemetryDALService>();
					this.m_defaultAggOp = service.TelemetrySystem.GetDefaultAggregationOp();
					this.m_aggOps = service.TelemetrySystem.GetAggregationOps();
				}
				DeferredTelemetryStream.DeferredSchedule schedule2 = new DeferredTelemetryStream.DeferredSchedule(schedule, jitter);
				DeferredTelemetryStream.DeferredMeasure orCreateDeferredMeasure = this.GetOrCreateDeferredMeasure(schedule2);
				foreach (Measure msr in msrs)
				{
					this.InsertMeasureWithAggregation(orCreateDeferredMeasure.Measures, msr);
				}
				if (orCreateDeferredMeasure.Timer == null)
				{
					this.ScheduleDeferredMeasure(orCreateDeferredMeasure);
				}
			}
		}

		// Token: 0x0600252A RID: 9514 RVA: 0x0009B3EC File Offset: 0x000997EC
		private void ScheduleDeferredMeasure(DeferredTelemetryStream.DeferredMeasure dm)
		{
			if (dm.Timer != null)
			{
				dm.Timer.Dispose();
				dm.Timer = null;
			}
			DateTime now = DateTime.Now;
			TimeSpan t = TimeSpan.FromSeconds(dm.Sched.Jitter.TotalSeconds * 0.5);
			TimeSpan t2 = TimeSpan.FromSeconds((this.m_jitterRand.NextDouble() - 0.5) * dm.Sched.Jitter.TotalSeconds);
			DateTime nextOccurrence = dm.Sched.Schedule.GetNextOccurrence(now + t);
			TimeSpan dueTime = nextOccurrence + t2 - now;
			dm.Timer = new SafeTimer(new TimerCallback(this.onTimeTick), new KeyValuePair<DeferredTelemetryStream.DeferredMeasure, DateTime>(dm, nextOccurrence), dueTime, TimeSpan.FromMilliseconds(-1.0));
		}

		// Token: 0x0600252B RID: 9515 RVA: 0x0009B4CC File Offset: 0x000998CC
		private void onTimeTick(object state)
		{
			object deferredMeasures = this.m_deferredMeasures;
			lock (deferredMeasures)
			{
				KeyValuePair<DeferredTelemetryStream.DeferredMeasure, DateTime> keyValuePair = (KeyValuePair<DeferredTelemetryStream.DeferredMeasure, DateTime>)state;
				DeferredTelemetryStream.DeferredMeasure key = keyValuePair.Key;
				DateTime value = keyValuePair.Value;
				string text = value.ToString("yyyy-MM-dd HH:mm:00");
				key.InvokeCallbacks(value);
				for (int num = 0; num != key.Measures.Count; num++)
				{
					if (!key.Measures[num].Dimensions.ContainsKey("date"))
					{
						string value2 = text;
						string format;
						if (key.Measures[num].Dimensions.TryGetValue("date_fmt", out format))
						{
							value2 = value.ToString(format);
						}
						key.Measures[num].Dimensions["date"] = value2;
					}
				}
				this.m_service.AddMeasure(key.Measures);
				key.Measures.Clear();
				if (key.CallbacksCount != 0)
				{
					this.ScheduleDeferredMeasure(key);
				}
				else
				{
					key.Timer.Dispose();
					key.Timer = null;
				}
			}
		}

		// Token: 0x0600252C RID: 9516 RVA: 0x0009B628 File Offset: 0x00099A28
		private void InsertMeasureWithAggregation(List<Measure> measures, Measure msr)
		{
			for (int num = 0; num != measures.Count; num++)
			{
				Measure value = measures[num];
				if (msr.DimensionsEqual(value.Dimensions))
				{
					value.ApplyAggregation(msr);
					measures[num] = value;
					return;
				}
			}
			if (!this.m_aggOps.TryGetValue(msr.Dimensions["stat"], out msr.AggregateOp))
			{
				msr.AggregateOp = this.m_defaultAggOp;
			}
			measures.Add(msr);
		}

		// Token: 0x040012BD RID: 4797
		private TelemetryService m_service;

		// Token: 0x040012BE RID: 4798
		private Dictionary<DeferredTelemetryStream.DeferredSchedule, DeferredTelemetryStream.DeferredMeasure> m_deferredMeasures = new Dictionary<DeferredTelemetryStream.DeferredSchedule, DeferredTelemetryStream.DeferredMeasure>();

		// Token: 0x040012BF RID: 4799
		private EAggOperation m_defaultAggOp;

		// Token: 0x040012C0 RID: 4800
		private Dictionary<string, EAggOperation> m_aggOps;

		// Token: 0x040012C1 RID: 4801
		private Random m_jitterRand = new Random((int)DateTime.Now.Ticks);

		// Token: 0x020006E9 RID: 1769
		private class DeferredSchedule
		{
			// Token: 0x0600252D RID: 9517 RVA: 0x0009B6B9 File Offset: 0x00099AB9
			public DeferredSchedule(string expr, TimeSpan jitter)
			{
				this.Expression = expr;
				this.Schedule = CrontabSchedule.Parse(expr);
				this.Jitter = jitter;
				this.ValidateConfig();
			}

			// Token: 0x0600252E RID: 9518 RVA: 0x0009B6E4 File Offset: 0x00099AE4
			private void ValidateConfig()
			{
				DateTime nextOccurrence = this.Schedule.GetNextOccurrence(DateTime.Now);
				DateTime nextOccurrence2 = this.Schedule.GetNextOccurrence(nextOccurrence);
				if (nextOccurrence2 - nextOccurrence <= this.Jitter)
				{
					throw new Exception("Jitter should be less than schedule interval");
				}
			}

			// Token: 0x0600252F RID: 9519 RVA: 0x0009B734 File Offset: 0x00099B34
			public override bool Equals(object obj)
			{
				return !object.ReferenceEquals(null, obj) && (object.ReferenceEquals(this, obj) || (!(obj.GetType() != typeof(DeferredTelemetryStream.DeferredSchedule)) && this.Equals((DeferredTelemetryStream.DeferredSchedule)obj)));
			}

			// Token: 0x06002530 RID: 9520 RVA: 0x0009B788 File Offset: 0x00099B88
			public bool Equals(DeferredTelemetryStream.DeferredSchedule other)
			{
				return !object.ReferenceEquals(null, other) && (object.ReferenceEquals(this, other) || (other.Expression.Equals(this.Expression) && other.Jitter.Equals(this.Jitter)));
			}

			// Token: 0x06002531 RID: 9521 RVA: 0x0009B7E0 File Offset: 0x00099BE0
			public override int GetHashCode()
			{
				return this.Expression.GetHashCode() ^ this.Jitter.GetHashCode();
			}

			// Token: 0x040012C2 RID: 4802
			public readonly string Expression;

			// Token: 0x040012C3 RID: 4803
			public readonly CrontabSchedule Schedule;

			// Token: 0x040012C4 RID: 4804
			public readonly TimeSpan Jitter;
		}

		// Token: 0x020006EA RID: 1770
		private class DeferredMeasure
		{
			// Token: 0x06002532 RID: 9522 RVA: 0x0009B80D File Offset: 0x00099C0D
			public DeferredMeasure(DeferredTelemetryStream.DeferredSchedule sched)
			{
				this.Sched = sched;
			}

			// Token: 0x140000A0 RID: 160
			// (add) Token: 0x06002533 RID: 9523 RVA: 0x0009B828 File Offset: 0x00099C28
			// (remove) Token: 0x06002534 RID: 9524 RVA: 0x0009B860 File Offset: 0x00099C60
			public event MeasureCallback Callbacks;

			// Token: 0x17000398 RID: 920
			// (get) Token: 0x06002535 RID: 9525 RVA: 0x0009B896 File Offset: 0x00099C96
			public int CallbacksCount
			{
				get
				{
					return (this.Callbacks == null) ? 0 : this.Callbacks.GetInvocationList().Length;
				}
			}

			// Token: 0x06002536 RID: 9526 RVA: 0x0009B8B6 File Offset: 0x00099CB6
			public void InvokeCallbacks(DateTime forDate)
			{
				if (this.Callbacks != null)
				{
					this.Callbacks(this.Measures, forDate);
				}
			}

			// Token: 0x040012C5 RID: 4805
			public readonly DeferredTelemetryStream.DeferredSchedule Sched;

			// Token: 0x040012C6 RID: 4806
			public readonly List<Measure> Measures = new List<Measure>();

			// Token: 0x040012C7 RID: 4807
			public SafeTimer Timer;
		}
	}
}
