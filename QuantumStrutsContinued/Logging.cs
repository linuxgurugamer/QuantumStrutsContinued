// ToadicusTools
//
// Logging.cs
//
// Copyright © 2015, toadicus
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
// 1. Redistributions of source code must retain the above copyright notice,
//    this list of conditions and the following disclaimer.
//
// 2. Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation and/or other
//    materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
//using ToadicusTools.Text;
using UnityEngine;
using System.Text;

namespace QuantumStrut
{
    public enum LogChannel
    {
        Log,
        Warning,
        Error
    }

    public static class Logging
	{
		public static void PostLogMessage(LogChannel channel, string Msg)
		{
			switch (channel)
			{
				case LogChannel.Log:
					#if DEBUG
					Debug.Log(Msg);
					#else
					KSPLog.print(Msg);
					#endif
					break;
				case LogChannel.Warning:
					Debug.LogWarning(Msg);
					break;
				case LogChannel.Error:
					Debug.LogError(Msg);
					break;
				default:
					throw new NotImplementedException("Invalid channel, must pick one of Log, Warning, or Error.");
			}
		}
#if true
        public static void PostLogMessage(LogChannel channel, string Format, params object[] args)
		{
			string message = string.Format(Format, args);

			PostLogMessage(message);
		}
#endif
		public static void PostLogMessage(string Msg)
		{
			PostLogMessage(LogChannel.Log, Msg);
		}

		public static void PostLogMessage(string Format, params object[] args)
		{
			PostLogMessage(LogChannel.Log, Format, args);
		}

		public static void PostWarningMessage(string Msg)
		{
			PostLogMessage(LogChannel.Warning, Msg);
		}

		public static void PostWarningMessage(string Format, params object[] args)
		{
			PostLogMessage(LogChannel.Warning, Format, args);
		}

		public static void PostErrorMessage(string Msg)
		{
			PostLogMessage(LogChannel.Error, Msg);
		}

		public static void PostErrorMessage(string Format, params object[] args)
		{
			PostLogMessage(LogChannel.Error, Format, args);
		}

		[System.Diagnostics.Conditional("DEBUG")]
		public static void PostDebugMessage(string Msg)
		{
			PostMessageWithScreenMsg(Msg);
		}

		[System.Diagnostics.Conditional("DEBUG")]
		public static void PostDebugMessage(object Sender, params object[] args)
		{

            StringBuilder sb = new StringBuilder();
			{
				sb.AppendFormat("{0}:", Sender.GetType().Name);

				object arg;
				for (int idx = 0; idx < args.Length; idx++)
				{
					arg = args[idx];

					sb.AppendFormat("\n\t{0}", arg.ToString());
				}

				PostMessageWithScreenMsg(sb.ToString());
			}
		}

		[System.Diagnostics.Conditional("DEBUG")]
		public static void PostDebugMessage(object Sender, string Format, params object[] args)
		{
            StringBuilder sb = new StringBuilder();
            {

				if (Sender != null)
				{
					Type type = (Sender is Type) ? Sender as Type : Sender.GetType();
					sb.Append(type.Name);
					sb.Append(": ");
				}

				sb.AppendFormat(Format, args);

				PostMessageWithScreenMsg(sb.ToString());
			}
		}


		private static ScreenMessage debugmsg = new ScreenMessage("", 4f, ScreenMessageStyle.UPPER_RIGHT);
		public static void PostMessageWithScreenMsg(string Msg)
		{
			if (HighLogic.LoadedScene > GameScenes.SPACECENTER)
			{

				debugmsg.message = Msg;
				ScreenMessages.PostScreenMessage(debugmsg);
			}

			PostLogMessage(Msg, LogChannel.Log);
		}
	}
}

