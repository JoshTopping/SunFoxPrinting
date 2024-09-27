// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

namespace Orleans.ShoppingCart.Silo.Services;

public sealed class PhotoService : BaseClusterService
{
    public PhotoService(
        IHttpContextAccessor httpContextAccessor, IClusterClient client) :
        base(httpContextAccessor, client)
    {
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
}
