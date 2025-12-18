using ForeverTools.APILayer;
using ForeverTools.APILayer.Models;
using Xunit;

namespace ForeverTools.APILayer.Tests;

public class ApiLayerClientTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidApiKey_CreatesClient()
    {
        var client = new ApiLayerClient("test-api-key");

        Assert.NotNull(client);
        Assert.Equal("test-api-key", client.Options.ApiKey);
    }

    [Fact]
    public void Constructor_WithOptions_UsesProvidedOptions()
    {
        var options = new ApiLayerOptions
        {
            ApiKey = "test-key",
            TimeoutSeconds = 60,
            BaseUrl = "https://custom.api.com"
        };

        var client = new ApiLayerClient(options);

        Assert.Equal("test-key", client.Options.ApiKey);
        Assert.Equal(60, client.Options.TimeoutSeconds);
        Assert.Equal("https://custom.api.com", client.Options.BaseUrl);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidApiKey_ThrowsArgumentException(string? invalidKey)
    {
        var options = new ApiLayerOptions { ApiKey = invalidKey };

        Assert.Throws<ArgumentException>(() => new ApiLayerClient(options));
    }

    [Fact]
    public void Constructor_WithNullOptions_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ApiLayerClient((ApiLayerOptions)null!));
    }

    #endregion

    #region ApiLayerOptions Tests

    [Fact]
    public void ApiLayerOptions_HasCorrectDefaults()
    {
        var options = new ApiLayerOptions();

        Assert.Null(options.ApiKey);
        Assert.Equal("https://api.apilayer.com", options.BaseUrl);
        Assert.Equal(30, options.TimeoutSeconds);
        Assert.False(options.HasApiKey);
    }

    [Fact]
    public void ApiLayerOptions_SectionName_IsCorrect()
    {
        Assert.Equal("APILayer", ApiLayerOptions.SectionName);
    }

    [Fact]
    public void ApiLayerOptions_HasApiKey_ReturnsTrueWhenSet()
    {
        var options = new ApiLayerOptions { ApiKey = "key" };
        Assert.True(options.HasApiKey);
    }

    [Fact]
    public void ApiLayerOptions_HasApiKey_ReturnsFalseWhenEmpty()
    {
        var options = new ApiLayerOptions();
        Assert.False(options.HasApiKey);
    }

    #endregion

    #region IpGeolocationResult Tests

    [Fact]
    public void IpGeolocationResult_DefaultValues_AreNull()
    {
        var result = new IpGeolocationResult();

        Assert.Null(result.Ip);
        Assert.Null(result.Type);
        Assert.Null(result.CountryCode);
        Assert.Null(result.CountryName);
        Assert.Null(result.City);
        Assert.Null(result.Latitude);
        Assert.Null(result.Longitude);
    }

    [Fact]
    public void IpGeolocationResult_CanSetAllProperties()
    {
        var result = new IpGeolocationResult
        {
            Ip = "8.8.8.8",
            Type = "ipv4",
            CountryCode = "US",
            CountryName = "United States",
            City = "Mountain View",
            Latitude = 37.386,
            Longitude = -122.0838
        };

        Assert.Equal("8.8.8.8", result.Ip);
        Assert.Equal("ipv4", result.Type);
        Assert.Equal("US", result.CountryCode);
        Assert.Equal("United States", result.CountryName);
        Assert.Equal("Mountain View", result.City);
        Assert.Equal(37.386, result.Latitude);
        Assert.Equal(-122.0838, result.Longitude);
    }

    [Fact]
    public void IpLocation_CanSetProperties()
    {
        var location = new IpLocation
        {
            Capital = "Washington D.C.",
            CallingCode = "1",
            IsEu = false
        };

        Assert.Equal("Washington D.C.", location.Capital);
        Assert.Equal("1", location.CallingCode);
        Assert.False(location.IsEu);
    }

    [Fact]
    public void IpSecurity_CanSetProperties()
    {
        var security = new IpSecurity
        {
            IsProxy = true,
            ProxyType = "vpn",
            IsTor = false,
            ThreatLevel = "low"
        };

        Assert.True(security.IsProxy);
        Assert.Equal("vpn", security.ProxyType);
        Assert.False(security.IsTor);
        Assert.Equal("low", security.ThreatLevel);
    }

    #endregion

    #region ExchangeRatesResult Tests

    [Fact]
    public void ExchangeRatesResult_DefaultValues_AreCorrect()
    {
        var result = new ExchangeRatesResult();

        Assert.False(result.Success);
        Assert.Equal(0, result.Timestamp);
        Assert.Null(result.Base);
        Assert.Null(result.Date);
        Assert.Null(result.Rates);
    }

    [Fact]
    public void ExchangeRatesResult_GetRate_ReturnsRateWhenExists()
    {
        var result = new ExchangeRatesResult
        {
            Rates = new Dictionary<string, decimal>
            {
                ["EUR"] = 0.85m,
                ["GBP"] = 0.73m
            }
        };

        Assert.Equal(0.85m, result.GetRate("EUR"));
        Assert.Equal(0.85m, result.GetRate("eur")); // Case insensitive
    }

    [Fact]
    public void ExchangeRatesResult_GetRate_ReturnsNullWhenNotExists()
    {
        var result = new ExchangeRatesResult
        {
            Rates = new Dictionary<string, decimal>
            {
                ["EUR"] = 0.85m
            }
        };

        Assert.Null(result.GetRate("JPY"));
    }

    [Fact]
    public void ExchangeRatesResult_GetRate_ReturnsNullWhenRatesIsNull()
    {
        var result = new ExchangeRatesResult();

        Assert.Null(result.GetRate("EUR"));
    }

    [Fact]
    public void ExchangeRatesResult_TimestampUtc_ConvertsCorrectly()
    {
        var result = new ExchangeRatesResult
        {
            Timestamp = 1704067200 // 2024-01-01 00:00:00 UTC
        };

        var expected = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        Assert.Equal(expected, result.TimestampUtc);
    }

    #endregion

    #region CurrencyConversionResult Tests

    [Fact]
    public void CurrencyConversionResult_CanSetAllProperties()
    {
        var result = new CurrencyConversionResult
        {
            Success = true,
            Query = new ConversionQuery
            {
                From = "USD",
                To = "EUR",
                Amount = 100
            },
            Info = new ConversionInfo
            {
                Timestamp = 1704067200,
                Rate = 0.85m
            },
            Historical = false,
            Date = "2024-01-01",
            Result = 85m
        };

        Assert.True(result.Success);
        Assert.Equal("USD", result.Query.From);
        Assert.Equal("EUR", result.Query.To);
        Assert.Equal(100, result.Query.Amount);
        Assert.Equal(0.85m, result.Info.Rate);
        Assert.Equal(85m, result.Result);
    }

    #endregion

    #region TimeSeriesResult Tests

    [Fact]
    public void TimeSeriesResult_GetRatesForDate_ReturnsRatesWhenExists()
    {
        var result = new TimeSeriesResult
        {
            Rates = new Dictionary<string, Dictionary<string, decimal>>
            {
                ["2024-01-01"] = new Dictionary<string, decimal> { ["EUR"] = 0.85m },
                ["2024-01-02"] = new Dictionary<string, decimal> { ["EUR"] = 0.86m }
            }
        };

        var rates = result.GetRatesForDate("2024-01-01");
        Assert.NotNull(rates);
        Assert.Equal(0.85m, rates["EUR"]);
    }

    [Fact]
    public void TimeSeriesResult_GetRatesForDate_ReturnsNullWhenNotExists()
    {
        var result = new TimeSeriesResult
        {
            Rates = new Dictionary<string, Dictionary<string, decimal>>
            {
                ["2024-01-01"] = new Dictionary<string, decimal> { ["EUR"] = 0.85m }
            }
        };

        Assert.Null(result.GetRatesForDate("2024-01-03"));
    }

    #endregion

    #region FluctuationData Tests

    [Fact]
    public void FluctuationData_CanSetAllProperties()
    {
        var data = new FluctuationData
        {
            StartRate = 0.85m,
            EndRate = 0.87m,
            Change = 0.02m,
            ChangePercentage = 2.35m
        };

        Assert.Equal(0.85m, data.StartRate);
        Assert.Equal(0.87m, data.EndRate);
        Assert.Equal(0.02m, data.Change);
        Assert.Equal(2.35m, data.ChangePercentage);
    }

    #endregion

    #region PhoneValidationResult Tests

    [Fact]
    public void PhoneValidationResult_DefaultValues_AreCorrect()
    {
        var result = new PhoneValidationResult();

        Assert.False(result.Valid);
        Assert.Null(result.Number);
        Assert.Null(result.LocalFormat);
        Assert.Null(result.InternationalFormat);
        Assert.Null(result.Carrier);
        Assert.Null(result.LineType);
    }

    [Fact]
    public void PhoneValidationResult_CanSetAllProperties()
    {
        var result = new PhoneValidationResult
        {
            Valid = true,
            Number = "14155552671",
            LocalFormat = "4155552671",
            InternationalFormat = "+14155552671",
            CountryPrefix = "1",
            CountryCode = "US",
            CountryName = "United States",
            Location = "California",
            Carrier = "AT&T Mobility LLC",
            LineType = "mobile"
        };

        Assert.True(result.Valid);
        Assert.Equal("14155552671", result.Number);
        Assert.Equal("+14155552671", result.InternationalFormat);
        Assert.Equal("US", result.CountryCode);
        Assert.Equal("AT&T Mobility LLC", result.Carrier);
        Assert.Equal("mobile", result.LineType);
    }

    #endregion

    #region PhoneLineTypes Tests

    [Fact]
    public void PhoneLineTypes_HasCorrectValues()
    {
        Assert.Equal("mobile", PhoneLineTypes.Mobile);
        Assert.Equal("landline", PhoneLineTypes.Landline);
        Assert.Equal("voip", PhoneLineTypes.Voip);
        Assert.Equal("toll_free", PhoneLineTypes.TollFree);
        Assert.Equal("premium_rate", PhoneLineTypes.PremiumRate);
        Assert.Equal("unknown", PhoneLineTypes.Unknown);
    }

    #endregion

    #region EmailValidationResult Tests

    [Fact]
    public void EmailValidationResult_DefaultValues_AreCorrect()
    {
        var result = new EmailValidationResult();

        Assert.Null(result.Email);
        Assert.False(result.FormatValid);
        Assert.False(result.MxFound);
        Assert.False(result.SmtpCheck);
        Assert.False(result.Disposable);
        Assert.False(result.Free);
        Assert.Equal(0, result.Score);
    }

    [Fact]
    public void EmailValidationResult_CanSetAllProperties()
    {
        var result = new EmailValidationResult
        {
            Email = "test@example.com",
            User = "test",
            Domain = "example.com",
            FormatValid = true,
            MxFound = true,
            SmtpCheck = true,
            CatchAll = false,
            Role = false,
            Disposable = false,
            Free = false,
            Score = 0.92
        };

        Assert.Equal("test@example.com", result.Email);
        Assert.Equal("test", result.User);
        Assert.Equal("example.com", result.Domain);
        Assert.True(result.FormatValid);
        Assert.True(result.MxFound);
        Assert.True(result.SmtpCheck);
        Assert.False(result.Disposable);
        Assert.Equal(0.92, result.Score);
    }

    [Fact]
    public void EmailValidationResult_IsDeliverable_ReturnsTrueWhenValid()
    {
        var result = new EmailValidationResult
        {
            FormatValid = true,
            MxFound = true,
            SmtpCheck = true,
            Disposable = false
        };

        Assert.True(result.IsDeliverable);
    }

    [Fact]
    public void EmailValidationResult_IsDeliverable_ReturnsFalseWhenDisposable()
    {
        var result = new EmailValidationResult
        {
            FormatValid = true,
            MxFound = true,
            SmtpCheck = true,
            Disposable = true
        };

        Assert.False(result.IsDeliverable);
    }

    [Fact]
    public void EmailValidationResult_IsDeliverable_ReturnsFalseWhenNoMx()
    {
        var result = new EmailValidationResult
        {
            FormatValid = true,
            MxFound = false,
            SmtpCheck = false,
            Disposable = false
        };

        Assert.False(result.IsDeliverable);
    }

    [Theory]
    [InlineData(0.9, EmailQuality.High)]
    [InlineData(0.8, EmailQuality.High)]
    [InlineData(0.7, EmailQuality.Medium)]
    [InlineData(0.5, EmailQuality.Medium)]
    [InlineData(0.3, EmailQuality.Low)]
    [InlineData(0.2, EmailQuality.Low)]
    [InlineData(0.1, EmailQuality.Invalid)]
    [InlineData(0.0, EmailQuality.Invalid)]
    public void EmailValidationResult_Quality_ReturnsCorrectLevel(double score, EmailQuality expected)
    {
        var result = new EmailValidationResult { Score = score };
        Assert.Equal(expected, result.Quality);
    }

    #endregion

    #region ApiLayerResponse Tests

    [Fact]
    public void ApiLayerResponse_Ok_CreatesSuccessfulResponse()
    {
        var data = new IpGeolocationResult { Ip = "8.8.8.8" };
        var response = ApiLayerResponse<IpGeolocationResult>.Ok(data);

        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal("8.8.8.8", response.Data.Ip);
        Assert.Null(response.Error);
    }

    [Fact]
    public void ApiLayerResponse_Fail_CreatesFailedResponse()
    {
        var response = ApiLayerResponse<IpGeolocationResult>.Fail("Test error", 404);

        Assert.False(response.Success);
        Assert.Null(response.Data);
        Assert.NotNull(response.Error);
        Assert.Equal("Test error", response.Error.Message);
        Assert.Equal(404, response.Error.Code);
    }

    #endregion

    #region CurrencyCodes Tests

    [Fact]
    public void CurrencyCodes_HasCommonCurrencies()
    {
        Assert.Equal("USD", CurrencyCodes.USD);
        Assert.Equal("EUR", CurrencyCodes.EUR);
        Assert.Equal("GBP", CurrencyCodes.GBP);
        Assert.Equal("JPY", CurrencyCodes.JPY);
        Assert.Equal("CHF", CurrencyCodes.CHF);
        Assert.Equal("CAD", CurrencyCodes.CAD);
        Assert.Equal("AUD", CurrencyCodes.AUD);
        Assert.Equal("CNY", CurrencyCodes.CNY);
        Assert.Equal("BTC", CurrencyCodes.BTC);
        Assert.Equal("XAU", CurrencyCodes.XAU);
        Assert.Equal("XAG", CurrencyCodes.XAG);
    }

    #endregion

    #region ApiLayerException Tests

    [Fact]
    public void ApiLayerException_StoresMessage()
    {
        var ex = new ApiLayerException("Test error", 500);

        Assert.Equal("Test error", ex.Message);
        Assert.Equal(500, ex.ErrorCode);
    }

    [Fact]
    public void ApiLayerException_DefaultErrorCode_IsZero()
    {
        var ex = new ApiLayerException("Test error");

        Assert.Equal("Test error", ex.Message);
        Assert.Equal(0, ex.ErrorCode);
    }

    #endregion

    #region FromEnvironment Tests

    [Fact]
    public void FromEnvironment_WithMissingVariable_ThrowsInvalidOperationException()
    {
        Environment.SetEnvironmentVariable("APILAYER_TEST_MISSING", null);

        Assert.Throws<InvalidOperationException>(
            () => ApiLayerClient.FromEnvironment("APILAYER_TEST_MISSING"));
    }

    [Fact]
    public void FromEnvironment_WithSetVariable_CreatesClient()
    {
        const string envVar = "APILAYER_TEST_KEY";
        Environment.SetEnvironmentVariable(envVar, "test-api-key");

        try
        {
            var client = ApiLayerClient.FromEnvironment(envVar);

            Assert.NotNull(client);
            Assert.Equal("test-api-key", client.Options.ApiKey);
        }
        finally
        {
            Environment.SetEnvironmentVariable(envVar, null);
        }
    }

    #endregion

    #region Validation Tests

    [Fact]
    public async Task GetIpGeolocationAsync_WithEmptyIp_ThrowsArgumentException()
    {
        var client = new ApiLayerClient("test-key");

        await Assert.ThrowsAsync<ArgumentException>(
            () => client.GetIpGeolocationAsync(""));
    }

    [Fact]
    public async Task GetHistoricalRatesAsync_WithEmptyDate_ThrowsArgumentException()
    {
        var client = new ApiLayerClient("test-key");

        await Assert.ThrowsAsync<ArgumentException>(
            () => client.GetHistoricalRatesAsync(""));
    }

    [Fact]
    public async Task ConvertCurrencyAsync_WithEmptyFrom_ThrowsArgumentException()
    {
        var client = new ApiLayerClient("test-key");

        await Assert.ThrowsAsync<ArgumentException>(
            () => client.ConvertCurrencyAsync("", "EUR", 100));
    }

    [Fact]
    public async Task ConvertCurrencyAsync_WithEmptyTo_ThrowsArgumentException()
    {
        var client = new ApiLayerClient("test-key");

        await Assert.ThrowsAsync<ArgumentException>(
            () => client.ConvertCurrencyAsync("USD", "", 100));
    }

    [Fact]
    public async Task GetTimeSeriesAsync_WithEmptyStartDate_ThrowsArgumentException()
    {
        var client = new ApiLayerClient("test-key");

        await Assert.ThrowsAsync<ArgumentException>(
            () => client.GetTimeSeriesAsync("", "2024-01-31"));
    }

    [Fact]
    public async Task GetTimeSeriesAsync_WithEmptyEndDate_ThrowsArgumentException()
    {
        var client = new ApiLayerClient("test-key");

        await Assert.ThrowsAsync<ArgumentException>(
            () => client.GetTimeSeriesAsync("2024-01-01", ""));
    }

    [Fact]
    public async Task ValidatePhoneAsync_WithEmptyNumber_ThrowsArgumentException()
    {
        var client = new ApiLayerClient("test-key");

        await Assert.ThrowsAsync<ArgumentException>(
            () => client.ValidatePhoneAsync(""));
    }

    [Fact]
    public async Task ValidateEmailAsync_WithEmptyEmail_ThrowsArgumentException()
    {
        var client = new ApiLayerClient("test-key");

        await Assert.ThrowsAsync<ArgumentException>(
            () => client.ValidateEmailAsync(""));
    }

    #endregion
}
