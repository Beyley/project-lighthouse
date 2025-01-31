﻿using LBPUnion.ProjectLighthouse.Helpers;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectLighthouse.Migrations
{
    public partial class RemoveStartupMigrations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            long timestamp = TimeHelper.UnixTimeMilliseconds();
            
            // Fix timestamps
            migrationBuilder.Sql($"UPDATE Slots SET FirstUploaded = {timestamp} WHERE FirstUploaded = 0");
            migrationBuilder.Sql($"UPDATE Slots SET LastUpdated = {timestamp} WHERE LastUpdated = 0");
            migrationBuilder.Sql($"UPDATE Slots SET FirstUploaded = LastUpdated WHERE FirstUploaded > LastUpdated");
            migrationBuilder.Sql($"UPDATE Comments SET Timestamp = {timestamp} WHERE Timestamp = 0");
            
            // Fix slot minimum players (old LBP1 uploads caused this to happen)
            migrationBuilder.Sql($"UPDATE Slots SET MinimumPlayers = 1, MaximumPlayers = 4 " +
                                 "WHERE MinimumPlayers = 0 OR MaximumPlayers = 0 OR MinimumPlayers > MaximumPlayers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
