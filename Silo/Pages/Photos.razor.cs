// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

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
}
