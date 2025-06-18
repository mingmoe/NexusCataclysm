using Autofac;
using Autofac.Extension;
using Microsoft.Extensions.Logging;
using SDL3;
using System.Runtime.InteropServices;

namespace NexusCataclysm.Client;

internal sealed class Client(ILifetimeScope scope) : ExtendedHost(scope)
{
	public required FNAGame Game { get; init; }

	protected override Task Main()
	{
		Game.Run();
		return Task.CompletedTask;
	}

	protected override Task Start(CancellationToken abortStart)
	{
		unsafe
		{
			// take over the SDL logging
			SDL.SDL_SetLogOutputFunction((nint userdata, int category, SDL.SDL_LogPriority priority, byte* message) =>
			{
				string cate = ((SDL.SDL_LogCategory) category).ToString();

				Action<string, object[]> func;

				if (priority == SDL.SDL_LogPriority.SDL_LOG_PRIORITY_DEBUG)
				{
					func = Logger.LogDebug;
				}
				else if (priority == SDL.SDL_LogPriority.SDL_LOG_PRIORITY_TRACE
				|| priority == SDL.SDL_LogPriority.SDL_LOG_PRIORITY_VERBOSE)
				{
					func = Logger.LogTrace;
				}
				else if (priority == SDL.SDL_LogPriority.SDL_LOG_PRIORITY_INFO
				|| priority == SDL.SDL_LogPriority.SDL_LOG_PRIORITY_COUNT)
				{
					func = Logger.LogInformation;
				}
				else if (priority == SDL.SDL_LogPriority.SDL_LOG_PRIORITY_ERROR)
				{
					func = Logger.LogError;
				}
				else if (priority == SDL.SDL_LogPriority.SDL_LOG_PRIORITY_CRITICAL)
				{
					func = Logger.LogCritical;
				}
				else
				{
					func = Logger.LogWarning;
				}

				var msg = Marshal.PtrToStringUTF8((nint) message);

				func("SDL log({category}):{message}", [cate, msg ?? string.Empty]);
			}, 0);
		}

		return Task.CompletedTask;
	}

	protected override Task Stop(CancellationToken stopGracefullyShutdown)
	{
		unsafe
		{
			nint @default = SDL.SDL_GetDefaultLogOutputFunction();
			SDL.SDL_SetLogOutputFunction(
				Marshal.GetDelegateForFunctionPointer<SDL.SDL_LogOutputFunction>(@default),
				0);
		}
		return Task.CompletedTask;
	}
}
