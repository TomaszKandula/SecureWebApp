using Xunit;
using Newtonsoft.Json;
using FluentAssertions;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using RazorWebApp.TestData;
using RazorWebApp.Shared.Dto;
using RazorWebApp.IntegrationTests.Extractor;
using RazorWebApp.IntegrationTests.Configuration;

namespace RazorWebApp.IntegrationTests
{
    public class ControllerTest_Ajax : IClassFixture<TestFixture<Startup>>
    {
        private readonly HttpClient FHttpClient;

        public ControllerTest_Ajax(TestFixture<Startup> ACustomFixture)
        {
            FHttpClient = ACustomFixture.Client;
        }

        [Theory]
        [InlineData("tokan@wp.pl")]
        public async Task Should_CheckEmail(string AEmailAddress) 
        {
            // Arrange
            var LRegisterPageResponse = await FHttpClient.GetAsync("/register");
            var LAntiForgeryValues = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(LRegisterPageResponse);

            // Act
            var LNewRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/ajax/validation/{AEmailAddress}");

            LNewRequest.Headers.Add("Cookie", new CookieHeaderValue(AntiForgeryTokenExtractor.AntiForgeryCookieName, LAntiForgeryValues.CookieValue).ToString());
            LNewRequest.Headers.TryAddWithoutValidation(AntiForgeryTokenExtractor.AntiForgeryFieldName, LAntiForgeryValues.FieldValue);

            var LResponse = await FHttpClient.SendAsync(LNewRequest);
            var LContent = await LResponse.Content.ReadAsStringAsync();

            // Assert
            LResponse.StatusCode.Should().Be(204);
        }

        [Fact]
        public async Task Should_ReturnCountry() 
        {
            // Arrange
            var LRegisterPageResponse = await FHttpClient.GetAsync("/register");
            var LAntiForgeryValues = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(LRegisterPageResponse);

            // Act
            var LNewRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1/ajax/countries");

            LNewRequest.Headers.Add("Cookie", new CookieHeaderValue(AntiForgeryTokenExtractor.AntiForgeryCookieName, LAntiForgeryValues.CookieValue).ToString());
            LNewRequest.Headers.TryAddWithoutValidation(AntiForgeryTokenExtractor.AntiForgeryFieldName, LAntiForgeryValues.FieldValue);

            var LResponse = await FHttpClient.SendAsync(LNewRequest);
            var LContent = await LResponse.Content.ReadAsStringAsync();

            // Assert
            LResponse.StatusCode.Should().Be(200);
            var LDeserialized = JsonConvert.DeserializeObject<ReturnCountryListDto>(LContent);
            LDeserialized.Countries.Should().HaveCount(249);
        }

        [Theory]
        [InlineData(1)]
        public async Task Should_ReturnCity(int ACountryId) 
        {
            // Arrange
            var LRegisterPageResponse = await FHttpClient.GetAsync("/register");
            var LAntiForgeryValues = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(LRegisterPageResponse);

            // Act
            var LNewRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/ajax/cities/?countryid={ACountryId}");

            LNewRequest.Headers.Add("Cookie", new CookieHeaderValue(AntiForgeryTokenExtractor.AntiForgeryCookieName, LAntiForgeryValues.CookieValue).ToString());
            LNewRequest.Headers.TryAddWithoutValidation(AntiForgeryTokenExtractor.AntiForgeryFieldName, LAntiForgeryValues.FieldValue);

            var LResponse = await FHttpClient.SendAsync(LNewRequest);
            var LContent = await LResponse.Content.ReadAsStringAsync();

            // Assert
            LResponse.StatusCode.Should().Be(200);
            var LDeserialized = JsonConvert.DeserializeObject<ReturnCityListDto>(LContent);
            LDeserialized.Cities.Should().HaveCount(11);
        }

        [Fact]
        public async Task Should_CreateAccount()
        {
            // Arrange
            var LRegisterPageResponse = await FHttpClient.GetAsync("/register");
            var LAntiForgeryValues = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(LRegisterPageResponse);

            // Act
            var LNewRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/ajax/users/signup/");

            LNewRequest.Headers.Add("Cookie", new CookieHeaderValue(AntiForgeryTokenExtractor.AntiForgeryCookieName, LAntiForgeryValues.CookieValue).ToString());
            LNewRequest.Headers.TryAddWithoutValidation(AntiForgeryTokenExtractor.AntiForgeryFieldName, LAntiForgeryValues.FieldValue);

            var LPayLoad = new UserCreateDto()
            {
                FirstName    = DataProvider.GetRandomString(),
                LastName     = DataProvider.GetRandomString(),
                NickName     = DataProvider.GetRandomString(),
                EmailAddress = DataProvider.GetRandomEmail(),
                Password     = DataProvider.GetRandomString(),
                CityId       = 187,
                CountryId    = 47
            };

            LNewRequest.Content = new StringContent(JsonConvert.SerializeObject(LPayLoad), System.Text.Encoding.Default, "application/json");
            var LResponse = await FHttpClient.SendAsync(LNewRequest);

            // Assert
            LResponse.StatusCode.Should().Be(204);
        }

        [Fact]
        public async Task Should_FailLogToAccount()
        {
            // Arrange
            var LRegisterPageResponse = await FHttpClient.GetAsync("/login");
            var LAntiForgeryValues = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(LRegisterPageResponse);

            // Act
            var LNewRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/ajax/users/signin/");

            LNewRequest.Headers.Add("Cookie", new CookieHeaderValue(AntiForgeryTokenExtractor.AntiForgeryCookieName, LAntiForgeryValues.CookieValue).ToString());
            LNewRequest.Headers.TryAddWithoutValidation(AntiForgeryTokenExtractor.AntiForgeryFieldName, LAntiForgeryValues.FieldValue);

            var LPayLoad = new UserLoginDto()
            {
                EmailAddr = DataProvider.GetRandomEmail(),
                Password  = DataProvider.GetRandomString()
            };

            LNewRequest.Content = new StringContent(JsonConvert.SerializeObject(LPayLoad), System.Text.Encoding.Default, "application/json");
            var LResponse = await FHttpClient.SendAsync(LNewRequest);

            // Assert
            LResponse.StatusCode.Should().Be(400);
        }
    }
}
