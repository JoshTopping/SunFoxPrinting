// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

namespace Orleans.ShoppingCart.Silo.Extensions;

internal static class PhotoDetailsExtensions
{
    internal static Faker<PhotoDetails> GetBogusFaker(this PhotoDetails _) =>
        new Faker<PhotoDetails>()
            .StrictMode(true)
            .RuleFor(p => p.Id, (f, p) => f.Random.Number(1, 1_000_000).ToString())
            .RuleFor(p => p.Description, (f, p) => f.Lorem.Sentence())
            .RuleFor(p => p.Quantity, (f, p) => f.Random.Number(0, 1_200))
            .RuleFor(p => p.ImageUrl, (f, p) => f.Image.PicsumUrl())
            .RuleFor(p => p.Category, (f, p) => f.PickRandom<PhotoCategory>());
    internal static bool MatchesFilter(this PhotoDetails photo, string? filter)
    {
        if (filter is null or { Length: 0 })
        {
            return true;
        }

        if (photo is not null)
        {
            return photo.Name.Contains(filter, StringComparison.OrdinalIgnoreCase)
                || photo.Description.Contains(filter, StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }
}
