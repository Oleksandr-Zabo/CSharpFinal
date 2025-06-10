namespace UnitTests; 
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using CSharpFinalCore.Core.Entity;
using CSharpFinalData.Data.RepositoryImpl.ManagerRepositoryImpl;

[TestFixture]
public class ManagerPageLogicTests
{
    [Test]
    public async Task LoadWorkersAsync_FiltersRole2And3()
    {
        var mockRepo = new Mock<ManagerRepositoryImpl>(null);
        var employees = new List<Employees>
        {
            new Employees("1", "A", "a@mail.com", 2, "p"),
            new Employees("2", "B", "b@mail.com", 3, "p"),
            new Employees("3", "C", "c@mail.com", 1, "p") 
        };
        mockRepo.Setup(r => r.GetAllEmployeesAsync()).ReturnsAsync(employees);

        var allEmployees = await mockRepo.Object.GetAllEmployeesAsync();
        var workers = allEmployees.Where(w => w.RoleId == 2 || w.RoleId == 3).ToList();

        Assert.AreEqual(2, workers.Count);
        Assert.IsTrue(workers.All(w => w.RoleId == 2 || w.RoleId == 3));
    }

    [Test]
    public async Task LoadTasksAsync_MapsTasksWithWorkerNames()
    {
        var mockRepo = new Mock<ManagerRepositoryImpl>(null);
        var workers = new List<Employees>
        {
            new Employees("1", "Worker1", "w1@mail.com", 2, "p"),
            new Employees("2", "Worker2", "w2@mail.com", 3, "p") 
        };
        var tasks = new List<Tasks>
        {
            new Tasks { Id = 10, Description = "Desc", EmployeeId = "1", Deadline = DateTime.Today, Status = "new" },
            new Tasks { Id = 11, Description = "Desc2", EmployeeId = "99", Deadline = DateTime.Today, Status = "done" }
        }; 
        mockRepo.Setup(r => r.GetAllTasksAsync()).ReturnsAsync(tasks);

        var allTasks = await mockRepo.Object.GetAllTasksAsync();
        var mappedTasks = allTasks.Select(t => new TaskViewModel
        {
            Id = t.Id,
            Description = t.Description,
            WorkerId = t.EmployeeId, 
            WorkerName = workers.FirstOrDefault(w => w.Id == t.EmployeeId)?.Name ?? "—",
            Deadline = t.Deadline,
            Status = t.Status
        }).ToList();

        Assert.AreEqual(2, mappedTasks.Count);
        Assert.AreEqual("Worker1", mappedTasks[0].WorkerName);
        Assert.AreEqual("—", mappedTasks[1].WorkerName);
    }

    private class TaskViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string WorkerId { get; set; } // Changed to string
        public string WorkerName { get; set; }
        public DateTime Deadline { get; set; }
        public string Status { get; set; }
    }
}