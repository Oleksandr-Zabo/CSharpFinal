using CSharpFinalCore.Core.Entity;

namespace CSharpFinalData.Data.Models;

using CSharpFinalCore.Core.Entity;

public class AdapterEmployeeFromModel : Employees
{
    public AdapterEmployeeFromModel(EmployeesModel model)
    {
        Id = model.Id;
        Name = model.Name;
        Email = model.Email;
        RoleId = model.RoleId;
        Password = model.Password;
    }
}

public class AdapterEmployeeToModel : EmployeesModel
{
    public AdapterEmployeeToModel(Employees employee)
    {
        Id = employee.Id;
        Name = employee.Name;
        Email = employee.Email;
        RoleId = employee.RoleId;
        Password = employee.Password;
    }
}

public class AdapterTaskFromModel : Tasks
{
    public AdapterTaskFromModel(TasksModel model)
    {
        Id = model.Id;
        Description = model.Description;
        EmployeeId = model.EmployeeId;
        Deadline = model.Deadline;
        Status = model.Status;
    }
}

public class AdapterTaskToModel : TasksModel
{
    public AdapterTaskToModel(Tasks task)
    {
        Id = task.Id;
        Description = task.Description;
        EmployeeId = task.EmployeeId;
        Deadline = task.Deadline;
        Status = task.Status;
    }
}
