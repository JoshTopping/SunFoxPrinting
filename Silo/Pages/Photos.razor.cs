// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using Azure.Storage.Files.Shares.Models;
using Azure.Storage.Files.Shares;
using Microsoft.AspNetCore.Components.Forms;
using Orleans.ShoppingCart.Silo.Components;
using System.Collections;

namespace Orleans.ShoppingCart.Silo.Pages;

public sealed partial class Photos
{
    

    private HashSet<PhotoDetails>? _photos;
    private ManagePhotoModal? _modal;
    private string _imageSource = null!;

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

    protected override void OnInitialized()
    {
        _photos = PhotoService.GetAllPhotos();

        //
        string connectionString = "DefaultEndpointsProtocol=https;AccountName=sunfoxprintingsa;AccountKey=N54lzRdCpBehzbs13MyukpaVSARUdJ7vgKKeR8vfuRXKWeDuf7QrRD86uWnm2whS9up9g06urcK9+AStkLoJXA==;EndpointSuffix=core.windows.net";

        // Name of the share, directory, and file we'll download from
        string shareName = "sunfoxprintingphotos";
        string dirName = "Photos";
        string fileName = "photo 2018-12-28, 4 43 20 pm (1).jpg";
        //string fileName = "IMG_1828.HEIC";

        // Path to the save the downloaded file
        //string localFilePath = @"<path_to_local_file>";

        // Get a reference to the file
        ShareClient share = new ShareClient(connectionString, shareName);
        ShareDirectoryClient directory = share.GetDirectoryClient(dirName);
        ShareFileClient file = directory.GetFileClient(fileName);

        // Download the file
        ShareFileDownloadInfo download = file.Download();
        //using (FileStream stream = File.OpenRead(download.Content))

        using (Stream stream = file.OpenRead())
        {
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            byte[] byteArray = ms.ToArray();
            var imagesrc = download.Content.ToString();
            var b64String = Convert.ToBase64String(byteArray);
            _imageSource = "data:image/jpg;base64," + b64String;
        }
    }

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
        //_photos = await PhotoService.GetAllPhotos();

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

        //PhotoService.CreateOrUpdatePhotoAsync(_fileUpload.Files.First());
    }

    private void SetDragClass()
        => _dragClass = $"{DefaultDragClass} mud-border-primary";

    private void ClearDragClass()
        => _dragClass = DefaultDragClass;
}
