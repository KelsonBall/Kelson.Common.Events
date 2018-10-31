using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace Kelson.Common.Events.Tests
{
    public class Requests_Should
    {
        [Fact]
        public void ReturnRequestedValue()
        {
            var events = new EventManager();
            events.Handle<int, string>(i => Convert.ToString(i, 16));

            var responses = events.Request<int, string>(127);
            responses.Single().Should().Be("7f");
        }

        [Fact]
        public void ReturnAllResponses()
        {
            var events = new EventManager();
            events.Handle<int, string>(i => Convert.ToString(i, 16));
            events.Handle<int, string>(i => Convert.ToString(i, 2));

            var responses = events.Request<int, string>(127);
            responses.Should().Contain(new string[] { "7f", "1111111" });
        }

        [Fact]
        public void ReturnNoResponses()
        {
            var events = new EventManager();

            var responses = events.Request<int, string>(127);
            responses.Should().BeEmpty();
        }

        [Fact]
        public void HandleUnsubscribes()
        {
            var events = new EventManager();
            events.Handle<int, string>(i => Convert.ToString(i, 16));
            var second = events.Handle<int, string>(i => Convert.ToString(i, 2));

            var responses = events.Request<int, string>(127);
            responses.Should().Contain(new string[] { "7f", "1111111" });

            second.Unsubscribe();

            responses = events.Request<int, string>(127);
            responses.Single().Should().Be("7f");
        }

        [Fact]
        public void FilterByResponseType()
        {
            var events = new EventManager();
            events.Handle<int, string>(i => Convert.ToString(i, 16));
            events.Handle<int, int>(i => -i);

            events.Request<int, string>(127).Single().Should().Be("7f");
            events.Request<int, int>(127).Single().Should().Be(-127);
        }

        [Fact]
        public void FilterByRequestType()
        {
            var events = new EventManager();
            events.Handle<int, string>(i => Convert.ToString(i, 16));
            events.Handle<string, string>(i => i + i);

            events.Request<int, string>(127).Single().Should().Be("7f");
            events.Request<string, string>("hello").Single().Should().Be("hellohello");
        }

        [Fact]
        public void HandleItemRequests()
        {
            var events = new EventManager();
            int i = 0;
            events.Provide<int>(() => i++);
            events.Provide<(string, string)>(() => ("Hello", "World"));

            events.Request<int>().Should().Be(0);
            events.Request<int>().Should().Be(1);
            events.Request<(string, string)>().Should().BeEquivalentTo(("Hello", "World"));
        }        
    }
}
