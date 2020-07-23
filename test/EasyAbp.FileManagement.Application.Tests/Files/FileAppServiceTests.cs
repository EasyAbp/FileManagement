using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace EasyAbp.FileManagement.Files
{
    public class FileAppServiceTests : FileManagementApplicationTestBase
    {
        private readonly IFileAppService _fileAppService;

        public FileAppServiceTests()
        {
            _fileAppService = GetRequiredService<IFileAppService>();
        }

        [Fact]
        public async Task Test1()
        {
            // Arrange

            // Act

            // Assert
        }
    }
}
