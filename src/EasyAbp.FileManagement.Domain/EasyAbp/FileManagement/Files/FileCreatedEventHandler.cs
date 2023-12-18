using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;

namespace EasyAbp.FileManagement.Files;

public class FileCreatedEventHandler : ILocalEventHandler<EntityCreatedEventData<File>>, ITransientDependency
{
    private readonly IFileRepository _repository;

    public FileCreatedEventHandler(IFileRepository repository)
    {
        _repository = repository;
    }

    public virtual async Task HandleEventAsync(EntityCreatedEventData<File> eventData)
    {
        // Update the entity to set the LastModificationTime property.
        await _repository.UpdateAsync(eventData.Entity, true);
    }
}