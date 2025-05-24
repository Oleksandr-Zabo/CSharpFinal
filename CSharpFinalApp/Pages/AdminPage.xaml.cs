using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CSharpFinalCore.Core.Entity;
using CSharpFinalData.Data.RepositoryImpl.AdminRepositoryImpl;

namespace CSharpFinal.Pages;

public partial class AdminPage : UserControl
{
    private readonly AdminRepositoryImpl _adminRepository;
    private readonly Employees _admin;
    private List<Employees> _employees = new();

    public AdminPage(Employees employee, AdminRepositoryImpl? repository)
    {
        InitializeComponent();
        _admin = employee ?? throw new ArgumentNullException(nameof(employee));
        _adminRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        Loaded += AdminPage_Loaded;
    }

    private async void AdminPage_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadEmployeesAsync();
    }

    private async Task LoadEmployeesAsync()
    {
        try
        {
            var list = await _adminRepository.GetAllEmployeesAsync();
            if (list != null)
            {
                _employees = list.Select(e => new Employees(e.Id, e.Name, e.Email, e.RoleId, e.Password)).ToList();
                EmployeesListBox.ItemsSource = _employees;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Не вдалося завантажити працівників: " + ex.Message);
        }
    }

    private async void OnAddEmployeeClick(object sender, RoutedEventArgs e)
    {
        var name = NameTextBox.Text.Trim();
        var email = EmailTextBox.Text.Trim();
        var password = PasswordBox.Password.Trim();
        int roleId = AdminRadio.IsChecked == true ? 0 :
                     ManagerRadio.IsChecked == true ? 1 :
                     WaiterRadio.IsChecked == true ? 2 :
                     ChefRadio.IsChecked == true ? 3 : -1;

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password) || roleId == -1)
        {
            MessageBox.Show("Будь ласка, заповніть всі поля та виберіть роль.");
            return;
        }

        try
        {
            var newEmployeeModel = new CSharpFinalData.Data.Models.EmployeesModel
            {
                Name = name,
                Email = email,
                Password = password,
                RoleId = roleId
            };
            var newEmployee = new CSharpFinalData.Data.Models.AdapterEmployeeFromModel(newEmployeeModel);
            await _adminRepository.AddEmployeeAsync(newEmployee);
            MessageBox.Show("Працівника додано успішно!");
            await LoadEmployeesAsync();
            NameTextBox.Text = "";
            EmailTextBox.Text = "";
            PasswordBox.Password = "";
            AdminRadio.IsChecked = false;
            ManagerRadio.IsChecked = false;
            WaiterRadio.IsChecked = false;
            ChefRadio.IsChecked = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Помилка при додаванні працівника: " + ex.Message);
        }
    }

    private async void OnDeleteEmployeeClick(object sender, RoutedEventArgs e)
    {
        if (EmployeesListBox.SelectedItem is not Employees selectedEmployee)
        {
            MessageBox.Show("Оберіть працівника для видалення.");
            return;
        }

        if (selectedEmployee.RoleId == 0)
        {
            MessageBox.Show("Адміністратор не може бути видалений.");
            return;
        }

        var result = MessageBox.Show($"Ви впевнені, що хочете видалити {selectedEmployee.Name}?", "Підтвердження", MessageBoxButton.YesNo);
        if (result != MessageBoxResult.Yes) return;

        try
        {
            await _adminRepository.DeleteEmployeeAsync(selectedEmployee.Id);
            MessageBox.Show("Працівника видалено.");
            await LoadEmployeesAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Помилка при видаленні працівника: " + ex.Message);
        }
    }

    private void OnLogoutClick(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("Ви впевнені, що хочете вийти?", "Підтвердження", MessageBoxButton.YesNo);
        if (result != MessageBoxResult.Yes) return;
        //logout from repository if needed
        _ = _adminRepository.Logout();
        var mainWindow = (MainWindow)Application.Current.MainWindow;
        mainWindow?.MainWindowFrame.Navigate(new LoginPage());
    }
}