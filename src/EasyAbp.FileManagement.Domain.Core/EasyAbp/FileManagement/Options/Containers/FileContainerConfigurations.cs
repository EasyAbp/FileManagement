using System;
using System.Collections.Generic;
using System.Linq;
using EasyAbp.FileManagement.Containers;
using JetBrains.Annotations;
using Volo.Abp;

namespace EasyAbp.FileManagement.Options.Containers
{
    public class FileContainerConfigurations<TConfiguration> where TConfiguration : IFileContainerConfiguration, new()
    {
        private readonly Dictionary<string, TConfiguration> _containers = new();

        public FileContainerConfigurations<TConfiguration> Configure<TContainer>(
            Action<TConfiguration> configureAction)
        {
            return Configure(
                FileContainerNameAttribute.GetContainerName<TContainer>(),
                configureAction
            );
        }

        public FileContainerConfigurations<TConfiguration> Configure(
            [NotNull] string name,
            [NotNull] Action<TConfiguration> configureAction)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(configureAction, nameof(configureAction));

            configureAction(
                _containers.GetOrAdd(
                    name,
                    () => new TConfiguration()
                )
            );

            return this;
        }

        public FileContainerConfigurations<TConfiguration> ConfigureAll(Action<string, TConfiguration> configureAction)
        {
            foreach (var container in _containers)
            {
                configureAction(container.Key, container.Value);
            }

            return this;
        }

        [NotNull]
        public TConfiguration GetConfiguration<TContainer>()
        {
            return GetConfiguration(FileContainerNameAttribute.GetContainerName<TContainer>());
        }

        [NotNull]
        public TConfiguration GetConfiguration([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            return _containers.GetOrDefault(name);
        }

        public List<TConfiguration> GetAllConfigurations()
        {
            return _containers.Values.ToList();
        }
    }
}