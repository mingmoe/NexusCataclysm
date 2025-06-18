// <copyright file="IPluginInformation.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace NexusCataclysm.Core;

public interface IPluginInformation
{
	public Guuid Id { get; }

	public string DisplayName { get; }

	public Semver.SemVersion Version { get; }

	public IEnumerable<(Guuid pluginId, Semver.SemVersionRange versionRange)> Dependencies { get; }
}
