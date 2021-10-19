using MrErsh.RadioRipper.Core;
using NUnit.Framework;

namespace RadioRipper.Core.Tests
{
    [TestFixture]
    public class MetadataHeaderTests
    {
        [Test]
        public void FullName_CorrectFullNameWasExtracted()
        {
            var header = "StreamTitle='Anitek - Tab & Anitek - ToyNBee';";

            var result = new MetadataHeader(header);

            Assert.AreEqual(result.StreamTitle, "Anitek - Tab & Anitek - ToyNBee");
        }

        [Test]
        public void FullName_EnptyStreamTitle_IsNull()
        {
            var header = "StreamTitle='';StreamUrl='';adId='25491';";

            var headerInfo = new MetadataHeader(header);

            Assert.IsEmpty(headerInfo.StreamTitle);
        }
    }
}
