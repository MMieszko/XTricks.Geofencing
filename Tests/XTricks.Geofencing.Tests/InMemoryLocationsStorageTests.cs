using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using XTricks.Geofencing.Abstractions;

namespace XTricks.Geofencing.Tests
{
    [TestFixture]
    public class InMemoryLocationsStorageTests
    {
        private ILocationLogsStorage _locationsStorage;

        [SetUp]
        public void Setup()
        {
            _locationsStorage = new InMemoryLocationsStorage(20);
        }

        [Test]
        public async Task AddAsync()
        {
            var locationLog = new LocationLog(1, 1);

            await _locationsStorage.AddAsync(locationLog);

            var result = await _locationsStorage.GetAsync();

            result.Should().HaveCount(1);
        }

        [Test]
        public async Task AddAsync_FullCapacity()
        {
            for (var i = 0; i < 20; i++)
            {
                await _locationsStorage.AddAsync(new LocationLog(i, i));
            }

            var result = await _locationsStorage.GetAsync();

            result.Should().HaveCount(20);
        }

        [Test]
        public async Task AddAsync_Exceed_ShouldContainMaximum()
        {
            for (var i = 0; i < 30; i++)
            {
                await _locationsStorage.AddAsync(new LocationLog(i, i));
            }

            var result = await _locationsStorage.GetAsync();

            result.Should().HaveCount(20);
        }

        [Test]
        public async Task AddAsync_ExceedCapacity_ShouldRemoveLast()
        {
            var first = new LocationLog(50, 50);

            await _locationsStorage.AddAsync(first);

            for (var i = 0; i < 19; i++)
            {
                await _locationsStorage.AddAsync(new LocationLog(i, i));
            }

            var last = new LocationLog(55, 55);

            await _locationsStorage.AddAsync(last);

            var result = await _locationsStorage.GetAsync();


            result.Should().NotContain(first);
            result.Should().Contain(last);
            result.Should().HaveCount(20);
        }

        [Test]
        public async Task AddAsync_ExceedCapacity_ShouldRemoveLast1()
        {
            for (var i = 0; i < 20; i++)
            {
                await _locationsStorage.AddAsync(new LocationLog(i, i));
            }

            for (var i = 50; i < 60; i++)
            {
                await _locationsStorage.AddAsync(new LocationLog(i, i));
            }

            var result = await _locationsStorage.GetAsync();


            result.First().Latitude.Should().Be(10);
        }
    }
}