using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;
using LightZ.ComponentModel.Core;

namespace LightZ.Tests.ComponentModel.Core
{
    [TestClass]
    public class AsyncBackgroundWorkerTests
    {
        private int _progress;
        private bool _cancelled;

        [TestInitialize]
        public void TestInitialize()
        {
            _progress = 0;
            _cancelled = false;
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
        }

        [TestMethod]
        public async Task AyncBackground()
        {
            var worker = new AsyncBackgroundWorker();

            worker.DoWork += Worker_DoWork;
            worker.WorkerPaused += Worker_WorkerPaused;
            worker.Resume();

            await Task.Delay(3000);

            Assert.AreEqual(_progress, 1);
            Assert.IsFalse(_cancelled);
        }

        [TestMethod]
        public async Task AyncBackgroundLoop()
        {
            var worker = new AsyncBackgroundWorker();

            worker.WorkInLoop = true;
            worker.DoWork += Worker_DoWork;
            worker.WorkerPaused += Worker_WorkerPaused;
            worker.Resume();

            await Task.Delay(600);

            Assert.IsTrue(_progress > 1);
            Assert.IsFalse(_cancelled);
        }

        [TestMethod]
        public async Task AyncBackgroundCancelled()
        {
            var worker = new AsyncBackgroundWorker();

            worker.DoWork += Worker_DoWork;
            worker.WorkerPaused += Worker_WorkerPaused;
            worker.Resume();

            await Task.Delay(50);

            worker.Pause();

            await Task.Delay(1000);

            Assert.IsTrue(_cancelled);
        }

        private void Worker_DoWork(object sender, EventArgs e)
        {
            var worker = (AsyncBackgroundWorker)sender;

            worker.ThrowIfCanceled();
            Task.Delay(100).Wait();

            worker.ThrowIfCanceled();
            Task.Delay(100).Wait();

            worker.ThrowIfCanceled();
            _progress++;
        }

        private void Worker_WorkerPaused(object sender, LightZ.ComponentModel.Events.AsyncBackgroundWorkerEndedEventArgs e)
        {
            _cancelled = e.IsCanceled;
        }
    }
}
