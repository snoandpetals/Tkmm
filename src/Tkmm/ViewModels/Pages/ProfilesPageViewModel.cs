﻿using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using Tkmm.Core.Components;
using Tkmm.Core.Components.Models;
using Tkmm.Core.Models.Mods;

namespace Tkmm.ViewModels.Pages;

public partial class ProfilesPageViewModel : ObservableObject
{
    [ObservableProperty]
    private ProfileMod? _selected;

    [ObservableProperty]
    private Mod? _masterSelected;

    [RelayCommand]
    private void Remove()
    {
        if (Selected is not null) {
            ProfileManager.Shared.Current.Mods.Remove(Selected);
        }
    }

    [RelayCommand]
    private Task MoveUp()
    {
        if (Selected is not null) {
            Selected = ProfileManager.Shared.Current.Move(Selected, -1);
        }

        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task MoveDown()
    {
        if (Selected is not null) {
            Selected = ProfileManager.Shared.Current.Move(Selected, 1);
        }

        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task Uninstall()
    {
        if (MasterSelected is not Mod mod) {
            return;
        }

        ContentDialog dialog = new() {
            Content = $"""
            Are you sure you would like to uninstall '{mod.Name}'?

            This cannot be undone.
            """,
            IsPrimaryButtonEnabled = true,
            IsSecondaryButtonEnabled = true,
            PrimaryButtonText = "Uninstall Mod",
            SecondaryButtonText = "Cancel",
            Title = "Warning"
        };

        if (await dialog.ShowAsync() == ContentDialogResult.Primary) {
            mod.Uninstall();
        }
    }

    [RelayCommand]
    private static async Task DeleteCurrentProfile()
    {
        if (ProfileManager.Shared.Profiles.Count < 2) {
            App.Toast("Cannot delete the last profile!", "Error", NotificationType.Error);
            return;
        }

        ContentDialog dialog = new() {
            Title = "Delete Profile",
            Content = $"""
            Are you sure you would like to delete the profile '{ProfileManager.Shared.Current.Name}'?

            This cannot be undone.
            """,
            PrimaryButtonText = "Delete",
            SecondaryButtonText = "Cancel",
        };

        if (await dialog.ShowAsync() != ContentDialogResult.Primary) {
            return;
        }

        int currentIndex = ProfileManager.Shared.Profiles.IndexOf(ProfileManager.Shared.Current);
        ProfileManager.Shared.Profiles.RemoveAt(currentIndex);
        ProfileManager.Shared.Current = ProfileManager.Shared.Profiles[currentIndex == ProfileManager.Shared.Profiles.Count
            ? --currentIndex : ++currentIndex
        ];
    }
}