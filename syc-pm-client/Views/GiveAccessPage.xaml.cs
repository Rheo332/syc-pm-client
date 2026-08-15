using Microsoft.UI.Xaml.Controls;
using syc_pm_client.Models;
using syc_pm_client.Viewmodels;
using System;
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

    private async void DeleteUser_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (DataContext is GiveAccessViewModel vm)
        {
            if (vm.SelectedUser == null)
            {
                await vm.DeleteUser();
            }
            else
            {
                var dialog = new ContentDialog
                {
                    Title = "Delete User",
                    Content = $"Are you sure you want to delete {vm.SelectedUser.Username}? This action cannot be undone.",
                    PrimaryButtonText = "Delete",
                    CloseButtonText = "Cancel",
                    DefaultButton = ContentDialogButton.Close,
                    XamlRoot = this.XamlRoot
                };

                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    await vm.DeleteUser();
                }
            }
        }
    }
}
