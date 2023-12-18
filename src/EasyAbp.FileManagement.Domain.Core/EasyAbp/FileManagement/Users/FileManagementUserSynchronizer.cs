using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace EasyAbp.FileManagement.Users;

public class FileManagementUserSynchronizer :
    IDistributedEventHandler<EntityUpdatedEto<UserEto>>,
    ITransientDependency
{
    protected IFileUserRepository UserRepository { get; }
    protected IFileUserLookupService UserLookupService { get; }

    public FileManagementUserSynchronizer(
        IFileUserRepository userRepository, 
        IFileUserLookupService userLookupService)
    {
        UserRepository = userRepository;
        UserLookupService = userLookupService;
    }

    [UnitOfWork]
    public async Task HandleEventAsync(EntityUpdatedEto<UserEto> eventData)
    {
        var user = await UserRepository.FindAsync(eventData.Entity.Id);
        if (user == null)
        {
            user = await UserLookupService.FindByIdAsync(eventData.Entity.Id);
            if (user == null)
            {
                return;
            }
        }

        if (user.Update(eventData.Entity))
        {
            await UserRepository.UpdateAsync(user);
        }
    }
}