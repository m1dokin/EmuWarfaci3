using System;

namespace MasterServer.GFaceAPI
{
	// Token: 0x02000637 RID: 1591
	public struct GFaceError
	{
		// Token: 0x06002217 RID: 8727 RVA: 0x0008E9E6 File Offset: 0x0008CDE6
		public GFaceError(GErrorCode errCode, string errMsg)
		{
			this.ErrorCode = errCode;
			this.ErrorMessage = errMsg;
		}

		// Token: 0x17000368 RID: 872
		// (get) Token: 0x06002218 RID: 8728 RVA: 0x0008E9F6 File Offset: 0x0008CDF6
		public bool HasError
		{
			get
			{
				return this.ErrorCode != GErrorCode.NoError;
			}
		}

		// Token: 0x06002219 RID: 8729 RVA: 0x0008EA04 File Offset: 0x0008CE04
		public void Rethrow()
		{
			if (this.ErrorCode == GErrorCode.NoError)
			{
				throw new InvalidOperationException("GFaceError: No error to throw!");
			}
			throw GFaceError.CreateException(this);
		}

		// Token: 0x0600221A RID: 8730 RVA: 0x0008EA27 File Offset: 0x0008CE27
		public override string ToString()
		{
			return string.Format("[{0}:{1}] {2}", this.ErrorCode, (int)this.ErrorCode, this.ErrorMessage ?? "<N/A>");
		}

		// Token: 0x0600221B RID: 8731 RVA: 0x0008EA5C File Offset: 0x0008CE5C
		public static GFaceException CreateException(GFaceError err)
		{
			GErrorCode errorCode = err.ErrorCode;
			switch (errorCode + 6)
			{
			case GErrorCode.NoError:
				return new GFaceRequestPoolTimeout(err);
			case (GErrorCode)1:
			case (GErrorCode)4:
				goto IL_B2;
			case (GErrorCode)2:
				break;
			case (GErrorCode)3:
				goto IL_A4;
			default:
				switch (errorCode)
				{
				case GErrorCode.InvalidSession:
					goto IL_A4;
				case GErrorCode.UserLocked:
					return new GFaceFatalException(err);
				case GErrorCode.InvalidAPIKey:
					break;
				default:
					switch (errorCode)
					{
					case GErrorCode.NoWallet:
					case GErrorCode.NotEnoughPoints:
					case GErrorCode.NotEnoughCredits:
						break;
					default:
						if (errorCode != GErrorCode.SeedNotExist && errorCode != GErrorCode.SeedHasBeenRemoved)
						{
							switch (errorCode)
							{
							case GErrorCode.InternalError:
								return new GFaceThirdPartyException(err);
							default:
								if (errorCode != GErrorCode.InvalidCredential)
								{
									return new GFaceException(err);
								}
								goto IL_9D;
							case GErrorCode.IncorrectArguments:
								goto IL_B2;
							}
						}
						break;
					}
					return new GFaceUserStateException(err);
				}
				break;
			}
			IL_9D:
			return new GFaceConfigException(err);
			IL_A4:
			return new GFaceSessionException(err);
			IL_B2:
			return new GFaceProtocolException(err);
		}

		// Token: 0x040010DE RID: 4318
		public GErrorCode ErrorCode;

		// Token: 0x040010DF RID: 4319
		public string ErrorMessage;

		// Token: 0x040010E0 RID: 4320
		public static readonly GFaceError TheNoError = new GFaceError
		{
			ErrorCode = GErrorCode.NoError
		};
	}
}
