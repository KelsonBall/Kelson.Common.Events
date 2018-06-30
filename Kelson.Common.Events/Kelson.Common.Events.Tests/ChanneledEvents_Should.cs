using FluentAssertions;
using System.Linq;
using Xunit;

namespace Kelson.Common.Events.Tests
{
    public class ChanneledEvents_Should
    {
        public enum Channels
        {
            One,
            Two,
            Three
        }

        [Fact]
        public void OnlyPubSubOnChannel()
        {
            var events = new ChanneledEventManager<Channels>();

            int value1 = 0;
            int value2 = 0;
            int value3 = 0;

            events.Subscribe<int>(Channels.One, i => value1 = i);
            events.Subscribe<int>(Channels.Two, i => value2 = i);
            events.Subscribe<int>(Channels.Three, i => value3 = i);

            events.Publish(Channels.One, 1);
            events.Publish(Channels.Two, 2);
            events.Publish(Channels.Three, 3);

            value1.Should().Be(1);
            value2.Should().Be(2);
            value3.Should().Be(3);
        }

        [Fact]
        public void OnlyHandleRequestsOnChannel()
        {
            var events = new ChanneledEventManager<Channels>();

            events.Handle<int, int>(Channels.One, i => i + 1);
            events.Handle<int, int>(Channels.Two, i => i + 2);
            events.Handle<int, int>(Channels.Three, i => i + 3);

            events.Request<int, int>(Channels.One, 0).Single().Should().Be(1);
            events.Request<int, int>(Channels.Two, 0).Single().Should().Be(2);
            events.Request<int, int>(Channels.Three, 0).Single().Should().Be(3);
        }
    }
}
