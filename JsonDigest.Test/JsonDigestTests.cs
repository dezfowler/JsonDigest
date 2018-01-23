using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonDigest.Test
{
    [TestClass]
    public class JsonDigestTests
    {
        [DataTestMethod]
        [DataRow("null", "37a6259cc0c1dae299a7866489dff0bd")]
        [DataRow("1.01", "d5a637cd11aa722a9b4c922c7b740a63")]
        [DataRow("true", "b326b5062b2f0e69046810717534cb09")]
        [DataRow(@"""hellow world""", "3eddb2499469f8e0b1c4bcfad2bc0d8a")]
        public void Make_digest(string inputJson, string expectedDigest)
        {
            var actualDigest = new JsonDigest().Generate(inputJson);
            Assert.AreEqual(expectedDigest, actualDigest);
        }

        [DataTestMethod]
        [DataRow(@"{""hello"": ""world""}", @"{    ""hello"":     ""world""   }    ", "fbc24bcc7a1794758fc1327fcfebdaf6", DisplayName = "Whitespace should normalize")]
        [DataRow(@"{""a"": ""hello"", ""b"": ""world""}", @"{""b"": ""world"", ""a"": ""hello""}", "00c73bed078a44de04afd7771c10651d", DisplayName = "Object property order should normalize")]
        [DataRow(@"[1, {""a"": ""hello"", ""b"": ""world""}]", @"[1, {""b"": ""world"", ""a"": ""hello""}]", "1a42d19f2742008df0d98060203a0fb9", DisplayName = "Objects within arrays also normalize")]
        public void Digests_should_match(string inputJsonA, string inputJsonB, string expectedDigest)
        {
            var actualDigestA = new JsonDigest().Generate(inputJsonA);
            var actualDigestB = new JsonDigest().Generate(inputJsonB);
            
            Assert.AreEqual(expectedDigest, actualDigestA);
            Assert.AreEqual(expectedDigest, actualDigestB);
        }

        [DataTestMethod]
        [DataRow(@"[1, 2]", @"[2, 1]", DisplayName = "Objects within arrays also normalize")]
        public void Digests_should_not_match(string inputJsonA, string inputJsonB)
        {
            var actualDigestA = new JsonDigest().Generate(inputJsonA);
            var actualDigestB = new JsonDigest().Generate(inputJsonB);

            Assert.AreNotEqual(actualDigestA, actualDigestB);
        }
    }
}
