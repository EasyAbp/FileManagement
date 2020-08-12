using System;
using JetBrains.Annotations;

namespace EasyAbp.FileManagement.Files
{
    [Serializable]
    public class FlagFileEto
    {
        public Guid FileId { get; set; }
        
        [CanBeNull]
        public string Flag { get; set; }
    }
}