// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using Azure.Storage.Files.Shares.Models;
using Azure.Storage.Files.Shares;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Npgsql;

namespace Orleans.ShoppingCart.Silo.Services;

public sealed class PhotoService : BaseClusterService
{
    //SunfoxPrintingContext Context { get; set; } 
    IConfiguration Configuration { get; set; }

    public PhotoService(
        IHttpContextAccessor httpContextAccessor, IClusterClient client, IConfiguration configuration) :
        base(httpContextAccessor, client)
    {
        Configuration = configuration;
    }

    public Task CreateOrUpdatePhotoAsync(PhotoDetails photo) =>
        _client.GetGrain<IPhotoGrain>(photo.Id).CreateOrUpdatePhotoAsync(photo);

    public Task<(bool IsAvailable, PhotoDetails? PhotoDetails)> TryTakePhotoAsync(
        string photoId, int quantity) =>
        TryUseGrain<IPhotoGrain, Task<(bool IsAvailable, PhotoDetails? PhotoDetails)>>(
            photos => photos.TryTakePhotoAsync(quantity),
            photoId,
            () => Task.FromResult<(bool IsAvailable, PhotoDetails? PhotoDetails)>(
                (false, null)));

    public Task ReturnPhotoAsync(string photoId, int quantity) =>
        TryUseGrain<IPhotoGrain, Task>(
            photos => photos.ReturnPhotoAsync(quantity),
            photoId,
            () => Task.CompletedTask);

    public Task<int> GetPhotoAvailability(string photoId) =>
        TryUseGrain<IPhotoGrain, Task<int>>(
            photos => photos.GetPhotoAvailabilityAsync(),
            photoId,
            () => Task.FromResult(0));

    public HashSet<PhotoDetails> GetAllPhotos()
    {
        HashSet<PhotoDetails> photos = new();
        using var context = new SunfoxPrintingContext(Configuration);

        if (context != null)
        {
            List<Photo> dbPhotos = context.Photos.ToList();
            foreach (var dbPhoto in dbPhotos)
            {
                PhotoDetails photoDetails = new PhotoDetails();
                photoDetails.Id = dbPhoto.PhotoId.ToString();
                photoDetails.Name = dbPhoto.FileName;
                photoDetails.Category = PhotoCategory.Other;
                photoDetails.ImageUrl = dbPhoto.Url != null ? dbPhoto.Url : string.Empty;
                photos.Add(photoDetails);

           
            }
        }
        
        return photos;
    }

}

