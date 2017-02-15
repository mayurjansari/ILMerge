﻿using System.IO;
using System.Reflection;
using ILMerging.Tests.Helpers;
using NUnit.Framework;

namespace ILMerging.Tests
{
    [TestFixture]
    public sealed class KeyTests
    {
        [Test]
        public void Can_sign_using_keyfile()
        {
            using (var outputFile = TempFile.WithExtension(".dll"))
            {
                var ilMerge = new ILMerge { KeyFile = TestFiles.TestSnk, OutputFile = outputFile };
                ilMerge.SetUpInputAssemblyForTest(Assembly.GetExecutingAssembly());
                ilMerge.Merge();

                Assert.That(
                    AssemblyName.GetAssemblyName(outputFile).GetPublicKey(),
                    Is.EqualTo(new StrongNameKeyPair(File.ReadAllBytes(TestFiles.TestSnk)).PublicKey));
            }
        }

        [Test]
        public void Bad_keyfile_gives_diagnostic_warning()
        {
            using (var logFile = new TempFile())
            using (var outputFile = TempFile.WithExtension(".dll"))
            {
                var ilMerge = new ILMerge
                {
                    KeyFile = TestFiles.TestPfx,
                    OutputFile = outputFile,
                    LogFile = logFile
                };
                ilMerge.SetUpInputAssemblyForTest(Assembly.GetExecutingAssembly());

                ilMerge.Merge();

                var logText = File.ReadAllText(logFile);
                Assert.That(logText, Contains.Substring("Unable to obtain public key for StrongNameKeyPair."));
                Assert.That(logText, Contains.Substring("PFX"));
                Assert.That(logText, Contains.Substring("key container"));
            }
        }
    }
}
