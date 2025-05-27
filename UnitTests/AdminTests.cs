using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using CSharpFinalCore.Core.Entity;
using CSharpFinalData.Data.RepositoryImpl.AdminRepositoryImpl;

public class AdminService
{
    private readonly AdminRepositoryImpl _adminRepository;

    public AdminService(AdminRepositoryImpl adminRepository)
    {
        _adminRepository = adminRepository;
    }

    public async Task<List<Employees>> GetEmployeesAsync()
    {
        var list = await _adminRepository.GetAllEmployeesAsync();
        return list?.Select(e => new Employees(e.Id, e.Name, e.Email, e.RoleId, e.Password)).ToList() ?? new List<Employees>();
    }
}

[TestFixture]
public class AdminServiceTests
{
    [Test]
    public async Task GetEmployeesAsync_ReturnsMappedEmployees()
    {
        // Arrange
        var mockRepo = new Mock<AdminRepositoryImpl>(null);
        var employees = new List<Employees>
        {
            new Employees(1, "Test", "test@mail.com", 1, "pass")
        };
        mockRepo.Setup(r => r.GetAllEmployeesAsync()).ReturnsAsync(employees);

        var service = new AdminService(mockRepo.Object);

        // Act
        var result = await service.GetEmployeesAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Test", result[0].Name);
    }
}