namespace ForeverTools.Apify.Constants;

/// <summary>
/// Popular Apify actor identifiers.
/// Find more at: https://www.apify.com/store?fpr=8hklqy
/// </summary>
public static class PopularActors
{
    #region Web Scraping - General

    /// <summary>
    /// Web Scraper - Crawl and scrape any website with a simple configuration.
    /// </summary>
    public const string WebScraper = "apify/web-scraper";

    /// <summary>
    /// Cheerio Scraper - Fast scraper for static websites using Cheerio (jQuery for Node.js).
    /// </summary>
    public const string CheerioScraper = "apify/cheerio-scraper";

    /// <summary>
    /// Puppeteer Scraper - Scrape websites with JavaScript rendering using Puppeteer.
    /// </summary>
    public const string PuppeteerScraper = "apify/puppeteer-scraper";

    /// <summary>
    /// Playwright Scraper - Modern scraper with Playwright for complex JavaScript websites.
    /// </summary>
    public const string PlaywrightScraper = "apify/playwright-scraper";

    /// <summary>
    /// Website Content Crawler - Extract text content from any website.
    /// </summary>
    public const string WebsiteContentCrawler = "apify/website-content-crawler";

    #endregion

    #region E-Commerce

    /// <summary>
    /// Amazon Product Scraper - Scrape Amazon product data (prices, reviews, etc.).
    /// </summary>
    public const string AmazonScraper = "junglee/amazon-crawler";

    /// <summary>
    /// Amazon Reviews Scraper - Extract reviews from Amazon products.
    /// </summary>
    public const string AmazonReviewsScraper = "junglee/amazon-reviews-scraper";

    /// <summary>
    /// eBay Scraper - Scrape eBay listings and product data.
    /// </summary>
    public const string EbayScraper = "dtrungtin/ebay-items-scraper";

    /// <summary>
    /// AliExpress Scraper - Scrape product data from AliExpress.
    /// </summary>
    public const string AliExpressScraper = "epctex/aliexpress-scraper";

    /// <summary>
    /// Walmart Scraper - Scrape Walmart product listings.
    /// </summary>
    public const string WalmartScraper = "epctex/walmart-scraper";

    /// <summary>
    /// Shopify Scraper - Scrape products from Shopify stores.
    /// </summary>
    public const string ShopifyScraper = "epctex/shopify-scraper";

    /// <summary>
    /// Etsy Scraper - Scrape Etsy listings and shops.
    /// </summary>
    public const string EtsyScraper = "epctex/etsy-scraper";

    #endregion

    #region Social Media

    /// <summary>
    /// Instagram Scraper - Scrape Instagram profiles, posts, and hashtags.
    /// </summary>
    public const string InstagramScraper = "apify/instagram-scraper";

    /// <summary>
    /// Instagram Profile Scraper - Scrape Instagram user profiles.
    /// </summary>
    public const string InstagramProfileScraper = "apify/instagram-profile-scraper";

    /// <summary>
    /// Instagram Hashtag Scraper - Scrape posts by hashtag.
    /// </summary>
    public const string InstagramHashtagScraper = "apify/instagram-hashtag-scraper";

    /// <summary>
    /// Twitter Scraper - Scrape tweets and Twitter profiles.
    /// </summary>
    public const string TwitterScraper = "quacker/twitter-scraper";

    /// <summary>
    /// TikTok Scraper - Scrape TikTok videos and profiles.
    /// </summary>
    public const string TikTokScraper = "clockworks/tiktok-scraper";

    /// <summary>
    /// YouTube Scraper - Scrape YouTube videos and channels.
    /// </summary>
    public const string YouTubeScraper = "bernardo/youtube-scraper";

    /// <summary>
    /// LinkedIn Profile Scraper - Scrape LinkedIn profiles.
    /// </summary>
    public const string LinkedInScraper = "anchor/linkedin-profile-scraper";

    /// <summary>
    /// Facebook Pages Scraper - Scrape Facebook page posts.
    /// </summary>
    public const string FacebookPagesScraper = "apify/facebook-pages-scraper";

    /// <summary>
    /// Reddit Scraper - Scrape Reddit posts and comments.
    /// </summary>
    public const string RedditScraper = "trudax/reddit-scraper";

    #endregion

    #region Search Engines

    /// <summary>
    /// Google Search Scraper - Scrape Google search results.
    /// </summary>
    public const string GoogleSearchScraper = "apify/google-search-scraper";

    /// <summary>
    /// Google Maps Scraper - Scrape Google Maps places and reviews.
    /// </summary>
    public const string GoogleMapsScraper = "compass/google-maps-scraper";

    /// <summary>
    /// Google Trends Scraper - Scrape Google Trends data.
    /// </summary>
    public const string GoogleTrendsScraper = "emastra/google-trends-scraper";

    /// <summary>
    /// Bing Search Scraper - Scrape Bing search results.
    /// </summary>
    public const string BingSearchScraper = "epctex/bing-scraper";

    #endregion

    #region Travel & Real Estate

    /// <summary>
    /// Booking.com Scraper - Scrape hotel data from Booking.com.
    /// </summary>
    public const string BookingScraper = "dtrungtin/booking-scraper";

    /// <summary>
    /// Airbnb Scraper - Scrape Airbnb listings.
    /// </summary>
    public const string AirbnbScraper = "dtrungtin/airbnb-scraper";

    /// <summary>
    /// TripAdvisor Scraper - Scrape TripAdvisor reviews and places.
    /// </summary>
    public const string TripAdvisorScraper = "maxcopell/tripadvisor";

    /// <summary>
    /// Zillow Scraper - Scrape Zillow real estate listings.
    /// </summary>
    public const string ZillowScraper = "maxcopell/zillow-scraper";

    #endregion

    #region Jobs & Business

    /// <summary>
    /// Indeed Scraper - Scrape job listings from Indeed.
    /// </summary>
    public const string IndeedScraper = "misceres/indeed-scraper";

    /// <summary>
    /// Glassdoor Scraper - Scrape company reviews and salaries.
    /// </summary>
    public const string GlassdoorScraper = "epctex/glassdoor-scraper";

    /// <summary>
    /// Crunchbase Scraper - Scrape company data from Crunchbase.
    /// </summary>
    public const string CrunchbaseScraper = "epctex/crunchbase-scraper";

    /// <summary>
    /// Yellow Pages Scraper - Scrape business listings.
    /// </summary>
    public const string YellowPagesScraper = "epctex/yellow-pages-scraper";

    #endregion

    #region Reviews & Ratings

    /// <summary>
    /// Trustpilot Scraper - Scrape Trustpilot reviews.
    /// </summary>
    public const string TrustpilotScraper = "epctex/trustpilot-scraper";

    /// <summary>
    /// Yelp Scraper - Scrape Yelp business reviews.
    /// </summary>
    public const string YelpScraper = "apify/yelp-scraper";

    /// <summary>
    /// App Store Scraper - Scrape App Store apps and reviews.
    /// </summary>
    public const string AppStoreScraper = "epctex/app-store-scraper";

    /// <summary>
    /// Google Play Scraper - Scrape Google Play apps and reviews.
    /// </summary>
    public const string GooglePlayScraper = "epctex/google-play-scraper";

    #endregion

    #region News & Data

    /// <summary>
    /// News Scraper - Scrape news articles from various sources.
    /// </summary>
    public const string NewsScraper = "apify/news-scraper";

    /// <summary>
    /// Article Text Extractor - Extract clean text from articles.
    /// </summary>
    public const string ArticleExtractor = "lukaskrivka/article-extractor-smart";

    /// <summary>
    /// Contact Info Scraper - Extract emails and phone numbers.
    /// </summary>
    public const string ContactInfoScraper = "apify/contact-info-scraper";

    #endregion
}
