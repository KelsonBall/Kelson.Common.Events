using FluentAssertions;
using System;
using Xunit;

namespace Kelson.Common.Events.Tests
{
    public class Events_Should
    {
        [Fact]
        public void ListenForEvents()
        {
            int value = 0;
            var events = new EventManager();
            var subscription = events.Listen<int>(i => value = i + 1);

            events.Publish(3);
            value.Should().Be(4);

            events.Publish(int.MaxValue);
            value.Should().Be(int.MinValue); // overflow (maxvalue + 1 = minvalue)

            subscription.Unsubscribe();
            events.Publish(0);
            value.Should().Be(int.MinValue); // no change, because unsubscribed
        }

        [Fact]
        public void SubscribeToEvents()
        {
            int value = 0;
            var events = new EventManager();

            var subscription = events.Subscribe<int>(i => value = i + 1);

            events.Publish(3);            
            value.Should().Be(4);

            events.Publish(int.MaxValue);            
            value.Should().Be(int.MinValue);

            subscription.Unsubscribe();
            events.Publish(0);            
            value.Should().Be(int.MinValue); // no change, because unsubscribed
        }

        [Fact]
        public void StopListeningWhenTokenGarbageCollected()
        {
            int value = 0;
            var events = new EventManager();

            ((Action)(() =>
            {
                var subscription = events.Listen<int>(i => value = i);
                events.Publish(3);
                value.Should().Be(3);

                events.Publish(int.MaxValue); // publish value before subscription goes out of scope
            }
            ))();

            GC.Collect(); // 'listen' subscriptions are weak references - subscription gets garbage collected
            events.Publish(-1);
            value.Should().Be(int.MaxValue); // check for last value before subscription token went out of scope
        }

        [Fact]
        public void StaySubscribedWhenTokenGarbageCollected()
        {
            int value = 0;
            var events = new EventManager();
            {
                var subscription = events.Subscribe<int>(i => value = i);
                events.Publish(3);
                value.Should().Be(3);
                
                events.Publish(int.MaxValue);
            }

            GC.Collect();
            events.Publish(-1);
            value.Should().Be(-1); // subscription not garbage collection when token went out of scope
        }

        [Fact]
        public void PublishDifferentEventTypesOnDifferentChannels()
        {
            int intVal = 0;
            string stringVal = "0";
            var events = new EventManager();

            events.Subscribe<int>(i => intVal = i);
            events.Subscribe<string>(s => stringVal = s);

            events.Publish(1);
            events.Publish("2");

            intVal.Should().Be(1);                        
            stringVal.Should().Be("2");
        }

        [Fact]
        public void PushEventsToAllSubscriptions()
        {
            int value1 = 0;
            int value2 = 0;
            var events = new EventManager();

            events.Subscribe<int>(i => value1 = i);
            var v2sub = events.Subscribe<int>(i => value2 -= i);

            events.Publish(1);

            value1.Should().Be(1);
            value2.Should().Be(-1); // both subscriptions executed
            
            v2sub.Unsubscribe(); // unsubscribe from 2nd subscription
            events.Publish(2);
            
            value1.Should().Be(2);            
            value2.Should().Be(-1); // second value unchanged
        }
    }
}
