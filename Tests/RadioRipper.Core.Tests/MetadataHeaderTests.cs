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

            Assert.AreEqual("Anitek - Tab & Anitek - ToyNBee", result.StreamTitle);
        }

        [Test]
        public void FullName_EmptyStreamTitle_IsNull()
        {
            var header = "StreamTitle='';StreamUrl='';adId='25491';";

            var headerInfo = new MetadataHeader(header);

            Assert.IsEmpty(headerInfo.StreamTitle);
        }

        [Test]
        public void FullName_StremUrlIsNull_TrackNameHaveNoStreamUrlKeyWord()
        {
            var header = "StreamTitle='4hero - The Awakening';StreamUrl='';";

            var headerInfo = new MetadataHeader(header);

            Assert.AreEqual("4hero - The Awakening", headerInfo.StreamTitle);
            Assert.AreEqual("The Awakening", headerInfo.TrackName);
        }
    }
}
