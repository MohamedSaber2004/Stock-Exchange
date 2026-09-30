namespace Stock_Exchange.Persistance.Seeding
{
    public static class DataSeeder
    {
        public static async Task SeedAllAsync(StockExchangeDbContext context)
        {
            await CountrySeeder.SeedCountriesAsync(context);
            await AboutUsSeeder.SeedAboutUsAsync(context);
            await AboutUsFeatureSeeder.SeedFeaturesAsync(context);
            await HelpCenterCategorySeeder.SeedCategoriesAsync(context);
            await HelpCenterSeeder.SeedHelpCenterAsync(context);
            await PrivacyPolicySeeder.SeedPrivacyPolicyAsync(context);
        }
    }
}
