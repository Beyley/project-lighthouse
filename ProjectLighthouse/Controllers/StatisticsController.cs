using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Helpers;
using LBPUnion.ProjectLighthouse.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace LBPUnion.ProjectLighthouse.Controllers
{
    [ApiController]
    [Route("LITTLEBIGPLANETPS3_XML/")]
    [Produces("text/plain")]
    public class StatisticsController : ControllerBase
    {
        private readonly Database database;
        public StatisticsController(Database database)
        {
            this.database = database;
        }

        [HttpGet("playersInPodCount")]
        [HttpGet("totalPlayerCount")]
        public async Task<IActionResult> TotalPlayerCount() => this.Ok((await StatisticsHelper.RecentMatches()).ToString()!);

        [HttpGet("planetStats")]
        public async Task<IActionResult> PlanetStats()
        {
            int totalSlotCount = await StatisticsHelper.SlotCount();
            int mmPicksCount = await StatisticsHelper.MMPicksCount();

            return this.Ok
            (
                LbpSerializer.StringElement
                    ("planetStats", LbpSerializer.StringElement("totalSlotCount", totalSlotCount) + LbpSerializer.StringElement("mmPicksCount", mmPicksCount))
            );
        }

        [HttpGet("planetStats/totalLevelCount")]
        public async Task<IActionResult> TotalLevelCount() => this.Ok((await StatisticsHelper.SlotCount()).ToString());
    }
}