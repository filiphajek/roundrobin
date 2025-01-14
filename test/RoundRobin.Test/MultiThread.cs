using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace RoundRobin.Test
{
    public class MultiThread
    {
        private List<int> _data;

        [SetUp]
        public void Setup()
        {
            _data = new List<int>()
            {
                1, 2, 3, 4, 5
            };
        }

        [Test]
        public async Task DecreasePriority()
        {
            var rb = new RoundRobinList<int>(_data);

            rb.ResetAllWeights(1);
            rb.DecreaseWeight(1, 2);

            var tasks = new Task[10];
            var result = new List<int>();
            for (var i = 0; i < 10; i++)
            {
                tasks[i] = Task.Run(() => { result.Add(rb.Next()); });
            }

            await Task.WhenAll(tasks);

            result.ToList().ForEach(z => { TestContext.Write($"{z},"); });

            var mustBe = new List<int>()
            {
                1, 2, 2, 3, 3, 4, 4, 5, 5, 1
            };

            Assert.AreEqual(mustBe, result);
        }

        [Test]
        public async Task IncreasePriority()
        {
            var rb = new RoundRobinList<int>(_data);

            rb.IncreaseWeight(1, 2);

            var result = new List<int>();
            var tasks = new Task[10];
            for (var i = 0; i < 10; i++)
            {
                tasks[i] = Task.Run(() => { result.Add(rb.Next()); });
            }

            await Task.WhenAll(tasks);

            result.ToList().ForEach(z => { TestContext.Write($"{z},"); });

            var mustBe = new List<int>()
            {
                1, 1, 1, 2, 3, 4, 5, 1, 1, 1
            };

            Assert.AreEqual(mustBe, result);
        }

        
        [Test]
        public async Task RoundRobin_StartTo()
        {
            var rb = new RoundRobinList<int>(_data);
            rb.ResetTo(4);

            var result = new List<int>();
            var tasks = new Task[10];
            for (var i = 0; i < 10; i++)
            {
                tasks[i] = Task.Run(() => { result.Add(rb.Next()); });
            }

            await Task.WhenAll(tasks);
            result.ToList().ForEach(z => { TestContext.Write($"{z},"); });

            var mustBe = new List<int>()
            {
                5, 1, 2, 3, 4, 5,1,2,3,4
            };

            Assert.AreEqual(result, mustBe);
        }
        
        [Test]
        public async Task RoundRobin()
        {
            var rb = new RoundRobinList<int>(_data);

            var result = new List<int>();
            var tasks = new Task[10];
            for (var i = 0; i < 10; i++)
            {
                tasks[i] = Task.Run(() => { result.Add(rb.Next()); });
            }

            await Task.WhenAll(tasks);
            result.ToList().ForEach(z => { TestContext.Write($"{z},"); });

            var mustBe = new List<int>()
            {
                1, 2, 3, 4, 5, 1, 2, 3, 4, 5
            };

            Assert.AreEqual(result, mustBe);
        }

        [Test]
        public async Task RoundRobinChunkByChunk()
        {
            var rb = new RoundRobinList<int>(_data);

            var result = new List<int>();
            var tasks = new Task[2];
            for (var i = 0; i < 2; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    var taskResult = rb.Nexts(5);
                    result.AddRange(taskResult);
                });
            }

            await Task.WhenAll(tasks);

            result.ToList().ForEach(z => { TestContext.Write($"{z},"); });

            Assert.AreEqual(new List<int>()
            {
                1, 2, 3, 4, 5, 1, 2, 3, 4, 5
            }, result);
        }
    }
}