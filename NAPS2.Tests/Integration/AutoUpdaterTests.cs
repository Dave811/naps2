﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAPS2.Update;
using NUnit.Framework;

namespace NAPS2.Tests.Integration
{
    [TestFixture(Category = "Integration,Slow")]
    public class AutoUpdaterTests
    {
        private AutoUpdater autoUpdater;

        [SetUp]
        public void SetUp()
        {
            const string versionFileUrl = "https://sourceforge.net/p/naps2/code/ci/master/tree/version.xml?format=raw";
            autoUpdater = new AutoUpdater(new LatestVersionSource(versionFileUrl, new UrlStreamReader()),
                new CurrentVersionSource(), new UrlFileDownloader(new UrlStreamReader()));
        }

        [TearDown]
        public void TearDown()
        {
            autoUpdater = null;
        }

        [Test]
        public void CheckForUpdate_ReturnsNoUpdate()
        {
            var updateInfo = autoUpdater.CheckForUpdate().Result;

            // Since we're (presumably) running from up-to-date source, the version should be up-to-date or newer
            Assert.False(updateInfo.HasUpdate);
            // No real checks to do on the version info, so just ensure it's specified
            Assert.NotNull(updateInfo.VersionInfo);
        }

        [Test]
        public void DownloadUpdate_DownloadsUpdate()
        {
            var updateInfo = autoUpdater.CheckForUpdate().Result;
            string savePath = updateInfo.VersionInfo.FileName;
            CleanupFile(savePath);
            try
            {
                var result = autoUpdater.DownloadUpdate(updateInfo.VersionInfo, savePath).Result;
                Assert.True(result);
                Assert.True(File.Exists(savePath));
            }
            finally
            {
                CleanupFile(savePath);
            }
        }

        private static void CleanupFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}