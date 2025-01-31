using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace LBPUnion.ProjectLighthouse.Tests
{
    public class UploadTests : LighthouseTest
    {
        public UploadTests()
        {
            string assetsDirectory = Path.Combine(Environment.CurrentDirectory, "r");
            if (Directory.Exists(assetsDirectory)) Directory.Delete(assetsDirectory, true);
        }

        [Fact]
        public async Task ShouldNotAcceptScript()
        {
            HttpResponseMessage response = await this.UploadFileEndpointRequest("ExampleFiles/TestScript.ff");
            Assert.False(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task ShouldNotAcceptFarc()
        {
            HttpResponseMessage response = await this.UploadFileEndpointRequest("ExampleFiles/TestFarc.farc");
            Assert.False(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task ShouldNotAcceptGarbage()
        {
            HttpResponseMessage response = await this.UploadFileEndpointRequest("ExampleFiles/TestGarbage.bin");
            Assert.False(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task ShouldAcceptTexture()
        {
            HttpResponseMessage response = await this.UploadFileEndpointRequest("ExampleFiles/TestTexture.tex");
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task ShouldAcceptLevel()
        {
            HttpResponseMessage response = await this.UploadFileEndpointRequest("ExampleFiles/TestLevel.lvl");
            Assert.True(response.IsSuccessStatusCode);
        }
    }
}