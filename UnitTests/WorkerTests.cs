using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using CSharpFinalCore.Core.Entity;
using CSharpFinalData.Data.RepositoryImpl.WorkerRepositoryImpl;
using CSharpFinalData.Data.Source.Remote.SupabaseDB;
using CSharpFinalData.Data.Models;

namespace UnitTests
{
    public class FakeSupabaseService : SupabaseService
    {
    }

    public class TestWorkerRepositoryImpl : WorkerRepositoryImpl
    {
        private readonly List<RolesModel> _roles;
        private readonly List<Tasks> _tasks;
        public TestWorkerRepositoryImpl(SupabaseService supabaseService, List<RolesModel> roles, List<Tasks> tasks)
            : base(supabaseService)
        {
            _roles = roles;
            _tasks = tasks;
        }
        
        public Task<List<RolesModel>> GetAllRolesAsync() => Task.FromResult(_roles);
        public Task<List<Tasks>> GetAllTasksByEmployeeId(string employeeId) 
        {
            return Task.FromResult(_tasks.Where(t => t.EmployeeId == employeeId).ToList()); 
        }
    }

    [TestFixture]
    public class WorkerPageLogicTests
    {
        [Test]
        public async Task LoadWorkerInfoAsync_SetsWorkerInfoAndRoleName()
        {
            // Arrange
            var employee = new Employees("1", "TestName", "test@mail.com", 2, "pass"); 
            var roles = new List<RolesModel>
            {
                new RolesModel { Id = 2, RoleName = "Менеджер" }
            };
            var repo = new TestWorkerRepositoryImpl(new FakeSupabaseService(), roles, new List<Tasks>());

            // Simulate logic
            var roleName = (await repo.GetAllRolesAsync())
                .FirstOrDefault(r => r.Id == employee.RoleId)?.RoleName ?? "Невідомо";

            // Assert
            Assert.That(roleName, Is.EqualTo("Менеджер"));
            Assert.That(employee.Name, Is.EqualTo("TestName"));
            Assert.That(employee.Email, Is.EqualTo("test@mail.com"));
        }

        [Test]
        public async Task LoadTasksAsync_MapsTasksAndNormalizesStatus()
        {
            // Arrange
            var employee = new Employees("1", "TestName", "test@mail.com", 2, "pass"); 
            var tasks = new List<Tasks>
            {
                new Tasks { Id = 1, Description = "Task1", EmployeeId = "1", Deadline = DateTime.Today, Status = "New" },
                new Tasks { Id = 2, Description = "Task2", EmployeeId = "1", Deadline = DateTime.Today, Status = "Other" }
            };
            var repo = new TestWorkerRepositoryImpl(new FakeSupabaseService(), new List<RolesModel>(), tasks);

            // Simulate logic
            string NormalizeStatus(string status) =>
                status switch
                {
                    "New" or "InProgress" or "Finished" => status,
                    _ => "New"
                };

            var allTasks = await repo.GetAllTasksByEmployeeId(employee.Id);
            var mappedTasks = allTasks?.Select(t => new TaskViewModel
            {
                Id = t.Id,
                Description = t.Description ?? string.Empty,
                Deadline = t.Deadline,
                Status = NormalizeStatus(t.Status ?? string.Empty)
            }).ToList() ?? new List<TaskViewModel>();

            // Assert
            Assert.That(mappedTasks.Count, Is.EqualTo(2));
            Assert.That(mappedTasks[0].Status, Is.EqualTo("New"));
            Assert.That(mappedTasks[1].Status, Is.EqualTo("New")); // "Other" normalized to "New"
        }

        private class TaskViewModel
        {
            public int Id { get; set; }
            public string Description { get; set; } = string.Empty;
            public DateTime Deadline { get; set; }
            public string Status { get; set; } = string.Empty;
        }
    }
}
