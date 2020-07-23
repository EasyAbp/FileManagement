using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Volo.Abp;

namespace EasyAbp.FileManagement.Containers
{
    public class FileContainerConfigurations
    {
        private readonly Dictionary<string, FileContainerConfiguration> _containers;
        
        public FileContainerConfigurations()
        {
            _containers = new Dictionary<string, FileContainerConfiguration>();
        }

        public FileContainerConfigurations Configure<TContainer>(
            Action<FileContainerConfiguration> configureAction)
        {
            return Configure(
                FileContainerNameAttribute.GetContainerName<TContainer>(),
                configureAction
            );
        }

        public FileContainerConfigurations Configure(
            [NotNull] string name,
            [NotNull] Action<FileContainerConfiguration> configureAction)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(configureAction, nameof(configureAction));

            configureAction(
                _containers.GetOrAdd(
                    name,
                    () => new FileContainerConfiguration()
                )
            );

            return this;
        }

        public FileContainerConfigurations ConfigureAll(Action<string, FileContainerConfiguration> configureAction)
        {
            foreach (var container in _containers)
            {
                configureAction(container.Key, container.Value);
            }
            
            return this;
        }

        [NotNull]
        public FileContainerConfiguration GetConfiguration<TContainer>()
        {
            return GetConfiguration(FileContainerNameAttribute.GetContainerName<TContainer>());
        }

        [NotNull]
        public FileContainerConfiguration GetConfiguration([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            return _containers.GetOrDefault(name);
        }
    }
}