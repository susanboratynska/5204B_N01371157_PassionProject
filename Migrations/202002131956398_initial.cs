namespace PassionProject_SusanBoratynska.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Players",
                c => new
                    {
                        playerID = c.Int(nullable: false, identity: true),
                        firstname = c.String(),
                        lastname = c.String(),
                        teamID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.playerID)
                .ForeignKey("dbo.Teams", t => t.teamID, cascadeDelete: true)
                .Index(t => t.teamID);
            
            CreateTable(
                "dbo.Positions",
                c => new
                    {
                        positionID = c.Int(nullable: false, identity: true),
                        position = c.String(),
                    })
                .PrimaryKey(t => t.positionID);
            
            CreateTable(
                "dbo.Teams",
                c => new
                    {
                        teamID = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        city = c.String(),
                        state = c.String(),
                        division = c.String(),
                    })
                .PrimaryKey(t => t.teamID);
            
            CreateTable(
                "dbo.UpdatePlayer",
                c => new
                    {
                        Position_positionID = c.Int(nullable: false),
                        Player_playerID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Position_positionID, t.Player_playerID })
                .ForeignKey("dbo.Positions", t => t.Position_positionID, cascadeDelete: true)
                .ForeignKey("dbo.Players", t => t.Player_playerID, cascadeDelete: true)
                .Index(t => t.Position_positionID)
                .Index(t => t.Player_playerID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Players", "teamID", "dbo.Teams");
            DropForeignKey("dbo.PositionPlayers", "Player_playerID", "dbo.Players");
            DropForeignKey("dbo.PositionPlayers", "Position_positionID", "dbo.Positions");
            DropIndex("dbo.PositionPlayers", new[] { "Player_playerID" });
            DropIndex("dbo.PositionPlayers", new[] { "Position_positionID" });
            DropIndex("dbo.Players", new[] { "teamID" });
            DropTable("dbo.PositionPlayers");
            DropTable("dbo.Teams");
            DropTable("dbo.Positions");
            DropTable("dbo.Players");
        }
    }
}
