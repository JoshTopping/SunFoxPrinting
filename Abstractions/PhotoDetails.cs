// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

namespace Orleans.ShoppingCart.Abstractions;

[GenerateSerializer, Immutable]
public sealed record class PhotoDetails
{
    [Id(0)] public string Id { get; set; } = Random.Shared.Next(1, 1_000_000).ToString();
    [Id(1)] public string Name { get; set; } = null!;
    [Id(2)] public string Description { get; set; } = null!;
    [Id(3)] public ProductCategory Category { get; set; }
    [Id(4)] public int Quantity { get; set; }
    [Id(7)] public string ImageUrl { get; set; } = null!;
}
