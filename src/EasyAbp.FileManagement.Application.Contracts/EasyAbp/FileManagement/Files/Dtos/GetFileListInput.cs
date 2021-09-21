using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace EasyAbp.FileManagement.Files.Dtos
{
    public class GetFileListInput : PagedAndSortedResultRequestDto
    {
        public Guid? ParentId { get; set; }
        
        [Required]
        public string FileContainerName { get; set; }
        
        public Guid? OwnerUserId { get; set; }
        
        public bool DirectoryOnly { get; set; }

        public string Filter { get; set; }
    }
}