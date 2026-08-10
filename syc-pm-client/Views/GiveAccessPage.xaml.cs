using Microsoft.UI.Xaml.Controls;
using syc_pm_client.Models;
using syc_pm_client.Viewmodels;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;

namespace syc_pm_client.Views;

public sealed partial class GiveAccessPage : Page
{
    private PwEntry? _draggedEntry;
    private bool _draggedFromAvailable;

    public GiveAccessPage(GiveAccessViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }

    private void AvailableListView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
    {
        _draggedEntry = e.Items.FirstOrDefault() as PwEntry;
        _draggedFromAvailable = true;
    }

    private void GrantedListView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
    {
        _draggedEntry = e.Items.FirstOrDefault() as PwEntry;
        _draggedFromAvailable = false;
    }

    private void ListView_DragOver(object sender, Microsoft.UI.Xaml.DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Move;
    }

    private async void AvailableListView_Drop(object sender, Microsoft.UI.Xaml.DragEventArgs e)
    {
        if (_draggedEntry != null && !_draggedFromAvailable)
        {
            if (DataContext is GiveAccessViewModel vm)
            {
                await vm.RevokeAccessAsync(_draggedEntry);
            }
        }
        _draggedEntry = null;
    }

    private async void GrantedListView_Drop(object sender, Microsoft.UI.Xaml.DragEventArgs e)
    {
        if (_draggedEntry != null && _draggedFromAvailable)
        {
            if (DataContext is GiveAccessViewModel vm)
            {
                await vm.GrantAccessAsync(_draggedEntry);
            }
        }
        _draggedEntry = null;
    }
}
