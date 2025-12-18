using ForeverTools.Apify;
using ForeverTools.Apify.Constants;
using ForeverTools.Apify.Models;
using Xunit;

namespace ForeverTools.Apify.Tests;

public class ApifyClientTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidToken_CreatesClient()
    {
        var client = new ApifyClient("test-api-token");

        Assert.NotNull(client);
        Assert.Equal("test-api-token", client.Options.Token);
    }

    [Fact]
    public void Constructor_WithOptions_UsesProvidedOptions()
    {
        var options = new ApifyOptions
        {
            Token = "test-token",
            TimeoutSeconds = 600,
            DefaultMemoryMb = 512,
            DefaultTimeoutSeconds = 300
        };

        var client = new ApifyClient(options);

        Assert.Equal("test-token", client.Options.Token);
        Assert.Equal(600, client.Options.TimeoutSeconds);
        Assert.Equal(512, client.Options.DefaultMemoryMb);
        Assert.Equal(300, client.Options.DefaultTimeoutSeconds);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidToken_ThrowsArgumentException(string? invalidToken)
    {
        var options = new ApifyOptions { Token = invalidToken };

        Assert.Throws<ArgumentException>(() => new ApifyClient(options));
    }

    [Fact]
    public void Constructor_WithNullToken_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new ApifyClient((string)null!));
    }

    [Fact]
    public void Constructor_WithNullOptions_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ApifyClient((ApifyOptions)null!));
    }

    [Fact]
    public void Constructor_WithOptionsAndHttpClient_BothRequired()
    {
        var options = new ApifyOptions { Token = "test" };
        var httpClient = new HttpClient();

        var client = new ApifyClient(options, httpClient);

        Assert.NotNull(client);
        Assert.Equal("test", client.Options.Token);
    }

    [Fact]
    public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
    {
        var options = new ApifyOptions { Token = "test" };

        Assert.Throws<ArgumentNullException>(() => new ApifyClient(options, null!));
    }

    #endregion

    #region ApifyOptions Tests

    [Fact]
    public void ApifyOptions_HasCorrectDefaults()
    {
        var options = new ApifyOptions();

        Assert.Null(options.Token);
        Assert.Equal("https://api.apify.com/v2", options.BaseUrl);
        Assert.Equal(300, options.TimeoutSeconds);
        Assert.Equal(256, options.DefaultMemoryMb);
        Assert.Equal(300, options.DefaultTimeoutSeconds);
        Assert.Equal(2000, options.DefaultPollIntervalMs);
    }

    [Fact]
    public void ApifyOptions_SectionName_IsCorrect()
    {
        Assert.Equal("Apify", ApifyOptions.SectionName);
    }

    [Fact]
    public void ApifyOptions_HasToken_ReturnsTrueWhenSet()
    {
        var options = new ApifyOptions { Token = "my-token" };
        Assert.True(options.HasToken);
    }

    [Fact]
    public void ApifyOptions_HasToken_ReturnsFalseWhenEmpty()
    {
        var options = new ApifyOptions();
        Assert.False(options.HasToken);
    }

    [Fact]
    public void ApifyOptions_HasToken_ReturnsFalseWhenWhitespace()
    {
        var options = new ApifyOptions { Token = "   " };
        Assert.False(options.HasToken);
    }

    #endregion

    #region Actor Model Tests

    [Fact]
    public void Actor_FullName_CombinesUsernameAndName()
    {
        var actor = new Actor
        {
            Username = "apify",
            Name = "web-scraper"
        };

        Assert.Equal("apify/web-scraper", actor.FullName);
    }

    [Fact]
    public void Actor_FullName_ReturnsNullWhenMissingParts()
    {
        var actor1 = new Actor { Username = "apify" };
        var actor2 = new Actor { Name = "web-scraper" };
        var actor3 = new Actor();

        Assert.Null(actor1.FullName);
        Assert.Null(actor2.FullName);
        Assert.Null(actor3.FullName);
    }

    [Fact]
    public void Actor_DefaultValues_AreCorrect()
    {
        var actor = new Actor();

        Assert.Null(actor.Id);
        Assert.Null(actor.Name);
        Assert.Null(actor.Username);
        Assert.Null(actor.Description);
        Assert.Null(actor.Title);
        Assert.False(actor.IsPublic);
        Assert.False(actor.IsDeprecated);
        Assert.Null(actor.Versions);
        Assert.Null(actor.DefaultRunOptions);
        Assert.Null(actor.Stats);
        Assert.Null(actor.CreatedAt);
        Assert.Null(actor.ModifiedAt);
    }

    #endregion

    #region ActorRun Model Tests

    [Fact]
    public void ActorRun_IsRunning_TrueForRunningStatus()
    {
        var run = new ActorRun { Status = RunStatuses.Running };
        Assert.True(run.IsRunning);

        run.Status = RunStatuses.Ready;
        Assert.True(run.IsRunning);
    }

    [Fact]
    public void ActorRun_IsRunning_FalseForFinishedStatuses()
    {
        var run = new ActorRun { Status = RunStatuses.Succeeded };
        Assert.False(run.IsRunning);

        run.Status = RunStatuses.Failed;
        Assert.False(run.IsRunning);

        run.Status = RunStatuses.Aborted;
        Assert.False(run.IsRunning);

        run.Status = RunStatuses.TimedOut;
        Assert.False(run.IsRunning);
    }

    [Fact]
    public void ActorRun_IsSucceeded_OnlyTrueForSucceeded()
    {
        var run = new ActorRun { Status = RunStatuses.Succeeded };
        Assert.True(run.IsSucceeded);

        run.Status = RunStatuses.Failed;
        Assert.False(run.IsSucceeded);
    }

    [Fact]
    public void ActorRun_IsFailed_OnlyTrueForFailed()
    {
        var run = new ActorRun { Status = RunStatuses.Failed };
        Assert.True(run.IsFailed);

        run.Status = RunStatuses.Succeeded;
        Assert.False(run.IsFailed);
    }

    [Fact]
    public void ActorRun_IsAborted_OnlyTrueForAborted()
    {
        var run = new ActorRun { Status = RunStatuses.Aborted };
        Assert.True(run.IsAborted);

        run.Status = RunStatuses.Succeeded;
        Assert.False(run.IsAborted);
    }

    [Fact]
    public void ActorRun_IsTimedOut_OnlyTrueForTimedOut()
    {
        var run = new ActorRun { Status = RunStatuses.TimedOut };
        Assert.True(run.IsTimedOut);

        run.Status = RunStatuses.Succeeded;
        Assert.False(run.IsTimedOut);
    }

    [Fact]
    public void ActorRun_IsFinished_TrueForAllTerminalStatuses()
    {
        var run = new ActorRun { Status = RunStatuses.Succeeded };
        Assert.True(run.IsFinished);

        run.Status = RunStatuses.Failed;
        Assert.True(run.IsFinished);

        run.Status = RunStatuses.Aborted;
        Assert.True(run.IsFinished);

        run.Status = RunStatuses.TimedOut;
        Assert.True(run.IsFinished);
    }

    [Fact]
    public void ActorRun_IsFinished_FalseForRunningStatuses()
    {
        var run = new ActorRun { Status = RunStatuses.Running };
        Assert.False(run.IsFinished);

        run.Status = RunStatuses.Ready;
        Assert.False(run.IsFinished);
    }

    #endregion

    #region RunStatuses Constants Tests

    [Fact]
    public void RunStatuses_HasCorrectValues()
    {
        Assert.Equal("READY", RunStatuses.Ready);
        Assert.Equal("RUNNING", RunStatuses.Running);
        Assert.Equal("SUCCEEDED", RunStatuses.Succeeded);
        Assert.Equal("FAILED", RunStatuses.Failed);
        Assert.Equal("ABORTED", RunStatuses.Aborted);
        Assert.Equal("TIMED-OUT", RunStatuses.TimedOut);
    }

    #endregion

    #region ActorRunOptions Tests

    [Fact]
    public void ActorRunOptions_DefaultValues_AreNull()
    {
        var options = new ActorRunOptions();

        Assert.Null(options.MemoryMb);
        Assert.Null(options.TimeoutSeconds);
        Assert.Null(options.Build);
        Assert.Null(options.WaitForFinish);
        Assert.Null(options.Webhooks);
    }

    [Fact]
    public void ActorRunOptions_CanSetAllProperties()
    {
        var options = new ActorRunOptions
        {
            MemoryMb = 1024,
            TimeoutSeconds = 600,
            Build = "latest",
            WaitForFinish = 60,
            Webhooks = new List<Webhook>
            {
                new Webhook { RequestUrl = "https://example.com/webhook" }
            }
        };

        Assert.Equal(1024, options.MemoryMb);
        Assert.Equal(600, options.TimeoutSeconds);
        Assert.Equal("latest", options.Build);
        Assert.Equal(60, options.WaitForFinish);
        Assert.Single(options.Webhooks);
    }

    #endregion

    #region Dataset Model Tests

    [Fact]
    public void Dataset_DefaultValues_AreCorrect()
    {
        var dataset = new Dataset();

        Assert.Null(dataset.Id);
        Assert.Null(dataset.Name);
        Assert.Null(dataset.UserId);
        Assert.Equal(0, dataset.ItemCount);
        Assert.Null(dataset.CreatedAt);
        Assert.Null(dataset.ModifiedAt);
        Assert.Null(dataset.AccessedAt);
        Assert.Equal(0, dataset.CleanItemCount);
        Assert.Null(dataset.ActorId);
        Assert.Null(dataset.ActorRunId);
    }

    [Fact]
    public void DatasetItemsOptions_DefaultValues_AreNull()
    {
        var options = new DatasetItemsOptions();

        Assert.Null(options.Offset);
        Assert.Null(options.Limit);
        Assert.Null(options.Clean);
        Assert.Null(options.Format);
        Assert.Null(options.Fields);
        Assert.Null(options.OmitFields);
        Assert.Null(options.Unwind);
        Assert.Null(options.Flatten);
    }

    [Fact]
    public void DatasetItemsOptions_CanSetAllProperties()
    {
        var options = new DatasetItemsOptions
        {
            Offset = 10,
            Limit = 100,
            Clean = true,
            Format = "json",
            Fields = new List<string> { "title", "price" },
            OmitFields = new List<string> { "internalId" },
            Unwind = "items",
            Flatten = true
        };

        Assert.Equal(10, options.Offset);
        Assert.Equal(100, options.Limit);
        Assert.True(options.Clean);
        Assert.Equal("json", options.Format);
        Assert.Equal(2, options.Fields!.Count);
        Assert.Single(options.OmitFields!);
        Assert.Equal("items", options.Unwind);
        Assert.True(options.Flatten);
    }

    #endregion

    #region KeyValueStore Model Tests

    [Fact]
    public void KeyValueStore_DefaultValues_AreCorrect()
    {
        var store = new KeyValueStore();

        Assert.Null(store.Id);
        Assert.Null(store.Name);
        Assert.Null(store.UserId);
        Assert.Null(store.CreatedAt);
        Assert.Null(store.ModifiedAt);
        Assert.Null(store.AccessedAt);
        Assert.Null(store.ActorId);
        Assert.Null(store.ActorRunId);
    }

    [Fact]
    public void KeyValueRecord_DefaultValues_AreCorrect()
    {
        var record = new KeyValueRecord();

        Assert.Equal(string.Empty, record.Key);
        Assert.Null(record.Value);
        Assert.Null(record.ContentType);
    }

    [Fact]
    public void KeyValueRecordGeneric_DefaultValues_AreCorrect()
    {
        var record = new KeyValueRecord<string>();

        Assert.Equal(string.Empty, record.Key);
        Assert.Null(record.Value);
        Assert.Null(record.ContentType);
    }

    #endregion

    #region Schedule Model Tests

    [Fact]
    public void Schedule_DefaultValues_AreCorrect()
    {
        var schedule = new Schedule();

        Assert.Null(schedule.Id);
        Assert.Null(schedule.UserId);
        Assert.Null(schedule.Name);
        Assert.Null(schedule.CronExpression);
        Assert.Null(schedule.Timezone);
        Assert.False(schedule.IsEnabled);
        Assert.False(schedule.IsExclusive);
        Assert.Null(schedule.Description);
        Assert.Null(schedule.CreatedAt);
        Assert.Null(schedule.ModifiedAt);
        Assert.Null(schedule.LastRunAt);
        Assert.Null(schedule.NextRunAt);
        Assert.Null(schedule.Actions);
    }

    [Fact]
    public void ScheduleRequest_DefaultValues_AreCorrect()
    {
        var request = new ScheduleRequest();

        Assert.Null(request.Name);
        Assert.Null(request.CronExpression);
        Assert.Null(request.Timezone);
        Assert.True(request.IsEnabled);
        Assert.False(request.IsExclusive);
        Assert.Null(request.Description);
        Assert.Null(request.Actions);
    }

    [Fact]
    public void ScheduleActionTypes_HasCorrectValues()
    {
        Assert.Equal("RUN_ACTOR", ScheduleActionTypes.RunActor);
        Assert.Equal("RUN_ACTOR_TASK", ScheduleActionTypes.RunActorTask);
    }

    [Fact]
    public void ScheduleAction_DefaultValues_AreCorrect()
    {
        var action = new ScheduleAction();

        Assert.Null(action.Type);
        Assert.Null(action.ActorId);
        Assert.Null(action.ActorTaskId);
        Assert.Null(action.RunInput);
        Assert.Null(action.RunOptions);
    }

    #endregion

    #region User Model Tests

    [Fact]
    public void User_DefaultValues_AreCorrect()
    {
        var user = new User();

        Assert.Null(user.Id);
        Assert.Null(user.Username);
        Assert.Null(user.Email);
        Assert.Null(user.Profile);
        Assert.Null(user.Plan);
        Assert.Null(user.Limits);
    }

    [Fact]
    public void UserProfile_DefaultValues_AreCorrect()
    {
        var profile = new UserProfile();

        Assert.Null(profile.Name);
        Assert.Null(profile.PictureUrl);
        Assert.Null(profile.Bio);
    }

    [Fact]
    public void UserPlan_DefaultValues_AreCorrect()
    {
        var plan = new UserPlan();

        Assert.Null(plan.Id);
        Assert.Null(plan.Name);
        Assert.Null(plan.MonthlyBasePriceUsd);
        Assert.Null(plan.MonthlyUsageCreditsUsd);
    }

    [Fact]
    public void UserLimits_DefaultValues_AreCorrect()
    {
        var limits = new UserLimits();

        Assert.Null(limits.MaxMonthlyUsageUsd);
        Assert.Equal(0, limits.MaxConcurrentActorJobs);
        Assert.Equal(0, limits.MaxActorCount);
        Assert.Equal(0, limits.MaxActorTaskCount);
        Assert.Equal(0, limits.MaxScheduleCount);
        Assert.Equal(0, limits.MaxActorMemoryMb);
    }

    #endregion

    #region PopularActors Constants Tests

    [Fact]
    public void PopularActors_WebScraping_HasCorrectValues()
    {
        Assert.Equal("apify/web-scraper", PopularActors.WebScraper);
        Assert.Equal("apify/cheerio-scraper", PopularActors.CheerioScraper);
        Assert.Equal("apify/puppeteer-scraper", PopularActors.PuppeteerScraper);
        Assert.Equal("apify/playwright-scraper", PopularActors.PlaywrightScraper);
        Assert.Equal("apify/website-content-crawler", PopularActors.WebsiteContentCrawler);
    }

    [Fact]
    public void PopularActors_ECommerce_HasCorrectValues()
    {
        Assert.Equal("junglee/amazon-crawler", PopularActors.AmazonScraper);
        Assert.Equal("junglee/amazon-reviews-scraper", PopularActors.AmazonReviewsScraper);
        Assert.Equal("dtrungtin/ebay-items-scraper", PopularActors.EbayScraper);
        Assert.Equal("epctex/aliexpress-scraper", PopularActors.AliExpressScraper);
        Assert.Equal("epctex/walmart-scraper", PopularActors.WalmartScraper);
        Assert.Equal("epctex/shopify-scraper", PopularActors.ShopifyScraper);
        Assert.Equal("epctex/etsy-scraper", PopularActors.EtsyScraper);
    }

    [Fact]
    public void PopularActors_SocialMedia_HasCorrectValues()
    {
        Assert.Equal("apify/instagram-scraper", PopularActors.InstagramScraper);
        Assert.Equal("apify/instagram-profile-scraper", PopularActors.InstagramProfileScraper);
        Assert.Equal("apify/instagram-hashtag-scraper", PopularActors.InstagramHashtagScraper);
        Assert.Equal("quacker/twitter-scraper", PopularActors.TwitterScraper);
        Assert.Equal("clockworks/tiktok-scraper", PopularActors.TikTokScraper);
        Assert.Equal("bernardo/youtube-scraper", PopularActors.YouTubeScraper);
        Assert.Equal("anchor/linkedin-profile-scraper", PopularActors.LinkedInScraper);
        Assert.Equal("apify/facebook-pages-scraper", PopularActors.FacebookPagesScraper);
        Assert.Equal("trudax/reddit-scraper", PopularActors.RedditScraper);
    }

    [Fact]
    public void PopularActors_Search_HasCorrectValues()
    {
        Assert.Equal("apify/google-search-scraper", PopularActors.GoogleSearchScraper);
        Assert.Equal("compass/google-maps-scraper", PopularActors.GoogleMapsScraper);
        Assert.Equal("emastra/google-trends-scraper", PopularActors.GoogleTrendsScraper);
        Assert.Equal("epctex/bing-scraper", PopularActors.BingSearchScraper);
    }

    [Fact]
    public void PopularActors_Travel_HasCorrectValues()
    {
        Assert.Equal("dtrungtin/booking-scraper", PopularActors.BookingScraper);
        Assert.Equal("dtrungtin/airbnb-scraper", PopularActors.AirbnbScraper);
        Assert.Equal("maxcopell/tripadvisor", PopularActors.TripAdvisorScraper);
        Assert.Equal("maxcopell/zillow-scraper", PopularActors.ZillowScraper);
    }

    [Fact]
    public void PopularActors_Jobs_HasCorrectValues()
    {
        Assert.Equal("misceres/indeed-scraper", PopularActors.IndeedScraper);
        Assert.Equal("epctex/glassdoor-scraper", PopularActors.GlassdoorScraper);
        Assert.Equal("epctex/crunchbase-scraper", PopularActors.CrunchbaseScraper);
        Assert.Equal("epctex/yellow-pages-scraper", PopularActors.YellowPagesScraper);
    }

    [Fact]
    public void PopularActors_Reviews_HasCorrectValues()
    {
        Assert.Equal("epctex/trustpilot-scraper", PopularActors.TrustpilotScraper);
        Assert.Equal("apify/yelp-scraper", PopularActors.YelpScraper);
        Assert.Equal("epctex/app-store-scraper", PopularActors.AppStoreScraper);
        Assert.Equal("epctex/google-play-scraper", PopularActors.GooglePlayScraper);
    }

    [Fact]
    public void PopularActors_News_HasCorrectValues()
    {
        Assert.Equal("apify/news-scraper", PopularActors.NewsScraper);
        Assert.Equal("lukaskrivka/article-extractor-smart", PopularActors.ArticleExtractor);
        Assert.Equal("apify/contact-info-scraper", PopularActors.ContactInfoScraper);
    }

    #endregion

    #region ApifyException Tests

    [Fact]
    public void ApifyException_WithMessage_SetsMessage()
    {
        var ex = new ApifyException("Test error");

        Assert.Equal("Test error", ex.Message);
        Assert.Equal(0, ex.StatusCode);
    }

    [Fact]
    public void ApifyException_WithStatusCode_SetsStatusCode()
    {
        var ex = new ApifyException("Not found", 404);

        Assert.Equal("Not found", ex.Message);
        Assert.Equal(404, ex.StatusCode);
    }

    #endregion

    #region Webhook Model Tests

    [Fact]
    public void Webhook_DefaultValues_AreCorrect()
    {
        var webhook = new Webhook();

        Assert.Null(webhook.EventTypes);
        Assert.Null(webhook.RequestUrl);
        Assert.Null(webhook.PayloadTemplate);
    }

    [Fact]
    public void Webhook_CanSetAllProperties()
    {
        var webhook = new Webhook
        {
            EventTypes = new List<string> { "ACTOR.RUN.SUCCEEDED", "ACTOR.RUN.FAILED" },
            RequestUrl = "https://example.com/webhook",
            PayloadTemplate = "{\"runId\": \"{{runId}}\"}"
        };

        Assert.Equal(2, webhook.EventTypes.Count);
        Assert.Equal("https://example.com/webhook", webhook.RequestUrl);
        Assert.Contains("runId", webhook.PayloadTemplate);
    }

    #endregion

    #region ActorStats Model Tests

    [Fact]
    public void ActorStats_DefaultValues_AreCorrect()
    {
        var stats = new ActorStats();

        Assert.Equal(0, stats.TotalRuns);
        Assert.Equal(0, stats.TotalUsers);
        Assert.Equal(0, stats.TotalBuilds);
        Assert.Equal(0, stats.TotalRuns30Days);
    }

    #endregion

    #region ActorRunStats Model Tests

    [Fact]
    public void ActorRunStats_DefaultValues_AreCorrect()
    {
        var stats = new ActorRunStats();

        Assert.Null(stats.InputBodyLen);
        Assert.Equal(0, stats.RestartCount);
        Assert.Null(stats.DurationMillis);
        Assert.Null(stats.ComputeUnits);
    }

    #endregion

    #region ActorRunUsage Model Tests

    [Fact]
    public void ActorRunUsage_DefaultValues_AreCorrect()
    {
        var usage = new ActorRunUsage();

        Assert.Null(usage.ActorComputeUnits);
        Assert.Null(usage.DatasetReads);
        Assert.Null(usage.DatasetWrites);
        Assert.Null(usage.KeyValueStoreReads);
        Assert.Null(usage.KeyValueStoreWrites);
    }

    #endregion

    #region Validation Tests

    [Fact]
    public async Task StartActorAsync_WithEmptyActorId_ThrowsArgumentException()
    {
        var client = new ApifyClient("test-token");

        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.StartActorAsync(""));

        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.StartActorAsync("   "));
    }

    [Fact]
    public async Task GetRunAsync_WithEmptyRunId_ThrowsArgumentException()
    {
        var client = new ApifyClient("test-token");

        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.GetRunAsync(""));
    }

    [Fact]
    public async Task GetActorAsync_WithEmptyActorId_ThrowsArgumentException()
    {
        var client = new ApifyClient("test-token");

        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.GetActorAsync(""));
    }

    [Fact]
    public async Task AbortRunAsync_WithEmptyRunId_ThrowsArgumentException()
    {
        var client = new ApifyClient("test-token");

        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.AbortRunAsync(""));
    }

    [Fact]
    public async Task GetRunLogAsync_WithEmptyRunId_ThrowsArgumentException()
    {
        var client = new ApifyClient("test-token");

        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.GetRunLogAsync(""));
    }

    [Fact]
    public async Task GetDatasetItemsAsync_WithEmptyDatasetId_ThrowsArgumentException()
    {
        var client = new ApifyClient("test-token");

        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.GetDatasetItemsAsync<object>(""));
    }

    [Fact]
    public async Task PushDatasetItemsAsync_WithEmptyDatasetId_ThrowsArgumentException()
    {
        var client = new ApifyClient("test-token");

        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.PushDatasetItemsAsync("", new[] { new { foo = "bar" } }));
    }

    [Fact]
    public async Task GetKeyValueRecordAsync_WithEmptyStoreId_ThrowsArgumentException()
    {
        var client = new ApifyClient("test-token");

        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.GetKeyValueRecordAsync<object>("", "key"));
    }

    [Fact]
    public async Task GetKeyValueRecordAsync_WithEmptyKey_ThrowsArgumentException()
    {
        var client = new ApifyClient("test-token");

        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.GetKeyValueRecordAsync<object>("store-id", ""));
    }

    [Fact]
    public async Task SetKeyValueRecordAsync_WithEmptyStoreId_ThrowsArgumentException()
    {
        var client = new ApifyClient("test-token");

        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.SetKeyValueRecordAsync("", "key", new { }));
    }

    [Fact]
    public async Task SetKeyValueRecordAsync_WithEmptyKey_ThrowsArgumentException()
    {
        var client = new ApifyClient("test-token");

        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.SetKeyValueRecordAsync("store-id", "", new { }));
    }

    [Fact]
    public async Task DeleteKeyValueRecordAsync_WithEmptyStoreId_ThrowsArgumentException()
    {
        var client = new ApifyClient("test-token");

        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.DeleteKeyValueRecordAsync("", "key"));
    }

    [Fact]
    public async Task DeleteKeyValueRecordAsync_WithEmptyKey_ThrowsArgumentException()
    {
        var client = new ApifyClient("test-token");

        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.DeleteKeyValueRecordAsync("store-id", ""));
    }

    [Fact]
    public async Task GetScheduleAsync_WithEmptyScheduleId_ThrowsArgumentException()
    {
        var client = new ApifyClient("test-token");

        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.GetScheduleAsync(""));
    }

    [Fact]
    public async Task UpdateScheduleAsync_WithEmptyScheduleId_ThrowsArgumentException()
    {
        var client = new ApifyClient("test-token");

        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.UpdateScheduleAsync("", new ScheduleRequest()));
    }

    [Fact]
    public async Task UpdateScheduleAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        var client = new ApifyClient("test-token");

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            client.UpdateScheduleAsync("schedule-id", null!));
    }

    [Fact]
    public async Task DeleteScheduleAsync_WithEmptyScheduleId_ThrowsArgumentException()
    {
        var client = new ApifyClient("test-token");

        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.DeleteScheduleAsync(""));
    }

    [Fact]
    public async Task CreateScheduleAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        var client = new ApifyClient("test-token");

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            client.CreateScheduleAsync(null!));
    }

    #endregion

    #region Dispose Tests

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        var client = new ApifyClient("test-token");
        client.Dispose();
    }

    [Fact]
    public void Dispose_MultipleTimes_DoesNotThrow()
    {
        var client = new ApifyClient("test-token");
        client.Dispose();
        client.Dispose();
    }

    #endregion
}
