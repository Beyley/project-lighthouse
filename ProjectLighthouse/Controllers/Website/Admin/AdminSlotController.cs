#nullable enable
using System;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Types;
using LBPUnion.ProjectLighthouse.Types.Levels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LBPUnion.ProjectLighthouse.Controllers.Website.Admin
{
    [ApiController]
    [Route("admin/slot/{id:int}")]
    public class AdminSlotController : ControllerBase
    {
        private readonly Database database;

        public AdminSlotController(Database database)
        {
            this.database = database;
        }

        [Route("teamPick")]
        public async Task<IActionResult> TeamPick([FromRoute] int id)
        {
            User? user = this.database.UserFromWebRequest(this.Request);
            if (user == null || !user.IsAdmin) return this.StatusCode(403, "");

            Slot? slot = await this.database.Slots.FirstOrDefaultAsync(s => s.SlotId == id);
            if (slot == null) return this.NotFound();

            slot.TeamPick = true;

            await this.database.SaveChangesAsync();

            return this.Ok();
        }

        [Route("removeTeamPick")]
        public async Task<IActionResult> RemoveTeamPick([FromRoute] int id)
        {
            User? user = this.database.UserFromWebRequest(this.Request);
            if (user == null || !user.IsAdmin) return this.StatusCode(403, "");

            Slot? slot = await this.database.Slots.FirstOrDefaultAsync(s => s.SlotId == id);
            if (slot == null) return this.NotFound();

            slot.TeamPick = false;

            await this.database.SaveChangesAsync();

            return this.Ok();
        }

        [Route("delete")]
        public async Task<IActionResult> DeleteLevel([FromRoute] int id)
        {
            User? user = this.database.UserFromWebRequest(this.Request);
            if (user == null || !user.IsAdmin) return this.StatusCode(403, "");

            Slot? slot = await this.database.Slots.FirstOrDefaultAsync(s => s.SlotId == id);
            if (slot == null) return this.Ok();

            if (slot.Location == null) throw new ArgumentNullException();

            this.database.Locations.Remove(slot.Location);
            this.database.Slots.Remove(slot);

            await this.database.SaveChangesAsync();

            return this.Ok();
        }
    }
}