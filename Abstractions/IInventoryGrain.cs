// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

namespace Orleans.ShoppingCart.Abstractions;

public interface IInventoryGrain : IGrainWithStringKey
{
    Task<HashSet<ProductDetails>> GetAllProductsAsync();
    Task<HashSet<PhotoDetails>> GetAllPhotosAsync();

    Task AddOrUpdateProductAsync(ProductDetails photoDetails);
    Task AddOrUpdatePhotoAsync(PhotoDetails photoDetails);

    Task RemoveProductAsync(string productId);
    Task RemovePhotoAsync(string photoId);
}
