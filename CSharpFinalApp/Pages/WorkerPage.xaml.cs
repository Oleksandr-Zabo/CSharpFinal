using System.Windows.Controls;
using CSharpFinalCore.Core.Entity;
using CSharpFinalData.Data.Source.Remote.SupabaseDB;

namespace CSharpFinal.Pages;

public partial class WorkerPage : UserControl
{
    private SupabaseService _supabaseService;
    private Employees _employee;
    public WorkerPage(Employees employee, SupabaseService? service)
    {
        InitializeComponent();
        _employee = employee ?? throw new ArgumentNullException(nameof(employee));
        _supabaseService = service ?? throw new ArgumentNullException(nameof(service));
    }
}