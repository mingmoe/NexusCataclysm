// <copyright file="IPlugin.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Autofac.Extension;

namespace NexusCataclysm.Core;

public interface IPlugin
{
	static abstract IPluginInformation Information { get; }

	static abstract void Initialize(ILauncher launcher);

	void Run(ExtendedHostBuilder builder);
}
