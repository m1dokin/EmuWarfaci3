using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterServer.GameLogic.GameModes
{
	// Token: 0x02000300 RID: 768
	public class RoomRestrictionDesc
	{
		// Token: 0x060011C1 RID: 4545 RVA: 0x000463A8 File Offset: 0x000447A8
		public RoomRestrictionDesc(bool isGlobal, string channels)
		{
			this.Channels = channels;
			this.isGlobal = isGlobal;
			this.options = new Dictionary<string, RoomRestrictionOption>();
			this.defaultValues = new Dictionary<string, string>();
		}

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x060011C2 RID: 4546 RVA: 0x000463D4 File Offset: 0x000447D4
		// (set) Token: 0x060011C3 RID: 4547 RVA: 0x000463DC File Offset: 0x000447DC
		public string Channels { get; private set; }

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x060011C4 RID: 4548 RVA: 0x000463E5 File Offset: 0x000447E5
		// (set) Token: 0x060011C5 RID: 4549 RVA: 0x000463ED File Offset: 0x000447ED
		public bool isGlobal { get; private set; }

		// Token: 0x17000198 RID: 408
		// (get) Token: 0x060011C6 RID: 4550 RVA: 0x000463F6 File Offset: 0x000447F6
		public string FirstOption
		{
			get
			{
				return this.options.Keys.First<string>();
			}
		}

		// Token: 0x060011C7 RID: 4551 RVA: 0x00046408 File Offset: 0x00044808
		public void Add(string key, string defaultValue, RoomRestrictionOption option)
		{
			this.options.Add(key, option);
			this.defaultValues.Add(key, defaultValue);
		}

		// Token: 0x060011C8 RID: 4552 RVA: 0x00046424 File Offset: 0x00044824
		public bool ContainsValue(string optionId, string value)
		{
			RoomRestrictionOption roomRestrictionOption;
			this.options.TryGetValue(optionId, out roomRestrictionOption);
			return roomRestrictionOption != null && roomRestrictionOption.values.Contains(value);
		}

		// Token: 0x060011C9 RID: 4553 RVA: 0x00046458 File Offset: 0x00044858
		public bool HasValues(string optionId)
		{
			RoomRestrictionOption roomRestrictionOption;
			this.options.TryGetValue(optionId, out roomRestrictionOption);
			return roomRestrictionOption != null && roomRestrictionOption.values.Any<string>();
		}

		// Token: 0x060011CA RID: 4554 RVA: 0x00046488 File Offset: 0x00044888
		public string GetDefault(string optionId)
		{
			string text;
			this.defaultValues.TryGetValue(optionId, out text);
			return (!string.IsNullOrEmpty(text)) ? text : null;
		}

		// Token: 0x060011CB RID: 4555 RVA: 0x000464B6 File Offset: 0x000448B6
		public void SetDefault(string optionId, string value)
		{
			this.defaultValues[optionId] = value;
		}

		// Token: 0x040007E5 RID: 2021
		private Dictionary<string, RoomRestrictionOption> options;

		// Token: 0x040007E6 RID: 2022
		private Dictionary<string, string> defaultValues;
	}
}
