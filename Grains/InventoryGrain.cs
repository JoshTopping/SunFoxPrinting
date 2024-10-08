﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

namespace Orleans.ShoppingCart.Grains;

[Reentrant]
public sealed class InventoryGrain(
    [PersistentState(
            stateName: "Inventory",
            storageName: "shopping-cart")]
        IPersistentState<HashSet<string>> state) : Grain, IInventoryGrain
{
    private readonly Dictionary<string, ProductDetails> _productCache = [];
    private readonly Dictionary<string, PhotoDetails> _photoCache = [];

    public override Task OnActivateAsync(CancellationToken _) => PopulateProductCacheAsync();

    Task<HashSet<ProductDetails>> IInventoryGrain.GetAllProductsAsync() =>
        Task.FromResult(_productCache.Values.ToHashSet());

    Task<HashSet<PhotoDetails>> IInventoryGrain.GetAllPhotosAsync() =>
       Task.FromResult(_photoCache.Values.ToHashSet());

    async Task IInventoryGrain.AddOrUpdateProductAsync(ProductDetails product)
    {
        state.State.Add(product.Id);
        _productCache[product.Id] = product;

        await state.WriteStateAsync();
    }

    async Task IInventoryGrain.AddOrUpdatePhotoAsync(PhotoDetails photo)
    {
        state.State.Add(photo.Id);
        _photoCache[photo.Id] = photo;

        await state.WriteStateAsync();
    }

    public async Task RemoveProductAsync(string productId)
    {
        state.State.Remove(productId);
        _productCache.Remove(productId);

        await state.WriteStateAsync();
    }

    public async Task RemovePhotoAsync(string photoId)
    {
        state.State.Remove(photoId);
        _productCache.Remove(photoId);

        await state.WriteStateAsync();
    }

    private async Task PopulateProductCacheAsync()
    {
        if (state is not { State.Count: > 0 })
        {
            return;
        }

        await Parallel.ForEachAsync(
            state.State,
            async (id, _) =>
            {
                var productGrain = GrainFactory.GetGrain<IProductGrain>(id);
                _productCache[id] = await productGrain.GetProductDetailsAsync();
            });
    }
}
