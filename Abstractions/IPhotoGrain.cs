// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

namespace Orleans.ShoppingCart.Abstractions;

public interface IPhotoGrain : IGrainWithStringKey
{
    Task<(bool IsAvailable, PhotoDetails? PhotoDetails)> TryTakePhotoAsync(int quantity);

    Task ReturnPhotoAsync(int quantity);

    Task<int> GetPhotoAvailabilityAsync();

    Task CreateOrUpdatePhotoAsync(PhotoDetails productDetails);

    Task<PhotoDetails> GetPhotoDetailsAsync();
}
