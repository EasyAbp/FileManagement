using System;
using System.Reflection;
using JetBrains.Annotations;
using Volo.Abp;

namespace EasyAbp.FileManagement.Options.Containers
{
    public class FileContainerNameAttribute : Attribute
    {
        [NotNull]
        public string Name { get; }

        public FileContainerNameAttribute([NotNull] string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            Name = name;
        }

        public virtual string GetName(Type type)
        {
            return Name;
        }

        public static string GetContainerName<T>()
        {
            return GetContainerName(typeof(T));
        }

        public static string GetContainerName(Type type)
        {
            var nameAttribute = type.GetCustomAttribute<FileContainerNameAttribute>();

            if (nameAttribute == null)
            {
                return type.FullName;
            }

            return nameAttribute.GetName(type);
        }
    }
}