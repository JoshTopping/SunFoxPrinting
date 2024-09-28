// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Components.Forms;
using Orleans.ShoppingCart.Silo.Components;

namespace Orleans.ShoppingCart.Silo.Pages;

public sealed partial class Photos
{
    private HashSet<PhotoDetails>? _photos;
    private ManagePhotoModal? _modal;

    [Parameter]
    public string? Id { get; set; }

    [Inject]
    public InventoryService InventoryService { get; set; } = null!;

    [Inject]
    public PhotoService PhotoService { get; set; } = null!;

    [Inject]
    public IDialogService DialogService { get; set; } = null!;

#nullable enable
    private const string DefaultDragClass = "relative rounded-lg border-2 border-dashed pa-4 mt-4 mud-width-full mud-height-full";
    private string _dragClass = DefaultDragClass;
    private readonly List<string> _fileNames = new();
    private MudFileUpload<IReadOnlyList<IBrowserFile>>? _fileUpload;

    protected override async Task OnInitializedAsync() =>
        _photos = await InventoryService.GetAllPhotosAsync();

    private void UploadPhotos()
    {
        if (_modal is not null)
        {
            var photo = new PhotoDetails();
           
            _modal.Photo = photo with
            {
                Id = "1",
            };
            _modal.Open("Create Photo", OnPhotoUpdated);
        }
    }

    private async Task OnPhotoUpdated(PhotoDetails photo)
    {
        await PhotoService.CreateOrUpdatePhotoAsync(photo);
        _photos = await InventoryService.GetAllPhotosAsync();

        _modal?.Close();

        StateHasChanged();
    }

    private Task OnEditPhoto(PhotoDetails photo) =>
        photo is not null ? OnPhotoUpdated(photo) : Task.CompletedTask;



    private async Task ClearAsync()
    {
        await (_fileUpload?.ClearAsync() ?? Task.CompletedTask);
        _fileNames.Clear();
        ClearDragClass();
    }

    private Task OpenFilePickerAsync()
        => _fileUpload?.OpenFilePickerAsync() ?? Task.CompletedTask;

    private void OnInputFileChanged(InputFileChangeEventArgs e)
    {
        ClearDragClass();
        var files = e.GetMultipleFiles();
        foreach (var file in files)
        {
            _fileNames.Add(file.Name);
        }
    }

    private void Upload()
    {
        // Upload the files here
        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
        Snackbar.Add("TODO: Upload your files!");



        PhotoService.CreateOrUpdatePhotoAsync(_fileUpload.Files.First());
    }

    private void SetDragClass()
        => _dragClass = $"{DefaultDragClass} mud-border-primary";

    private void ClearDragClass()
        => _dragClass = DefaultDragClass;
}
